using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/DoSprint")]
public class DoSprint : AiState {

	[Header("DoSprint")]
	public AbilitySlots abilitySlot;
	public StatusEffect sprintBuff;
	private Ability ability;

	public override void Reset() {
		base.Reset();
		desire = (int) Desire.MAX;
		abilitySlot = AbilitySlots.ABILITY_1;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);	
		ability = unit.GetAbilityInSlot(abilitySlot);

		if (ability == null)
			Debug.Log("Error: AiState set to null ability");
	}

	public override void Evaluate() {
		if (ability.IsCooldownReady() && unit.HasStatusEffect(StatusEffectTypes.WELL_FED) && !unit.HasStatusEffect(sprintBuff)) {
			desire = desireDefault;
		}

		else
			desire = (int) Desire.NONE;

	}

	public override void Execute() {
		base.Execute();
		unit.DoAbility(abilitySlot);
	}

	public override void Update() {
		base.Update();
	}

	public override void End() {
		base.End();

	}

}
