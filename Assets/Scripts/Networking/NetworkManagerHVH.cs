using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerHVH : NetworkManager {

    public override void OnStartHost()
    {
		Debug.Log("Host has started");
    }

    public override void OnStartServer()
    {
		Debug.Log("Server has started");
    }

	public override void OnStartClient()
	{
		Debug.Log("Client has started");
	}

	public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage) {
        Debug.Log("Client has requested to get his player added to the game");
	}

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
		NetworkServer.SetClientReady(conn);
		Debug.Log("Client is set to the ready state (ready to receive state updates): " + conn);

		Player player;
		player = GameRules.Instance.GetNextUnassignedPlayer();
		player.GetComponent<NetworkHelper>().isUnassigned = false;

		GameObject playerGO = player.gameObject;
		NetworkServer.AddPlayerForConnection(conn, playerGO);
    }

	public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
	{
        //if (player.gameObject != null)
        //    NetworkServer.Destroy(player.gameObject);
	}

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Server network error occurred: ???"); //+ (NetworkError)errorCode);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //NetworkServer.DestroyPlayersForConnection(conn);

        //if (conn.lastError != NetworkError.Ok)
        //{
        //    if (LogFilter.logError)
        //        Debug.LogError("ServerDisconnected due to error: " + conn.lastError);
        //}

        Debug.Log("A client disconnected from the server: " + conn);
    }

    public override void OnStopServer()
    {
        Debug.Log("Server has stopped");
    }

    public override void OnStopHost()
    {
        Debug.Log("Host has stopped");
    }

    // Client callbacks
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        Debug.Log("Connected successfully to server, now to set up other stuff for the client...");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();

        //if (conn.lastError != NetworkError.Ok)
        //{
        //    if (LogFilter.logError)
        //        Debug.LogError("ClientDisconnected due to error: " + conn.lastError);
        //}

        Debug.Log("Client disconnected from server: " + conn);
    }
	
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Client network error occurred: ???"); //+ (NetworkError)errorCode);
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("Server has set client to be not-ready (stop getting state updates)");
    }

    public override void OnStopClient() {
        Debug.Log("Client has stopped");
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        Debug.Log("Server triggered scene change and we've done the same, do any extra work here for the client...");
    }

	public List<GameObject> GetSpawnPrefabList() {
		return base.spawnPrefabs;
	}

}
