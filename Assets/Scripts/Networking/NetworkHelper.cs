﻿using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Smooth;

public class NetworkHelper : NetworkBehaviour {

	Owner owner;
	//Player player;
	UnitController unit;
	[SyncVar] public float currentHealth = 1;
	[SyncVar] public string unitInfo;
	private SmoothSyncMirror smooth;
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

	void Awake() {
		owner = GetComponent<Owner>();
		unit = owner.unit;

		//if (unit.IsPlayerOwned())
		//	player = GetComponent<Player>();
		
		var smooths = GetComponents<SmoothSyncMirror>();
		foreach (SmoothSyncMirror sm in smooths) {
			if (sm.childObjectToSync != null) {
				smooth = sm;
				break;
			}
		}
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
		int bodyLocation = (int)ability.projectileSpawner;
	   	Cmd_CreateProjectile(prefabIndex, bodyLocation, abilitySlotIndex, targetLocation);
	}

	[Command]
	private void Cmd_CreateProjectile(int prefabIndex, int bodyLocation, int abilitySlotIndex, Vector3 targetLocation) {
		GameObject prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];
		Transform trans = Util.GetBodyLocationTransform((BodyLocations)bodyLocation, unit);
		GameObject projectileObject = Instantiate(prefab, trans.position, trans.rotation);
	
