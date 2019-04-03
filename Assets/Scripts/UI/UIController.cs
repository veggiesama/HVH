using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	private GameObject uiCanvas;
	private Slider castbar;

    // Start is called before the first frame update
    void Start()
    {
		uiCanvas = GameObject.Find("UI Canvas");
		castbar = uiCanvas.GetComponent<UICanvas>().castbar;

		// show each hud container
		for (int n = 0; n < uiCanvas.transform.childCount; n++) {
			uiCanvas.transform.GetChild(n).gameObject.SetActive(true);
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
