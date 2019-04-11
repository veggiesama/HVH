using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Mirror;

public class Player : Owner {
	//public ControlScheme controlScheme;
	//public PlayerClass playerClass;

	//public ControlScheme controlScheme;
	public UIController uiController;
	public GameObject camObject;
	private MouseTargeter mouseTargeter;

	//public bool isNPC = false; // inspector
	public bool isDisconnected = false;
	public bool isUnassigned = true;
	[SyncVar] public int playerID;

	public void Start() {
		unit.body.GetComponent<Renderer>().material.color = bodyColor;
	}

	public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer(); // does this do anything?
		mouseTargeter = GetComponent<MouseTargeter>();
		if (isLocalPlayer) {
			camObject.SetActive(true);
			uiController.gameObject.SetActive(true);

			//Debug.Log("FOUND DWARF: " + FindObjectOfType<GameController>().Cmd_GetDwarf(DwarfTeamSlots.ALLY_1).name);
		}
	}

	// Update is called once per frame. Use for input. Physics unstable.
	void Update () {
		//controlScheme.UpdateInputs();

		if (isUnassigned) return;
		if (!isLocalPlayer) return;

		// always register selections, even while disabled
		if (Input.GetButtonDown("L-Click")) {

			if(!mouseTargeter.IsTargetingEnabled()) 
				SelectAtMouseCursor();
			else
				unit.DoAbility(mouseTargeter.storedSlot);
		}

		//if (!CanIssueCommands())
		//	return;

		if (Input.GetButtonDown("R-Click")) {
			if(!mouseTargeter.IsTargetingEnabled()) 
				MoveToMouseCursor();
			else
				SetMouseTargeting(false);
		}

		if (Input.GetButtonDown("MoveAttack")) {
			//
		}

		if (Input.GetButtonDown("Move")) {
			//
		}

		if (Input.GetButtonDown("Stop")) {
			unit.Stop();
		}

		if (Input.GetButtonDown("Attack")) {
			unit.DoAbility(AbilitySlots.ATTACK);
		}

		if (Input.GetButtonDown("Ability 1")) {
			unit.DoAbility(AbilitySlots.ABILITY_1);
		}

		if (Input.GetButtonDown("Ability 2")) {
			unit.DoAbility(AbilitySlots.ABILITY_2);
		}

		if (Input.GetButtonDown("Ability 3")) {
			unit.DoAbility(AbilitySlots.ABILITY_3);
		}

		if (Input.GetButtonDown("Ability 4")) {
			unit.DoAbility(AbilitySlots.ABILITY_4);
		}

		if (Input.GetButtonDown("Ability 5")) {
			unit.DoAbility(AbilitySlots.ABILITY_5);
		}

		if (Input.GetButtonDown("Ability 6")) {
			unit.DoAbility(AbilitySlots.ABILITY_6);
		}

		if (Input.GetButtonDown("Item 1")) {
			unit.DoAbility(AbilitySlots.ITEM_1);
		}

		if (Input.GetButtonDown("Item 2")) {
			unit.DoAbility(AbilitySlots.ITEM_2);
		}

		if (Input.GetButtonDown("Item 3")) {
			unit.DoAbility(AbilitySlots.ITEM_3);
		}

		if (Input.GetButtonDown("Item 4")) {
			unit.DoAbility(AbilitySlots.ITEM_4);
		}

		if (Input.GetButtonDown("Item 5")) {
			unit.DoAbility(AbilitySlots.ITEM_5);
		}

		if (Input.GetButtonDown("Item 6")) {
			unit.DoAbility(AbilitySlots.ITEM_6);
		}
	}

	// Called at end of frame. Use for physics updates. If used for input, may result in input loss.
	void FixedUpdate() {}

	public void UI_ClickedAbilityButton(AbilitySlots slot) {
		//print("Owner null: " + (this == this.isActiveAndEnabled) + ", Unit null: " + (this.unit.isActiveAndEnabled == null) );
		//AbilitySlots slot = EventSystem.current.currentSelectedGameObject.GetComponent<AbilityButtonInfo>().abilitySlot;
		unit.DoAbility(slot);
	}

	// check if UI should block raycast into world
	public bool DoesUIBlockClick() {
		return EventSystem.current.IsPointerOverGameObject();
	}

	public void MoveToMouseCursor() {
		if(DoesUIBlockClick()) return;

		Vector3 destination = GetMouseLocationToGround();
		if (destination != Util.GetNullVector())
			unit.MoveTo(destination);
	}

	public void SelectAtMouseCursor() {
		if(DoesUIBlockClick()) return;

		UnitController targetUnit = mouseTargeter.GetUnitAtMouseLocation();
		if (targetUnit != null) {
			unit.SetCurrentTarget(targetUnit);
		}
		
		// no target unselects enemies, retains friends
		else { 
			unit.SetCurrentTarget(null);
		}
	}

	public Tree GetTreeAtMouseLocation() {
		return mouseTargeter.GetTreeAtMouseLocation();
	}

	public Vector3 GetMouseLocationToGround() {
		if (isUnassigned) return mouseTargeter.GetNPCLocationToGround();
		return mouseTargeter.GetMouseLocationToGround();
	}

	public bool IsMouseTargeting() {
		if (isUnassigned) return false;
		return mouseTargeter.IsTargetingEnabled();
	}

	public void SetMouseTargeting(bool enable, Ability ability = null, AbilitySlots slot = AbilitySlots.NONE) {
		mouseTargeter.SetMouseTargeting(enable, ability, slot);
	}

	public void MakeNPC() {
		isUnassigned = true;
		Transform[] children = GetComponentsInChildren<Transform>();
		foreach (Transform child in children) {
			if (child.CompareTag("PlayerOnly"))
				child.gameObject.SetActive(false);
		}

		//InvokeRepeating("PewPewNPCs", Random.Range(0f,1.2f), 1.2f);
	}

	private void PewPewNPCs() {
		unit.DoAbility(AbilitySlots.ATTACK);
	}

	public void DestroyTree(Tree tree, Vector3 destroyedFromDirection = default, float delay = 0f) {
		int treeSiblingIndex = tree.GetSiblingIndex();
		GameObject treeHandlerGO = tree.GetTreeHandlerGO();
		Cmd_DestroyTree(treeHandlerGO, treeSiblingIndex, destroyedFromDirection, delay);
	}

	[Command]
	public void Cmd_DestroyTree(GameObject treeHandlerGO, int treeSiblingIndex, Vector3 destroyedFromDirection, float delay) {
		treeHandlerGO.GetComponent<TreeHandler>().Rpc_DestroyTree(treeSiblingIndex, destroyedFromDirection, delay);	
	}

	/*
	public void CreateGhostProjectile(GameObject objectToClone) {
		uint netID = objectToClone.GetComponent<NetworkIdentity>().netId;
		Cmd_CreateGhostProjectile(netID);
	}

	[Command]
	public void Cmd_CreateGhostProjectile(uint netID) {
		GameObject objectToClone = NetworkIdentity.spawned[netID].gameObject;

		GameObject clone = Instantiate(objectToClone);
		clone.name = clone.name + " (Ghost)";

		clone.AddComponent<NetworkGhost>();
		clone.GetComponent<NetworkGhost>().Initialize(objectToClone);
		//clone.AddComponent<NetworkTransform>();

		NetworkServer.Spawn(clone);
	}*/

	/*
	public void CreateServerProjectile(int prefabIndex, Transform t) {
		Cmd_CreateServerProjectile(prefabIndex, t.position, t.rotation);
	}

	[SyncVar] public int lastProjectileNetID;

	[Command]
	public void Cmd_CreateServerProjectile(int prefabIndex, Vector3 position, Quaternion rotation) {
		GameObject prefab = FindObjectOfType<NetworkManagerHVH>().spawnPrefabs[prefabIndex];
		GameObject projectileObject = Instantiate(prefab, position, rotation, unit.transform);
		NetworkServer.SpawnWithClientAuthority(projectileObject, this.gameObject);
		Rpc_SetLastProjectileNetID( (int) projectileObject.GetComponent<NetworkIdentity>().netId );
	}

	[Command]
	public void Cmd_DestroyServerProjectile(GameObject serverProjectile) {
		//NetworkServer.UnSpawn(serverProjectile);
		NetworkServer.Destroy(serverProjectile);
	}

	[ClientRpc]
	public void Rpc_SetLastProjectileNetID(int netID) {
		Debug.Log("Rpc_SetLastProjectileNetID");
		lastProjectileNetID = netID;
	}*/

	public void CreateProjectile(GameObject projectilePrefab, Ability ability, Vector3 targetLocation, float missChance) {
		int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(projectilePrefab); // FindObjectOfType<NetworkManagerHVH>().GetSpawnPrefabList().IndexOf(serverProjectilePrefab);
		int abilitySlotIndex = (int) unit.GetAbilitySlot(ability);
	   	Cmd_CreateProjectile(prefabIndex, abilitySlotIndex, targetLocation, missChance);
	}

	[Command]
	public void Cmd_CreateProjectile(int prefabIndex, int abilitySlotIndex, Vector3 targetLocation, float missChance) {
		GameObject prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

		GameObject projectileObject = Instantiate(prefab,
			unit.attackInfo.spawnerObject.transform.position,
			unit.attackInfo.spawnerObject.transform.rotation);
			//unit.transform); // bad for NetworkIdentity to be spawned as child?
		NetworkServer.SpawnWithClientAuthority(projectileObject, this.gameObject); // connectionToClient);

		uint projectileNetID = projectileObject.GetComponent<NetworkIdentity>().netId;
		TargetRpc_InitializeProjectile(connectionToClient, projectileNetID, abilitySlotIndex, targetLocation, missChance);
	}

	[TargetRpc]
	public void TargetRpc_InitializeProjectile(NetworkConnection conn, uint projectileNetID, int abilitySlotIndex, Vector3 targetLocation, float missChance) {
		GameObject projectileObject = NetworkIdentity.spawned[projectileNetID].gameObject;
		Ability ability = unit.GetAbilityInSlot((AbilitySlots)abilitySlotIndex);
		BulletBehaviour bullet = projectileObject.GetComponent<BulletBehaviour>();

		bullet.Initialize(ability, targetLocation, missChance);
	}

	public void DestroyProjectile(GameObject projectileObject) {
		uint projectileNetID = projectileObject.GetComponent<NetworkIdentity>().netId;
		Cmd_DestroyProjectile(projectileNetID);
	}


	//TODO: Add some safety. Change from netID to NetworkIdentity or GameObject probably.
	[Command]
	public void Cmd_DestroyProjectile(uint projectileNetID) {
		GameObject projectileObject = NetworkIdentity.spawned[projectileNetID].gameObject;
		NetworkServer.Destroy(projectileObject);
		// Destroy?
	}
}
