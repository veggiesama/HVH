using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Interactions;
using Mirror;

public class Player : Owner {
	//public ControlScheme controlScheme;
	//public PlayerClass playerClass;

	//public ControlScheme controlScheme;
	public Camera cam;
	public MouseTargeter mouseTargeter;
	//private bool holdingShift = false;
	//private bool holdingCtrl = false;

	public HVH_Inputs hvhInputs;
	public GameObject networkPlayer;

	//public bool isNPC = false; // inspector
	[SyncVar] public int playerID;

	public override void Awake() {
		base.Awake();
		hvhInputs = new HVH_Inputs();
	}

	public void OnEnable() {
		hvhInputs.Enable();
	}

	public void OnDisable() {
		hvhInputs.Disable();
	}

	//public override void OnStartLocalPlayer() {
	public void Initialize() {

		//if (!isLocalPlayer) {
		//	Debug.Log("Can't initialize non-local player");
		//	return;
		//}

		GameResources.Instance.SetLocalPlayer(this);

		EnableLocalPlayerOnlyObjects(true);
		UpdateTeamVision();

		TeamFieldOfView.Instance.Initialize((Teams)team);
		GameResources.Instance.DisableTreeHighlighting();
		Debug.Log("Setup Ally Cameras");
		GameplayCanvas.Instance.RegisterAllyPortraits(this);
		GameplayCanvas.Instance.ResetButtons();
		GameplayCanvas.Instance.debugMenu.Initialize();
		cam.GetComponent<CameraFollow>().Initialize();
		DayNight.Instance.Initialize();

		//if (networkHelper.isUnassigned) return;
		//if (!isLocalPlayer) return;

		hvhInputs.Player.Attack.started += _ => unit.DoAbility(AbilitySlots.ATTACK);
		hvhInputs.Player.Ability1.started += _ => unit.DoAbility(AbilitySlots.ABILITY_1);
		hvhInputs.Player.Ability2.started += _ => unit.DoAbility(AbilitySlots.ABILITY_2);
		hvhInputs.Player.Ability3.started += _ => unit.DoAbility(AbilitySlots.ABILITY_3);
		hvhInputs.Player.Ability4.started += _ => unit.DoAbility(AbilitySlots.ABILITY_4);
		hvhInputs.Player.Ability5.started += _ => unit.DoAbility(AbilitySlots.ABILITY_5);
		hvhInputs.Player.Ability6.started += _ => unit.DoAbility(AbilitySlots.ABILITY_6);
		hvhInputs.Player.Item1.started += _ => unit.DoAbility(AbilitySlots.ITEM_1);
		hvhInputs.Player.Item2.started += _ => unit.DoAbility(AbilitySlots.ITEM_2);
		hvhInputs.Player.Item3.started += _ => unit.DoAbility(AbilitySlots.ITEM_3);
		hvhInputs.Player.Item4.started += _ => unit.DoAbility(AbilitySlots.ITEM_4);
		hvhInputs.Player.Item5.started += _ => unit.DoAbility(AbilitySlots.ITEM_5);
		hvhInputs.Player.Item6.started += _ => unit.DoAbility(AbilitySlots.ITEM_6);

		hvhInputs.Player.LClick.started += _ => DoLeftClick();
		hvhInputs.Player.RClick.started += _ => DoRightClick();
		hvhInputs.Player.Stop.started += _ => unit.Stop();

		//if (!CanIssueCommands())
		//	return;

	}

	public void RefreshInputActions() {
		hvhInputs.Disable();
		hvhInputs = new HVH_Inputs();
		hvhInputs.Enable();
	}

	private void DoLeftClick() {
		if(!IsMouseTargeting()) {
			SelectAtMouseCursor();
		}
		else {
			unit.DoAbility(mouseTargeter.storedSlot);
			SetMouseTargeting(false);
		}
	}

	private void DoRightClick() {
		if(!IsMouseTargeting()) 
			MoveToMouseCursor();
		else
			SetMouseTargeting(false);
	}

	public bool IsShiftQueuing() {
		return (hvhInputs.Player.QueueHold.ReadValue<float>() != 0f);
	}

	public bool IsCtrlZooming() {
		return (hvhInputs.Player.ZoomHold.ReadValue<float>() != 0f);
	}

	public void UI_ClickedAbilityButton(AbilitySlots slot) {
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
			unit.RemoveCurrentTarget(AbilityTargetTeams.ENEMY);
		}
	}

	public Tree GetTreeAtMouseLocation() {
		return mouseTargeter.GetTreeAtMouseLocation();
	}

	public UnitController GetUnitAtMouseLocation() {
		return mouseTargeter.GetUnitAtMouseLocation();
	}

	public Vector3 GetMouseLocationToGround() {
		return mouseTargeter.GetMouseLocationToGround();
	}

	public bool IsMouseTargeting() {
		if (networkHelper.isUnassigned) return false;
		return mouseTargeter.IsTargetingEnabled();
	}

	public void SetMouseTargeting(bool enable, Ability ability = null, AbilitySlots slot = AbilitySlots.NONE) {
		mouseTargeter.SetMouseTargeting(enable, ability, slot);
	}

	public void MakeNPC() {
		networkHelper.isUnassigned = true;
		Transform[] children = GetComponentsInChildren<Transform>();
		EnableLocalPlayerOnlyObjects(false);

		//InvokeRepeating("PewPewNPCs", Random.Range(0f,1.2f), 1.2f);
	}

	private void PewPewNPCs() {
		unit.DoAbility(AbilitySlots.ATTACK);
	}

	public void EnableLocalPlayerOnlyObjects(bool enable) {
		Transform[] children = GetComponentsInChildren<Transform>(true);
		foreach (Transform child in children) {
			if (child.CompareTag("LocalPlayerOnly"))
				child.gameObject.SetActive(enable);
		}
	}

	public void UpdateTeamVision() {
		 foreach (UnitController u in FindObjectsOfType<UnitController>()) {
			if (u.SharesTeamWith(this.unit))
				u.EnableVision(true);
			else
				u.EnableVision(false);
		}
	}

}
