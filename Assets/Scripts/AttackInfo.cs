using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo : MonoBehaviour {
	public int range = 100;
	public int damage = 5;
	public float swingSpeed = 0.7f;
	public float backswingSpeed = 0.3f;
	public GameObject projectilePrefab;
	public GameObject spawnerObject;

	public float GetAttackSpeed() {
		return swingSpeed + backswingSpeed;
	}
}
