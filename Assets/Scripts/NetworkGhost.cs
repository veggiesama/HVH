using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Idea: https://forum.unity.com/threads/copy-an-object-to-clients-instantiate-only-visuals.84037/
public class NetworkGhost : NetworkBehaviour {

	private GameObject sourceGO;
	
	[Server]
	public void Initialize(GameObject objectToClone) {
		this.sourceGO = objectToClone;
		DisableAllComponentsExceptRenderer();

		GetComponent<Renderer>().material.color = Color.magenta; // DEBUG
	}

	[Server]
    private void Update() {
		if (sourceGO == null) {
			DestroySelf();
			return;
		}

		this.transform.position = sourceGO.transform.position;
		this.transform.rotation = sourceGO.transform.rotation;
		this.transform.localScale = sourceGO.transform.localScale;
    }

	[Server]
	private void DisableAllComponentsExceptRenderer() {
		foreach (Component c in GetComponents<Component>()) {
			if (c is Behaviour && !(c is NetworkIdentity))
				(c as Behaviour).enabled = false;
			if (c is Collider)
				(c as Collider).enabled = false;
		}
	}

	[Server]
	private void DestroySelf() {
		NetworkServer.UnSpawn(this.gameObject);
		Destroy(this.gameObject);
	}
}
