using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class OwnerController : MonoBehaviour {
	public UnitController unit;
	public bool isNPC = false; // inspector
	public Teams team;
	public Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponentInChildren<Camera>();
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
			this.unit.DoAbility(AbilitySlots.ATTACK);
		}

		if (Input.GetButtonDown("MoveAttack")) {
			//
		}

		if (Input.GetButtonDown("Move")) {
			//
		}

		if (Input.GetButtonDown("Stop")) {
			this.unit.Stop();
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
	void FixedUpdate() {}

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

		Vector3 destination = GetMouseLocationToGround();
		if (destination != Util.GetNullVector())
			unit.MoveTo(destination);
	}

	public void SelectAtMouseCursor() {
		if(DoesUIBlockClick()) return;

		UnitController targetUnit = GetUnitAtMouseLocation();
		if (targetUnit != null) {
			if (unit.GetTeam() == targetUnit.GetTeam())	 
				unit.SetCurrentTarget(targetUnit, AbilityTargetTeams.ALLY);
			else										 
				unit.SetCurrentTarget(targetUnit, AbilityTargetTeams.ENEMY);
		}
		
		// no target unselects enemies, retains friends
		else { 
			this.unit.SetCurrentTarget(null, AbilityTargetTeams.ENEMY);
		}
	}

	public UnitController GetUnitAtMouseLocation() {
		int layerMask = ~((int)LayerMasks.TERRAIN | (int)LayerMasks.TREE); // cast at everything except terrain + tree
		Ray ray = (Ray)cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Constants.RaycastLength, layerMask)) {
			GameObject targetObject = hit.transform.gameObject;
			if (targetObject.tag == "Body") {
				UnitController targetUnit = targetObject.GetComponent<BodyController>().GetUnitController();
				return targetUnit;
			}
		}

		return null;
	}

	public Vector3 GetMouseLocationToGround() {
		if (isNPC) return GetNPCLocationToGround();

		int layerMask = (int)LayerMasks.TERRAIN; //LayerMask.GetMask("Terrain"); // 1 << (int)Layers.GROUND;
		Ray ray = (Ray)cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Constants.RaycastLength, layerMask)) {
			return hit.point;
		}

		return Util.GetNullVector();
	}

	public Teams GetTeam() {
		return this.team;
	} 

	public void SetTeam(Teams team) {
		this.team = team;
	}

	private Vector3 GetNPCLocationToGround() {
		RaycastHit hit;
		Vector3 rngOrigin = Util.GetRandomVectorAround(unit, 10.0f);
		int layerMask = LayerMask.GetMask("Terrain");
		Physics.Raycast(rngOrigin, Vector3.down, out hit, 100f, layerMask);
		return hit.point;
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
		this.unit.DoAbility(AbilitySlots.ATTACK);
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