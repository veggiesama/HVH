using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCoverable : MonoBehaviour
{
    Renderer rend;


    void Start()
    {
        //rend = GetComponent<Renderer>();
        //FieldOfView.OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;
    }

    void OnDestroy()
    {
       // FieldOfView.OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    }

    void FieldOfViewOnTargetsVisibilityChange(List<Transform> newTargets)
    {
        //rend.enabled = newTargets.Contains(transform);
    }
}