using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.LSky;
using Mirror;

public class DayNightController : NetworkBehaviour {
	private LSky lsky;
	private LSkyTOD tod;

	[Range(0f, 24f)]
	public float startingTime;
	[Range(0f, 24f)]
	public float dayBegins;
	[Range(0f, 24f)]
	public float nightBegins;

	public float lengthOfDay;
	public float lengthOfNight;
	public float lengthOfTransition;

	[SyncVar] private bool isDay = true;
	[SyncVar] private float currentTimer;
	private float updateEvery = 1.0f;
	private float transitionUpdateEvery = 0.02f;
	//private bool isNight;

	public void Awake() {
		lsky = GetComponentInChildren<LSky>();
        tod = GetComponentInChildren<LSkyTOD>();
	}

	public override void OnStartClient() {
		if (!isDay) {
			SetTimeOfDay(nightBegins);
			currentTimer = lengthOfNight;
		}
		else {
			SetTimeOfDay(startingTime);
			currentTimer = lengthOfDay;
		}
	}

    public void Start() {
		StartCoroutine( SlowUpdate() ); // is Server check?
	}

    // Update is called once per frame
    IEnumerator SlowUpdate() {
		while (true) {
			currentTimer -= updateEvery;
			isDay = lsky.IsDay;
			//isNight = lsky.IsNight;

			Debug.Log("Time: " + GetTimeOfDay() + ", Timer: " + currentTimer +  ", isDay: " + isDay);

			if (isServer && currentTimer <= 0) {
				if (isDay)
					Rpc_StartNight();
				else
					Rpc_StartDay();
			}

			yield return new WaitForSeconds(updateEvery);
		}
    }

	[ClientRpc]
	public void Rpc_StartDay() {
		isDay = true;
		StartCoroutine( TransitionToDay() );
		currentTimer = lengthOfDay;
	}

	[ClientRpc]
	public void Rpc_StartNight() {
		isDay = false;
		StartCoroutine( TransitionToNight() );
		currentTimer = lengthOfNight;
	}

	IEnumerator TransitionToDay() {
		Debug.Log("Transitioning to day!");
		float originalTimeline = GetTimeOfDay();
		for (float t = 0; t <= lengthOfTransition; t += transitionUpdateEvery) {
			SetTimeOfDay( Mathf.SmoothStep(originalTimeline, dayBegins, t / lengthOfTransition) );
			yield return new WaitForSeconds(transitionUpdateEvery);
		}
	}

	IEnumerator TransitionToNight() {
		Debug.Log("Transitioning to night!");
		float originalTimeline = GetTimeOfDay();
		for (float t = 0; t <= lengthOfTransition; t += transitionUpdateEvery) {
			SetTimeOfDay( Mathf.SmoothStep(originalTimeline, nightBegins, t / lengthOfTransition) );
			yield return new WaitForSeconds(transitionUpdateEvery);
		}
	}

	// HELPERS

	public float GetTimeOfDay() {
		return tod.timeline;
	}

	public void SetTimeOfDay(float newTime) {
		tod.timeline = newTime;
	}

}
