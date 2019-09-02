using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitInfo/UnitInfo")]
public class UnitInfo : ScriptableObject {
	public string unitName = "Unit name";
	public float maxHealth = 100.0f;
	public float healthRegen = 1.0f;
	public int armor = 0;
	public int resist = 0;
	public float movementSpeed = 10;
	public float turnRate = 1000f;
	public int size = 1; // TODO: enum
	public float daySightRange = 30f;
	public float nightSightRange = 22f;
	public float fovViewAngle = 360f;
	[HideInInspector] public float movementSpeedOriginal;
	[HideInInspector] public float turnRateOriginal;
	[HideInInspector] public int sizeOriginal;
	[SerializeField] private Color bodyColorA = new Color();
	[SerializeField] private Color bodyColorB = new Color();
	[HideInInspector] public Color bodyColor = new Color();

	[Header("Status effects")]
	public Dead onDeathStatusEffect;
	public Airborn onAirbornStatusEffect;

	[Header("Starting abilities")]
	public Ability startingAttackAbility;
	public List<Ability> startingAbilitiesList;
	public List<Ability> startingItemsList;

	[Header("Body locations")]
	public GameObject animationPrefab;

	[Header("AI")]
	public bool hasAI = false;
	public List<AiState> aiStatesDay;
	public List<AiState> aiStatesNight;
	public float aiUpdateEvery = 0.2f;

	// Use this for initialization
	public void Initialize () {
		movementSpeedOriginal = movementSpeed;
		turnRateOriginal = turnRate;
		sizeOriginal = size;
		bodyColor = Color.Lerp(bodyColorA, bodyColorB, Random.Range(0f, 1f));
	}

}