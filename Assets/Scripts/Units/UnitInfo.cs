using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitInfo : NetworkBehaviour {
	public float maxHealth = 100.0f;
	public float healthRegen = 1.0f;
	public int armor = 0;
	public int resist = 0;
	public float movementSpeed = 10;
	[HideInInspector] public float movementSpeedOriginal;
	public float turnRate = 0.7f;
	public int size = 1; // TODO: enum

	[Header("Status effects")]
	public Dead onDeathStatusEffect;
	public Airborn onAirbornStatusEffect;

	// Use this for initialization
	void Start () {
		movementSpeedOriginal = movementSpeed;
	}

}