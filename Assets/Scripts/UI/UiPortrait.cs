using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UiPortrait {
	public Slider slider;
	public Slider bigSlider;
	public RenderTexture renderTexture;
	
	[HideInInspector] public UiPortraitSlots slot;
	[HideInInspector] public UnitController unit;
	[HideInInspector] public UnityEventDamage onTakeDamage;
	[HideInInspector] public UnityEventDamage onTakeHealing;

	public void OnTakeDamage(float dmg) {
		GameplayCanvas.Instance.UpdateHealth(slot, unit.GetHealthPercentage());
	}

	public void OnTakeHealing(float heal) {
		GameplayCanvas.Instance.UpdateHealth(slot, unit.GetHealthPercentage());
	}
}
