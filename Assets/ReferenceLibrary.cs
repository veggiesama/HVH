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

	public NetworkHUD networkHUD;

}
