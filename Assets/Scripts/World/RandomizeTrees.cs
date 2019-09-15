using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class RandomMaterialWeight : SerializableDictionaryBase<Material, float> {}

[System.Serializable]
public class RandomGameObjectWeight : SerializableDictionaryBase<GameObject, float> {}

[System.Serializable]
public class RandomTreeColorWeight : SerializableDictionaryBase<TreeColor, float> {}

public class RandomizeTrees : MonoBehaviour {
	public RandomGameObjectWeight treePrefabs;
	public RandomTreeColorWeight pineColors;
	public RandomTreeColorWeight leavesColors;
}