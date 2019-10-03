using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[System.Serializable]
public class UiPortrait {
	public Slider slider;
	public Slider bigSlider;
	public RenderTexture renderTexture;
	public GameObject statusEffectContainer;
	
	[HideInInspector] public UiPortraitSlots slot;
	[HideInInspector] public UnitController unit;
	[HideInInspector] public UnityEventDamage onTakeDamage;
	[HideInInspector] public UnityEventDamage onTakeHealing;

	public void OnTakeDamage(float dmg) {
		if (unit != null)
			UpdateHealth(unit.GetHealthPercentage());
	}

	public void OnTakeHealing(float heal) {
		if (unit != null)
			UpdateHealth(unit.GetHealthPercentage());
	}

	public void Initialize(UiPortraitSlots slot) {
		this.slot = slot;

		if (IsTargetingPanel()) {
			EnableSliders(false);
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// SLIDERS
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void UpdateHealth(float percentage) {
		if (slider != null)
			slider.value = Mathf.Clamp01(percentage);
		if (bigSlider != null)
			bigSlider.value = Mathf.Clamp01(percentage);
	}

	private void EnableSliders(bool enable) {
		if (slider != null)
			slider.gameObject.SetActive(enable);
		if (bigSlider != null)
			bigSlider.gameObject.SetActive(enable);
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// STATUS EFFECTS
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void AddStatusEffect(NetworkStatusEffect nse) {
		GameObject panel = GameObject.Instantiate(GameplayCanvas.Instance.statusEffectPrefab, statusEffectContainer.transform);
		UiStatusEffect uiStatusEffect = panel.GetComponent<UiStatusEffect>();
		uiStatusEffect.Initialize(nse);
	}

	public void RemoveStatusEffect(NetworkStatusEffect nse) {
		var transforms = statusEffectContainer.GetComponentsInChildren<Transform>();
		for (int i = 0; i < transforms.Length; i++) {
			if (transforms[i] == statusEffectContainer.transform) continue; // don't include the container itself

			var se = transforms[i].GetComponent<UiStatusEffect>();
			if (se != null && nse.statusName == se.GetStatusName()) {
				GameObject.Destroy(transforms[i].gameObject);
				break;
			}
		}
	}

	public void UpdateStatusEffect(NetworkStatusEffect nse) {
		//Debug.Log("NSE: Update");

		var transforms = statusEffectContainer.GetComponentsInChildren<Transform>();
		for (int i = 0; i < transforms.Length; i++) {
			if (transforms[i] == statusEffectContainer.transform) continue; // don't include the container itself

			var se = transforms[i].GetComponent<UiStatusEffect>();
			if (se != null && nse.statusName == se.GetStatusName()) {
				se.UpdateTimers(nse.startTime, nse.duration);
				break;
			}
		}
	}

	public void ClearStatusEffects() {
		var transforms = statusEffectContainer.GetComponentsInChildren<Transform>();

		foreach (Transform t in transforms) {
			if (t == statusEffectContainer.transform) continue;
			GameObject.Destroy(t.gameObject);
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REGISTRATION
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void RegisterPortrait(UnitController target) {
		SetPortraitRegistration(target, true);
	}

	public void UnregisterPortrait(UnitController target) {
		SetPortraitRegistration(target, false);
	}

	private void SetPortraitRegistration(UnitController target, bool enabled) {
		if (enabled) {
			SetPortraitCamera(target);
			target.networkHelper.networkStatusEffects.Callback += StatusEffectCallback;
			target.networkHelper.onUpdateNetworkStatusEffect.AddListener(UpdateStatusEffect);

			foreach (NetworkStatusEffect nse in target.networkHelper.networkStatusEffects) {
				AddStatusEffect(nse);
			}
		}

		else {
			SetPortraitCamera(null);
			target.networkHelper.networkStatusEffects.Callback -= StatusEffectCallback;
			ClearStatusEffects();
		}

	}

	private void StatusEffectCallback(NetworkStatusEffectSyncList.Operation op, int index, NetworkStatusEffect nse) {
		//Debug.Log("NSE: " + System.Enum.GetName(typeof(NetworkStatusEffectSyncList.Operation), op));

		switch (op) {
			case NetworkStatusEffectSyncList.Operation.OP_ADD:
				AddStatusEffect(nse);
				break;
			case NetworkStatusEffectSyncList.Operation.OP_CLEAR:
				ClearStatusEffects();
				break;
			case NetworkStatusEffectSyncList.Operation.OP_INSERT:
				AddStatusEffect(nse);
				break;
			case NetworkStatusEffectSyncList.Operation.OP_REMOVE:
				RemoveStatusEffect(nse);
				break;
			case NetworkStatusEffectSyncList.Operation.OP_REMOVEAT:
				RemoveStatusEffect(nse);
				break;
			case NetworkStatusEffectSyncList.Operation.OP_SET:
				//UpdateStatusEffect(nse);
				break;
			case NetworkStatusEffectSyncList.Operation.OP_DIRTY:
				//UpdateStatusEffect(nse);
				break;
		}

	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PORTRAIT CAMERA
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void SetPortraitCamera(UnitController newUnit) {
		// null the current camera
		if (unit != null) {
			Camera activeCam = GetActiveCameraForSlot(unit);		
			
			activeCam.enabled = false;
			activeCam.targetTexture.Release();

			unit.onTakeDamage.RemoveListener(OnTakeDamage); // TODO: remove listeners on disable?
			unit.onTakeHealing.RemoveListener(OnTakeHealing);
		}
		else {
			EnableSliders(true);
		}

		this.unit = newUnit;

		if (newUnit != null) {
			Camera activeCam = GetActiveCameraForSlot(newUnit);

			activeCam.targetTexture = renderTexture;
			activeCam.enabled = true;

			UpdateHealth(newUnit.GetHealthPercentage());
			newUnit.onTakeDamage.AddListener(OnTakeDamage);
			newUnit.onTakeHealing.AddListener(OnTakeHealing);
		}
		else {
			EnableSliders(false);
		}
	}

	private Camera GetActiveCameraForSlot(UnitController unit) {
		if (IsTargetingPanel())
			return unit.body.targetCam;
		else
			return unit.body.allyCam;
	}

	private bool IsTargetingPanel() {
		return (slot == UiPortraitSlots.ALLY_TARGET || slot == UiPortraitSlots.ENEMY_TARGET);
	}

}
