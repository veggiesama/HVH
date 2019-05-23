using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	private GameObject gameplayCanvas;
	private Slider castbar;

    // Start is called before the first frame update
    void Start() {
		gameplayCanvas = GameObject.Find("GameplayCanvas");
		castbar = gameplayCanvas.GetComponent<GameplayCanvas>().castbar;

		// show each hud container
		for (int n = 0; n < gameplayCanvas.transform.childCount; n++) {
			gameplayCanvas.transform.GetChild(n).gameObject.SetActive(true);
		}
	}

	public void EnableCastbar(bool enable) {
		castbar.gameObject.SetActive(enable);
		//Debug.Log("Castbar " + enable);

		if (enable) {
			castbar.value = 1;
		}
	}

	public void UpdateCastbar(float percentage) {
		castbar.value = percentage;
	}
}
