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

	[SyncVar] private float currentTimeOfDay;
	[SyncVar] private float currentTimer;
	private float updateEvery = 1.0f;
	private float transitionUpdateEvery = 0.02f;
	private bool isDay;
	//private bool isNight;

    // Start is called before the first frame update
    void Start() {
		lsky = GetComponentInChildren<LSky>();
        tod = GetComponentInChildren<LSkyTOD>();
		tod.timeline = startingTime;
		currentTimer = lengthOfDay;

		StartCoroutine( SlowUpdate() ); // is Server check?
    }

    // Update is called once per frame
    IEnumerator SlowUpdate() {
		while (true) {
			currentTimer -= updateEvery;
			currentTimeOfDay = tod.timeline;
			isDay = lsky.IsDay;
			//isNight = lsky.IsNight;

			Debug.Log("Time: " + currentTimeOfDay + ", Timer: " + currentTimer +  ", isDay: " + isDay);

			if (currentTimer <= 0) {
				if (isDay) {
					isDay = false;
					//isNight = true;
					StartCoroutine( TransitionToNight() );
					currentTimer = lengthOfNight;
				}
				else {
					isDay = true;
					//isNight = false;
					StartCoroutine( TransitionToDay() );
					currentTimer = lengthOfDay;
				}
			}

			yield return new WaitForSeconds(updateEvery);
		}
    }

	IEnumerator TransitionToDay() {
		Debug.Log("Transitioning to day!");
		float originalTimeline = tod.timeline;
		for (float t = 0; t <= lengthOfTransition; t += transitionUpdateEvery) {
			tod.timeline = Mathf.SmoothStep(originalTimeline, dayBegins, t / lengthOfTransition);
			yield return new WaitForSeconds(transitionUpdateEvery);
		}
	}

	IEnumerator TransitionToNight() {
		Debug.Log("Transitioning to night!");
		float originalTimeline = tod.timeline;
		for (float t = 0; t <= lengthOfTransition; t += transitionUpdateEvery) {
			tod.timeline = Mathf.SmoothStep(originalTimeline, nightBegins, t / lengthOfTransition);
			yield return new WaitForSeconds(transitionUpdateEvery);
		}
	}

}
