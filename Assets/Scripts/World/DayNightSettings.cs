using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.LSky;

public class DayNightSettings : MonoBehaviour {
	public LSky lsky;
	public LSkyTOD tod;

	[Range(0f, 24f)] public float startingTime = 10f;
	[Range(0f, 24f)] public float dayBegins = 10;
	[Range(0f, 24f)] public float nightBegins = 22f;

	public float dayDuration = 30f;
	public float nightDuration = 30f;
	public float transitionDuration = 2f;

	public float updateEvery = 1.0f;
	public float transitionUpdateEvery = 0.02f;
}
