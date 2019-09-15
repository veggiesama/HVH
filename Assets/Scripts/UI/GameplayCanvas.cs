using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.Events;

public class GameplayCanvas : Singleton<GameplayCanvas> {

	// Singleton constructor
	public static GameplayCanvas Instance {
		get {
			return ((GameplayCanvas)mInstance);
		} set {
			mInstance = value;
		}
	}

	[System.Serializable]
	public class UiPortaitDictionary : SerializableDictionaryBase<UiPortraitSlots, UiPortrait> {}
	public UiPortaitDictionary uiPortraits;

	[Header("Self")]
    public Slider castbar;
	private AbilityButtonInfo[] abilityButtons;

	[Header("Debug")]
	public DebugMenu debugMenu;

    // Start is called before the first frame update
    void Awake() {

		// show each hud container
		for (int n = 0; n < transform.childCount; n++) {
			transform.GetChild(n).gameObject.SetActive(true);
		}

		abilityButtons = GetComponentsInChildren<AbilityButtonInfo>();
		//ResetButtons();

		EnableSliders(uiPortraits[UiPortraitSlots.ALLY_TARGET], false);
		EnableSliders(uiPortraits[UiPortraitSlots.ENEMY_TARGET], false);
	}

	// Start() not getting run because LocalPlayerOnly

	public void ResetButtons() {
		foreach (AbilityButtonInfo button in abilityButtons) {
			button.Initialize();
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

	public void UpdateHealth(UiPortraitSlots slot, float percentage) {
		if (uiPortraits[slot].slider != null)
			uiPortraits[slot].slider.value = Mathf.Clamp01(percentage);
		if (uiPortraits[slot].bigSlider != null)
			uiPortraits[slot].bigSlider.value = Mathf.Clamp01(percentage);
	}

	public void SetPortraitCamera(UiPortraitSlots slot, UnitController unit) {
		UiPortrait port = uiPortraits[slot];

		// null the current camera
		if (port.unit != null) {
			Camera activeCam = GetActiveCameraForSlot(slot, port.unit);		
			
			activeCam.enabled = false;
			activeCam.targetTexture.Release();

			port.unit.onTakeDamage.RemoveListener(port.OnTakeDamage); // TODO: remove listeners on disable?
			port.unit.onTakeHealing.RemoveListener(port.OnTakeHealing);
		}
		else {
			EnableSliders(port, true);
		}

		port.slot = slot;
		port.unit = unit;

		if (unit != null) {
			Camera activeCam = GetActiveCameraForSlot(slot, port.unit);

			activeCam.targetTexture = port.renderTexture;
			activeCam.enabled = true;

			UpdateHealth(slot, unit.GetHealthPercentage());
			unit.onTakeDamage.AddListener(port.OnTakeDamage);
			unit.onTakeHealing.AddListener(port.OnTakeHealing);
		}
		else {
			EnableSliders(port, false);
		}
	}

	private void EnableSliders(UiPortrait port, bool enable) {
		if (port.slider != null)
			port.slider.gameObject.SetActive(enable);
		if (port.bigSlider != null)
			port.bigSlider.gameObject.SetActive(enable);
	}

	private bool IsTargetingPanel(UiPortraitSlots slot) {
		return (slot == UiPortraitSlots.ALLY_TARGET || slot == UiPortraitSlots.ENEMY_TARGET);
	}

	private Camera GetActiveCameraForSlot(UiPortraitSlots slot, UnitController unit) {
		if (IsTargetingPanel(slot))
			return unit.body.targetCam;
		else
			return unit.body.allyCam;
	}

}