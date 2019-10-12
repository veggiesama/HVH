using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Dead")]
public class Dead : StatusEffect {

	private UnitController killer;
	private float killingDmg;

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Dead";
		type = StatusEffectTypes.DEAD;
		duration = 3f;
	}
	
	// initializer
	public void Initialize(GameObject obj, UnitController killer, float killingDmg) {
		base.Initialize(obj, null);
		this.killer = killer;
		this.killingDmg = killingDmg;
	}

	public override void Apply() {
		base.Apply();
		unit.CancelAllOrders();
		unit.SetOrderRestricted(true);

		Vector3 killerLocation = default;

		if (ability != null)
			killerLocation = ability.caster.GetBodyPosition();
		//else
		//	killerLocation = unit.GetBodyPosition() + Random.insideUnitSphere * 1.5f;
		
		unit.Die(killerLocation, killingDmg);
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		//base.Stack(status);
	}

	public override void End() {
		unit.Respawn();
		//networkHelper.Respawn();
		unit.SetOrderRestricted(false);
		base.End();
	}
}
