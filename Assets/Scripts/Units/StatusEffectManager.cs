using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour {
	[SerializeField] private List<StatusEffect> statusEffectList = new List<StatusEffect>();
	private List<StatusEffect> removalList = new List<StatusEffect>();
	private NetworkHelper networkHelper;
	public float networkRefreshEvery = 0.1f;
	private float currentTimer;

	private void Start() {
		networkHelper = GetComponentInParent<NetworkHelper>();
		currentTimer = networkRefreshEvery;
	}

	void Update() {
		// never update non-local players
		if (!networkHelper.HasControllableAuthority()) return;

		foreach (StatusEffect status in statusEffectList) {
		//	if (!status.applied)
		//		status.Apply();

			status.Update();
		}

		UpdateNetworkDurations();
		EmptyRemovalList();
    }

	void FixedUpdate() {
		// never update non-local players
		if (!networkHelper.HasControllableAuthority()) return;

		foreach (StatusEffect status in statusEffectList) {
			status.FixedUpdate();
		}
	}

	// add
	public void Add(StatusEffect status) {
		// never add status effects to non-local players
		if (!networkHelper.HasControllableAuthority()) return;

 		var existingStatus = GetStatusEffect(status.type);
		if(existingStatus)
			existingStatus.Stack(status);
		else {
			statusEffectList.Add(status);
			status.Apply(); // new
			networkHelper.TrackStatus(status);
		}
	}

	// remove (actually moves to removal list)
	public void Remove(string name) {
 		StatusEffect status = GetStatusEffect(name);
		if (status)
			status.End();
	}

	public void Remove(StatusEffectTypes type) {
 		StatusEffect status = GetStatusEffect(type);
		if (status)
			status.End();
	}

	public void RemoveAll() {
		foreach (StatusEffect status in statusEffectList)
			status.End();
		//statusEffectList.Clear();
	}

	// removal list (items deleted at the end of update)
	public void AddToRemovalList(StatusEffect status) {
		removalList.Add(status);
	}

	private void EmptyRemovalList() {
		foreach (StatusEffect status in removalList) {
			networkHelper.StopTrackingStatus(status.statusName);
			statusEffectList.Remove(status);
		}
		removalList.Clear();
	}

	// get
	public StatusEffect GetStatusEffect(string name) {
		foreach (StatusEffect status in statusEffectList) {
			if (status.statusName == name)
				return status;
		}
		return null;
	}

	public StatusEffect GetStatusEffect(StatusEffectTypes type) {
		foreach (StatusEffect status in statusEffectList) {
			if (status.type == type)
				return status;
		}
		return null;
	}


	// has
	public bool HasStatusEffect(string name) {
		if (!networkHelper.HasControllableAuthority())
			return networkHelper.HasStatusEffect(name);

		foreach (StatusEffect status in statusEffectList) {
			if (status.statusName == name)
				return true;
		}
		return false;
	}

	public bool HasStatusEffect(StatusEffectTypes type) {
		if (!networkHelper.HasControllableAuthority())
			return networkHelper.HasStatusEffect(type);

		foreach (StatusEffect status in statusEffectList) {
			if (status.type == type)
				return true;
		}
		return false;
	}

	public List<StatusEffect> GetStatusEffectList() {
		return statusEffectList;
	}

	public void UpdateNetworkDurations() {
		//Debug.Log("UpdateNetworkDurations");

		currentTimer -= Time.deltaTime;
		if (currentTimer < 0) {
			networkHelper.UpdateNetworkStatusSyncListDurations();
			currentTimer = networkRefreshEvery;
		}
	}

}
