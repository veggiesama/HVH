using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAoeGeneratorAbility {
	GameObject GetAoeGenParticlePrefab();
	bool GetDestroysTrees();
	float GetReappliesEvery();
	StatusEffect[] GetStatusEffects();
	float GetDelay();
}

