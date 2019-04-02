using UnityEngine;
using Mirror;

public class NetworkManagerHVH : NetworkManager {

	public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
	{
		Debug.Log("OnServerAddPlayer called");
		GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		NetworkServer.AddPlayerForConnection(conn, player);
	}

}
