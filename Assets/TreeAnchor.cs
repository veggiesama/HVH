using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAnchor : MonoBehaviour
{
    [SerializeField] private Transform abilityAnchor;

	public Vector3 GetAnchorPoint() {
		return abilityAnchor.position;
	}
}
