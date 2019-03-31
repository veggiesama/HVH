using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
	private Dictionary<string, UnityEvent> eventDictionary;
	private static EventManager eventManager;
	public static EventManager instance {
		get {
			if (!eventManager) {
				eventManager = FindObjectOfType (typeof(EventManager)) as EventManager;
//				eventManager = FindObjectOfType<EventManager>();
				if (eventManager != null)
					eventManager.Initialize();
				else
					Debug.Log("There needs to be one active EventManger script on a GameObject in the scene.");
			}
			return eventManager;
		}
	}



	private void Initialize() {
		if (eventDictionary == null)
			eventDictionary = new Dictionary<string, UnityEvent>();
	}


	public static void StartListening(string eventName, UnityAction listener) {
		if (instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent)) { // supposedly more efficient than ContainsKey
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			instance.eventDictionary.Add(eventName, thisEvent);
		}
	}

	public static void StopListening(string eventName, UnityAction listener) {
		if (eventManager == null) return;
		if (instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent)) {
			thisEvent.RemoveListener(listener);
		}
	}

	public static void TriggerEvent(string eventName) {
		if (instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent)) {
			thisEvent.Invoke();
		}
	}

}
