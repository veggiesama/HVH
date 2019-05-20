using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Initializer : MonoBehaviour {

	public string sceneName;
	public NetworkManager networkManager;

    // Start is called before the first frame update
    void Start() {
		//networkManager.ServerChangeScene(sceneName); //, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
        //SceneManager.LoadScene(sceneName);
    }

}
