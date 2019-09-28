using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject {

	[Header("Runtime")]
	public bool applied;
	public float remainingTime;
	private float startTime;

	protected StatusEffectManager statusEffectManager;
	public UnitController unit;
	public Ability ability;
	protected NetworkHelper networkHelper;

	[Header("Modifiable")]
	public string statusName = "None";
	public Sprite icon;
	public StatusEffectTypes type = StatusEffectTypes.WELL_FED;

	public float duration = 0f;
	public bool overrideAbilityDuration = false;
	public bool hideIcon = false;
	public bool dispellable = false;
	
	public virtual void Reset() {}

	public virtual void Initialize(GameObject obj, Ability ability) {
		this.statusEffectManager = obj.GetComponentInChildren<StatusEffectManager>();
		this.networkHelper = obj.GetComponentInParent<NetworkHelper>();
		this.unit = obj.GetComponentInParent<UnitController>();
		this.ability = ability;

		if (ability != null && !overrideAbilityDuration)
			this.duration = ability.duration;
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

	public virtual void FixedUpdate() {}

	// by default, duration resets but nothing else carries over
	// TODO: could be more granular depending on the type of effect, counts, inflictor, etc.
	public virtual void Stack(StatusEffect status) {
		remainingTime = duration;
	}
	
	public virtual void End() {
		statusEffectManager.AddToRemovalList(this);
	}

}
 