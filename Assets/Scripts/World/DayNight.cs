using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.LSky;
using Mirror;
using UnityEngine.Events;

public class DayNight : Singleton<DayNight> {

	public static DayNight Instance {
		get {
			return ((DayNight)mInstance);
		} set {
			mInstance = value;
		}
	}

	public DayNightSettings settings;
	public DayNightServer server;

	[Header("Readonly")]
	[HideInInspector] public UnityEvent onStartDay;
	[HideInInspector] public UnityEvent onStartNight;

	public void OnDisable() {
		onStartDay.RemoveAllListeners();
		onStartNight.RemoveAllListeners();
	}

	public void Initialize() {
		if (IsDay())
			StartDay(true);
		else
			StartNight(true);
	}

	public void StartDay(bool skipTransition = false) {
		if (!skipTransition)
			StartCoroutine( TransitionToDay() );
	}

	public void StartNight(bool skipTransition = false) {
		if (!skipTransition)
			StartCoroutine( TransitionToNight() );
	}

	public IEnumerator TransitionToDay() {
		//Debug.Log("Transitioning to day!");
		onStartDay.Invoke();
		float originalTimeline = GetTimeOfDay();
		for (float t = 0; t <= settings.transitionDuration; t += settings.transitionUpdateEvery) {
			SetTimeOfDay( Mathf.SmoothStep(originalTimeline, settings.dayBegins, t / settings.transitionDuration) );
			yield return new WaitForSeconds(settings.transitionUpdateEvery);
		}
	}

	public IEnumerator TransitionToNight() {
		//Debug.Log("Transitioning to night!");
		onStartNight.Invoke();
		float originalTimeline = GetTimeOfDay();
		for (float t = 0; t <= settings.transitionDuration; t += settings.transitionUpdateEvery) {
			SetTimeOfDay( Mathf.SmoothStep(originalTimeline, settings.nightBegins, t / settings.transitionDuration) );
			yield return new WaitForSeconds(settings.transitionUpdateEvery);
		}
	}

	// HELPERS

	public float GetTimeOfDay() {
		return settings.tod.timeline;
	}

	public void SetTimeOfDay(float newTime) {
		settings.tod.timeline = newTime;
	}

	public bool IsDay() {
		return server.IsDay();
	}

	public bool IsNight() {
		return server.IsNight();
	}

	public float GetTimeRemaining() {
		return server.GetTimeRemaining();
	}

}
