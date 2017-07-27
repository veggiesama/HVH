using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastbarController : MonoBehaviour {

	public Slider castbar;

	private float maxTime = 1;
	private float currentTime = 0;
	private float reverseTime = 0;
	//private OwnerController owner;
	private UnitController unit;
	private Vector3 initialPosition;

	// Use this for initialization
	void Start () {
	}
	
	public void Initialize(UnitController unit, float maxTime, float reverseTime = 0.0f) {
		this.unit = unit;
		this.maxTime = maxTime;
		this.currentTime = maxTime;
		this.reverseTime = reverseTime;		

		if (unit != null)
			initialPosition = unit.GetBodyPosition();
	}

	public void Stop()
	{
		currentTime = 0;
		reverseTime = 0;
	}

	private bool DidControllerMove() {
		return unit != null &&
			initialPosition != unit.GetBodyPosition();
	}

	private bool ReadyForReverseTime() {
		return !DidControllerMove() &&
			reverseTime > 0 &&
			currentTime > -reverseTime;
	}

	// Update is called once per frame
	void Update () {
		currentTime -= Time.deltaTime;

		if (currentTime <= 0) {
			if (ReadyForReverseTime()) {
				castbar.gameObject.SetActive(true);
				castbar.value = (currentTime / -reverseTime);
			}
			else
				castbar.gameObject.SetActive(false);
		}
		else if (DidControllerMove()) {
			castbar.gameObject.SetActive(false);
		}
		else {
			castbar.gameObject.SetActive(true);
			castbar.value = currentTime / maxTime;
		}

	}
}
