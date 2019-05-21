using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.LSky;
using Mirror;

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
	private bool gameplayStarted = false;

    public void Start() {
		NetworkSceneManager.Instance.OnGameplayScenesInitializedEventHandler += OnGameplayScenesInitialized;
		currentTimer = lengthOfDay;
		StartCoroutine( SlowUpdate() ); // is Server check?
	}

	private void OnGameplayScenesInitialized() {
		gameplayStarted = true;
	}	

    // Update is called once per frame
    IEnumerator SlowUpdate() {
		while (!gameplayStarted) {
			yield return new WaitForSeconds(updateEvery);
		}

		while (true) {
			currentTimer -= updateEvery;
			isDay = lsky.IsDay;

			//Debug.Log("Time: " + GetTimeOfDay() + ", Timer: " + currentTimer +  ", isDay: " + isDay);
			GameRules.Instance.networkGameRules.CheckServerDayNightTransition(isDay, currentTimer);
			yield return new WaitForSeconds(updateEvery);
		}
    }

	public void StartDay() {
		isDay = true;
		StartCoroutine( TransitionToDay() );
		currentTimer = lengthOfDay;
	}

	public void StartNight() {
		isDay = false;
		StartCoroutine( TransitionToNight() );
		currentTimer = lengthOfNight;
	}

	public IEnumerator TransitionToDay() {
		//Debug.Log("Transitioning to day!");
		float originalTimeline = GetTimeOfDay();
		for (float t = 0; t <= lengthOfTransition; t += transitionUpdateEvery) {
			SetTimeOfDay( Mathf.SmoothStep(originalTimeline, dayBegins, t / lengthOfTransition) );
			yield return new WaitForSeconds(transitionUpdateEvery);
		}
	}

	public IEnumerator TransitionToNight() {
		//Debug.Log("Transitioning to night!");
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