		if (connectionToClient != null) {
			NetworkServer.SpawnWithClientAuthority(projectileObject, this.gameObject); // connectionToClient);
			TargetRpc_InitializeProjectile(connectionToClient, projectileObject, abilitySlotIndex, targetLocation);
		}
		else {
			NetworkServer.Spawn(projectileObject);	
			InitializeProjectile(projectileObject, abilitySlotIndex, targetLocation);
		}
	}

	[TargetRpc]
	private void TargetRpc_InitializeProjectile(NetworkConnection conn, GameObject projectileObject, int abilitySlotIndex, Vector3 targetLocation) {
		InitializeProjectile(projectileObject, abilitySlotIndex, targetLocation);
	}

	private void InitializeProjectile(GameObject projectileObject, int abilitySlotIndex, Vector3 targetLocation) {
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

	public void DealDamageTo(UnitController targetUnit, int dmg) {
		NetworkIdentity targetNetIdentity = targetUnit.networkHelper.netIdentity;
		Cmd_DealDamageTo(targetNetIdentity, dmg);
	}

	[Command]
	private void Cmd_DealDamageTo(NetworkIdentity targetNetIdentity, int dmg) {
		UnitController targetUnit = targetNetIdentity.GetComponent<Owner>().unit;
		if (targetUnit.HasStatusEffect(StatusEffectTypes.INVULNERABLE)) return;

		Rpc_OnTakeDamage(targetNetIdentity, dmg);
		targetUnit.networkHelper.currentHealth -= dmg;

		if (targetUnit.networkHelper.currentHealth < 0) {
			NetworkConnection conn = targetNetIdentity.connectionToClient; // connectionToServer?
			if (conn != null)
				targetUnit.networkHelper.TargetRpc_ApplyOnDeathStatusEffect(conn);
			else
				targetUnit.ApplyStatusEffect(targetUnit.unitInfo.onDeathStatusEffect, null);
		}
	}

	[ClientRpc]
	private void Rpc_OnTakeDamage(NetworkIdentity targetNetIdentity, int dmg) {
		UnitController targetUnit = targetNetIdentity.GetComponent<Owner>().unit;
		targetUnit.OnTakeDamage(dmg);
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

		GameObject inflictedGO = targetUnit.owner.gameObject;
		if (ability != null) {
			int abilitySlotIndex = PackAbility(ability);
			GameObject inflictorGO = ability.caster.owner.gameObject;
			Cmd_ApplyStatusEffectTo(inflictedGO, statusName, abilitySlotIndex, inflictorGO);
		}
		else
			Cmd_ApplyStatusEffectTo(inflictedGO, statusName, -1, null);
	}

	[Command]
	private void Cmd_ApplyStatusEffectTo(GameObject inflictedGO, string statusName, int abilitySlotIndex, GameObject inflictorGO) {
		NetworkConnection conn = inflictedGO.GetComponent<Owner>().connectionToClient;

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
		unit.Die(killFromDirection, true);
	}

	public void Respawn() {
		Cmd_Respawn();
	}

	[Command]
	private void Cmd_Respawn() {
		Transform spawnLoc = GameRules.GetRandomSpawnPoint();
		currentHealth = unit.unitInfo.maxHealth;
		Rpc_RespawnAt(spawnLoc.position, spawnLoc.rotation);
	}

	[ClientRpc]
	private void Rpc_RespawnAt(Vector3 position, Quaternion rotation) {
		unit.RespawnAt(position, rotation);
	}

	// KNOCKBACK
	public void ApplyKnockbackTo(UnitController targetUnit, Vector3 velocityVector, Ability ability) {
		GameObject targetOwnerGO = targetUnit.owner.gameObject;
		GameObject inflictorGO = owner.gameObject;
		int abilitySlotIndex = PackAbility(ability);
		Cmd_ApplyKnockbackTo(targetOwnerGO, inflictorGO, velocityVector, abilitySlotIndex);
	}

	[Command]
	private void Cmd_ApplyKnockbackTo(GameObject targetOwnerGO, GameObject inflictorGO, Vector3 velocityVector, int abilitySlotIndex) {
		NetworkConnection conn = targetOwnerGO.GetComponent<NetworkIdentity>().connectionToClient;

		NetworkHelper helper = targetOwnerGO.GetComponent<NetworkHelper>();
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
		UnitController inflictor = inflictorGO.GetComponent<Owner>().unit;
		Ability ability = UnpackAbility(inflictor, abilitySlotIndex);
		unit.Knockback(velocityVector, ability);
	}

	// PARTICLES
	public void InstantiateParticle(GameObject prefab, UnitController unit, BodyLocations loc, float duration = 0f) {
		if (!IsValidParticle(prefab.name)) return;
		Cmd_InstantiateParticleOnUnit(prefab.name, unit.owner.gameObject, (int)loc, duration);
	}

	public void InstantiateParticle(GameObject prefab, Vector3 location, Quaternion rotation, float duration = 0f) {
		if (!IsValidParticle(prefab.name)) return;
		Cmd_InstantiateParticleAtLocation(prefab.name, location, rotation, duration);
	}

	private bool IsValidParticle(string prefabName) {
		bool valid = ResourceLibrary.Instance.particlePrefabDictionary.ContainsKey(prefabName);
		if (!valid) {
			Debug.Log(prefabName + " not found in particle prefab dictionary.");
			return false;
		}
		else return true;
	}


	[Command]
	private void Cmd_InstantiateParticleOnUnit(string particlePrefabName, GameObject ownerGO, int bodyLocation, float duration) {
		Rpc_InstantiateParticleOnUnit(particlePrefabName, ownerGO, bodyLocation, duration);
	}

	[Command]
	private void Cmd_InstantiateParticleAtLocation(string particlePrefabName, Vector3 location, Quaternion rotation, float duration) {
		Rpc_InstantiateParticleAtLocation(particlePrefabName, location, rotation, duration);
	}

	[ClientRpc]
	private void Rpc_InstantiateParticleOnUnit(string particlePrefabName, GameObject ownerGO, int bodyLocation, float duration) {
		GameObject prefab = ResourceLibrary.Instance.particlePrefabDictionary[particlePrefabName];
		Owner o = ownerGO.GetComponent<Owner>();
		Transform trans = Util.GetBodyLocationTransform((BodyLocations)bodyLocation, o.unit);
		GameObject particleSystemGO = Instantiate(prefab, trans.position + prefab.transform.position, trans.rotation * prefab.transform.rotation, trans);

		if (duration != 0f) {
			ParticleSystem[] psArray = particleSystemGO.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps in psArray) {
				ps.Stop();
				var main = ps.main;
				main.duration = duration;
				ps.Play();
			}
		}
	}

	[ClientRpc]
	private void Rpc_InstantiateParticleAtLocation(string particlePrefabName, Vector3 location, Quaternion rotation, float duration) {
		GameObject prefab = ResourceLibrary.Instance.particlePrefabDictionary[particlePrefabName];
		GameObject particleSystemGO = Instantiate(prefab, location, rotation);

		// TODO: simplify this (combine with above)
		if (duration != 0f) {
			ParticleSystem[] psArray = particleSystemGO.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps in psArray) {
				ps.Stop();
				var main = ps.main;
				main.duration = duration;
				ps.Play();
			}
		}
	}

	// VISIBILITY
	public void SetVisibilityState(VisibilityState state) {
		Cmd_SetVisibilityState((int)state);
	}

	[Command]
	private void Cmd_SetVisibilityState(int state) {
		Rpc_SetVisibilityState(state);
	}

	[ClientRpc]
	private void Rpc_SetVisibilityState(int nState) {
		VisibilityState state = (VisibilityState)nState;

		UnitController localUnit = GameRules.Instance.GetLocalPlayer().unit;
		if (state == VisibilityState.INVISIBLE && unit.SharesTeamWith(localUnit)) {
			state = VisibilityState.VISIBLE_TO_TEAM_ONLY;
		}

		unit.body.SetVisibilityState(state); //Debug.Log("State: " + state.ToString());
	}

	// UNIT INFO
	public void SetUnitInfo(string unitInfoName) {
		unitInfo = unitInfoName;
		//Rpc_SetUnitInfo(unitInfoName);
		unit.SetUnitInfo(unitInfoName);
	}

	/*[ClientRpc]
	private void Rpc_SetUnitInfo(string unitInfoName) {
		unit.SetUnitInfo(unitInfoName);
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

	public void SyncTransform() {
		smooth.forceStateSendNextFixedUpdate();
		
	}

	public void SyncTeleport() {
		smooth.teleportOwnedObjectFromOwner();
	}

	// unused
	public void SyncFixedUpdate(bool enable) {
		if (enable)
			smooth.whenToUpdateTransform = SmoothSyncMirror.WhenToUpdateTransform.FixedUpdate;
		else
			smooth.whenToUpdateTransform = SmoothSyncMirror.WhenToUpdateTransform.Update;
	}





	// SCENE MANAGEMENT

	//public enum LoadAction { Load, Unload }
	// bool isBusyLoadingScene = false;     // isBusy protects us from being overwhelmed by server messages to load several subscenes at once.

	/*
    IEnumerator LoadUnloadScene(string sceneName, LoadAction loadAction)
    {
        while (isBusyLoadingScene) yield return null;

        isBusyLoadingScene = true;

        if (loadAction == LoadAction.Load)
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        else
        {
            yield return SceneManager.UnloadSceneAsync(sceneName);
            yield return Resources.UnloadUnusedAssets();
        }

        isBusyLoadingScene = false;
        Debug.LogFormat("{0} {1} Done", sceneName, loadAction.ToString());

        Cmd_SceneDone(sceneName, loadAction);
    }

    [Command]
    public void Cmd_SceneDone(string sceneName, LoadAction loadAction)
    {
        // The point of this is to show the client telling server it has loaded the subscene
        // so the server might take some further action, e.g. reposition the player.
        Debug.LogFormat("{0} {1} done on client", sceneName, loadAction.ToString());
    }
	*/
}
