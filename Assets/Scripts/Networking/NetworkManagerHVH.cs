using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerHVH : NetworkManager {

	[Header("Scene Manager")]
	public AdditiveSceneManager sceneManager;
	public NetworkHUD networkHUD;

	public bool isHost = false;
	public bool isServer = false;
	//private bool isClient = false;

	public override void Awake() {
		base.Awake();
		//sceneManager.OnGameplayScenesInitializedEventHandler += OnGameplayScenesInitialized;
		sceneManager.OnGameplayerScenesInitialized.AddListener(OnGameplayScenesInitialized);
	}

	public override void Start() {
		base.Start();
		networkHUD.enabled = true;
	}



	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// HOST CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// When the host is started:
	// Start()

	public override void OnStartHost() {
		base.OnStartHost();
		Debug.Log("Host: Started");
		isHost = true;
    }
	
	// OnStartServer()
	// OnServerConnect
	// OnStartClient
	// OnClientConnect
	// OnServerSceneChanged
	// OnServerReady
	// OnServerAddPlayer
	// OnClientChangeScene
	// OnClientSceneChanged
	
	// When a client connects:
	// OnServerConnect
	// OnServerReady
	// OnServerAddPlayer

	// OnServerDisconnect

	public override void OnStopHost() {
        Debug.Log("Host: Stopped");
    }

	// OnStopServer
	// OnStopClient

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// SERVER CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Start()

    public override void OnStartServer() {
		Debug.Log("Server: Started");
		sceneManager.InitializeGameplayScenes();
		isServer = true;
    }

	public override void OnServerSceneChanged(string sceneName) {
		base.OnServerSceneChanged(sceneName);
	}

	public override void OnServerConnect(NetworkConnection conn) {
		Debug.Log("Server: Client connected to the server: " + conn);
		NetworkServer.SetClientReady(conn);
    }

    public override void OnServerReady(NetworkConnection conn) {
		base.OnServerReady(conn);
		Debug.Log("Server: Client is ready");
		UpdateTransforms();

		//for (int i = 0; i < GameResources.Instance.networkGameResources.playerObjectList.Count; i++) {
		//	GameResources.Instance.networkGameResources.playerObjectList.Dirty(i);
		//}
    }

	public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage) {
		base.OnServerAddPlayer(conn, extraMessage);
        Debug.Log("Server: Client has requested to get his player added to the game");
	}

    public override void OnServerDisconnect(NetworkConnection conn) {
        Debug.Log("Server: Client disconnected from the server: " + conn);
    }

    public override void OnStopServer() {
        Debug.Log("Server: Stopped");
		sceneManager.UnloadSubScenes();
    }

	public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player) {
	}

    public override void OnServerError(NetworkConnection conn, int errorCode) {
        Debug.Log("Server: Network error occurred: " + (UnityEngine.Networking.NetworkError)errorCode);
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// CLIENT CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Start()

	public override void OnStartClient() {
		Debug.Log("Client: Started");
		//isClient = true;
	}

    public override void OnClientConnect(NetworkConnection conn) {
		//base.OnClientConnect(conn);
		Debug.Log("Client: Connecting");
		if (!isHost) {
			sceneManager.InitializeGameplayScenes();
		}

		StartCoroutine( WaitClientConnect(conn) );
        //base.OnClientConnect(conn);
    }

	IEnumerator WaitClientConnect(NetworkConnection conn) {
		while (!sceneManager.gameplayScenesInitialized) {
			yield return null;
		}

		ClientScene.Ready(conn);
		ClientScene.AddPlayer();
		//UpdateTransforms();

		//Observer obs = conn.identity.GetComponent<Observer>();

		//obs.Request

		Debug.Log("Client: Successfully connected to server");
	}

	private static void UpdateTransforms() {
		foreach (var kv in NetworkIdentity.spawned) {
			NetworkIdentity netId = kv.Value;
			NetworkHelper helper = netId.GetComponent<NetworkHelper>();
			if (helper != null)
				helper.SyncTransform();
		}
	}

	//public override void OnClientChangeScene(string newSceneName) {
	//	base.OnClientChangeScene(newSceneName);
	//}

    public override void OnClientSceneChanged(NetworkConnection conn) {
        base.OnClientSceneChanged(conn);
        Debug.Log("Client: Server triggered scene change, and so have we");
    }

    public override void OnStopClient() {
        Debug.Log("Client: Stopped");
		
		if (!isHost) {
			sceneManager.UnloadSubScenes();
		}

    }

	public override void OnClientDisconnect(NetworkConnection conn) {
		base.OnClientDisconnect(conn);
		Debug.Log("Client: Disconnected " + conn);
    }
	
    public override void OnClientError(NetworkConnection conn, int errorCode) {
        Debug.Log("Client: Network error occurred: ???"); //+ (NetworkError)errorCode);
    }

    public override void OnClientNotReady(NetworkConnection conn) {
        Debug.Log("Client: Server has set client to be not-ready (stop getting state updates)");
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// OTHER
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void OnGameplayScenesInitialized() {
		if (isServer) {
			GameRules.Instance.SetupGame();
		}
	}

	public List<GameObject> GetSpawnPrefabList() {
		return base.spawnPrefabs;
	}

}
