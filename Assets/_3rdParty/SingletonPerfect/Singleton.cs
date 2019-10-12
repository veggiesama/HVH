using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {

	/*
	This is required in every script that inherits from Singleton for it to work properly:
	
	:: First, Inherit from Singleton class like so,
	public class YOURTYPE : Singleton<YOURTYPE> {}
	
	:: Second, include this variable so you can access the instance of your singleton.
	public static YOURTYPE Instance {
		get {
			return ((YOURTYPE)mInstance);
		} set {
			mInstance = value;
		}
	}
	
	:: Third, Voila! Now you can access the instance of your singleton with YOURTYPE.Instance
	*/
	
	protected static Singleton<T> mInstance {
		get {
			if(!_mInstance)
			{
				T [] managers = FindObjectsOfType<T>();
				if(managers.Length != 0)
				{
					if(managers.Length == 1)
					{
						_mInstance = managers[0];
						//_mInstance.gameObject.name = typeof(T).Name;
						return _mInstance;
					} else {
						Debug.LogError("You have more than one " + typeof(T).Name + " in the scene. You only need 1, it's a singleton!");
						foreach(T manager in managers)
						{
							Destroy(manager.gameObject);
						}
					}
				}
				//GameObject gO = new GameObject(typeof(T).Name, typeof(T));
				//_mInstance = gO.GetComponent<T>();
				
				// VEG: removing this line
				//DontDestroyOnLoad(gO);
			}
			return _mInstance;
		} set {
			_mInstance = value as T;
		}
	}
	private static T _mInstance;
}
