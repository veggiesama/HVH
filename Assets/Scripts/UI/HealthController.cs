using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;


public class HealthController : MonoBehaviour {

	[SerializeField] private GameController gc;
	private Player player;
	public Slider allyTargetHealthbarSlider, enemyTargetHealthbarSlider;
	
	// pulling lists from inspector to force into dictionaries
	public List<Slider> allySliderList;
	public List<Slider> enemySliderList;
	private Dictionary<DwarfTeamSlots, Slider> allies = new Dictionary<DwarfTeamSlots, Slider>();
	private Dictionary<MonsterTeamSlots, Slider> enemies = new Dictionary<MonsterTeamSlots, Slider>();

	// Use this for initialization
	void Start () {
		player = GetComponentInParent<UICanvas>().GetLocalPlayer();
		allyTargetHealthbarSlider.gameObject.SetActive(false);
		enemyTargetHealthbarSlider.gameObject.SetActive(false);

		int n = 0;
		foreach (Slider slider in allySliderList) {
			allies.Add((DwarfTeamSlots)n, slider);
			slider.gameObject.SetActive(false);
			n++;
		}

		n = 0;
		foreach (Slider slider in enemySliderList) {
			enemies.Add((MonsterTeamSlots)n, slider);
			slider.gameObject.SetActive(false);
			n++;
		}
	}

	// Update is called once per frame
	void Update () {
		UnitController allyTarget = player.unit.GetTarget(AbilityTargetTeams.ALLY);
		UnitController enemyTarget = player.unit.GetTarget(AbilityTargetTeams.ENEMY);

		if (allyTarget != null) {
			allyTargetHealthbarSlider.gameObject.SetActive(true);
			allyTargetHealthbarSlider.value = allyTarget.networkHelper.currentHealth / allyTarget.unitInfo.maxHealth;
		}
		else {
			allyTargetHealthbarSlider.gameObject.SetActive(false);
		}

		if (enemyTarget != null) {
			enemyTargetHealthbarSlider.gameObject.SetActive(true);
			enemyTargetHealthbarSlider.value = enemyTarget.networkHelper.currentHealth / enemyTarget.unitInfo.maxHealth;
		}
		else {
			enemyTargetHealthbarSlider.gameObject.SetActive(false);
		}
		
		Player[] playerArray = FindObjectsOfType<Player>();

		foreach (KeyValuePair<int,int> kv in gc.dwarfDictionary) {
			DwarfTeamSlots slot = (DwarfTeamSlots)kv.Key;
			Player p = gc.GetPlayer(kv.Value);
		
			Slider slider = allies[slot];
			slider.gameObject.SetActive(true);
			slider.value = p.unit.networkHelper.currentHealth / p.unit.unitInfo.maxHealth;
		}

		foreach (KeyValuePair<int,int> kv in gc.monsterDictionary) {
			MonsterTeamSlots slot = (MonsterTeamSlots)kv.Key;
			Player p = gc.GetPlayer(kv.Value);
	
			Slider slider = enemies[slot];
			slider.gameObject.SetActive(true);
			slider.value = p.unit.networkHelper.currentHealth / p.unit.unitInfo.maxHealth;
		}
		
	}
}
