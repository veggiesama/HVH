using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

[System.Serializable]
public class UiStatusEffect : MonoBehaviour {
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
		SetTimerText(GetTimeRemaining());
		StartCoroutine(SlowUpdate(0.1f));
	}

	IEnumerator SlowUpdate(float updateEvery) {
		while (true) {
			yield return new WaitForSeconds(updateEvery);
			SetTimerText(GetTimeRemaining());

			if (GetTimeRemaining() <= 0)
				Destroy(this.gameObject);
		}
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

	void SetTimerText(float time) {

		TimeSpan t = TimeSpan.FromSeconds(time);
		/*
		string minutes = "";
		int mins = (int) (time / 60);
		if (mins >= 1)
			minutes = string.Format("{0:D2}m", mins);
	
		string seconds = string.Format("{0:D2}s", time % 60);
		timerText.text = minutes + seconds;*/

		timerText.text = string.Format("{0:D2}m:{1:D2}s", 
                t.Minutes, 
                t.Seconds);
	}


}
