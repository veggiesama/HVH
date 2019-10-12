using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceLibrary : Singleton<ReferenceLibrary> {

	// Singleton constructor
	public static ReferenceLibrary Instance {
		get {
			return ((ReferenceLibrary)mInstance);
		} set {
			mInstance = value;
		}
	}

	[Header("Network")]
	public NetworkHUD networkHUD;

	[Header("World projectors")]
	public GameObject allyTargetProjector;
	public GameObject enemyTargetProjector;
	public GameObject movementProjector;
	public GameObject aoeProjector;

}
