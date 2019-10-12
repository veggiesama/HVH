using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

public class MinimapIcon : MonoBehaviour {
	Owner owner;
	Player localPlayer;
	SpriteRenderer rend;

	[System.Serializable]
	public class StringToSpriteDictionary : SerializableDictionaryBase<string, Sprite> {}
	public StringToSpriteDictionary spriteDict = new StringToSpriteDictionary();

    void Awake() {
        owner = GetComponentInParent<Owner>();
		rend = GetComponent<SpriteRenderer>();
    }

    public void Initialize() {
		localPlayer = GameResources.Instance.GetLocalPlayer();

		switch (owner.GetTeam()) {
			case Teams.NONE:
				Debug.Log("None team icon.");
				break;
			case Teams.DWARVES:
				rend.sprite = spriteDict["Dwarf"];
				break;
			case Teams.MONSTERS:
				rend.sprite = spriteDict["Monster"];
				break;
			case Teams.NEUTRALS:
				rend.sprite = spriteDict["Neutral"];
				break;
			case Teams.OBSERVERS:
				Debug.Log("No observer team icon.");
				break;
			default:
				break;
		}

	}
}
