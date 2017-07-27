using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour {

	[HideInInspector] public AbilityInfo abilityInfo;
	[HideInInspector] public UnitController caster;
	[HideInInspector] public UnitInfo unitInfo;

	// Use this for initialization
	void Start () {
		abilityInfo = GetComponent<AbilityInfo>();
		caster = GetComponentInParent<UnitController>();
		unitInfo = GetComponentInParent<UnitInfo>();
	}

	public virtual void Cast() {
		print("AbilityController.Cast()");
	}


}
