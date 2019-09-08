using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/DoWarn")]
public class DoWarn : AiState {

	[Header("DoWarn")]
	public AbilitySlots abilitySlot;
	private Ability ability;
	private float warnRadius = 12f;

	public override void Reset() {
		base.Reset();
		desire = (int) Desire.HIGH;
		abilitySlot = AbilitySlots.ABILITY_3;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);	
		ability = unit.GetAbilityInSlot(abilitySlot);

		if (ability == null)
			Debug.Log("Error: AiState set to null ability");
	}

	public override void Evaluate() {
		if (ability.IsCooldownReady() && AiUtil.DetectNearbyEnemies(unit, warnRadius)) {
			desire = desireDefault;
		}

		else
			desire = (int) Desire.NONE;
	}

	public override void Execute() {
		base.Execute();
		unit.DoAbility(abilitySlot);
	}

}
