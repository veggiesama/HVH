using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach this script to GameObject container that holds desired Game Objects to snap
public class SnapToTerrain : MonoBehaviour {

	public float flatnessTolerance = 0.05f;
	public float lowerBoundForY = 8;
	public float upperBoundForY = -2;
	
	public void Activate()
	{
		float minTolerance = flatnessTolerance;
		float maxTolerance = 1 - flatnessTolerance;	

		List<Transform> executionList_NoFlatBase = new List<Transform>();
		List<Transform> executionList_OutOfBounds = new List<Transform>();
		Transform child;
		for (int i = 0; i < this.transform.childCount; i++) {
			child = this.transform.GetChild(i);

			// snap to grid
			float x = Mathf.Floor(child.position.x) + 0.5f;
			float y = child.position.y;
			float z = Mathf.Floor(child.position.z) + 0.5f;
			child.position = new Vector3(x,y,z);
			
			Vector3[] vertices = new Vector3[5];
			Vector3 extents = child.GetComponent<Collider>().bounds.extents;
			vertices[0] = new Vector3(x + extents.x, y, z);
			vertices[1] = new Vector3(x - extents.x, y, z);
			vertices[2] = new Vector3(x + extents.x, y, z + extents.z);
			vertices[3] = new Vector3(x - extents.x, y, z - extents.z);
			vertices[4] = new Vector3(x,y,z);

			bool brokeLoop = false;
			foreach (Vector3 v in vertices) {
				float terrainHeight = Terrain.activeTerrain.SampleHeight(v);
				float terrainHeightDecimal = terrainHeight - Mathf.Floor(terrainHeight);
				if (terrainHeightDecimal > minTolerance && terrainHeightDecimal < maxTolerance) {
					executionList_NoFlatBase.Add(child);
					brokeLoop = true;
					break;
				}
			}

			if (brokeLoop)
				continue;

			y = Mathf.Round(Terrain.activeTerrain.SampleHeight(child.position) / 2) * 2 + Terrain.activeTerrain.GetPosition().y;
			//print("SampleHeight: " + Mathf.Round(Terrain.activeTerrain.SampleHeight(child.position) / 2) * 2 + ", Y: " + y);

			if (y < lowerBoundForY || y > upperBoundForY) {
				executionList_OutOfBounds.Add(child);
				continue;
			}

			child.position = new Vector3(x,y,z);
		}

		foreach (Transform c in executionList_NoFlatBase) {
			DestroyImmediate(c.gameObject, false);
		}

		foreach (Transform c in executionList_OutOfBounds) {
			DestroyImmediate(c.gameObject, false);
		}

		int remainingChildren = this.transform.childCount - executionList_NoFlatBase.Count - executionList_OutOfBounds.Count;
		print("Snapping " + this.transform.childCount + " children." +
			" No flat base: removing " + executionList_NoFlatBase.Count + " children. " +
			" Out of bounds: removing " + executionList_OutOfBounds.Count + " children. " +
			remainingChildren + " remaining.");

		// apply rotations
		for (int i = 0; i < this.transform.childCount; i++) {
			child = this.transform.GetChild(i);
			MeshRenderer[] meshes = child.GetComponentsInChildren<MeshRenderer>();

			int rng = Random.Range(0, 180);
			foreach (MeshRenderer mesh in meshes) {
				mesh.transform.Rotate(Vector3.up, rng);
			}
		}
	}

}
