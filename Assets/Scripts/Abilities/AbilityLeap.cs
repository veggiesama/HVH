using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLeap : AbilityController {

	public float forceUpwards;
	public float forceForwards;

	public override void Cast() {
		base.Cast();
		StartCoroutine( Jump() );
	}

	public IEnumerator Jump() {
		if (!caster.IsReadyForNav())
			yield break;

		caster.StartJump(forceUpwards, forceForwards);
		yield return new WaitForSeconds(abilityInfo.duration);
		caster.EndJump();
	}

}
