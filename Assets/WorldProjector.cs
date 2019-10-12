using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldProjector : MonoBehaviour {

	public float yOffset = 10f;
	public float rotationDegreesPerSecond = 60f;
	private Projector projector;

	private void Awake() {
		projector = GetComponent<Projector>();
	}

	// Update is called once per frame
	void Update() {
        gameObject.transform.Rotate(Vector3.forward, rotationDegreesPerSecond * Time.deltaTime);
    }

	public void SetSize(float newSize) {
		projector.orthographicSize = newSize;
	}
}
