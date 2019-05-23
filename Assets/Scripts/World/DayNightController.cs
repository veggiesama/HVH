using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.LSky;
using Mirror;
using UnityEngine.Events;

public class DayNightController : MonoBehaviour {
	public LSky lsky;
	public LSkyTOD tod;

	[Range(0f, 24f)] public float startingTime;
	[Range(0f, 24f)] public float dayBegins;
	[Range(0f, 24f)] public float nightBegins;

	public float lengthOfDay;
	public float lengthOfNight;
	public float lengthOfTransition;

	private float updateEvery = 1.0f;
	private float transitionUpdateEvery = 0.02f;
	//private bool isNight;

	[Header("Readonly")]
	[SerializeField] private bool isDay = true;
	[SerializeField] private float currentTimer;
	public UnityEvent onStartDay;
	public UnityEvent onStartNight;

    public void Start() {
		if (Constants.StartDay)
			StartDay(true);
		else
			StartNight(true);
			
		StartCoroutine( SlowUpdate() ); // is Server check?
	}

    IEnumerator SlowUpdate() {

		while (true) {
			currentTimer -= updateEvery;
			isDay = lsky.IsDay;

			//Debug.Log("Time: " + GetTimeOfDay() + ", Timer: " + currentTimer +  ", isDay: " + isDay);
			GameRules.Instance.networkGameRules.CheckServerDayNightTransition(isDay, currentTimer);
			yield return new WaitForSeconds(updateEvery);
		}
    }

	public void StartDay(bool skipTransition = false) {
		isDay = true;
		currentTimer = lengthOfDay;
		if (!skipTransition)
			StartCoroutine( TransitionToDay() );
	}

	public void StartNight(bool skipTransition = false) {
		isDay = false;
		currentTimer = lengthOfNight;
		if (!skipTransition)
			StartCoroutine( TransitionToNight() );
	}

	public IEnumerator TransitionToDay() {
		//Debug.Log("Transitioning to day!");
		onStartDay.Invoke();
		float originalTimeline = GetTimeOfDay();
		for (float t = 0; t <= lengthOfTransition; t += transitionUpdateEvery) {
			SetTimeOfDay( Mathf.SmoothStep(originalTimeline, dayBegins, t / lengthOfTransition) );
			yield return new WaitForSeconds(transitionUpdateEvery);
		}
	}

	public IEnumerator TransitionToNight() {
		//Debug.Log("Transitioning to night!");
		onStartNight.Invoke();
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

	public bool IsDay() {
		return isDay;
	}

	public bool IsNight() {
		return !isDay;
	}

}
