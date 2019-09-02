using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/DoSleep")]
public class DoSleep : AiState {

	[Header("DoSleep")]
	public AbilitySlots abilitySlot;
	public StatusEffect loyaltyBuff;
	private Ability ability;

	public override void Reset() {
		base.Reset();
		desire = (int) Desire.MEDIUM;
		abilitySlot = AbilitySlots.ABILITY_1;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);	
		ability = unit.GetAbilityInSlot(abilitySlot);

		if (ability == null)
			Debug.Log("Error: AiState set to null ability");
	}

	public override void Evaluate() {
		if (!unit.HasStatusEffect(loyaltyBuff)) {
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
