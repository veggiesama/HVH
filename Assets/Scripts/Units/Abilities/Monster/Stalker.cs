using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Monster/Stalker")]
public class Stalker : Ability {

	[Header("Stalker")]
	public float nightMoveSpeedBonus;
	public float nightSwipeCooldownReduction;
	public float invisDelay;
	public float invisDelayAfterDamage;
	public float invisDelayAfterMoving;
	public int minimumDmgToBreakInvis; // above hound dmg, below sniper dmg
	public float invisRegenPercentPerSec;
	public float killRegenDuration;
	public float killRegenPercentPerSec;
	public StatusEffect nightStatusEffect;
	public StatusEffect invisStatusEffect;

	public override void Reset() {
		abilityName = "Stalker";
		isPassive = true;
	}

	public override void Initialize(GameObject obj) {
		base.Initialize(obj);
		SetCooldown(invisDelay);

		OnStartDay();
		DayNight.Instance.onStartDay.AddListener(OnStartDay);
		DayNight.Instance.onStartNight.AddListener(OnStartNight);
	}

	/*
	public override void OnDisable() {
		base.OnDisable();
		if (DayNight.Instance != null
		  && DayNight.Instance.onStartDay != null
		  && DayNight.Instance.onStartNight != null)
		{ 
			DayNight.Instance.onStartDay.RemoveListener(OnStartDay);
			DayNight.Instance.onStartNight.RemoveListener(OnStartNight);
		}
	}*/

	public override void Update() {
		base.Update();

		//if (caster.player == GameRules.Instance.GetLocalPlayer())
		//	Debug.Log("Cooldown: " + GetCooldown());

		if (caster.HasStatusEffect(StatusEffectTypes.DEAD))
			return;

		if (GetCooldown() <= 0 && !caster.HasStatusEffect(invisStatusEffect)) {
			caster.ApplyStatusEffect(invisStatusEffect);
		}
	}

	protected override void OnCastAbility() {
		ResetInvisDelay(invisDelay);
	}

	protected override void OnTakeDamage(float dmg) {
		if (dmg > minimumDmgToBreakInvis) {
			ResetInvisDelay(invisDelayAfterDamage);
		}
	}

	protected override void OnMoved() {
		if (DayNight.Instance.IsDay())
			ResetInvisDelay(invisDelayAfterMoving);
	}
	
	private void ResetInvisDelay(float delay) {
		if (caster.HasStatusEffect(invisStatusEffect)) {
			caster.RemoveStatusEffect(invisStatusEffect);
		}

		if (delay > GetCooldown())
			SetCooldown(delay);
	}

	private void OnStartDay() {
		caster.RemoveStatusEffect(nightStatusEffect);
	}

	private void OnStartNight() {
		caster.ApplyStatusEffect(nightStatusEffect, this);
	}

	//public override void OnKill() {
	//}

	public override void OnLearn() {

	}

	public override void OnUnlearn() {

	}

}