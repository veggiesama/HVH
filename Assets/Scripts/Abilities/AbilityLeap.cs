using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLeap : AbilityController {

	public float duration;
	public float forceUpwards;
	public float forceForwards;

	public override void Cast() {
		StartCoroutine( Jump() );
	}

	public IEnumerator Jump() {
		if (!caster.IsReadyForNav())
			yield break;

		caster.StartJump(forceUpwards, forceForwards);
		yield return new WaitForSeconds(duration);
		caster.EndJump();
	}

}
