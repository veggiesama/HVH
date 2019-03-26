using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
	public static float ProjectileSelfDestructTime = 6.0f;
	public static float FrontAngle = 15.0f; //30.0f;
	public static bool SpawnNPCs = false;
	public static int RaycastLength = 100;
}

// Casting abilities
public enum CastResults {
	SUCCESS, FAILURE_COOLDOWN_NOT_READY, FAILURE_TARGET_OUT_OF_RANGE, FAILURE_NOT_FACING_TARGET, FAILURE_NAVIGATION_NOT_READY,
	FAILURE_ALREADY_AIRBORN
}

// Layers
public enum LayerBits {
	TERRAIN = 16, TREE = 17
}

public enum LayerMasks {
	TERRAIN = 1 << LayerBits.TERRAIN,
	TREE = 1 << LayerBits.TREE,
}

// UI slots
public enum AbilitySlots {
	ATTACK,
	ABILITY_1, ABILITY_2, ABILITY_3, ABILITY_4, ABILITY_5, ABILITY_6,
	ITEM_1, ITEM_2, ITEM_3, ITEM_4, ITEM_5, ITEM_6
};


// Teams, allies, enemies
public enum Teams {
	GOODGUYS, BADGUYS, NEUTRALS
};

public enum Allies {
	ALLY_1, ALLY_2, ALLY_3, ALLY_4
};

public enum Enemies {
	ENEMY_1, ENEMY_2, ENEMY_3, ENEMY_4
};


// abilities
public enum AbilityTargetTypes {
	NONE, UNIT, POINT, PASSIVE, CHANNELLED, SHAPE
};

public enum AbilityTargetTeams {
	NONE, ALLY, ENEMY, BOTH
};

public enum DamageTypes {
	NONE, NORMAL, MAGICAL, PURE
};

public enum OrderTypes {
	NONE, MOVE_TO_POSITION,	MOVE_TO_TARGET, CAST_POSITION, CAST_TARGET, CAST_NO_TARGET, STOP,
	TURN_TO_FACE
}

public enum StatusEffectTypes {
	BLINDED, DEAD, INVISIBLE, IMMOBILIZED, STUNNED, SLOWED, SILENCED, AIRBORN, REVEALED, WELL_FED, INVULNERABLE, RESISTANT,
	SHARE_VISION, ORDER_RESTRICTED
}

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


