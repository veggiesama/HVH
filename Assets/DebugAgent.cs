using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class DebugAgent : MonoBehaviour
{
	public NavMeshAgent agentToDebug;
	public LineRenderer lineRenderer;

	// Update is called once per frame
	void Update()
    {
        if (agentToDebug != null && agentToDebug.hasPath) {
			lineRenderer.positionCount = agentToDebug.path.corners.Length;
			lineRenderer.SetPositions(agentToDebug.path.corners);
			lineRenderer.enabled = true;
		}

		else {
			lineRenderer.enabled = false;
		}
    }
}
