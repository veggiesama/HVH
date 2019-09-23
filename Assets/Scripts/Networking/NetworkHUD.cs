using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.Events;
using System.Linq;

public enum NetworkStates {
	MAIN_MENU, CONNECTING, LOADING, REQUESTING_SELECTION, MAKING_SELECTION, GAME
}

// Request: client to server
// Reply: server to client

public class RequestCharacterSelectionMsg : MessageBase { }

public class ReplyCharacterSelectionMsg : MessageBase {
	public bool enabled;
	public bool[] playerAssignments;
	public int[] playerIDs;
	//private static readonly int playerCount = Constants.DwarvesTotal + Constants.MonstersTotal;

	public ReplyCharacterSelectionMsg() {}

	public ReplyCharacterSelectionMsg(bool enabled, List<Player> players) {
		playerAssignments = new bool[players.Count];
		playerIDs = new int[players.Count];
		for (int i = 0; i < players.Count; i++) {
			playerAssignments[i] = players[i].networkHelper.isUnassigned;
			playerIDs[i] = players[i].playerID;
		}
	}
	/*
	public override void Serialize(NetworkWriter writer) {
		writer.WriteBoolean(enabled);
		for (int i = 0; i < 8; i++) {
			writer.WriteBoolean(playerAssignments[i]);
		}
		for (int i = 0; i < 8; i++) {
			writer.WritePackedInt32(playerIDs[i]);
		}
	}

	public override void Deserialize(NetworkReader reader) {
		enabled = reader.ReadBoolean();
		for (int i = 0; i < 8; i++) {
			playerAssignments[i] = reader.ReadBoolean();
		}
		for (int i = 0; i < 8; i++) {
			playerIDs[i] = reader.ReadPackedInt32();
		}
		
	}*/
}

public class RequestPlayerIdMsg : MessageBase {
	public int playerID;
}

public class ReplyPlayerIdMsg : MessageBase { 
	public bool success;
	public GameObject playerGO;
}

public class NetworkHUD : MonoBehaviour {

	public NetworkManagerHVH networkManager;
	public GameObject mainMenuHudGO;
	public GameObject gameHudGO;
	public GameObject chooseCharacterHudGO;
	public GameObject chooseCharacterButtonContainer;
	public GameObject chooseCharacterButtonPrefab;

	public Button startHostButton;
	public Button startClientButton;
	public Button startServerButton;
	public TMP_InputField ipField;

	public Button stopButton;
	public TMP_Text statusText;

	private NetworkStates currentState;
	private bool[] playerAssignments;
	private int[] playerIDs;

	private void OnEnable() {
		startHostButton.onClick.AddListener(StartHost);
		startClientButton.onClick.AddListener(StartClient);
		startServerButton.onClick.AddListener(StartServer);
		stopButton.onClick.AddListener(StopServer);
		NetworkServer.RegisterHandler<RequestCharacterSelectionMsg>(ReplyCharacterSelection);
		NetworkServer.RegisterHandler<RequestPlayerIdMsg>(ReplyPlayerID);
		NetworkClient.RegisterHandler<ReplyCharacterSelectionMsg>(ReceiveCharacterSelection);
		NetworkClient.RegisterHandler<ReplyPlayerIdMsg>(ReceivePlayerID);
	}

	private void OnDisable() {
		startHostButton.onClick.RemoveListener(StartHost);
		startClientButton.onClick.RemoveListener(StartClient);
		startServerButton.onClick.RemoveListener(StartServer);
		stopButton.onClick.RemoveListener(StopServer);
		NetworkServer.UnregisterHandler<RequestCharacterSelectionMsg>();
		NetworkServer.UnregisterHandler<RequestPlayerIdMsg>();
		NetworkClient.UnregisterHandler<ReplyCharacterSelectionMsg>();
		NetworkClient.UnregisterHandler<ReplyPlayerIdMsg>();
	}

	//public void OnShowChooseCharacter(NetworkConnection conn, ChooseCharacterMessage msg) {
	//	Debug.Log("OnShowChooseCharacter");
	//	SetState(NetworkStates.MAKING_SELECTION);
	//}

	public void RequestCharacterSelection() {
		var msg = new RequestCharacterSelectionMsg();
		NetworkClient.Send(msg);
	}
	
	public void ReplyCharacterSelection(NetworkConnection conn, RequestCharacterSelectionMsg msg) {
		//NetworkServer.SetClientReady(conn);

		//List<Player> playerList = GameResources.Instance.GetAllPlayers();
		//var reply = new ReplyCharacterSelectionMsg() {
		//	enabled = true
		//};
		//reply.SetPlayerAssignments( GameResources.Instance.GetAllPlayers() );

		var reply = new ReplyCharacterSelectionMsg(true, GameResources.Instance.GetAllPlayers());

		NetworkServer.SendToClient(conn.connectionId, reply);
	}

	public void ReceiveCharacterSelection(NetworkConnection conn, ReplyCharacterSelectionMsg msg) {
		this.playerAssignments = msg.playerAssignments;
		this.playerIDs = msg.playerIDs;
		//NetworkServer.SetClientReady(conn);
		//ClientScene.PrepareToSpawnSceneObjects();
		SetState(NetworkStates.MAKING_SELECTION);
	}

	public void RequestPlayerID(int pid) {
		Debug.Log("Request player ID");
		var msg = new RequestPlayerIdMsg() {
			playerID = pid
		};
		NetworkClient.Send(msg);
	}

