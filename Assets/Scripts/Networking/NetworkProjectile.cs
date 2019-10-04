using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct NetworkProjectile {

	public int prefabIndex;
	public int bodyLocationInt;
	public int abilitySlotIndex;
	public Vector3 targetLocation;
	public uint targetNetId;

	public NetworkProjectile(int prefabIndex, int bodyLocationInt, int abilitySlotIndex, Vector3 targetLocation) {
		this.prefabIndex = prefabIndex;
		this.bodyLocationInt = bodyLocationInt;
		this.abilitySlotIndex = abilitySlotIndex;
		this.targetLocation = targetLocation;
		this.targetNetId = 0;
	}

	public NetworkProjectile(int prefabIndex, int bodyLocationInt, int abilitySlotIndex, uint targetNetId) {
		this.prefabIndex = prefabIndex;
		this.bodyLocationInt = bodyLocationInt;
		this.abilitySlotIndex = abilitySlotIndex;
		this.targetLocation = default;
		this.targetNetId = targetNetId;
	}

	public bool TargetsUnit() {
		return (targetNetId != 0); // TODO: this might be a bad assumption
	}

	public bool TargetsWorldspace() {
		return (targetLocation != default);
	}
}
