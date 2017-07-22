using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {

	public Image allyTargetPanel;
	public Slider allyTargetHealthbarSlider;
	public List<Slider> allies;

	public Image enemyTargetPanel;
	public Slider enemyTargetHealthbarSlider;
	public List<Slider> enemies;

	// Use this for initialization
	void Start () {
		allyTargetPanel.gameObject.SetActive(false);
		allyTargetHealthbarSlider.gameObject.SetActive(false);

		//enemies.Add()
		
		/*
		ally1.gameObject.SetActive(false);
		ally2.gameObject.SetActive(false);
		ally3.gameObject.SetActive(false);
		ally4.gameObject.SetActive(false);

		enemyTargetPanel.gameObject.SetActive(false);
		enemyTargetHealthbarSlider.gameObject.SetActive(false);
		enemy1.gameObject.SetActive(false);
		enemy2.gameObject.SetActive(false);
		enemy3.gameObject.SetActive(false);
		enemy4.gameObject.SetActive(false);*/
	}
	
	// Update is called once per frame
	void Update () {
		//Controller playerController = 
		//allyTargetHealthbarSlider.value = playerController.
	}
}
