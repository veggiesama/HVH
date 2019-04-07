﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {

	public GameController gameController;
	public Player player;

	public Slider allyTargetHealthbarSlider, enemyTargetHealthbarSlider;
	
	// pulling lists from inspector to force into dictionaries
	public List<Slider> allySliderList;
	public List<Slider> enemySliderList;
	private Dictionary<DwarfTeamSlots, Slider> allies;
	private Dictionary<MonsterTeamSlots, Slider> enemies;

	// Use this for initialization
	void Start () {
		allies = new Dictionary<DwarfTeamSlots, Slider>();
		enemies = new Dictionary<MonsterTeamSlots, Slider>();

		allyTargetHealthbarSlider.gameObject.SetActive(false);
		enemyTargetHealthbarSlider.gameObject.SetActive(false);

		int n = 0;
		foreach (Slider slider in allySliderList) {
			allies.Add((DwarfTeamSlots)n, slider);
			n++;
		}

		n = 0;
		foreach (Slider slider in enemySliderList) {
			enemies.Add((MonsterTeamSlots)n, slider);
			n++;
		}

		foreach (Slider slider in allies.Values) {
			slider.gameObject.SetActive(false);
		}

		foreach (Slider slider in enemies.Values) {
			slider.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update () {
		if (!Constants.SpawnNPCs) return;

		UnitController allyTarget = player.unit.GetTarget(AbilityTargetTeams.ALLY);
		UnitController enemyTarget = player.unit.GetTarget(AbilityTargetTeams.ENEMY);

		if (allyTarget != null) {
			allyTargetHealthbarSlider.gameObject.SetActive(true);
			allyTargetHealthbarSlider.value = allyTarget.unitInfo.currentHealth / allyTarget.unitInfo.maxHealth;
		}
		else {
			allyTargetHealthbarSlider.gameObject.SetActive(false);
		}

		if (enemyTarget != null) {
			enemyTargetHealthbarSlider.gameObject.SetActive(true);
			enemyTargetHealthbarSlider.value = enemyTarget.unitInfo.currentHealth / enemyTarget.unitInfo.maxHealth;
		}
		else {
			enemyTargetHealthbarSlider.gameObject.SetActive(false);
		}

		foreach (DwarfTeamSlots key in gameController.dwarfTeamPlayers.Keys) {
			Slider slider = allies[key];
			UnitController unit = gameController.dwarfTeamPlayers[key].unit;
			slider.gameObject.SetActive(true);
			slider.value = unit.unitInfo.currentHealth / unit.unitInfo.maxHealth;
		}

		foreach (MonsterTeamSlots key in gameController.monsterTeamPlayers.Keys) {
			Slider slider = enemies[key];
			UnitController unit = gameController.monsterTeamPlayers[key].unit;
			slider.gameObject.SetActive(true);
			slider.value = unit.unitInfo.currentHealth / unit.unitInfo.maxHealth;
		}


	}

	void UpdateHealthbarTargets() {
		//player.unit.GetTarget(true);
		//player.unit.GetTarget(false);
	}
}
