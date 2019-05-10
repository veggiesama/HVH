using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour {
    public Slider castbar;
	[SerializeField] private Player localPlayer;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void SetLocalPlayer(Player player) {
		localPlayer = player;
	}

	public Player GetLocalPlayer() {
		return localPlayer;
	}
}
