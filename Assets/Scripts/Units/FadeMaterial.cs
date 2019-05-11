using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assumes shader has float property "_Alpha"

public class FadeMaterial : MonoBehaviour {
	public float initialDelay = 2f;
	public float fadeOverSeconds = 3f;
    private Material[] materials;
	private float[] lastAlphas;
	private float currentTimer;

	private void Start() {
		materials = GetComponent<Renderer>().materials;
		lastAlphas = new float[materials.Length];
		currentTimer = initialDelay;
	}

	private void Update() {

		if (currentTimer <= 0) {
	
			for (int n = 0; n < materials.Length; n++) {
				if (materials[n].shader.name == "SyntyStudios/Trees") {
					lastAlphas[n] = materials[n].GetFloat("_Alpha");
					float newAlpha = lastAlphas[n] - (1 / fadeOverSeconds * Time.deltaTime);
					materials[n].SetFloat("_Alpha", newAlpha);
				}
				else {
					lastAlphas[n] = materials[n].GetColor("_Color").a;
					float newAlpha = lastAlphas[n] - (1 / fadeOverSeconds * Time.deltaTime);
					Color lastColor = materials[n].GetColor("_Color");
					materials[n].SetColor("_Color", new Color(lastColor.r, lastColor.g, lastColor.b, newAlpha));
				}
				//materials[n].color = new Color(lastColors[n].r, lastColors[n].g, lastColors[n].b, lastColors[n].a - (1 / fadeOverSeconds * Time.deltaTime));
			}
		}

		else 
			currentTimer -= Time.deltaTime;
	}
}