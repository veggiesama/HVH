using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamFieldOfView : Singleton<TeamFieldOfView> {

	private Teams team;
	private List<UnitController> registeredViewers = new List<UnitController>();
	private List<Transform> visibleTargets = new List<Transform>();
	public float findTargetsEvery = 0.2f;
	private bool initialized = false;

	// Singleton constructor
	public static TeamFieldOfView Instance {
		get {
			return ((TeamFieldOfView)mInstance);
		} set {
			mInstance = value;
		}
	}

	public void Initialize(Teams team) {
		this.team = team;	
		registeredViewers.Clear();
		FieldOfView[] fows = FindObjectsOfType<FieldOfView>();
		foreach (FieldOfView fow in fows) {
			UnitController unit = fow.unit;
			if (unit != null && unit.GetTeam() == team) {
				RegisterViewer(unit);
			}
		}

		if (!initialized) {
			StartCoroutine( SlowUpdate(findTargetsEvery) );
			initialized = true;
		}
	}

    IEnumerator SlowUpdate(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

	private void FindVisibleTargets() {
		List<Transform> newVisibleTargets = new List<Transform>();
		foreach (UnitController unit in registeredViewers) {
			if (newVisibleTargets.Count == 0)
				newVisibleTargets = unit.GetFieldOfView().FindVisibleTargets();
			else
				newVisibleTargets = newVisibleTargets.Union( unit.GetFieldOfView().FindVisibleTargets() ).ToList();
		}

		UnitController localUnit = GameRules.Instance.GetLocalPlayer().unit;
		UnitController localTarget = localUnit.GetTarget(AbilityTargetTeams.ENEMY);

		// make newly visible bodies visible
		foreach (Transform t in newVisibleTargets) {
			if (!visibleTargets.Contains(t)) {
				BodyController b = t.gameObject.GetComponent<BodyController>();
				if (b != null && b.unit.GetTeam() != this.team) {
					b.SetVisibility(true);
					if (localUnit.IsForgottenTarget(b.unit))
						localUnit.RememberTarget();
				}
			}
		}

		// make newly hidden bodies invisible
		foreach (Transform t in visibleTargets) {
			if (!newVisibleTargets.Contains(t)) {
				BodyController b = t.gameObject.GetComponent<BodyController>();
				if (b != null && b.unit.GetTeam() != this.team) {
					b.SetVisibility(false);
					if (b.unit == localTarget)
						localUnit.ForgetTarget();
				}
			}
		}

		visibleTargets = newVisibleTargets;
	}

	public void RegisterViewer(UnitController unit) {
		if (!registeredViewers.Contains(unit)) {
			registeredViewers.Add(unit);
		}
	}

	/*
	public void UnregisterViewer(UnitController unit) {
		FieldOfView fov = unit.GetFieldOfView();

		if (registeredViewers.Contains(fov)) {
			registeredViewers.Remove(fov);
		}
	}

	public void AddVisibleTarget(UnitController target) {
		if (!visibleTargets.Contains(target.transform)) {
			visibleTargets.Add(target.transform);
		}
	}

	public List<Transform> GetVisibleTargets() {
		return visibleTargets;
	}
	*/
}
