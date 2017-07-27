using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class GameSettings : ScriptableObject {

	[Header("Debug")]
	public bool debugMode;
 
	[Header("Wheel positions")]
	public float nearZPosition;
	public float middleZPosition;
	public float distantZPosition;

	[Header("Colors")]
	public Color buttonBgColor;
	public Color hudForegroundColor;

	// etc
 
}