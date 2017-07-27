using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class OwnerController : MonoBehaviour {
	public UnitController unit;
	public bool isNPC = false; // inspector
	public Teams team;

	// Use this for initialization
	void Start () {
		//rb = (Rigidbody)GetComponentInChildren<Rigidbody>();
		//agent = (NavMeshAgent)GetComponentInChildren<NavMeshAgent>(); // found in PlayerBody
	}

	// Update is called once per frame. Use for input. Physics unstable.
	void Update () {
		if (isNPC) return;

		// always register selections, even while disabled
		if (Input.GetButtonDown("L-Click")) {
			SelectAtMouseCursor();
		}

		//if (!CanIssueCommands())
		//	return;

		if (Input.GetButtonDown("R-Click")) {
			MoveToMouseCursor();
		}

		if (Input.GetButtonDown("Attack")) {
			//
		}

		if (Input.GetButtonDown("Move")) {
			//
		}

		if (Input.GetButtonDown("Stop")) {
			//
		}

		if (Input.GetButtonDown("Ability 1")) {
			this.unit.DoAbility(AbilitySlots.ABILITY_1);
		}

		if (Input.GetButtonDown("Ability 2")) {
			this.unit.DoAbility(AbilitySlots.ABILITY_2);
		}

		if (Input.GetButtonDown("Ability 3")) {
			this.unit.DoAbility(AbilitySlots.ABILITY_3);
		}

		if (Input.GetButtonDown("Ability 4")) {
			this.unit.DoAbility(AbilitySlots.ABILITY_4);
		}

		if (Input.GetButtonDown("Ability 5")) {
			this.unit.DoAbility(AbilitySlots.ABILITY_5);
		}

		if (Input.GetButtonDown("Ability 6")) {
			this.unit.DoAbility(AbilitySlots.ABILITY_6);
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
	void FixedUpdate() {

	}

	//public bool CanIssueCommands() {
	//	return !isNPC; // && !isImmobile;
	//}
	
	public void UI_ClickedAbilityButton(AbilitySlots slot) {
		//print("Owner null: " + (this == this.isActiveAndEnabled) + ", Unit null: " + (this.unit.isActiveAndEnabled == null) );
		//AbilitySlots slot = EventSystem.current.currentSelectedGameObject.GetComponent<AbilityButtonInfo>().abilitySlot;
		this.unit.DoAbility(slot);
	}

	// check if UI should block raycast into world
	public bool DoesUIBlockClick() {
		return EventSystem.current.IsPointerOverGameObject();
	}

	public void MoveToMouseCursor() {
		if(DoesUIBlockClick())
			return;

		RaycastHit hit;
		Ray ray = (Ray)Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 100)) {
			unit.MoveTo(hit.point);
		}
	}

	public Teams GetTeam() {
		return this.team;
	} 

	public void SetTeam(Teams team) {
		this.team = team;
	}

	public void SelectAtMouseCursor() {
		if(DoesUIBlockClick())
			return;

		RaycastHit hit;
		Ray ray = (Ray)Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, 100)) {
			//Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 1.0f);
			GameObject targetObject = hit.transform.gameObject;
			if (targetObject.tag == "Body") {
				UnitController targetUnit = targetObject.GetComponent<BodyController>().GetUnitController();
				if (this.unit.GetTeam() == targetUnit.GetTeam())	 
					this.unit.SetCurrentTarget(targetUnit, true); // friendly
				else										 
					this.unit.SetCurrentTarget(targetUnit, false); // enemy
			}
			// no target unselects enemies, retains friends
			else {
				this.unit.SetCurrentTarget(null, false);
			}
		}	
	}

	public void MakeNPC() {
		isNPC = true;
		Transform[] children = GetComponentsInChildren<Transform>();
		foreach (Transform child in children) {
			if (child.CompareTag("PlayerOnly"))
				child.gameObject.SetActive(false);
		}
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