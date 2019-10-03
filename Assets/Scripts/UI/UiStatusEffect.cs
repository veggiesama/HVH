using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Mirror;

[System.Serializable]
public class UiStatusEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private string statusName;
	private TMP_Text timerText;
	private Image icon;
	private double startTime;
	private float duration;
	//private Ability ability;
	private StatusEffect statusEffect;


	void Awake() {
		timerText = GetComponentInChildren<TMP_Text>();
		icon = GetComponentInChildren<Image>();
	}

	public void Initialize(NetworkStatusEffect nse) {
		duration = nse.duration;
		startTime = nse.startTime;
		statusName = nse.statusName;

		SetIcon(nse.statusName);
		SetTimerText();
		StartCoroutine( SlowUpdate(0.1f) );
	}

	IEnumerator SlowUpdate(float updateEvery) {
		while (true) {
			yield return new WaitForSeconds(updateEvery);
			SetTimerText();
			if (HasDuration() && GetTimeRemaining() <= 0)
				Destroy(this.gameObject);
		}
	}

	public void UpdateTimers(double startTime, float duration) {
		this.startTime = startTime;
		this.duration = duration;
	}

	bool HasDuration() {
		return (duration != 0f);
	}

	float GetTimeRemaining() {
		return (float)((startTime + duration) - NetworkTime.time);
	}

	public string GetStatusName() {
		return statusName;
	}

	void SetIcon(string statusName) {
		statusEffect = ResourceLibrary.Instance.statusEffectDictionary[statusName];
		icon.sprite = statusEffect.icon;
	}

	void SetTimerText() {

		//TimeSpan t = TimeSpan.FromSeconds(time);
		//timerText.text = string.Format("{0:D2}m:{1:D2}s", 
        //        t.Minutes, 
        //        t.Seconds);

		if (!HasDuration()) {
			timerText.text = "";
		}

		else {
			float time = GetTimeRemaining();

			string minutes = "";
			int mins = (int) (time / 60);
			if (mins >= 1)
				minutes = string.Format("{0:D}m", mins);
	
			int secs = (int) (time % 60);
			string seconds = string.Format("{0:D}s", secs);
			timerText.text = minutes + seconds;
		}
	}

	//TODO: fix bug: when status effect is destroyed, tooltip stays enabled screen until next EnableTooltip
	public void OnPointerEnter(PointerEventData eventData) {
		Tooltip.Instance.EnableTooltip(true);
		Tooltip.Instance.SetName(statusName);
	}

	public void OnPointerExit(PointerEventData eventData) {
		Tooltip.Instance.EnableTooltip(false);
	}


}
