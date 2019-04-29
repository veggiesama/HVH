using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour {
    public Slider castbar;
	[SerializeField] private Player localPlayer;

	public void SetLocalPlayer(Player player) {
		localPlayer = player;
	}

	public Player GetLocalPlayer() {
		return localPlayer;
	}
}
