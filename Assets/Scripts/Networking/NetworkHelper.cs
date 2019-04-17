﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkHelper : NetworkBehaviour {

	Player player;
	UnitController unit;
	[SyncVar] public float currentHealth;
	public bool isDisconnected = false;
	public bool isUnassigned = true;
	
	public class NetworkStatusEffectSyncList: SyncList<NetworkStatusEffect> {}
	public NetworkStatusEffectSyncList networkStatusEffects = new NetworkStatusEffectSyncList();

	public struct NetworkStatusEffect {
		public string statusName;
		public int type;
		public float duration;
		public float remainingTime;

		public NetworkStatusEffect(string statusName, int type, float duration, float remainingTime) {
			this.statusName = statusName;
			this.type = type;
			this.duration = duration;
			this.remainingTime = remainingTime;
		}
	}

	public void Start() {
		player = GetComponent<Player>();
		unit = player.unit;
		currentHealth = unit.unitInfo.maxHealth;
	}


	// TODO: add every network status effect from server (with current duration), remove status effects from server
	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		isUnassigned = false;
		isDisconnected = false;
	}

	// TODO: remove every status effect from client, add effects to server representation
	private void OnPlayerDisconnected(NetworkIdentity id){
		isDisconnected = true;
	}

	public bool HasControllableAuthority() {	
		if (hasAuthority) return true; // you're the client or host GameObject owner
		if (isServer && (isUnassigned || isDisconnected)) return true; // you're a server, and GameObject is unassigned or player left
		return false;
	}

	// TREE DESTRUCTION

	public void DestroyTree(Tree tree, Vector3 destroyedFromDirection = default, float delay = 0f) {
		int treeSiblingIndex = tree.GetSiblingIndex();
		GameObject treeHandlerGO = tree.GetTreeHandlerGO();
		Cmd_DestroyTree(treeHandlerGO, treeSiblingIndex, destroyedFromDirection, delay);
	}

	[Command]
	private void Cmd_DestroyTree(GameObject treeHandlerGO, int treeSiblingIndex, Vector3 destroyedFromDirection, float delay) {
		treeHandlerGO.GetComponent<TreeHandler>().Rpc_DestroyTree(treeSiblingIndex, destroyedFromDirection, delay);	
	}

	// PROJECTILES

	public void CreateProjectile(Ability ability, Order castOrder) {
		GameObject projectilePrefab = ability.projectilePrefab;
		Vector3 targetLocation = castOrder.targetLocation;

		int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(projectilePrefab);
		int abilitySlotIndex = PackAbility(ability); // (int) unit.GetAbilitySlot(ability);
	   	Cmd_CreateProjectile(prefabIndex, abilitySlotIndex, targetLocation);
	}

	[Command]
	private void Cmd_CreateProjectile(int prefabIndex, int abilitySlotIndex, Vector3 targetLocation) {
		GameObject prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

		GameObject projectileObject = Instantiate(prefab,
			unit.attackInfo.spawnerObject.transform.position,
			unit.attackInfo.spawnerObject.transform.rotation);
			//unit.transform); // bad for NetworkIdentity to be spawned as child?
		NetworkServer.SpawnWithClientAuthority(projectileObject, this.gameObject); // connectionToClient);
		TargetRpc_InitializeProjectile(connectionToClient, projectileObject, abilitySlotIndex, targetLocation);
	}

	[TargetRpc]
	private void TargetRpc_InitializeProjectile(NetworkConnection conn, GameObject projectileObject, int abilitySlotIndex, Vector3 targetLocation) {
		Ability ability = UnpackAbility(unit, abilitySlotIndex);

		switch (ability.projectileBehaviour) {
			case ProjectileBehaviourTypes.BULLET:
				BulletBehaviour bullet = projectileObject.GetComponent<BulletBehaviour>();
				bullet.Initialize(ability, targetLocation);
				break;
			case ProjectileBehaviourTypes.CONE:
				ConeBehaviour cone = projectileObject.GetComponent<ConeBehaviour>();
				cone.Initialize(ability, targetLocation);
				break;
			case ProjectileBehaviourTypes.GRENADE:
				GrenadeBehaviour grenade = projectileObject.GetComponent<GrenadeBehaviour>();
				grenade.Initialize(ability, targetLocation);
				break;
			case ProjectileBehaviourTypes.HOMING:
				Debug.Log("Homing type projectile (unimplemented)");
				break;
			default:
				Debug.Log("Trying to spawn unknown projectile type.");
				break;
		}

	}

	public void DestroyProjectile(GameObject projectileObject) {
		Cmd_DestroyProjectile(projectileObject);
	}

	[Command]
	private void Cmd_DestroyProjectile(GameObject projectileObject) {
		if (projectileObject != null)
			NetworkServer.Destroy(projectileObject);
	}

	// DAMAGE

	public void DealDamageTo(UnitController targetUnit, float dmg) {
		NetworkIdentity targetNetIdentity = targetUnit.networkHelper.netIdentity;
		Cmd_DealDamageTo(targetNetIdentity, dmg);
	}

	[Command]
	private void Cmd_DealDamageTo(NetworkIdentity targetNetIdentity, float dmg) {
		UnitController targetUnit = targetNetIdentity.GetComponent<Player>().unit;
		if (targetUnit.HasStatusEffect(StatusEffectTypes.INVULNERABLE)) return;

		targetUnit.networkHelper.currentHealth -= dmg;
		if (targetUnit.networkHelper.currentHealth < 0) {
			NetworkConnection conn = targetNetIdentity.connectionToClient; // connectionToServer?
			if (conn != null)
				targetUnit.networkHelper.TargetRpc_ApplyOnDeathStatusEffect(conn);
			else
				targetUnit.ApplyStatusEffect(targetUnit.unitInfo.onDeathStatusEffect, null);
		}
	}

	[TargetRpc]
	private void TargetRpc_ApplyOnDeathStatusEffect(NetworkConnection conn) {
		unit.ApplyStatusEffect(unit.unitInfo.onDeathStatusEffect, null);
	}

	// STATUS EFFECTS
	public bool HasStatusEffect(string statusName) {
		foreach (NetworkStatusEffect status in networkStatusEffects) {
			if (status.statusName == statusName)
				return true;
		}
		return false;
	}

	public bool HasStatusEffect(StatusEffectTypes type) {
		foreach (NetworkStatusEffect status in networkStatusEffects) {
			if (status.type == (int)type)
				return true;
		}
		return false;
	}

	public void TrackStatus(StatusEffect status) {
		NetworkStatusEffect nse = new NetworkStatusEffect(status.statusName, (int)status.type, status.duration, status.remainingTime);
		Cmd_TrackStatus(nse);
	}

	[Command]
	private void Cmd_TrackStatus(NetworkStatusEffect nse) {
		networkStatusEffects.Add(nse);
	}
	
	public void StopTrackingStatus(string statusName) {
		Cmd_StopTrackingStatus(statusName);
	}

	[Command]
	private void Cmd_StopTrackingStatus(string statusName) {
		foreach (NetworkStatusEffect status in networkStatusEffects) {
			if (status.statusName == statusName) {
				networkStatusEffects.Remove(status);
				return;
			}
		}
	}

	public void UpdateNetworkStatusSyncListDurations() {
		for (int i = 0; i < networkStatusEffects.Count; i++) {
			NetworkStatusEffect nse = networkStatusEffects[i];
			StatusEffectTypes nseType = (StatusEffectTypes) nse.type;
		
			if (unit.HasStatusEffect(nseType))
				nse.duration = unit.GetStatusEffectDuration(nseType);
			//Debug.Log("(Network) " + nse.statusName + ": " + nse.duration);
		}
	}

	public void ApplyStatusEffectTo(StatusEffect status) {
		ApplyStatusEffectTo(unit, status, null); // self
	}
	
	public void ApplyStatusEffectTo(UnitController targetUnit, StatusEffect status) {
		ApplyStatusEffectTo(targetUnit, status, null); // no associated ability
	}

	public void ApplyStatusEffectTo(UnitController targetUnit, StatusEffect status, Ability ability) {
		string statusName = status.statusName;

		GameObject inflictedGO = targetUnit.player.gameObject;
		if (ability != null) {
			int abilitySlotIndex = PackAbility(ability);
			GameObject inflictorGO = ability.caster.player.gameObject;
			Cmd_ApplyStatusEffectTo(inflictedGO, statusName, abilitySlotIndex, inflictorGO);
		}
		else
			Cmd_ApplyStatusEffectTo(inflictedGO, statusName, -1, null);
	}

	[Command]
	private void Cmd_ApplyStatusEffectTo(GameObject inflictedGO, string statusName, int abilitySlotIndex, GameObject inflictorGO) {
		NetworkConnection conn = inflictedGO.GetComponent<Player>().connectionToClient;

		NetworkHelper helper = inflictedGO.GetComponent<NetworkHelper>();
		if (conn != null)
			helper.TargetRpc_ApplyStatusEffectTo(conn, statusName, abilitySlotIndex, inflictorGO); // client
		else {
			helper.ApplyStatusEffectToLocalUnit(statusName, abilitySlotIndex, inflictorGO); // on server
		}
	}

	[TargetRpc]
	private void TargetRpc_ApplyStatusEffectTo(NetworkConnection conn, string statusName, int abilitySlotIndex, GameObject inflictorGO) {
		ApplyStatusEffectToLocalUnit(statusName, abilitySlotIndex, inflictorGO);
	}

	private void ApplyStatusEffectToLocalUnit(string statusName, int abilitySlotIndex, GameObject inflictorGO) {
		if (ResourceLibrary.Instance.statusEffectDictionary.TryGetValue(statusName, out StatusEffect statusEffect)) {
			if (!(abilitySlotIndex == -1 || inflictorGO == null)) {
				UnitController inflictor = inflictorGO.GetComponentInChildren<UnitController>();
				Ability ability = UnpackAbility(inflictor, abilitySlotIndex);
				unit.ApplyStatusEffect(statusEffect, ability);
			}
			else {
				Debug.Log("Invalid abilitySlotIndex or null inflictor, but applying status effect anyway."); // not a bug
				unit.ApplyStatusEffect(statusEffect, null);
			}
		}
		else
			Debug.Log(statusName + " status not found.");
	}

	// DEATH AND RESPAWN

	public void Die(Vector3 killFromDirection) {
		Cmd_Die(killFromDirection);
	}

	[Command]
	private void Cmd_Die(Vector3 killFromDirection) {
		Rpc_Die(killFromDirection);
	}

	[ClientRpc]
	private void Rpc_Die(Vector3 killFromDirection) {
		unit.DetachFromNav();
		unit.EnableNav(false);
		unit.body.PerformDeath(killFromDirection);
	}

	public void Respawn() {
		Cmd_Respawn();
	}

	[Command]
	private void Cmd_Respawn() {
		Transform spawnLoc = GameController.GetRandomSpawnPoint();
		currentHealth = unit.unitInfo.maxHealth;
		Rpc_Respawn(spawnLoc.position, spawnLoc.rotation);
	}

	[ClientRpc]
	private void Rpc_Respawn(Vector3 position, Quaternion rotation) {
		unit.body.transform.SetPositionAndRotation(position, rotation);
		unit.body.ResetBody();
		unit.EnableNav(true);
		unit.AttachToNav();
	}

	// KNOCKBACK
	public void ApplyKnockbackTo(UnitController targetUnit, Vector3 velocityVector, Ability ability) {
		GameObject targetPlayerGO = targetUnit.player.gameObject;
		GameObject inflictorGO = player.gameObject;
		int abilitySlotIndex = PackAbility(ability);
		Cmd_ApplyKnockbackTo(targetPlayerGO, velocityVector, inflictorGO, abilitySlotIndex);
	}

	[Command]
	private void Cmd_ApplyKnockbackTo(GameObject targetPlayerGO, Vector3 velocityVector, GameObject inflictorGO, int abilitySlotIndex) {
		NetworkConnection conn = targetPlayerGO.GetComponent<NetworkIdentity>().connectionToClient;

		NetworkHelper helper = targetPlayerGO.GetComponent<NetworkHelper>();
		if (conn != null)
			helper.TargetRpc_ApplyKnockbackTo(conn, velocityVector, inflictorGO, abilitySlotIndex);
		else
			helper.ApplyKnockbackToLocalUnit(velocityVector, inflictorGO, abilitySlotIndex);
	}

	[TargetRpc]
	private void TargetRpc_ApplyKnockbackTo(NetworkConnection conn, Vector3 velocityVector, GameObject inflictorGO, int abilitySlotIndex) {
		ApplyKnockbackToLocalUnit(velocityVector, inflictorGO, abilitySlotIndex);
	}

	private void ApplyKnockbackToLocalUnit(Vector3 velocityVector, GameObject inflictorGO, int abilitySlotIndex) {
		UnitController inflictor = inflictorGO.GetComponent<Player>().unit;
		Ability ability = UnpackAbility(inflictor, abilitySlotIndex);
		unit.Knockback(velocityVector, ability);
	}

	/*
	private void UpdateNetworkStatusSyncList() {
		currentTimer -= Time.deltaTime;
		if (currentTimer < 0) {
			networkStatusEffects.Clear();

			List<StatusEffect> list = unit.GetStatusEffectList();
			foreach (StatusEffect s in list) {
				networkStatusEffects.Add( new NetworkStatusEffect(
				  s.statusName,
				  (int)s.type,
				  s.duration,
				  s.remainingTime
				));
			}
			
			currentTimer = refreshEvery;
		}
	}*/

	// HELPER FUNCTIONS

	private int PackAbility(Ability ability) {
		if (ability != null) {
			int abilitySlotIndex = (int) ability.caster.GetAbilitySlot(ability);
			return abilitySlotIndex;
		}
		return -1;
	}

	private Ability UnpackAbility(UnitController caster, int abilitySlotIndex) {
		if (caster != null)
			return caster.GetAbilityInSlot((AbilitySlots)abilitySlotIndex);
		return null;
	}

}