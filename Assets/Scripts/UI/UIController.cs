using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	private Slider castbar;

    // Start is called before the first frame update
    void Start()
    {
		castbar = ReferenceManager.Instance.Castbar;
		ShowHUD();
	}

	void ShowHUD() {
		foreach (GameObject obj in ReferenceManager.Instance.UICanvasList) {
			obj.SetActive(true);
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
