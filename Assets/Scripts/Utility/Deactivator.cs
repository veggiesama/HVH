using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivator : MonoBehaviour {
     void OnApplicationQuit()
     {
         MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
         foreach (MonoBehaviour script in scripts)
         {
             script.enabled = false;
         }

		 Debug.Log("Deactivation complete.");
     }
}
