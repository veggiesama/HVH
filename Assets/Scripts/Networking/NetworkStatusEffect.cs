using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkStatusEffectSyncList: SyncList<NetworkStatusEffect> {}

public struct NetworkStatusEffect {
	public string statusName;
	public int type;
	public double startTime;
	public float duration;
	//public float remainingTime;

	public NetworkStatusEffect(string statusName, int type, double startTime, float duration) {
		this.statusName = statusName;
		this.type = type;
		this.startTime = startTime;
		this.duration = duration;
		//this.remainingTime = remainingTime;
	}

	public void UpdateTimers(double startTime, float duration) {
		this.startTime = startTime;
		this.duration = duration;
	}

}
