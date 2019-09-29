﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
	public static float TreeRespawnTime = 16f; //30.0f;
	public static float ProjectileSelfDestructTime = 6.0f;
	public static float FrontAngle = 15.0f; //30.0f;
	public static int RaycastLength = 100;
	public static bool StartDay = true;

	public static int DwarvesTotal = 4;
	public static int MonstersTotal = 4;

	public static Color AllyColor = Color.green;
	public static Color EnemyColor = Color.red;

	public static float OnMoveTolerance = 1f; // actual distance a body needs to travel before OnMove event invoked
}

// Projectile types
public enum ProjectileBehaviourTypes {
	NONE, BULLET, CONE, GRENADE, HOMING, LINE
}

// Casting abilities
public enum CastResults {
	SUCCESS, FAILURE_COOLDOWN_NOT_READY, FAILURE_TARGET_OUT_OF_RANGE, FAILURE_NOT_FACING_TARGET, FAILURE_NAVIGATION_NOT_READY,
	FAILURE_ALREADY_AIRBORN, FAILURE_INVALID_TARGET
}

// Layers
public enum LayerBits {
	TERRAIN = 16, TREE = 17, BODY = 23, BODY_RAGDOLL = 24, BODY_SELECTABLE = 25
}

public enum LayerMasks {
	TERRAIN = 1			<< LayerBits.TERRAIN,
	TREE = 1			<< LayerBits.TREE,
	BODY = 1			<< LayerBits.BODY,
	BODY_RAGDOLL = 1	<< LayerBits.BODY_RAGDOLL,
	BODY_SELECTABLE = 1 << LayerBits.BODY_SELECTABLE
}

// UI slots
public enum AbilitySlots {
	ATTACK,
	ABILITY_1, ABILITY_2, ABILITY_3, ABILITY_4, ABILITY_5, ABILITY_6,
	ITEM_1, ITEM_2, ITEM_3, ITEM_4, ITEM_5, ITEM_6,
	NONE
};

public enum AbilitySlotTypes {
	ABILITY,
	ITEM,
	NONE
}


// Teams, allies, enemies
[System.Flags]
public enum Teams {
	DWARVES = 0x0,
	MONSTERS = 0x1,
	NEUTRALS = 0x2,
	OBSERVERS = 0x3
};

public enum UiPortraitSlots {
	ALLY_1, ALLY_2, ALLY_3, ALLY_TARGET, ENEMY_TARGET, SELF
}

// abilities
public enum AbilityTargetTypes {
	NONE, UNIT, POINT, TREE, AREA
};

public enum AbilityTargetTeams {
	NONE, ALLY, ENEMY, BOTH
};

public enum DamageTypes {
	NONE, NORMAL, MAGICAL, PURE
};

public enum OrderTypes {
	NONE, MOVE_TO_POSITION,	MOVE_TO_TARGET, CAST_POSITION, CAST_TARGET, CAST_NO_TARGET, STOP,
	TURN_TO_FACE, CAST_TREE
}

public enum StatusEffectTypes {
	BLINDED, DEAD, INVISIBLE, IMMOBILIZED, STUNNED, SLOWED, SILENCED, AIRBORN, REVEALED, WELL_FED, INVULNERABLE, RESISTANT,
	SHARE_VISION, ORDER_RESTRICTED, BUFF_DISPELLABLE, BUFF_NOTDISPELLABLE
}

public enum AirbornClippingTypes {
	NO_CLIP, TREE_CLIP, TREE_DESTROY
}

// hit locations
public enum BodyLocations {
	NONE, HEAD, MOUTH, WEAPON, FEET
}

public enum VisionType {
	NORMAL, FLYING
}

public enum HighlightingState {
	NONE, NORMAL, INTEREST, ENEMY
}

public enum VisibilityState {
	VISIBLE, VISIBLE_TO_TEAM_ONLY, FADING, INVISIBLE
}

public enum Desire {
	NONE = 0,
	LOW = 25,
	MEDIUM = 50,
	HIGH = 75,
	MAX = 100
}

public static class AnimFloats {
	public static string SPEED { get { return "speed"; } }
}

public static class AnimBools {
	public static string DEAD	 { get { return "isDead"; } }
	public static string RESTING { get { return "isResting"; } }
}

public static class AnimTriggers {
	public static string ATTACK   { get { return "StartAttack"; } }
	public static string CAST     { get { return "StartCast"; } }
	public static string DIE      { get { return "StartDie"; } }
	public static string ATTACK_A { get { return "AttackA"; } }
	public static string ATTACK_B { get { return "AttackB"; } }
	public static string CAST_A   { get { return "CastA"; } }
	public static string CAST_B   { get { return "CastB"; } }
	public static string REST   { get { return "StartRest"; } }
}

public enum Animations {
	NONE, ATTACK_A, ATTACK_B, CAST_A, CAST_B, DIE, RESPAWN, IDLE, LAYDOWN, WAKEUP
}



