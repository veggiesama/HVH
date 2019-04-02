using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// SINGLETON
public class ReferenceManager : MonoBehaviour {
	public static ReferenceManager Instance { get; private set; }

	public MaterialsLibrary MaterialsLibrary;
	public StatusEffectsLibrary StatusEffectsLibrary;
	public GameObject UICanvas;
	public GameObject SpawnPoints;
	[HideInInspector] public List<GameObject> UICanvasList;
	public Slider Castbar;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);

		
		UICanvasList = new List<GameObject>();
		for (int n = 0; n < UICanvas.transform.childCount; n++) {
			UICanvasList.Add( UICanvas.transform.GetChild(n).gameObject );
		}

	}
}

