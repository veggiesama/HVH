using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AiManager: MonoBehaviour {

	[HideInInspector] public UnitController unit;
	[SerializeField] private List<AiState> aiStates;
	[SerializeField] private List<AiState> aiStatesDay;
	[SerializeField] private List<AiState> aiStatesNight;
	private AiState currentState;
	public float updateEvery;

	private void Awake() {
		unit = GetComponent<UnitController>();		
	}

	public void OnEnable() {
		DayNight.Instance.onStartDay.AddListener(OnStartDay);
		DayNight.Instance.onStartNight.AddListener(OnStartNight);
	}

	/*
	public void OnDisable() {
		if (DayNight.Instance == null) return;
		DayNight.Instance.onStartDay.RemoveListener(OnStartDay);
		DayNight.Instance.onStartNight.RemoveListener(OnStartNight);
		//DayNight.Instance = null;
	}*/

	public void Initialize(UnitInfo unitInfo) {
		this.aiStatesDay = CloneAiStateList(unitInfo.aiStatesDay);
		this.aiStatesNight = CloneAiStateList(unitInfo.aiStatesNight);
		this.updateEvery = unitInfo.aiUpdateEvery;
		OnStartDay();

		StartCoroutine( SlowUpdate() );
	}

	private void OnStartDay() {
		aiStates = aiStatesDay;
	}

	private void OnStartNight() {
		aiStates = aiStatesNight;
	}

	// clone state list so that each NPC controls their own states
	private List<AiState> CloneAiStateList(List<AiState> list) {
		List<AiState> cloneList = new List<AiState>();
		foreach (AiState state in list) {
			AiState cloneState = Instantiate(state);
			cloneList.Add(cloneState);
			cloneState.Initialize(this);
		}
		return cloneList;
	}

	IEnumerator SlowUpdate() {
		while (true) {
			yield return new WaitForSeconds(updateEvery);
						
			AiState desiredState = DetermineDesiredState();
			if (desiredState == null) {
				EndCurrentState();
				continue;
			}

			if (currentState != null && currentState.Equals(desiredState) && currentState.hasExecuted) {
				currentState.Update();
			}

			else {
				EndCurrentState();
				currentState = desiredState;
				currentState.Execute();
			}
		}
	}

	public AiState DetermineDesiredState() {
		if (aiStates.Count > 0) {	
			foreach (AiState state in aiStates) {
				state.Evaluate();
			}
			aiStates.Sort((x, y) => y.desire.CompareTo(x.desire)); // descending sort by desire

			if (aiStates[0].desire > (int)Desire.NONE)
				return aiStates[0];
		}

		// if no states or all states have 0 desire
		return null;
	}

	private void EndCurrentState() {
		if (currentState != null) {
			currentState.End();
			currentState = null;
		}
	}
	
	public string GetCurrentState() {
		if (currentState != null)
			return currentState.GetType().ToString();
		else
			return "null";
	}

}
