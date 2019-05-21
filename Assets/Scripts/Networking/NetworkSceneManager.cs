using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkSceneManager : Singleton<NetworkSceneManager> {

	[Scene]
	[Tooltip("Add all sub-scenes to this list")]
	public string[] subScenes;
	public AsyncOperation[] asyncOps;
	public Scene initializerScene;

	// Singleton constructor
	public static NetworkSceneManager Instance {
		get {
			return ((NetworkSceneManager)mInstance);
		} set {
			mInstance = value;
		}
	}

	// EVENT HANDLERS
	public delegate void OnGameplayScenesInitializedDelegate();
	public event OnGameplayScenesInitializedDelegate OnGameplayScenesInitializedEventHandler;

	public void OnGameplayScenesInitialized() {
		if (OnGameplayScenesInitializedEventHandler != null) {
			OnGameplayScenesInitializedEventHandler();	
		}
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

	//////////////////////////////////////////////////////////////////////////////////

	public void InitializeGameplayScenes() {

		StartCoroutine( _InitializeGameplayScenes() );

	}

	IEnumerator _InitializeGameplayScenes() {

		Debug.Log("Initializing gameplay scenes.");

		asyncOps = new AsyncOperation[subScenes.Length];
		for (int n = 0; n < subScenes.Length; n++) {
			string sceneName = subScenes[n];
			AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			asyncOps[n] = op;
			Debug.LogFormat("Loaded {0}", sceneName);
		}

		while(!AreAllScenesLoaded()) {
			yield return null;
		}

		//SceneManager.SetActiveScene( SceneManager.GetSceneAt(1) );
		AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(initializerScene);

		while (!unloadScene.isDone) {
			yield return null;
		}

		yield return new WaitForEndOfFrame();

		Debug.Log("All gameplay scenes initialized.");
		OnGameplayScenesInitialized();
	}

	public bool AreAllScenesLoaded() {
		foreach (AsyncOperation op in asyncOps) {
			if (!op.isDone) return false;
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