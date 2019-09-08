using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class ResourceLibrary : Singleton<ResourceLibrary> {

	public GameObject npcPrefab;

	public RenderTexture allyRenderTexture;
	public RenderTexture enemyRenderTexture;

	public Dictionary<string, UnitInfo> unitInfoDictionary = new Dictionary<string, UnitInfo>();
	public Dictionary<string, StatusEffect> statusEffectDictionary = new Dictionary<string, StatusEffect>();
	public Dictionary<string, GameObject> particlePrefabDictionary = new Dictionary<string, GameObject>();

	// Singleton constructor
	public static ResourceLibrary Instance {
		get {
			return ((ResourceLibrary)mInstance);
		} set {
			mInstance = value;
		}
	}

    void Awake() {
		//DontDestroyOnLoad(this.gameObject);
	
	    UnitInfo[] unitArray = Resources.LoadAll<UnitInfo>("ScriptableObjects\\UnitInfo");
		foreach (UnitInfo o in unitArray) {
			unitInfoDictionary.Add(o.name, o);
		}

        StatusEffect[] seArray = Resources.LoadAll<StatusEffect>("ScriptableObjects\\StatusEffects");
		foreach (StatusEffect o in seArray) {
			statusEffectDictionary.Add(o.statusName, o);
		}

		GameObject[] particlePrefabArray = Resources.LoadAll<GameObject>("ParticlePrefabs");
		foreach (GameObject o in particlePrefabArray) {
			particlePrefabDictionary.Add(o.name, o);
		}

		//Debug.Log(unitInfoDictionary.DebugToString());
    }

}
