using Tree = HVH.Tree;
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

	//private bool isVisible = true;
	private Vector3 lastPosition = Vector3.zero;
	private float updateAnimationSpeedFloatEvery = 0.1f;

	public UnityEventTree onCollidedTree;
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
		invisMaterial = unit.GetComponent<UnitMaterials>().invisibility;
		bodyCollider = GetComponent<Collider>();

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
		head = finder.head;
		mouth = finder.mouth;
		feet = finder.feet;
		bodyMeshes = finder.bodyMeshes;
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
	public void PerformDeath(Vector3 killedFromDirection) {
		//rb.isKinematic = false;
		//rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;



		//rb.AddForce(Vector3.up * upwardMagnitude);
		//if (!Util.IsNullVector(killedFromDirection))
		//	rb.AddForce((transform.position - killedFromDirection).normalized * impactMagnitude);
		
		//PlayAnimation(Animations.DIE);

		EnableRagdoll(true);
		//bodyCollider.enabled = false;

		float upwardMagnitude = Random.Range(50f, 150f);
		float impactMagnitude = 400f;
		AddForceToRagdoll(killedFromDirection, upwardMagnitude, impactMagnitude);
	}

	public void AddForceToRagdoll(Vector3 fromDirection, float upwardMagnitude, float impactMagnitude) {
		Vector3 upwardForce = Vector3.up * upwardMagnitude;
		Vector3 impactForce = default;

		if (fromDirection != default) {
			Debug.DrawLine(transform.position, fromDirection, Color.green, 3.0f);
			impactForce = (transform.position - fromDirection).normalized * impactMagnitude;
		}

		foreach (Rigidbody ragdollRB in ragdollRigidbodies) {
			ragdollRB.AddForce(upwardForce);
			if (fromDirection != default) {
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

	//public void SetTreeClipOnly() {
	//	gameObject.layer = LayerMask.NameToLayer("PhysicsIgnoreTrees"); 
	//}

	public void SetNoclip() {
		gameObject.layer = LayerMask.NameToLayer("PhysicsNoclip"); 
	}

	public void SetDefaultClip() {
		gameObject.layer = LayerMask.NameToLayer("Body");
	}

	public void ResetBody() {
		bodyCollider.enabled = true;
		EnableRagdoll(false);
		rb.constraints = RigidbodyConstraints.FreezeRotationX |
			RigidbodyConstraints.FreezeRotationY |
			RigidbodyConstraints.FreezeRotationZ;
		rb.isKinematic = true;
		SetDefaultClip();
		PlayAnimation(Animations.RESPAWN);
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

		UnitController localUnit = GameResources.Instance.GetLocalPlayer().unit;
		UnitController localEnemyTarget = localUnit.GetTarget(AbilityTargetTeams.ENEMY);

		if (state == VisibilityState.VISIBLE && localUnit.IsForgottenTarget(unit)) {
			localUnit.RememberTarget();
		}

		if (state == VisibilityState.INVISIBLE && unit == localEnemyTarget) {
			localUnit.ForgetTarget();
		}

		for (int i = 0; i < bodyMeshes.Length; i++) {
			Renderer rend = bodyMeshes[i];

			switch (state) {

				case VisibilityState.VISIBLE:
					bodyMeshes[i].enabled = true;
					outlineScripts[i].enabled = true;
					rend.material = originalBodyMeshMaterials[i];
					if (rend.material.HasProperty("_Color")) {
						rend.material.color = originalBodyMeshColors[i];
					}

					break;

				case VisibilityState.VISIBLE_TO_TEAM_ONLY:
					bodyMeshes[i].enabled = true;
					outlineScripts[i].enabled = false;
					rend.material = invisMaterial;
					break;

				case VisibilityState.FADING:
					bodyMeshes[i].enabled = true;
					outlineScripts[i].enabled = true;
					rend.material = originalBodyMeshMaterials[i];
					if (rend.material.HasProperty("_Color")) {
						rend.material.color = fadeColors[i];
					}
					break;

				case VisibilityState.INVISIBLE:
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

	public string GetVisibilityState() {
		return System.Enum.GetName(typeof(VisibilityState), (int)localVisibilityState);
	}

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

}