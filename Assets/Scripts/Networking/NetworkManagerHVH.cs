using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerHVH : NetworkManager {

	[Header("Scene Manager")]
	public AdditiveSceneManager sceneManager;

	public bool isHost = false;
	public bool isServer = false;
	//private bool isClient = false;

	public override void Awake() {
		base.Awake();
		//sceneManager.OnGameplayScenesInitializedEventHandler += OnGameplayScenesInitialized;
		sceneManager.OnGameplayerScenesInitialized.AddListener(OnGameplayScenesInitialized);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// HOST CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// When the host is started:
	// Start()

	public override void OnStartHost() {
		base.OnStartHost();
		Debug.Log("Host has started");
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
        Debug.Log("Host has stopped");
    }

	// OnStopServer
	// OnStopClient

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// SERVER CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Start()

    public override void OnStartServer() {
		Debug.Log("Server has started");
		sceneManager.InitializeGameplayScenes();
		isServer = true;
    }

	public override void OnServerSceneChanged(string sceneName) {
		base.OnServerSceneChanged(sceneName);
	}

	public override void OnServerConnect(NetworkConnection conn) {
		Debug.Log("OnServerConnect Is ready: " + conn.isReady);
		Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerReady(NetworkConnection conn) {
		//base.OnServerReady(conn);
		StartCoroutine( WaitServerReady(conn) );
    }

	public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage) {
        Debug.Log("Client has requested to get his player added to the game");
	}

    public override void OnServerDisconnect(NetworkConnection conn) {
        Debug.Log("A client disconnected from the server: " + conn);
    }

    public override void OnStopServer() {
        Debug.Log("Server has stopped");
		sceneManager.UnloadSubScenes();
    }

	public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player) {
	}

    public override void OnServerError(NetworkConnection conn, int errorCode) {
        Debug.Log("Server network error occurred: ???"); //+ (NetworkError)errorCode);
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// CLIENT CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Start()

	public override void OnStartClient() {
		Debug.Log("Client has started");
		//isClient = true;
	}

    public override void OnClientConnect(NetworkConnection conn) {
		if (!isHost) {
			sceneManager.InitializeGameplayScenes();
		}

		StartCoroutine( WaitClientConnect(conn) );
        //base.OnClientConnect(conn);
    }

	public override void OnClientChangeScene(string newSceneName) {
		base.OnClientChangeScene(newSceneName);
	}

    public override void OnClientSceneChanged(NetworkConnection conn) {
        base.OnClientSceneChanged(conn);
        Debug.Log("Server triggered scene change and we've done the same, do any extra work here for the client...");
    }

    public override void OnStopClient() {
        Debug.Log("Client has stopped");
		
		if (!isHost) {
			sceneManager.UnloadSubScenes();
		}
    }

	public override void OnClientDisconnect(NetworkConnection conn) {
		base.OnClientDisconnect(conn);
		Debug.Log("Client disconnected from server: " + conn);
    }
	
    public override void OnClientError(NetworkConnection conn, int errorCode) {
        Debug.Log("Client network error occurred: ???"); //+ (NetworkError)errorCode);
    }

    public override void OnClientNotReady(NetworkConnection conn) {
        Debug.Log("Server has set client to be not-ready (stop getting state updates)");
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// OTHER
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void OnGameplayScenesInitialized() {
		if (isServer) {
			GameRules.Instance.SetupGame();
		}
	}

	IEnumerator WaitServerReady(NetworkConnection conn) {
		while (!sceneManager.gameplayScenesInitialized) {
			yield return null;
		}

		Debug.Log("Assigning client to next available player slot.");
		Player player = GameResources.Instance.GetNextUnassignedPlayer();
		player.networkHelper.isUnassigned = false;

		NetworkIdentity playerNetId = player.GetComponent<NetworkIdentity>();

		if (playerNetId.clientAuthorityOwner != null)
			playerNetId.RemoveClientAuthority(NetworkServer.localConnection);

		GameObject playerGO = player.gameObject;
		NetworkServer.AddPlayerForConnection(conn, playerGO);

		// explicitly grant client authority to host (fixes Smooth Sync not syncing server-caused forced movement)
		if (conn == NetworkServer.localConnection) {
			foreach (Player p in GameResources.Instance.GetAllPlayers()) {
				p.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
			}
		}

		NetworkServer.SetClientReady(conn);
	}

	IEnumerator WaitClientConnect(NetworkConnection conn) {
		while (!sceneManager.gameplayScenesInitialized) {
			yield return null;
		}

		ClientScene.Ready(conn);
		Debug.Log("Connected successfully to server, now to set up other stuff for the client...");
	}

	public List<GameObject> GetSpawnPrefabList() {
		return base.spawnPrefabs;
	}

}
