using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.LSky;
using Mirror;
using UnityEngine.Events;

public class DayNightServer : NetworkBehaviour {
	public DayNightSettings settings;
	public DayNight client;

	[SyncVar] private bool isDay = true;
	[SyncVar] private float currentTimer;

	[Server]
    public void Initialize() {
		if (Constants.StartDay) {
			currentTimer = settings.dayDuration;
			isDay = true;
		}
		else {
			currentTimer = settings.nightDuration;
			isDay = false;
		}

		StartCoroutine( SlowUpdate() );
	}

    IEnumerator SlowUpdate() {
		while (true) {
			currentTimer -= settings.updateEvery;

			//Debug.Log("Time: " + GetTimeOfDay() + ", Timer: " + currentTimer +  ", isDay: " + isDay);
			CheckDayNightTransition();
			yield return new WaitForSeconds(settings.updateEvery);
		}
    }

	public void CheckDayNightTransition() {
		if (currentTimer <= 0) {
			if (isDay) {
				isDay = false;
				currentTimer = settings.nightDuration;
				Rpc_StartNight();
			}
			else {
				isDay = true;
				currentTimer = settings.dayDuration;
				Rpc_StartDay();
			}
		}
	}

	[ClientRpc]
	public void Rpc_StartDay() {
		if (client != null)
			client.StartDay();
	}

	[ClientRpc]
	public void Rpc_StartNight() {
		if (client != null)
			client.StartNight();
	}

	public bool IsDay() {
		return isDay;
	}

	public bool IsNight() {
		return !isDay;
	}

	public float GetTimeRemaining() {
		return currentTimer;
	}

}
