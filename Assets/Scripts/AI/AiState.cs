﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiState : ScriptableObject {

	protected AiManager aiManager;
	protected UnitController unit;

	public bool hasExecuted;

	[Range(0, 100)]
	public int desire;
	protected int desireDefault;

	public bool hasDuration;
	public float duration;
	protected float currentTimer;

	void Reset() {
		desire = (int) Desire.MEDIUM;
	}

	public virtual void Initialize(AiManager aiManager) {
		this.aiManager = aiManager;
		unit = aiManager.unit;
		desireDefault = desire;
	}

	public virtual void Evaluate() {
	}

	public virtual void Execute() {
		hasExecuted = true;
		if (hasDuration)
			currentTimer = duration;
	}

	public virtual void Update() {
		if (currentTimer > 0) {
			currentTimer -= aiManager.updateEvery;
			if (currentTimer <= 0)
				desire = (int) Desire.NONE; //End();
		}
	}

	public virtual void End() {
		desire = desireDefault;
		hasExecuted = false;
	}

}
