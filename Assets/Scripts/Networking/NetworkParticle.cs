using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct NetworkParticle {

	public string prefabName;
	public uint unitNetId;
	public int bodyLocationInt;
	public Vector3 location;
	public Quaternion rotation;
	public double startTime;
	public float duration;
	public float radius;

	public NetworkParticle(string prefabName, uint unitNetId, int bodyLocationInt, float duration, float radius) {
		this.prefabName = prefabName;
		this.unitNetId = unitNetId;
		this.bodyLocationInt = bodyLocationInt;
		this.startTime = NetworkTime.time;
		this.duration = duration;
		this.radius = radius;

		this.location = default;
		this.rotation = default;
	}

	public NetworkParticle(string prefabName,  Vector3 location, Quaternion rotation, float duration, float radius) {
		this.prefabName = prefabName;
		this.location = location;
		this.rotation = rotation;
		this.startTime = NetworkTime.time;
		this.duration = duration;
		this.radius = radius;

		this.unitNetId = 0;
		this.bodyLocationInt = -1;
	}

	public bool TargetsUnit() {
		return (unitNetId != 0 && bodyLocationInt != -1);
	}

	public bool TargetsWorldspace() {
		return (location != default);
	}

	public bool OverridesScale() {
		return (radius != 0f);
	}

	public bool OverridesDuration() {
		return (duration != 0f);
	}

}
