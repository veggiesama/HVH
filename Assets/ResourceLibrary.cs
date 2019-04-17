using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class ResourceLibrary : Singleton<ResourceLibrary> {

	public Dictionary<string, StatusEffect> statusEffectDictionary = new Dictionary<string, StatusEffect>();

	// Singleton constructor
	public static ResourceLibrary Instance {
		get {
			return ((ResourceLibrary)mInstance);
		} set {
			mInstance = value;
		}
	}

    void Start()
    {
        StatusEffect[] array = Resources.LoadAll<StatusEffect>("ScriptableObjects\\StatusEffects");
		foreach (StatusEffect o in array) {
			statusEffectDictionary.Add(o.statusName, o);
		}

		//Debug.Log(statusEffectDictionary.DebugToString());
    }

}
