using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour {

	public bool shallowWater = false;

	private void OnTriggerEnter(Collider col) {
		if (Util.IsBody(col.gameObject)) {

			GameObject splashPrefab;
			

			if (!shallowWater) {
				splashPrefab = ResourceLibrary.Instance.waterSplash;
				col.gameObject.GetComponent<BodyController>().unit.OnEnterWater();
			}
			else {
				splashPrefab = ResourceLibrary.Instance.waterRipple;
			}


			Instantiate(splashPrefab, col.transform.position, splashPrefab.transform.rotation);

		}
	}

}
