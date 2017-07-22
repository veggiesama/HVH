using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public static class Constants {

public enum AbilitySlots {
	ABILITY_1, ABILITY_2, ABILITY_3, ABILITY_4, ABILITY_5, ABILITY_6,
	ITEM_1, ITEM_2, ITEM_3, ITEM_4, ITEM_5, ITEM_6
};

public enum Teams {
	GOODGUYS, BADGUYS, NEUTRALS
};

public enum Allies {
	ALLY_1, ALLY_2, ALLY_3, ALLY_4
};

public enum Enemies {
	ENEMY_1, ENEMY_2, ENEMY_3, ENEMY_4
};

// returns the screen position for ally and enemy viewports
public static class CameraViewports {

	public static Rect GetAllyViewport()
	{
		return new Rect(0.6668f, 0.0f, 0.082f, 0.165f);
	}

	public static Rect GetEnemyViewport()
	{
		return new Rect(0.917f, 0.0f, 0.082f, 0.165f);
	}
}



//}
