using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Grapple")]
public class Grapple : Ability {

	[Header("Grapple")]
	public StatusEffect airbornStatusEffect;
	public float launchForce;
	public float collisionMagnitudeThreshold;
	public float safetyPostCollisionTimer;
	private float currentSafetyTimer = 0;
	private bool isLaunching = false;
	private Vector3 debugVecStart;
	public LineRenderer ropePrefab;
	private LineRenderer rope;
	private float ropeLength = 0f;
	private float ropeLengthLast = 0f;

	public override void Reset()
	{
		abilityName = "Grapple";
		targetType = AbilityTargetTypes.TREE;
		targetTeam = AbilityTargetTeams.NONE;
		damageType = DamageTypes.NONE;
		damage = 0;
		cooldown = 1.0f;
		castRange = 100f;
		castTime = 0f;
		duration = 0f;
		doNotCancelOrderQueue = false;

		projectilePrefab = null;
		projectileSpeed = 5f;
		projectileTimeAlive = 10f;
		grenadeTimeToHitTarget = 0;

		launchForce = 20f;
		collisionMagnitudeThreshold = 10f;
		safetyPostCollisionTimer = 2.0f;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
	}

	// you must initialize after instantiation
	public override CastResults Cast(Order castOrder) {
		CastResults baseCastResults = base.Cast(castOrder);
		if (baseCastResults != CastResults.SUCCESS) return baseCastResults;
		if (caster.IsAirborn()) return CastResults.FAILURE_ALREADY_AIRBORN;

		Vector3 fromAnchor = castOrder.tree.GetAnchorPoint();
		Debug.DrawLine(caster.GetBodyPosition(), fromAnchor, Color.yellow, 2.0f);
		BeginLaunch(fromAnchor);

		return CastResults.SUCCESS;
	}

	public override void Update() {
		base.Update();

		//Debug.Log("Current safety timer: " + currentSafetyTimer);

		if (!isLaunching) return;

		if (rope != null) {
			UpdateRope();
		}

		if (currentSafetyTimer > 0) {
			currentSafetyTimer -= Time.deltaTime;
			if (currentSafetyTimer <= 0) {
				EndLaunch();
			}
		}
	}

	private void BeginLaunch(Vector3 anchor) {
		caster.ApplyStatusEffect(airbornStatusEffect, this, caster);
		Vector3 velocityVector = (anchor - caster.GetBodyPosition()).normalized * launchForce;
		//caster.EnableNav(false);
		caster.body.PerformAirborn(velocityVector);
		caster.body.SetTreeClipOnly();
		caster.body.OnCollisionEventHandler += OnBodyCollision; // event sub
		isLaunching = true;

		debugVecStart = caster.GetBodyPosition();

		rope = Instantiate(ropePrefab);
		rope.SetPosition(0, caster.attackInfo.spawnerObject.transform.position);
		rope.SetPosition(1, anchor);
	}

	private void EndLaunch() {
		caster.RemoveStatusEffect(airbornStatusEffect.statusName);
		//caster.EnableNav(true);
		caster.body.ResetBody();
		caster.body.OnCollisionEventHandler -= OnBodyCollision; // event unsub
		isLaunching = false;

		Vector3 debugVecEnd = caster.GetBodyPosition();
		Debug.Log("Launch distance: " + Util.GetDistanceIn2D(debugVecStart, debugVecEnd));
		
		// cleanup
		currentSafetyTimer = 0;
		ropeLengthLast = 0;
		DestroyRope();
	}

	// allows body to hilariously bounce
	private void OnBodyCollision(Collision col) {
		if (Util.GetDistanceIn2D(debugVecStart, caster.GetBodyPosition()) < 1.0f) return;

		float magnitude = col.relativeVelocity.magnitude;
		//Debug.Log("MAGNITUDE:" + magnitude);
		currentSafetyTimer = safetyPostCollisionTimer; // if body lands too softly after bounce, end launch through Update()

		if (col.gameObject.layer == LayerMask.NameToLayer("Terrain") &&
			magnitude < collisionMagnitudeThreshold)
		{
			EndLaunch();
		}
	}

	private void DestroyRope() {
		if (rope != null) Destroy(rope);
		ropeLengthLast = 0f;
	}

	private void UpdateRope() {
		rope.SetPosition(0, caster.attackInfo.spawnerObject.transform.position);
		ropeLength = Util.GetDistanceIn2D(rope.GetPosition(0), rope.GetPosition(1));

		if (ropeLengthLast == 0 || ropeLength <= ropeLengthLast) // rope is shrinking
			ropeLengthLast = ropeLength;
		else { // rope snaps
			DestroyRope();
			castOrder.tree.DestroyThisTree();
		}
	}
}
