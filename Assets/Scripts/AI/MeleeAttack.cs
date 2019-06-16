using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/MeleeAttack")]
public class MeleeAttack : AiState {

	[Header("MeleeAttack")]
	public AbilitySlots abilitySlot;
	private Ability ability;
	public float meleeDistance;

	public override void Reset() {
		base.Reset();
		desire = (int) Desire.HIGH;
		meleeDistance = 1.5f;
		abilitySlot = AbilitySlots.ATTACK;
	}

	public override void Initialize(AiManager aiManager) {
		base.Initialize(aiManager);	
		ability = unit.GetAbilityInSlot(abilitySlot);

		if (ability == null)
			Debug.Log("Error: AiState set to null ability");
	}

	public override void Evaluate() {
		if (!ability.IsCooldownReady()) {
			desire = (int) Desire.NONE;
			return;
		}

		UnitController target = unit.GetTarget(AbilityTargetTeams.ENEMY);

		if (target != null && Util.GetDistanceIn2D(unit.GetBodyPosition(), target.GetBodyPosition()) <= meleeDistance) {
			desire = desireDefault;
		}

		else
			desire = (int) Desire.NONE;

	}

	public override void Execute() {
		base.Execute();
		unit.owner.SetVirtualPointerLocation(unit.GetTarget(AbilityTargetTeams.ENEMY).GetBodyPosition());
		unit.DoAbility(abilitySlot);
	}

	public override void Update() {
		base.Update();

	}

	public override void End() {
		base.End();

	}

}
