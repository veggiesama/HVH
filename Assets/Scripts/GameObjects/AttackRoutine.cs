using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Attack logic:
// 1. initiate attack and wait for swing
// 2. if swing successful (ie: wait successful), fire the attack
// 3. wait for backswing (freely interruptable with no penalty)
// 4. continue destination movement

public class AttackRoutine : MonoBehaviour {

	private UnitController caster, target;
	private GameObject spawnerObject, projectilePrefab;
	private Vector3 startingPosition, storedDestination;
	private float swingSpeed, backswingSpeed;

	private bool waitedForSwing = true;
	private bool waitedForBackswing = true;

	private CastbarController castbar;

	// you must initialize after instantiation
	public void Initialize(UnitController caster, UnitController target) {
		this.caster = caster;
		this.target = target;

		startingPosition = caster.GetBodyPosition();
		swingSpeed = caster.attackInfo.swingSpeed;
		backswingSpeed = caster.attackInfo.backswingSpeed;
		//damage = casterHeroInfo.attack.damage;
		//casterAgent = caster.GetComponentInChildren<NavMeshAgent>(); // might break?

		spawnerObject = caster.attackInfo.spawnerObject;
		projectilePrefab = caster.attackInfo.projectilePrefab;
		castbar = GameObject.Find("CastbarContainer").GetComponent<CastbarController>();

		StartCoroutine( DoAttackLogic() );
	}

	private IEnumerator DoAttackLogic() {
		PauseMovement();
		castbar.Initialize(caster, swingSpeed, backswingSpeed);

		yield return StartCoroutine( DoWaitForSwing() );
		
		if (waitedForSwing)
			Fire();
		else {
			Destroy(this.gameObject);
			yield break;
		}

		yield return StartCoroutine( DoWaitForBackswing() );
		
		if (waitedForBackswing)
			ResumeMovement();

		Destroy(this.gameObject); // garbage cleanup
	}
	
	private void PauseMovement() {
		if (caster.IsReadyForNav()) {
			storedDestination = caster.GetDestination();
			caster.MoveTo(startingPosition);
		}
	}

	private IEnumerator DoWaitForSwing() {
		yield return new WaitForSeconds(swingSpeed);
		var newPosition = caster.GetBodyPosition();
		if (newPosition != startingPosition) {
			waitedForSwing = false;
		}

	}

	private IEnumerator DoWaitForBackswing() {
		yield return new WaitForSeconds(backswingSpeed);
		var newPosition = caster.GetBodyPosition();
		if (newPosition != startingPosition) {
			waitedForBackswing = false;
		}
	}

	private void Fire() {
		GameObject projectileObject = Instantiate(projectilePrefab,
			spawnerObject.transform.position,
			spawnerObject.transform.rotation,
			caster.transform);

		ProjectileBehaviour projectile = projectileObject.GetComponent<ProjectileBehaviour>();
		projectile.Initialize(caster, target);

		//SetImmobile(0.3f);	
	}

	// if you were patient enough to stand still during the entire attack animation...
	private void ResumeMovement() {
		if (caster.IsReadyForNav()) {
			caster.MoveTo(storedDestination);
		}
	}
}
