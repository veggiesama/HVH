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
	private float ropeLengthLast = 999f;

	public override void Reset()
	{
		abilityName = "Grapple";
		targetType = AbilityTargetTypes.POINT;
		targetTeam = AbilityTargetTeams.NONE;
		damageType = DamageTypes.NONE;
		damage = 0;
		cooldown = 1.0f;
		castRange = 100f;
		castTime = 0f;
		duration = 0f;
		bypassOrderQueue = false;

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
		
		Vector3 targetLocationAdjusted = castOrder.targetLocation;
		targetLocationAdjusted.y = caster.GetBodyPosition().y;
		Vector3 direction = targetLocationAdjusted - caster.GetBodyPosition(); // to - from = direction

		int layerMask = LayerMask.GetMask("Tree");
		if (Physics.Raycast(caster.GetBodyPosition(), direction, out RaycastHit hit, castRange, layerMask)) {
			Debug.DrawLine(caster.GetBodyPosition(), direction * castRange, Color.green, 2.0f);
			Vector3 fromAnchor = hit.collider.GetComponent<TreeAnchor>().GetAnchorPoint();
			Debug.DrawLine(caster.GetBodyPosition(), fromAnchor, Color.yellow, 2.0f);

			BeginLaunch(fromAnchor);

		}
		else {
			Debug.DrawRay(caster.GetBodyPosition(), direction * castRange, Color.red, 2.0f);
		}

		return CastResults.SUCCESS;
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
	}

	// allows body to hilariously bounce
	private void OnBodyCollision(Collision col) {
		if (Util.GetDistanceIn2D(debugVecStart, caster.GetBodyPosition()) < 1.0f) return;

		float magnitude = col.relativeVelocity.magnitude;
		Debug.Log("MAGNITUDE:" + magnitude);
		currentSafetyTimer = safetyPostCollisionTimer; // if body lands too softly after bounce, end launch through Update()

		if (col.gameObject.layer == LayerMask.NameToLayer("Terrain") &&
			magnitude < collisionMagnitudeThreshold)
		{
			EndLaunch();
		}
	}

	public override void Update() {
		base.Update();

		if (!isLaunching) return;

		if (rope != null) {
			rope.SetPosition(0, caster.attackInfo.spawnerObject.transform.position);
			ropeLength = Util.GetDistanceIn2D(rope.GetPosition(0), rope.GetPosition(1));

			if (ropeLength <= ropeLengthLast) // rope is shrinking
				ropeLengthLast = ropeLength;
			else { // rope snaps
				Destroy(rope);
				ropeLengthLast = 999f;
			}
		}

		if (currentSafetyTimer > 0) {
			currentSafetyTimer -= Time.deltaTime;
			if (currentSafetyTimer <= 0) {
				EndLaunch();
			}
		}
	}
}
