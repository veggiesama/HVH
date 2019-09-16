using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.Events;

public enum NetworkStates {
	MAIN_MENU, CONNECTING, LOADING, CHOOSE_CHARACTER, GAME
}

public class ChooseCharacterMessage : MessageBase {
	public bool activate;
}

public class NetworkHUD : MonoBehaviour {

	public NetworkManagerHVH networkManager;
	public GameObject mainMenuHudGO;
	public GameObject gameHudGO;
	public GameObject chooseCharacterHudGO;
	public GameObject chooseCharacterButtonPrefab;
	public UnityEventChooseCharacter OnChooseCharacter;

	public Button startHostButton;
	public Button startClientButton;
	public Button startServerButton;
	public TMP_InputField ipField;

	public Button stopButton;
	public TMP_Text statusText;

	private NetworkStates currentState;

	private void Start() {
		EndState(NetworkStates.CONNECTING);
		EndState(NetworkStates.LOADING);
		EndState(NetworkStates.CHOOSE_CHARACTER);
		EndState(NetworkStates.GAME);
		StartState(NetworkStates.MAIN_MENU);

		Debug.Log("Registering handle for ChooseCharacterMessage");
		NetworkClient.RegisterHandler<ChooseCharacterMessage>(OnShowChooseCharacter);
	}

	public void OnShowChooseCharacter(NetworkConnection conn, ChooseCharacterMessage msg) {
		Debug.Log("OnShowChooseCharacter OnShowChooseCharacter OnShowChooseCharacter");
		SetState(NetworkStates.CHOOSE_CHARACTER);
	}

	public void SetState(NetworkStates state) {
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
				break;

			case NetworkStates.CHOOSE_CHARACTER:
				chooseCharacterHudGO.SetActive(true);
				
				foreach (Player p in GameResources.Instance.GetAllPlayers()) {

					Button b = Instantiate(chooseCharacterButtonPrefab, chooseCharacterHudGO.transform.GetChild(1).transform).GetComponent<Button>();
					b.GetComponentInChildren<TMP_Text>().text = "Player " + p.playerID;
					b.onClick.AddListener(delegate {
						//Debug.Log("Invoking p" + p.playerID);
						OnChooseCharacter.Invoke(p.playerID);
						SetState(NetworkStates.GAME);
					});
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

			case NetworkStates.CHOOSE_CHARACTER:
				chooseCharacterHudGO.SetActive(false);

				foreach (Button b in chooseCharacterHudGO.GetComponentsInChildren<Button>()) {
					b.onClick.RemoveAllListeners();
				}
				
				foreach (Transform child in chooseCharacterHudGO.transform.GetChild(1)) {
					Destroy(child.gameObject);
				}
				
				break;

			case NetworkStates.GAME:
				gameHudGO.SetActive(false);
				statusText.text = "";
				break;
		}

	}
	private void OnEnable() {
		startHostButton.onClick.AddListener(StartHost);
		startClientButton.onClick.AddListener(StartClient);
		startServerButton.onClick.AddListener(StartServer);
		stopButton.onClick.AddListener(StopServer);
	}

	private void OnDisable() {
		startHostButton.onClick.RemoveListener(StartHost);
		startClientButton.onClick.RemoveListener(StartClient);
		startServerButton.onClick.RemoveListener(StartServer);
		stopButton.onClick.RemoveListener(StopServer);
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

}
