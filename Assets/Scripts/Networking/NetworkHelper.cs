using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Smooth;
using UnityEngine.Events;

public class NetworkHelper : NetworkBehaviour {

	Owner owner;
	UnitController unit;



	[SyncVar(hook = nameof(OnHealthChanged))] public float currentHealth = 1;
	[SyncVar] public string unitInfo;
	private SmoothSyncMirror smoothBody;
	public bool isDisconnected = false;
	[SyncVar] public bool isUnassigned = true;
	
	public NetworkStatusEffectSyncList networkStatusEffects = new NetworkStatusEffectSyncList();
	public UnityEventNetworkStatusEffect onUpdateNetworkStatusEffect;

	void Awake() {
		owner = GetComponent<Owner>();
		unit = owner.unit;

		//if (unit.IsPlayerOwned())
		//	player = GetComponent<Player>();
		
		var smooths = GetComponents<SmoothSyncMirror>();
		foreach (SmoothSyncMirror sm in smooths) {
			if (sm.childObjectToSync != null) {
				smoothBody = sm;
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

		int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(projectilePrefab);
		int abilitySlotIndex = PackAbility(ability); // (int) unit.GetAbilitySlot(ability);
		int bodyLocation = (int)ability.projectileSpawner;

		Vector3 targetLocation;
		uint targetNetId;

		switch (ability.projectileBehaviour) {
			case ProjectileBehaviourTypes.HOMING:
				if (ability.targetTeam == AbilityTargetTeams.ALLY)
					targetNetId = castOrder.allyTarget.networkHelper.netId;
				else
					targetNetId = castOrder.enemyTarget.networkHelper.netId;
				Cmd_CreateProjectile(new NetworkProjectile(prefabIndex, bodyLocation, abilitySlotIndex, targetNetId));
				break;

			default:
				targetLocation = castOrder.targetLocation;
				Cmd_CreateProjectile(new NetworkProjectile(prefabIndex, bodyLocation, abilitySlotIndex, targetLocation));
				break;
		}
	}

	[Command]
	private void Cmd_CreateProjectile(NetworkProjectile np) {

		GameObject prefab = NetworkManager.singleton.spawnPrefabs[np.prefabIndex];
		Transform trans = Util.GetBodyLocationTransform((BodyLocations)np.bodyLocationInt, unit);
		GameObject projectileObject = Instantiate(prefab, trans.position, trans.rotation);
		NetworkServer.Spawn(projectileObject);

		Rpc_InitializeProjectile(projectileObject, np);

	}

	[ClientRpc]
	private void Rpc_InitializeProjectile(GameObject projectileObject, NetworkProjectile np) {
		Ability ability = UnpackAbility(unit, np.abilitySlotIndex);

		switch (ability.projectileBehaviour) {
			case ProjectileBehaviourTypes.BULLET:
				BulletBehaviour bullet = projectileObject.GetComponent<BulletBehaviour>();
				bullet.Initialize(ability, np.targetLocation);
				break;
			case ProjectileBehaviourTypes.CONE:
				ConeBehaviour cone = projectileObject.GetComponent<ConeBehaviour>();
				cone.Initialize(ability, np.targetLocation);
				break;
			case ProjectileBehaviourTypes.GRENADE:
				GrenadeBehaviour grenade = projectileObject.GetComponent<GrenadeBehaviour>();
				grenade.Initialize(ability, np.targetLocation);
				break;
			case ProjectileBehaviourTypes.HOMING:
				HomingBehaviour homer = projectileObject.GetComponent<HomingBehaviour>();
				UnitController targetUnit = NetworkIdentity.spawned[np.targetNetId].GetComponent<Owner>().unit;
				homer.Initialize(ability, targetUnit);
				break;
			case ProjectileBehaviourTypes.LINE:
				LineBehaviour line = projectileObject.GetComponent<LineBehaviour>();
				line.Initialize(ability, np.targetLocation);
				break;
			default:
				Debug.Log("Trying to spawn unknown projectile type.");
				break;
		}
	}

	// DUMMIES / AOE GENERATORS

	public void CreateAOEGenerator(Ability ability, Order castOrder) {
		GameObject prefab = ((Flare)ability).aoeGeneratorPrefab; // TODO: generalize
		Vector3 targetLocation = castOrder.targetLocation;

		int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(prefab);
		int abilitySlotIndex = PackAbility(ability); // (int) unit.GetAbilitySlot(ability);
	   	Cmd_CreateAOEGenerator(prefabIndex, abilitySlotIndex, targetLocation);
	}

	[Command]
	private void Cmd_CreateAOEGenerator(int prefabIndex, int abilitySlotIndex, Vector3 targetLocation) {
		GameObject prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];
		GameObject aoeGenObject = Instantiate(prefab, targetLocation, Quaternion.identity);
		NetworkServer.Spawn(aoeGenObject);
		Rpc_InitializeAOEGenerator(aoeGenObject, abilitySlotIndex, targetLocation);
	}

	[ClientRpc]
	private void Rpc_InitializeAOEGenerator(GameObject aoeGenObject, int abilitySlotIndex, Vector3 targetLocation) {
		Ability ability = UnpackAbility(unit, abilitySlotIndex);
		var aoeGenerator = aoeGenObject.GetComponent<AOEGenerator>();
		aoeGenerator.Initialize(unit, ability);
	}

	// DAMAGE AND HEALING

	// called by SyncVar hook
	private void OnHealthChanged(float newHealth) {
		if (unit.unitInfo == null) return;

		//Debug.Log("CurrentHealth: " + currentHealth + " vs NewHealth: " + newHealth);
		float dmg = currentHealth - newHealth;
		float newPercentage = newHealth / unit.unitInfo.maxHealth;

		unit.OnHealthChanged(newPercentage);

		if (dmg > 0)
			unit.OnTakeDamage(dmg);
		else
			unit.OnTakeHealing(Mathf.Abs(dmg));
	}

	public void HealDamageOn(UnitController targetUnit, float healing) {
		if (targetUnit.networkHelper.currentHealth == targetUnit.unitInfo.maxHealth) return;
		NetworkIdentity targetNetIdentity = targetUnit.networkHelper.netIdentity;
		Cmd_DealDamageTo(targetNetIdentity, -healing);
	}

	public void DealDamageTo(UnitController targetUnit, float dmg) {
		if (targetUnit.networkHelper.currentHealth <= 0f) return;
		NetworkIdentity targetNetIdentity = targetUnit.networkHelper.netIdentity;
		Cmd_DealDamageTo(targetNetIdentity, dmg);
	}

	[Command]
	private void Cmd_DealDamageTo(NetworkIdentity targetNetIdentity, float dmg) {
		if (dmg == 0) return;

		UnitController targetUnit = targetNetIdentity.GetComponent<Owner>().unit;
		if (dmg > 0 && targetUnit.HasStatusEffect(StatusEffectTypes.INVULNERABLE)) return;

		if (dmg < 0) {
			// prevent over-heal
			if ((targetUnit.networkHelper.currentHealth - dmg) > targetUnit.unitInfo.maxHealth) {
				dmg = targetUnit.networkHelper.currentHealth - targetUnit.unitInfo.maxHealth;
			}
		}

		// apply the damage
		targetUnit.networkHelper.currentHealth -= dmg;

		// check for death
		if (targetUnit.networkHelper.currentHealth <= 0) {
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

	public void AddStatusEffect(StatusEffect status) {
		NetworkStatusEffect nse = new NetworkStatusEffect(status.statusName, (int)status.type, NetworkTime.time, status.duration);
		Cmd_AddStatusEffect(nse);
	}

	[Command]
	private void Cmd_AddStatusEffect(NetworkStatusEffect nse) {
		networkStatusEffects.Add(nse);
	}

	public void StackStatusEffect(StatusEffect status) {
		NetworkStatusEffect nse = new NetworkStatusEffect(status.statusName, (int)status.type, NetworkTime.time, status.duration);
		Cmd_StackStatusEffect(nse);
	}

	[Command]
	private void Cmd_StackStatusEffect(NetworkStatusEffect nse) {
		for (int i = 0; i < networkStatusEffects.Count; i++) {
			if (networkStatusEffects[i].statusName == nse.statusName) {
				networkStatusEffects[i].UpdateTimers(nse.startTime, nse.duration);
				break;
			}
		}
		Rpc_InvokeUpdateNetworkStatusEffect(nse);
	}

	[ClientRpc]
	private void Rpc_InvokeUpdateNetworkStatusEffect(NetworkStatusEffect nse) {
		onUpdateNetworkStatusEffect.Invoke(nse);
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
		Transform spawnLoc = GameResources.Instance.GetRandomSpawnPoint();
		currentHealth = unit.unitInfo.maxHealth;
		Rpc_Respawn(spawnLoc.position, spawnLoc.rotation);

		//if (connectionToClient != null)
		//	TargetRpc_Respawn(connectionToClient, spawnLoc.position, spawnLoc.rotation);
		//else
		//	unit.Respawn(spawnLoc.position, spawnLoc.rotation);
	}

	[ClientRpc]
	private void Rpc_Respawn(Vector3 position, Quaternion rotation) {
		unit.Respawn(position, rotation);
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
	public void InstantiateParticle(NetworkParticle np) {
		if (!IsValidParticle(np.prefabName)) return;
		Cmd_InstantiateParticle(np);
	}

	[Command]
	private void Cmd_InstantiateParticle(NetworkParticle np) {
		Rpc_InstantiateParticle(np);
	}

	[ClientRpc]
	private void Rpc_InstantiateParticle(NetworkParticle np) {
		GameObject prefab = ResourceLibrary.Instance.particlePrefabDictionary[np.prefabName];

		GameObject particleSystemGO;
		if (np.TargetsUnit()) {
			NetworkIdentity ownerNetId = NetworkIdentity.spawned[np.unitNetId];
			Owner o = ownerNetId.GetComponent<Owner>();
			Transform trans = Util.GetBodyLocationTransform((BodyLocations)np.bodyLocationInt, o.unit);
			particleSystemGO = Instantiate(prefab, trans.position + prefab.transform.position, trans.rotation * prefab.transform.rotation, trans);
		}
		else if (np.TargetsWorldspace()) {
			particleSystemGO = Instantiate(prefab, np.location, np.rotation);
		}
		else {
			Debug.Log("Error in NetworkParticle");
			return;
		}

		if (np.OverridesScale()) {
			OverrideParticleScale(particleSystemGO, np.radius);
		}

		if (np.OverridesDuration()) {
			OverrideParticleChildrenDuration(particleSystemGO, np.duration);
		}
	}

	private bool IsValidParticle(string prefabName) {
		bool valid = ResourceLibrary.Instance.particlePrefabDictionary.ContainsKey(prefabName);
		if (!valid) {
			Debug.Log(prefabName + " not found in particle prefab dictionary.");
			return false;
		}
		else return true;
	}

	private void OverrideParticleChildrenDuration(GameObject particleSystemGO, float newDuration) {
		ParticleSystem[] psArray = particleSystemGO.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem ps in psArray) {
			ps.Stop();
			var main = ps.main;
			main.duration = newDuration;
			ps.Play();
		}
	}

	private void OverrideParticleScale(GameObject particleSystemGO, float newScale) {
		ParticleSystem ps = particleSystemGO.GetComponent<ParticleSystem>();
		ps.Stop();
		var shape = ps.shape;
		shape.scale = new Vector3(newScale, newScale, newScale);
		ps.Play();
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

		Player localPlayer = GameResources.Instance.GetLocalPlayer();
		if (localPlayer != null) {
			if (state == VisibilityState.INVISIBLE && unit.SharesTeamWith(localPlayer.unit)) {
				state = VisibilityState.VISIBLE_TO_TEAM_ONLY;
			}
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
		smoothBody.forceStateSendNextFixedUpdate();
		
	}

	public void SyncTeleport() {
		smoothBody.teleportOwnedObjectFromOwner();
	}

	public void SyncTeleport(Vector3 position, Quaternion rotation, Vector3 scale) {
		smoothBody.teleportAnyObjectFromServer(position, rotation, scale);
	}

	// unused
	public void SyncFixedUpdate(bool enable) {
		if (enable)
			smoothBody.whenToUpdateTransform = SmoothSyncMirror.WhenToUpdateTransform.FixedUpdate;
		else
			smoothBody.whenToUpdateTransform = SmoothSyncMirror.WhenToUpdateTransform.Update;
	}

	// ANIMATION
	public void PlayAnimation(Animations anim) {
		int nAnim = (int) anim;
		Cmd_PlayAnimation(nAnim);
	}

	[Command]
	private void Cmd_PlayAnimation(int nAnim) {
		Rpc_PlayAnimation(nAnim);
	}

	[ClientRpc]
	private void Rpc_PlayAnimation(int nAnim) {
		Animations anim = (Animations) nAnim;
		unit.body.PlayAnimation(anim);
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
