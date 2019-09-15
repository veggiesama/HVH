using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointHandler : MonoBehaviour {
	private void Awake() {
		for (int i = 0; i < transform.childCount; i++) {
			GameObject spawnPoint = transform.GetChild(i).gameObject;
			GameResources.Instance.RegisterSpawnPoint(spawnPoint);
		}	
	}

}
