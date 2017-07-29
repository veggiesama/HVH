using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Attack logic:
// 1. initiate attack and wait for swing
// 2. if swing successful (ie: wait successful), fire the attack
// 3. wait for backswing (freely interruptable with no penalty)
// 4. continue destination movement

public class AbilityFire : AbilityController {

	public float projectileSpeed;
	private GameObject spawnerObject, projectilePrefab;
	private Vector3 startingPosition, storedDestination;
	private float swingSpeed, backswingSpeed;

	private bool waitedForSwing, waitedForBackswing;

	private CastbarController castbar;

	protected override void Start() {
		base.Start();
		castbar = GameObject.Find("CastbarContainer").GetComponent<CastbarController>();
	}

	// you must initialize after instantiation
	public override void Cast() {
		base.Cast();

		if (enemyTarget == null)
			return;
		
		waitedForSwing = true;
		waitedForBackswing = true;

		startingPosition = caster.GetBodyPosition();
		swingSpeed = caster.attackInfo.swingSpeed;
		backswingSpeed = caster.attackInfo.backswingSpeed;

		spawnerObject = caster.attackInfo.spawnerObject;
		projectilePrefab = caster.attackInfo.projectilePrefab;

		StartCoroutine( DoAttackLogic() );
	}

	private IEnumerator DoAttackLogic() {
		PauseMovement();
		castbar.Initialize(caster, swingSpeed, backswingSpeed);

		yield return StartCoroutine( DoWaitForSwing() );
		
		if (waitedForSwing)
			Fire();
		else
			yield break;

		yield return StartCoroutine( DoWaitForBackswing() );
		
		if (waitedForBackswing)
			ResumeMovement();
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
		projectile.Initialize(this, enemyTarget);
	}

	// if you were patient enough to stand still during the entire attack animation...
	private void ResumeMovement() {
		if (caster.IsReadyForNav()) {
			caster.MoveTo(storedDestination);
		}
	}



}
