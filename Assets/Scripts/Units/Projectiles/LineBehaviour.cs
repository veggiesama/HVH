using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class LineBehaviour : ProjectileBehaviour {

	private LineRenderer line;
	private float lineLength;
	private float lineLengthLast;

	public override void Initialize(Ability ability, Vector3 targetLocation) {
		base.Initialize(ability, targetLocation);
		line = GetComponent<LineRenderer>();

		transform.position = Vector3.zero; // lines draw relative to transform, so we need to zero out transform first
		transform.rotation = Quaternion.identity;

		line.SetPosition(0, attacker.body.projectileSpawner.transform.position);
		line.SetPosition(1, targetLocation);
		UpdateLineLength();

		((Grapple)ability).onLineBreak.AddListener(BreakLine);
	}

	protected override void FixedUpdate () {
		if (!initialized) return;
		base.FixedUpdate();

		line.SetPosition(0, attacker.body.projectileSpawner.transform.position);
		UpdateLineLength();

		if (lineLengthLast == 0 || lineLength <= lineLengthLast) // rope is shrinking
			lineLengthLast = lineLength;
		else { // rope snaps
			BreakLine();
		}
	}

	private void UpdateLineLength() {
		lineLength = Util.GetDistanceIn2D(line.GetPosition(0), line.GetPosition(1));
	}

	private void BreakLine() {
		((Grapple)ability).onLineBreak.RemoveListener(BreakLine);

		//networkHelper.DestroyTree(castOrder.tree, caster.GetBodyPosition(), 0);
		if (line != null)
			NetworkServer.Destroy(this.gameObject);
		
	}
}
