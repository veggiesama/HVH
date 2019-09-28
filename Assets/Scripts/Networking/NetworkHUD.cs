using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.Events;
using System.Linq;

public enum NetworkStates {
	MAIN_MENU, CONNECTING, LOADING, SELECTING_PLAYER, GAME
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
	public Observer localObserver;

	private NetworkStates currentState;

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

	private void Start() {
		EndState(NetworkStates.CONNECTING);
		EndState(NetworkStates.LOADING);
		EndState(NetworkStates.SELECTING_PLAYER);
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
				StartCoroutine( WaitForLoading() );
				break;
			
			case NetworkStates.SELECTING_PLAYER:
				chooseCharacterHudGO.SetActive(true);
				StartCoroutine( WaitForObserver() );
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

			case NetworkStates.SELECTING_PLAYER:
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
		SetState(NetworkStates.LOADING);
		yield return null;
	}

	public IEnumerator WaitForLoading() {
		while (!networkManager.sceneManager.gameplayScenesInitialized) {
			Debug.Log("Waiting for gameplay scenes to initialize");
			yield return null;
		}

		if (networkManager.isHost || !networkManager.isServer)
			SetState(NetworkStates.SELECTING_PLAYER);
		else
			SetState(NetworkStates.GAME);

	}

	public void SetLocalObserver(Observer obs) {
		this.localObserver = obs;
	}

	public IEnumerator WaitForObserver() {
		float timeout = 0f;
		float maxTime = 10f;
		float inc = 0.1f;

		while (NetworkClient.connection.identity == null && timeout < maxTime) {
			timeout += inc;
			yield return new WaitForSeconds(inc);
		}

		if (timeout < maxTime)
			BuildSelectingPlayerButtons();
		else
			SetState(NetworkStates.MAIN_MENU);
	}

	private void BuildSelectingPlayerButtons() {
		Observer observer = NetworkClient.connection.identity.GetComponent<Observer>();
		foreach (Player p in GameResources.Instance.GetAllPlayers()) {
			int pid = p.playerID;
			Button b = Instantiate(chooseCharacterButtonPrefab, chooseCharacterButtonContainer.transform).GetComponent<Button>();
			b.GetComponentInChildren<TMP_Text>().text = "Player " + pid;
			if (p.networkHelper.isUnassigned) {
				b.onClick.AddListener(delegate {
					observer.RequestPlayerID(pid);
				});
			}
			else {
				b.interactable = false;
			}
		}
	}
}
