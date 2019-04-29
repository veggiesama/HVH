using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MouseTargeter : MonoBehaviour {
	public Camera cam;
 	private Player player;

	public Texture2D normalCursor = null;
	public Texture2D targetCursor;
	public Projector areaProjector;

	[HideInInspector] public AbilitySlots storedSlot;
	[HideInInspector] public Ability storedAbility;
	private bool targetingEnabled = false;
	private Tree lastTree;
	private UnitController lastUnit;

	// Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
		cam = player.camObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
		if (IsTargetingEnabled())
			DoTargetCheckingAtMouseCursor();
    }

	private void DoTargetCheckingAtMouseCursor() {
		switch (storedAbility.targetType) {
			case AbilityTargetTypes.AREA:
				DoAreaChecking();
				break;
			case AbilityTargetTypes.UNIT:
				DoUnitChecking();
				break;
			case AbilityTargetTypes.TREE:
				DoTreeChecking();
				break;
			default:
				break;
		}
	}

	// TODO: opportunity to implement interface
	private void DoTreeChecking() {
		Tree tree = GetTreeAtMouseLocation();
		if (tree != null) {
			if (!tree.Equals(lastTree)) {
				tree.SetHighlighted(true);
				if (lastTree != null) lastTree.SetHighlighted(false);
				lastTree = tree;
			}
		}
		else {
			if (lastTree != null) {
				lastTree.SetHighlighted(false);
				lastTree = null;
			}
		}
	}

	private void DoUnitChecking() {
		UnitController unit = GetUnitAtMouseLocation();
		if (unit != null) {
			if (!unit.Equals(lastUnit)) {
				//unit.SetHighlighted(true);
				if (lastUnit != null) {} //lastUnit.SetHighlighted(false);
				//lastUnit = unit;
			}
		}
		else {
			if (lastUnit != null) {
				//lastUnit.SetHighlighted(false);
				//lastUnit = null;
			}
		}
	}

	private void DoAreaChecking() {
		Vector3 newPos = GetMouseLocationToGround();
		newPos.y = areaProjector.transform.position.y;
		areaProjector.transform.position = newPos;
	}

	public Tree GetTreeAtMouseLocation() {
		int layerMask = (int)LayerMasks.TREE;
		Ray ray = (Ray)cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Constants.RaycastLength, layerMask)) {
			Tree tree =  hit.transform.gameObject.GetComponent<Tree>();
			return tree;
		}

		return null;
	}

	public UnitController GetUnitAtMouseLocation() {
		int layerMask = ~((int)LayerMasks.TERRAIN | (int)LayerMasks.TREE); // cast at everything except terrain + tree
		Ray ray = (Ray)cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Constants.RaycastLength, layerMask)) {
			GameObject targetObject = hit.transform.gameObject;
			if (targetObject.tag == "Body") {
				UnitController targetUnit = targetObject.GetComponent<BodyController>().unit;
				return targetUnit;
			}
		}

		return null;
	}

	public Vector3 GetMouseLocationToGround() {
		int layerMask = (int)LayerMasks.TERRAIN; //LayerMask.GetMask("Terrain"); // 1 << (int)Layers.GROUND;
		Ray ray = (Ray)cam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Constants.RaycastLength, layerMask)) {
			return hit.point;
		}

		return Util.GetNullVector();
	}

	public Vector3 GetNPCLocationToGround() {
		RaycastHit hit;
		Vector3 rngOrigin = Util.GetRandomVectorAround(player.unit, 10.0f);
		int layerMask = LayerMask.GetMask("Terrain");
		Physics.Raycast(rngOrigin, Vector3.down, out hit, 100f, layerMask);
		return hit.point;
	}

	public void SetMouseTargeting(bool enable, Ability ability = null, AbilitySlots slot = AbilitySlots.NONE) {

		if (enable) {

			if (ability != null && ability.targetType == AbilityTargetTypes.AREA) {
				Cursor.visible = false;
				DoAreaChecking(); // force it to move before revealing
				areaProjector.orthographicSize = ability.aoeRadius;
				areaProjector.gameObject.SetActive(true);
			}
			else
				Cursor.SetCursor(targetCursor, new Vector2(targetCursor.width/2, targetCursor.height/2), CursorMode.Auto);
				// CursorMode.ForceSoftware allows for greater than 32x32, but at what cost?

			targetingEnabled = true;
			storedSlot = slot;
			storedAbility = ability;
		}

		else {
			Cursor.visible = true;
			Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
			areaProjector.gameObject.SetActive(false);

			targetingEnabled = false;
			storedSlot = AbilitySlots.NONE;
			storedAbility = null;
			if (lastTree != null) lastTree.SetHighlighted(false);
			//if (lastUnit != null) lastUnit.SetHighlighted(false);
		}
	}

	public bool IsTargetingEnabled() {
		return targetingEnabled;
	}

}
