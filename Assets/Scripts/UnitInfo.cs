using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour {
	public float maxHealth = 100.0f;
	[HideInInspector] public float currentHealth;

	public float healthRegen = 1.0f;
	public int armor = 0;
	public int resist = 0;
	public int movementSpeed = 10;
	public float turnRate = 0.7f;
	public int size = 1; // TODO: enum

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}