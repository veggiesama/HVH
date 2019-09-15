using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Tree = HVH.Tree;

public class ResourceLibrary : Singleton<ResourceLibrary> {

	public GameObject npcPrefab;

	public Ability emptyAbility;
	public Ability emptyItem;

	public Dictionary<string, UnitInfo> unitInfoDictionary = new Dictionary<string, UnitInfo>();
	public Dictionary<string, StatusEffect> statusEffectDictionary = new Dictionary<string, StatusEffect>();
	public Dictionary<string, GameObject> particlePrefabDictionary = new Dictionary<string, GameObject>();

	private List<TreeHandler> treeHandlers;

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

		treeHandlers = new List<TreeHandler>();

		//Debug.Log(unitInfoDictionary.DebugToString());
    }

	public void RegisterTreeHandler(TreeHandler treeHandler) {
		Debug.Log("Register tree");
		treeHandlers.Add(treeHandler);
	}

	public void DisableTreeHighlighting() {
		Debug.Log("Disable tree highlighting");
		foreach (TreeHandler treeHandler in treeHandlers) {
			foreach (Tree t in treeHandler.GetComponentsInChildren<Tree>()) {
				t.SetHighlighted(HighlightingState.NONE);
			}
		}
	}

}
