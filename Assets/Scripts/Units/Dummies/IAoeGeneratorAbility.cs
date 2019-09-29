using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAoeGeneratorAbility {
	GameObject GetAoeGenParticlePrefab();
	StatusEffect[] GetStatusEffects();
	bool GetDestroysTrees();
	float GetReappliesEvery();
	float GetDelay();
}

