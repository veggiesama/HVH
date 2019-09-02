using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCollider : MonoBehaviour {
	[HideInInspector] public UnitController unit;

    void Start() {
		unit = GetComponentInParent<UnitController>();
    }

}