	public void ReplyPlayerID(NetworkConnection conn, RequestPlayerIdMsg msg) {
		var reply = new ReplyPlayerIdMsg() {
			success = true,
			playerGO = GameResources.Instance.GetPlayer(msg.playerID).gameObject
		};
		
		networkManager.AssignClient(conn, msg.playerID);
		NetworkServer.SendToClient(conn.connectionId, reply);
	}

	public void ReceivePlayerID(NetworkConnection conn, ReplyPlayerIdMsg msg) {
		Debug.Log("Receive player ID");
		if (msg.success) {
			msg.playerGO.GetComponent<Player>().Initialize();
			SetState(NetworkStates.GAME);
		}
	}

	private void Start() {
		EndState(NetworkStates.CONNECTING);
		EndState(NetworkStates.LOADING);
		EndState(NetworkStates.REQUESTING_SELECTION);
		EndState(NetworkStates.MAKING_SELECTION);
		EndState(NetworkStates.GAME);
		StartState(NetworkStates.MAIN_MENU);
	}

	public void SetState(NetworkStates state) {
		Debug.Log("SetState: " + System.Enum.GetName(typeof(NetworkStates), state));
		EndState(currentState);
		StartState(state);
	}

	private void StartState(NetworkStates state) {
		currentState = state;

		switch (state) {
			case NetworkStates.MAIN_MENU:
				mainMenuHudGO.SetActive(true);
				break;

			case NetworkStates.CONNECTING:
				gameHudGO.SetActive(true);
				statusText.text = "Connecting to: " + networkManager.networkAddress;
				StartCoroutine( WaitForConnect() );
				break;

			case NetworkStates.LOADING:
				gameHudGO.SetActive(true);
				statusText.text = "Loading scene"; 
				StartCoroutine( WaitForLoading() ); // networkManager.WaitServerReady() );
				break;
			
			case NetworkStates.REQUESTING_SELECTION:
				gameHudGO.SetActive(true);
				statusText.text = "Requesting...";

				if (networkManager.isHost || !networkManager.isServer)
					RequestCharacterSelection();
				else
					SetState(NetworkStates.GAME);

				break;

			case NetworkStates.MAKING_SELECTION:
				chooseCharacterHudGO.SetActive(true);

				for (int i = 0; i < playerIDs.Length; i++) {
					Button b = Instantiate(chooseCharacterButtonPrefab, chooseCharacterButtonContainer.transform).GetComponent<Button>();
					b.GetComponentInChildren<TMP_Text>().text = "Player " + playerIDs[i];
					if (playerAssignments[i]) {
						int pid = playerIDs[i];
						b.onClick.AddListener(delegate {
							RequestPlayerID(pid);
						});
					}
					else {
						b.interactable = false;
					}

				}

				break;

			case NetworkStates.GAME:
				gameHudGO.SetActive(true);

				statusText.text = "IP: " + networkManager.networkAddress + "\n" +
					//"Transport: " + Transport.activeTransport + "\n" +
					"isServer: " + networkManager.isServer + ", isHost:" + networkManager.isHost;
				break;
		}

	}

	private void EndState(NetworkStates state) {

		switch (state) {
			case NetworkStates.MAIN_MENU:
				mainMenuHudGO.SetActive(false);
				break;

			case NetworkStates.CONNECTING:
				gameHudGO.SetActive(false);
				statusText.text = "";
				break;

			case NetworkStates.LOADING:
				gameHudGO.SetActive(false);
				statusText.text = "";
				break;

			case NetworkStates.REQUESTING_SELECTION:
				gameHudGO.SetActive(false);
				statusText.text = "";
				break;

			case NetworkStates.MAKING_SELECTION:
				foreach (Button b in chooseCharacterButtonContainer.GetComponentsInChildren<Button>()) {
					b.onClick.RemoveAllListeners();
				}
				foreach (Transform child in chooseCharacterButtonContainer.transform) {
					Destroy(child.gameObject);
				}
				chooseCharacterHudGO.SetActive(false);
				break;

			case NetworkStates.GAME:
				gameHudGO.SetActive(false);
				statusText.text = "";
				break;
		}

	}

	void StartHost() {
		networkManager.StartHost();
		SetState(NetworkStates.CONNECTING);
	}

	void StartClient() {
		networkManager.StartClient();

		if (ipField.text == "")
			networkManager.networkAddress = "localhost";
		else
			networkManager.networkAddress = ipField.text;

		SetState(NetworkStates.CONNECTING);
	}

	void StartServer() {
		networkManager.StartServer();
		SetState(NetworkStates.CONNECTING);
	}

	void StopServer() {
		networkManager.StopHost();

		/*
		if (networkManager.isServer)
			networkManager.StopServer();
		else if (networkManager.isHost)
			networkManager.StopHost();
		else
			networkManager.StopClient();

		*/

		SetState(NetworkStates.MAIN_MENU);
	}

	IEnumerator WaitForConnect() {
		while (!NetworkClient.isConnected && !NetworkServer.active) {
			yield return new WaitForSeconds(0.1f);
			if (!NetworkClient.active) {
				SetState(NetworkStates.MAIN_MENU);
				yield return null;
			}
		}

		//SetState(NetworkStates.GAME);
		SetState(NetworkStates.LOADING);
		yield return null;
	}

	public IEnumerator WaitForLoading() {
		while (!networkManager.sceneManager.gameplayScenesInitialized) {
			Debug.Log("Waiting for gameplay scenes to initialize");
			yield return null;
		}

		SetState(NetworkStates.REQUESTING_SELECTION);
	}

}
