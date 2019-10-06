using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyLocationFinder : MonoBehaviour {
	[Header("Hitbox")]
	public GameObject clickableHitbox;

	[Header("Spawners")]
	public GameObject projectileSpawner;
	public GameObject head;
	public GameObject mouth;
	public GameObject feet;
	
	[Header("Cameras")]
	public Camera allyCam;
	public Camera targetCam;

	[Header("Materials")]
	public Material invisMaterial;
	public Renderer[] bodyMeshes;
}
