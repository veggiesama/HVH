using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterial : MonoBehaviour {
	public float initialDelay = 2f;
	public float fadeOverSeconds = 3f;
    private Material material;
	private Color lastColor;
	private float currentTimer;

	private void Start() {
		material = GetComponent<Renderer>().material;
		currentTimer = initialDelay;
	}

	private void Update() {
		if (currentTimer <= 0) {
			lastColor = material.color;
			material.color = new Color(lastColor.r, lastColor.g, lastColor.b, lastColor.a - (1 / fadeOverSeconds * Time.deltaTime));
		}

		else 
			currentTimer -= Time.deltaTime;
	}
}