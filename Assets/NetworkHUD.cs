using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkHUD : MonoBehaviour {

	public NetworkManagerHVH networkManager;
	public GameObject mainMenuState;
	public GameObject gameState;

	public Button startHostButton;
	public Button startClientButton;
	public Button startServerButton;
	public TMP_InputField ipField;

	public Button stopButton;
	public TMP_Text statusText;

	private enum NetworkStates {
		MAIN_MENU, CONNECTING, GAME
	}

	private void SetState(NetworkStates state) {

		switch (state) {
			case NetworkStates.MAIN_MENU:
				mainMenuState.SetActive(true);
				gameState.SetActive(false);
				break;

			case NetworkStates.CONNECTING:
				mainMenuState.SetActive(false);
				gameState.SetActive(true);
				statusText.text = "Connecting to: " + networkManager.networkAddress;
				StartCoroutine( WaitForConnect() );
				break;

			case NetworkStates.GAME:
				mainMenuState.SetActive(false);
				gameState.SetActive(true);

				statusText.text = "IP: " + networkManager.networkAddress + "\n" +
					//"Transport: " + Transport.activeTransport + "\n" +
					"isServer: " + networkManager.isServer + ", isHost:" + networkManager.isHost;
				break;
		}
	}

	private void Start() {
		SetState(NetworkStates.MAIN_MENU);
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

		SetState(NetworkStates.GAME);
		yield return null;
	}

}
