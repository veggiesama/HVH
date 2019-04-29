using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitInfo/UnitInfo")]
public class UnitInfo : ScriptableObject {
	public float maxHealth = 100.0f;
	public float healthRegen = 1.0f;
	public int armor = 0;
	public int resist = 0;
	public float movementSpeed = 10;
	public float turnRate = 1000f;
	public int size = 1; // TODO: enum
	[HideInInspector] public float movementSpeedOriginal;
	[HideInInspector] public float turnRateOriginal;
	[HideInInspector] public int sizeOriginal;

	[Header("Status effects")]
	public Dead onDeathStatusEffect;
	public Airborn onAirbornStatusEffect;

	[Header("Starting abilities")]
	public Ability startingAttackAbility;
	public List<Ability> startingAbilitiesList;
	public List<Ability> startingItemsList;

	[Header("Body locations")]
	public GameObject animationPrefab;

	// Use this for initialization
	public void Initialize () {
		movementSpeedOriginal = movementSpeed;
		turnRateOriginal = turnRate;
		sizeOriginal = size;
	}

}