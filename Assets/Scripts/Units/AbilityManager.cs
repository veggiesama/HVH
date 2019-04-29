﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour {
	private Dictionary<AbilitySlots, Ability> abilityDict = new Dictionary<AbilitySlots, Ability>();
	private UnitController unit;
	private float debugRefreshEvery = 1f;
	private float debugTimer = 0f;
	private bool initialized = false;

	[Header("READ-ONLY")]
	[SerializeField] private List<string> debugList = new List<string>();

    void Awake() {
		unit = GetComponentInParent<UnitController>();
		//LoadAbilities();
	}

	public void Initialize() {
		LoadAbilities();
		initialized = true;
	}

	public void LoadAbilities() {
		abilityDict.Clear();

		int n = 0;
		foreach (AbilitySlots slot in Enum.GetValues(typeof(AbilitySlots))) {
			string slotName = slot.ToString();

			if(slotName.Contains("ATTACK")) {
				Ability a = Instantiate(unit.unitInfo.startingAttackAbility);
				a.Initialize(unit.gameObject);
				abilityDict[slot] = a;
			}

			else if(slotName.Contains("ABILITY")) {
				Ability a = Instantiate(unit.unitInfo.startingAbilitiesList[n]);
				a.Initialize(unit.gameObject);
				abilityDict[slot] = a;
				n++;
			}

			else if(slotName.Contains("ITEM")) {
				Ability a = Instantiate(unit.unitInfo.startingItemsList[n-6]);
				a.Initialize(unit.gameObject);
				abilityDict[slot] = a;
				n++;
			}
		}
	}

	private void Update()
	{
		if (!initialized) return;
		BuildDebugList();

		foreach (KeyValuePair<AbilitySlots, Ability> kv in abilityDict) {
			AbilitySlots slot = kv.Key;
			Ability ability = kv.Value;

			ability.Update();
		}
	}

	private void FixedUpdate()
	{
		if (!initialized) return;

		foreach (KeyValuePair<AbilitySlots, Ability> kv in abilityDict) {
			AbilitySlots slot = kv.Key;
			Ability ability = kv.Value;

			ability.FixedUpdate();
		}
	}

	public void BuildDebugList() {
		if (debugTimer <= 0f) {
			debugList.Clear();
			
			foreach (KeyValuePair<AbilitySlots, Ability> kv in abilityDict) {
				debugList.Add(kv.Key + ": " + kv.Value.abilityName);
			}
			
			debugTimer = debugRefreshEvery;
		}

		debugTimer -= Time.deltaTime;
	}

	public bool HasAbilityInSlot(AbilitySlots slot) {
		return abilityDict.TryGetValue(slot, out Ability value);
		//return abilityDict[slot] != null;
	}

	public Ability GetAbilityInSlot(AbilitySlots slot) {
		return abilityDict[slot];
	}

	public AbilitySlots GetAbilitySlot(Ability ability) {
		foreach (KeyValuePair<AbilitySlots, Ability> kv in abilityDict) {
			AbilitySlots slot = kv.Key;
			Ability a = kv.Value;
			if (a.Equals(ability))
				return slot;
		}

		return AbilitySlots.NONE;
	}
}
