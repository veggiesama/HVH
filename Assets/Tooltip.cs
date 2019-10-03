using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Tooltip : Singleton<Tooltip> {

	// Singleton constructor
	public static Tooltip Instance {
		get {
			return ((Tooltip)mInstance);
		} set {
			mInstance = value;
		}
	}

	public TMP_Text nameText;
	public GameObject panel;

	void Awake() {
		panel.SetActive(false);
	}

	public void EnableTooltip(bool enable) {
		panel.SetActive(enable);
	}

	public void SetName(string name) {
		nameText.text = name;
	}

	private void Update() {
		if (panel.gameObject.activeInHierarchy)
			UpdatePosition();

		//RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(),
		//	Mouse.current.position.ReadValue(), cam, out Vector2 localPoint);
		//panel.localPosition = localPoint;
	}

	private void UpdatePosition() {
		Vector2 newPos = Mouse.current.position.ReadValue();
		newPos.x += 12;
		newPos.y += 12;

		panel.transform.position = newPos;
	}
}
