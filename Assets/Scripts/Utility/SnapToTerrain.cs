using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach this script to GameObject container that holds desired Game Objects to snap
public class SnapToTerrain : MonoBehaviour {
	
	public void Activate(float lowerBoundForY, float upperBoundForY)
	{
		print("SNAP! " + this.transform.childCount);
		
		List<Transform> executionList = new List<Transform>();
		Transform child;
		for (int i = 0; i < this.transform.childCount; i++) {
			child = this.transform.GetChild(i);

			// snap to grid
			float x = Mathf.Floor(child.position.x) + 0.5f;
			float y = child.position.y;
			float z = Mathf.Floor(child.position.z) + 0.5f;
			child.position = new Vector3(x,y,z);
			
			float terrainHeight = Terrain.activeTerrain.SampleHeight(child.position);
			float terrainHeightDecimal = terrainHeight - Mathf.Floor(terrainHeight);
			if (terrainHeightDecimal > 0.05 && terrainHeightDecimal < 0.95) {
				executionList.Add(child);
				continue;
			}

			y = Mathf.Round(terrainHeight / 2) * 2 + Terrain.activeTerrain.GetPosition().y;
			//print("SampleHeight: " + Mathf.Round(Terrain.activeTerrain.SampleHeight(child.position) / 2) * 2 + ", Y: " + y);

			if (y < lowerBoundForY || y > upperBoundForY) {
				executionList.Add(child);
				continue;
			}

			child.position = new Vector3(x,y,z);
		}

		foreach (Transform c in executionList) {
			DestroyImmediate(c.gameObject, false);
		}
	}

}
