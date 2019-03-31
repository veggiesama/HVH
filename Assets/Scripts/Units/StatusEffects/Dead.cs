using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Dead")]
public class Dead : StatusEffect {

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Dead";
		type = StatusEffectTypes.DEAD;
		duration = 3f;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability, UnitController inflictor) {
		base.Initialize(obj, ability, inflictor);
	}

	public override void Apply()
	{
		base.Apply();
		unit.CancelAllOrders();
		unit.SetOrderRestricted(true);
		unit.DetachFromNav();
		unit.EnableNav(false);
		Vector3 killFromDirection;// = Util.GetNullVector();
		if (inflictor)
			killFromDirection = inflictor.GetBodyPosition();
		else
			killFromDirection = unit.GetBodyPosition() + Random.insideUnitSphere * 1.5f;
		
		unit.body.PerformDeath(killFromDirection);
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		//base.Stack(status);
	}

	public override void End() {
		Transform spawnLoc = GameController.GetRandomSpawnPoint();
		unit.body.transform.SetPositionAndRotation(spawnLoc.position, spawnLoc.rotation);
		unit.unitInfo.currentHealth = unit.unitInfo.maxHealth;

		unit.EnableNav(true);
		unit.body.ResetBody();
		unit.AttachToNav();
		unit.SetOrderRestricted(false);
		//agent.enabled = true;

		base.End();
	}
}
