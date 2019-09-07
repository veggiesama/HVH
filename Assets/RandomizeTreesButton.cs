#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tree = HVH.Tree;

public class RandomizeTreesButton : MonoBehaviour {

	public void Activate() {
		
		RandomizeTrees randomizeTrees = GetComponent<RandomizeTrees>();

		WeightedRandomBag<GameObject> randomGoBag = new WeightedRandomBag<GameObject>();
		WeightedRandomBag<TreeColor> randomLeavesColorBag = new WeightedRandomBag<TreeColor>();
		WeightedRandomBag<TreeColor> randomPineColorBag = new WeightedRandomBag<TreeColor>();

		foreach (KeyValuePair<GameObject, float> kv in randomizeTrees.treePrefabs) {
			randomGoBag.AddEntry(kv.Key, kv.Value);
		}

		foreach (KeyValuePair<TreeColor, float> kv in randomizeTrees.leavesColors) {
			randomLeavesColorBag.AddEntry(kv.Key, kv.Value);
		}

		foreach (KeyValuePair<TreeColor, float> kv in randomizeTrees.pineColors) {
			randomPineColorBag.AddEntry(kv.Key, kv.Value);
		}

		List<Transform> executionList = new List<Transform>();
		Transform child;

		int count = this.transform.childCount;
		for (int i = 0; i < count; i++) {
			child = this.transform.GetChild(i);
			GameObject treeGo = (GameObject) PrefabUtility.InstantiatePrefab(randomGoBag.GetRandom(), this.transform);
			Tree tree =  treeGo.GetComponent<Tree>();

			TreeColor col;
			if (treeGo.name.Contains("Pine"))
				col = randomPineColorBag.GetRandom();
			else
				col = randomLeavesColorBag.GetRandom();

			treeGo.transform.position = child.position;
			treeGo.transform.rotation = child.rotation;
			executionList.Add(child);

			tree.SaveColors(col.colorTint, col.emissionColor); // the material block is remade
			tree.SetColors();
		}

		for (int i = 0; i < executionList.Count; i++) {
			DestroyImmediate(executionList[i].gameObject, false);
		}

		Debug.Log("Replaced " + count + " trees.");

	}
}
#endif
