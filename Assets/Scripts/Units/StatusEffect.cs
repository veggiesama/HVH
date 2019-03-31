using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject {

	public string statusName;
	public StatusEffectTypes type;

	public float duration;
	public bool hideIcon;
	public bool dispellable;
	
	[Header("Runtime")]
	public bool applied;
	public float remainingTime;
	private float startTime;

	protected StatusEffectManager statusEffectManager;
	protected UnitController unit;
	protected UnitController inflictor;
	protected Ability ability;

	public abstract void Reset();

	public virtual void Initialize(GameObject obj, Ability ability, UnitController inflictor) {
		this.statusEffectManager = obj.GetComponentInChildren<StatusEffectManager>();
		this.unit = obj.GetComponentInParent<UnitController>();
		this.inflictor = inflictor;
		this.ability = ability;
	}
	
	public virtual void Apply() {
		startTime = Time.time;
		remainingTime = duration;
		applied = true;
	}

	public virtual void Update() {
		if (this.type != StatusEffectTypes.DEAD && unit.HasStatusEffect(StatusEffectTypes.DEAD)) {
			End();
			return;
		}

		if (duration > 0) {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0)
				End();
		}
	}

	public abstract void FixedUpdate();

	// by default, duration resets but nothing else carries over
	// TODO: could be more granular depending on the type of effect, counts, inflictor, etc.
	public virtual void Stack(StatusEffect status) {
		remainingTime = duration;
	}
	
	public virtual void End() {
		statusEffectManager.AddToRemovalList(this);
	}

}
 