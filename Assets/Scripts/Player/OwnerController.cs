using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class OwnerController : MonoBehaviour {
	public UnitController unit;
	public bool isNPC = false; // inspector
	public Teams team;
	private MouseTargeter mouseTargeter;

	// Use this for initialization
	void Start () {
		mouseTargeter = GetComponent<MouseTargeter>();
	}

	// Update is called once per frame. Use for input. Physics unstable.
	void Update () {
		if (isNPC) return;

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

		if (Input.GetButtonDown("Attack")) {
			unit.DoAbility(AbilitySlots.ATTACK);
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
			//
		}

		if (Input.GetButtonDown("Item 2")) {
			//
		}

		if (Input.GetButtonDown("Item 3")) {
			//
		}

		if (Input.GetButtonDown("Item 4")) {
			//
		}

		if (Input.GetButtonDown("Item 5")) {
			//
		}

		if (Input.GetButtonDown("Item 6")) {
			//
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
		if(DoesUIBlockClick())
			return;

		Vector3 destination = GetMouseLocationToGround();
		if (destination != Util.GetNullVector())
			unit.MoveTo(destination);
	}

	public void SelectAtMouseCursor() {
		if(DoesUIBlockClick()) return;

		UnitController targetUnit = mouseTargeter.GetUnitAtMouseLocation();
		if (targetUnit != null) {
			if (unit.GetTeam() == targetUnit.GetTeam())	 
				unit.SetCurrentTarget(targetUnit, AbilityTargetTeams.ALLY);
			else										 
				unit.SetCurrentTarget(targetUnit, AbilityTargetTeams.ENEMY);
		}
		
		// no target unselects enemies, retains friends
		else { 
			unit.SetCurrentTarget(null, AbilityTargetTeams.ENEMY);
		}
	}

	public Teams GetTeam() {
		return team;
	} 

	public void SetTeam(Teams team) {
		this.team = team;
	}

	public Tree GetTreeAtMouseLocation() {
		return mouseTargeter.GetTreeAtMouseLocation();
	}

	public Vector3 GetMouseLocationToGround() {
		if (isNPC) return mouseTargeter.GetNPCLocationToGround();
		return mouseTargeter.GetMouseLocationToGround();
	}

	public bool IsMouseTargeting() {
		return mouseTargeter.IsTargetingEnabled();
	}

	public void SetMouseTargeting(bool enable, Ability ability = null, AbilitySlots slot = AbilitySlots.NONE) {
		mouseTargeter.SetMouseTargeting(enable, ability, slot);
	}

	public void MakeNPC() {
		isNPC = true;
		Transform[] children = GetComponentsInChildren<Transform>();
		foreach (Transform child in children) {
			if (child.CompareTag("PlayerOnly"))
				child.gameObject.SetActive(false);
		}

		InvokeRepeating("PewPewNPCs", Random.Range(0f,1.2f), 1.2f);
	}

	private void PewPewNPCs() {
		unit.DoAbility(AbilitySlots.ATTACK);
	}

}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class MouseInfo {
	public Vector3 loc; // 3D loc of mouse near z=0
	public Vector3 screenLoc; // screen position of the mouse
	public Ray ray; // ray from this mouse into 3D space
	public float time; // Time this mouseInfo was recorded
	public RaycastHit hitInfo; // info about what was hit by the ray
	public bool hit; // whether the mouse was over any collider

	public RaycastHit Raycast() {
		hit = Physics.Raycast(ray, out hitInfo);
		return hitInfo;
	}

	public RaycastHit Raycast(int mask) {
		hit = Physics.Raycast(ray, out hitInfo, mask);
		return hitInfo;
	}
}