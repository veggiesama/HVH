﻿using Tree = HVH.Tree;
using Outline = cakeslice.Outline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class BodyController : MonoBehaviour {

	private Rigidbody rb;
	[HideInInspector] public UnitController unit;
	[HideInInspector] public Animator anim;

	[HideInInspector] public GameObject clickableHitbox;
	[HideInInspector] public GameObject projectileSpawner;
	[HideInInspector] public GameObject head;
	[HideInInspector] public GameObject mouth;
	[HideInInspector] public GameObject feet;
	[HideInInspector] public Renderer[] bodyMeshes;
	[HideInInspector] public Camera allyCam;
	[HideInInspector] public Camera targetCam;
	private Outline[] outlineScripts;
	private VisibilityState localVisibilityState;

	[HideInInspector] public Material[] originalBodyMeshMaterials;
	[HideInInspector] public Color[] originalBodyMeshColors;
	[HideInInspector] public FieldOfView fov;

	[HideInInspector] public Material invisMaterial;
	[HideInInspector] public Color[] fadeColors;

	[HideInInspector] public Collider bodyCollider;
	[HideInInspector] public Collider[] ragdollColliders;
	[HideInInspector] public Rigidbody[] ragdollRigidbodies;

	[HideInInspector] public MinimapIcon minimapIcon;

	private Vector3 lastPosition = Vector3.zero;
	private float updateAnimationSpeedFloatEvery = 0.1f;

	public UnityEventCollision onCollidedTerrain;

	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Events
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public void OnCollisionEnter(Collision collision) {
		GameObject o = collision.collider.gameObject;

		if (Util.IsTerrain(o)) {
			onCollidedTerrain.Invoke(collision);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Initialization
	////////////////////////////////////////////////////////////////////////////////////////////////////
	void Awake () {
		unit = GetComponentInParent<UnitController>();
		rb = GetComponent<Rigidbody>();
		fov = GetComponentInChildren<FieldOfView>();
		bodyCollider = GetComponent<Collider>();
		minimapIcon = GetComponentInChildren<MinimapIcon>();

		StartCoroutine( UpdateAnimationSpeedFloat() );
		//InvokeRepeating("UpdateAnimationSpeedFloat", 0f, updateAnimationSpeedFloatEvery);
	}

	public void ResetAnimator() {
		if (anim != null)
			Destroy(anim.gameObject);
				
		GameObject animGO = Instantiate(unit.unitInfo.animationPrefab, this.transform);
		anim = animGO.GetComponentInChildren<Animator>();
		ragdollColliders = anim.GetComponentsInChildren<Collider>();
		ragdollRigidbodies = anim.GetComponentsInChildren<Rigidbody>();
		EnableRagdoll(false);

		BodyLocationFinder finder = anim.GetComponent<BodyLocationFinder>();
		projectileSpawner = finder.projectileSpawner;
		clickableHitbox = finder.clickableHitbox;
		head = finder.head;
		mouth = finder.mouth;
		feet = finder.feet;
		bodyMeshes = finder.bodyMeshes;
		invisMaterial = finder.invisMaterial;
		allyCam = finder.allyCam;
		targetCam = finder.targetCam;
		
		// NOTE: assumes only 1 material per body mesh
		originalBodyMeshMaterials = new Material[bodyMeshes.Length];
		originalBodyMeshColors = new Color[bodyMeshes.Length];
		outlineScripts = new Outline[bodyMeshes.Length];
		fadeColors = new Color[bodyMeshes.Length];

		for (int i = 0; i < bodyMeshes.Length; i++) {
			outlineScripts[i] = bodyMeshes[i].GetComponent<Outline>();
			originalBodyMeshMaterials[i] = bodyMeshes[i].material;
			originalBodyMeshColors[i] = bodyMeshes[i].material.color;
			fadeColors[i] = Util.CreateFadedColor(bodyMeshes[i].material.color);
		}

	}

	private void FixedUpdate() {
		if (unit.IsMouseLooking()) {
			Vector3 mousePosition = unit.player.GetMouseLocationToGround();
			unit.body.FixedUpdate_ForceTurn(mousePosition);
		}
	}

	IEnumerator UpdateAnimationSpeedFloat() {
		while (true) {
			yield return new WaitForSeconds(updateAnimationSpeedFloatEvery);
			if (anim == null) continue;

			float speed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;
			
			lastPosition = transform.position;
			SetAnimationSpeedFloat(speed);

			//if (speed > Constants.OnMoveTolerance) {
			//	unit.onMoved.Invoke();
			//}
		}
	}


	// performances
	public void PerformDeath(Vector3 killerLocation, float killingDmg) {
		EnableRagdoll(true);
		float upwardMagnitude = Random.Range(50f, 150f);
		float impactMagnitude = Mathf.Min(killingDmg, 100) * 10f; //  400f
		AddForceToRagdoll(killerLocation, upwardMagnitude, impactMagnitude);
	}

	public void AddForceToRagdoll(Vector3 killerLocation, float upwardMagnitude, float impactMagnitude) {
		Vector3 upwardForce = Vector3.up * upwardMagnitude;
		Vector3 impactForce = default;

		if (killerLocation != default) {
			Debug.DrawLine(transform.position, killerLocation, Color.green, 3.0f);
			impactForce = (transform.position - killerLocation).normalized * impactMagnitude;
		}

		foreach (Rigidbody ragdollRB in ragdollRigidbodies) {
			ragdollRB.AddForce(upwardForce);
			if (killerLocation != default) {
				ragdollRB.AddForce(impactForce);
			}
		}
	}

	public void PerformLaunch(Vector3 velocityVector) {
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
	}

	public void PerformAirborn(Vector3 velocityVector) {
		rb.isKinematic = false;
		rb.AddForce(velocityVector, ForceMode.VelocityChange);
	}

	public void PerformSink() {
		rb.velocity = rb.velocity * 0.05f;
	}

	//public void SetTreeClipOnly() {
	//	gameObject.layer = LayerMask.NameToLayer("PhysicsIgnoreTrees"); 
	//}

	public void SetNoclip() {
		gameObject.layer = (int)LayerBits.BODY_NOCLIP;
	}

	public void SetDefaultClip() {
		gameObject.layer = (int)LayerBits.BODY;
	}

	public void SetTreeClip() {
		gameObject.layer = (int)LayerBits.BODY_IGNORINGTREES;
	}

	public void ResetBody() {
		bodyCollider.enabled = true;
		EnableRagdoll(false);
		rb.constraints = RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY |
			RigidbodyConstraints.FreezeRotationZ;
		rb.isKinematic = true;
		SetDefaultClip();
		EnableClickableHitbox(true);
	}

	public void FixedUpdate_ForceTurn(Vector3 targetPosition) {
		float turnDampening = 0.5f;

		targetPosition.y = transform.position.y; // adjust to same plane
		Quaternion wantedRotation = Quaternion.LookRotation(targetPosition - transform.position);
		float step = unit.unitInfo.turnRate * Time.fixedDeltaTime * turnDampening;
		unit.body.transform.rotation = Quaternion.RotateTowards(unit.body.transform.rotation, wantedRotation, step);
									// Quaternion.Lerp(unit.body.transform.rotation, wantedRotation, percentageComplete);
	}


	public bool IsFacing(Vector3 targetPosition) {
		if (targetPosition.Equals(default)) return true;

		targetPosition.y = transform.position.y; // adjust to same plane
		Quaternion wantedRotation = Quaternion.LookRotation(targetPosition - transform.position);
		return Quaternion.Angle(transform.rotation, wantedRotation) < Constants.FrontAngle;
	}

	public void SetVisibilityState(VisibilityState state) {
		if (anim == null) return;

		localVisibilityState = state;

		Player localPlayer = GameResources.Instance.GetLocalPlayer();

		if (localPlayer != null) {
			UnitController localUnit = localPlayer.unit;
			UnitController localEnemyTarget = localUnit.GetTarget(AbilityTargetTeams.ENEMY);

			if (state == VisibilityState.VISIBLE && localUnit.IsForgottenTarget(unit)) {
				localUnit.RememberTarget();
			}

			if (state == VisibilityState.INVISIBLE && unit == localEnemyTarget) {
				localUnit.ForgetTarget();
			}
		}

		for (int i = 0; i < bodyMeshes.Length; i++) {
			Renderer rend = bodyMeshes[i];

			switch (state) {

				case VisibilityState.VISIBLE:
					EnableMinimapIcon(true);
					bodyMeshes[i].enabled = true;
					outlineScripts[i].enabled = true;
					rend.material = originalBodyMeshMaterials[i];
					if (rend.material.HasProperty("_Color")) {
						rend.material.color = originalBodyMeshColors[i];
					}

					break;

				case VisibilityState.VISIBLE_TO_TEAM_ONLY:
					EnableMinimapIcon(true);
					bodyMeshes[i].enabled = true;
					outlineScripts[i].enabled = false;
					rend.material = invisMaterial;
					break;

				case VisibilityState.FADING:
					EnableMinimapIcon(true);
					bodyMeshes[i].enabled = true;
					outlineScripts[i].enabled = true;
					rend.material = originalBodyMeshMaterials[i];
					if (rend.material.HasProperty("_Color")) {
						rend.material.color = fadeColors[i];
					}
					break;

				case VisibilityState.INVISIBLE:
					EnableMinimapIcon(false);
					bodyMeshes[i].enabled = false;
					outlineScripts[i].enabled = false;
					break;

			}

		}

	}

	public void AppearInFOV(bool enable) {
		UnitController localUnit = GameResources.Instance.GetLocalPlayer().unit;
		if (unit.SharesTeamWith(localUnit)) return;
		
		if (enable) {
			if (!unit.HasStatusEffect(StatusEffectTypes.INVISIBLE) || unit.HasStatusEffect(StatusEffectTypes.REVEALED)) {
				SetVisibilityState(VisibilityState.VISIBLE);
			}
		}

		else {
			SetVisibilityState(VisibilityState.INVISIBLE);
		}
	}

	public VisibilityState GetVisibilityState() {
		return localVisibilityState;
	}

	public string GetVisibilityStateToString() {
		return System.Enum.GetName(typeof(VisibilityState), (int)localVisibilityState);
	}

	// visible from unit's perspective (important for hosts and AI)
	public bool IsVisibleToUnit(UnitController viewer) {
		if (unit.SharesTeamWith(viewer))
			return true;
		else if (localVisibilityState == VisibilityState.INVISIBLE ||
				 localVisibilityState == VisibilityState.VISIBLE_TO_TEAM_ONLY)
			return false;
		else
			return true;
	}

	// visible on client's computer
	public bool IsVisible() {
		return (localVisibilityState != VisibilityState.INVISIBLE);
	}

	public void ApplyMaterial(Material mat) {
		foreach (Renderer rend in bodyMeshes) {
			rend.material = mat;
		}
	}

	public void ResetMaterial() {
		for (int i = 0; i < bodyMeshes.Length; i++) {
			bodyMeshes[i].material = originalBodyMeshMaterials[i];
		}
	}

	public void SetColor(Color color) {
		for (int i = 0; i < bodyMeshes.Length; i++) {
			bodyMeshes[i].material.color = color;
			originalBodyMeshColors[i] = color;
			fadeColors[i] = Util.CreateFadedColor(color);
		}
	}

	public void SetAnimationSpeedFloat(float speed) {
		anim.SetFloat(AnimFloats.SPEED, speed);
	}

	public void PlayAnimation(Animations a) {
		if (anim == null) return;

		switch (a) {
			case Animations.ATTACK_A:
				anim.SetTrigger(AnimTriggers.ATTACK);
				anim.SetTrigger(AnimTriggers.ATTACK_A);
				break;
			case Animations.ATTACK_B:
				anim.SetTrigger(AnimTriggers.ATTACK);
				anim.SetTrigger(AnimTriggers.ATTACK_B);
				break;
			case Animations.CAST_A:
				anim.SetTrigger(AnimTriggers.CAST);
				anim.SetTrigger(AnimTriggers.CAST_A);
				break;
			case Animations.CAST_B:
				anim.SetTrigger(AnimTriggers.CAST);
				anim.SetTrigger(AnimTriggers.CAST_B);
				break;
			case Animations.DIE:
				anim.SetBool(AnimBools.DEAD, true);
				anim.SetTrigger(AnimTriggers.DIE);
				break;
			case Animations.RESPAWN:
				anim.SetBool(AnimBools.DEAD, false);
				break;
			case Animations.LAYDOWN:
				anim.SetBool(AnimBools.RESTING, true);
				anim.SetTrigger(AnimTriggers.REST);
				break;
			case Animations.WAKEUP:
				anim.SetBool(AnimBools.RESTING, false);
				break;
			default:
				break;
		}
	}

	public void EnableRagdoll(bool enable) {
		if (anim == null) return;

		foreach (Rigidbody ragdollRB in ragdollRigidbodies) {
			if (enable) {
				ragdollRB.velocity = rb.velocity;
				ragdollRB.angularVelocity = rb.angularVelocity;
			}

			ragdollRB.isKinematic = !enable;
		}

		foreach (Collider col in ragdollColliders) {
			col.enabled = enable;
		}

		anim.enabled = !enable;
	}

	public void EnableClickableHitbox(bool enable) {
		if (clickableHitbox == null) return;
		clickableHitbox.SetActive(enable);
	}

	public void EnableMinimapIcon(bool enable) {
		minimapIcon.gameObject.SetActive(enable);
	}

}