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

		if (localPlayer.GetTeam() == owner.GetTeam())
			rend.sprite = spriteDict["Ally"];
		else
			rend.sprite = spriteDict["Enemy"];
    }
}
