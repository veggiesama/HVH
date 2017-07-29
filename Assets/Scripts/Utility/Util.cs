using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Util {

	// takes a public list of AbilityControllers and stitches them to enumerated AbilitySlots (ABILITY_1, etc.)
	public static Dictionary<AbilitySlots, AbilityController> DictionaryBindAbilitySlotsToAbilityControllers(List<AbilityController> abilityControllerList, GameObject parent) {
		
		Dictionary<AbilitySlots, AbilityController> abilityDictionary =
			new Dictionary<AbilitySlots, AbilityController>();
		
		int n = 0;
		foreach (AbilityController ability in abilityControllerList) {
			if (ability != null)
				abilityDictionary.Add((AbilitySlots)n, ability);
			//else
			//	abilityDictionary.Add((AbilitySlots)n,  ) ;
			n++;
		}

		return abilityDictionary;
	}

}
