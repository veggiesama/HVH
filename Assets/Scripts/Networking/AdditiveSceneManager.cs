using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Events;

public class AdditiveSceneManager : MonoBehaviour {

	public GameObject mainMenu;
	[Scene]
	public string[] subScenes;
	private Scene initializerScene;
	[HideInInspector]
	public bool gameplayScenesInitialized = false;

	public UnityEvent OnGameplayerScenesInitialized;

	public void Awake() {
		 //DontDestroyOnLoad(this.gameObject);
	}

	public void Start() {
		initializerScene = SceneManager.GetActiveScene();

		// unload all scenes except for initializer
		// (forces editor to start just like a build client)
		for (int i = 0; i < SceneManager.sceneCount; i++) {
			string sceneName = SceneManager.GetSceneAt(i).name;
			if (sceneName != initializerScene.name)
				StartCoroutine( UnloadScene(sceneName) );
		}

	}

	public void EnableMainMenu(bool enable) {
		mainMenu.SetActive(enable);
	}

	//////////////////////////////////////////////////////////////////////////////////

	public void InitializeGameplayScenes() {

		StartCoroutine( _InitializeGameplayScenes() );

	}

	IEnumerator _InitializeGameplayScenes() {

		Debug.Log("Initializing gameplay scenes.");

		AsyncOperation[] asyncOps = new AsyncOperation[subScenes.Length];
		for (int n = 0; n < subScenes.Length; n++) {
			string sceneName = subScenes[n];
			AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			op.allowSceneActivation = false;
			asyncOps[n] = op;
			Debug.LogFormat("Loaded {0}", sceneName);
		}

		while(!AreAllScenesLoaded(asyncOps)) {
			yield return null;
		}

		SceneManager.SetActiveScene( SceneManager.GetSceneAt(1) );
		yield return new WaitForEndOfFrame();

		gameplayScenesInitialized = true;
		EnableMainMenu(false);
		Debug.Log("All gameplay scenes initialized.");
		OnGameplayerScenesInitialized.Invoke();
	}

	public bool AreAllScenesLoaded(AsyncOperation[] asyncOps) {
		foreach (AsyncOperation op in asyncOps) {
			if (!op.isDone) {
				if (op.progress >= 0.9f)
					op.allowSceneActivation = true;
				return false;
			}
		}
		return true;
	}

	/*
	IEnumerator LoadScene(string sceneName)
	{
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		Debug.LogFormat("Loaded {0}", sceneName);
	}*/

	public void UnloadSubScenes() {
		gameplayScenesInitialized = false;
		EnableMainMenu(true);

		Debug.Log("Unloading Scenes");
		foreach (string sceneName in subScenes)
			if (SceneManager.GetSceneByName(sceneName).IsValid())
				StartCoroutine(UnloadScene(sceneName));
	}

	IEnumerator UnloadScene(string sceneName) {
		yield return SceneManager.UnloadSceneAsync(sceneName);
		yield return Resources.UnloadUnusedAssets();
		Debug.LogFormat("Unloaded {0}", sceneName);
	}
}