using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Invisible")]
public class Invisible : StatusEffect {

	private Material originalMat;
	public Material transMat;
	private Renderer rend;
	private bool isRevealed = false;
	private bool isFading = false;
	private float fadeTimer = 0;
	private Color originalColor;
	private Color transColor;

	[Header("Invisibility fade duration")]
	public float fadeDuration;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Invisible";
		type = StatusEffectTypes.INVISIBLE;
		duration = 60f;
		fadeDuration = 1f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability, UnitController inflictor) {
		base.Initialize(obj, ability, inflictor);

		rend = unit.body.GetComponent<Renderer>();
		transMat = ReferenceManager.Instance.MaterialsLibrary.invisibilityMaterial;
		originalMat = rend.material;
		if (rend.material.HasProperty("_Color")) {
			originalColor = rend.material.GetColor("_Color");
			transColor = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f);
		}
	}

	public override void Apply()
	{
		base.Apply();
		StartFadeToHide();
	}

	// Unit is fading => Count down fade timer and hide again (fade timer resets if revealed again)
	// Unit gains REVEALED status => show unit
	// Unit had REVEALED status but lost it => Start fading
	public override void Update() {
		base.Update(); // tracks duration

		if (isFading == true) {
			UpdateFadeTimer();
			if (fadeTimer <= 0) {
				isFading = false;
				HideModel();
			}
		}

		else if (unit.HasStatusEffect(StatusEffectTypes.REVEALED)) {
			if (isRevealed) {}
			else {
				isRevealed = true;
				ShowModel();
				return;
			}
		}
		else {
			if (isRevealed) {
				isRevealed = false;
				StartFadeToHide();
				return;
			}
			else {}
		}
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		base.Stack(status);
	}

	public override void End() {
		ShowModel();
		base.End();
	}


	// HELPERs
	private void UpdateFadeTimer() {
		if (unit.HasStatusEffect(StatusEffectTypes.REVEALED)) { // TODO: does this really need to update every frame?
			ShowModel();
			isFading = false;
			isRevealed = true;
		}
		else
			fadeTimer -= Time.deltaTime; // count-down
	}
	
	private void StartFadeToHide() {
		isFading = true;
		fadeTimer = fadeDuration;
		rend.material.color = transColor;
	}

	private void HideModel() {
		rend.material = transMat;
		// TODO: logic regarding which team you're on
		//rend.enabled = false;
	}

	private void ShowModel() {
		rend.material = originalMat;
		rend.material.color = originalColor;
		//isTransitioning = true;
		//transitionTimer = transitionDuration;
		//rend.enabled = true;
	}
}
