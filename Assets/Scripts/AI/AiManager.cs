using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AiManager: MonoBehaviour {

	[HideInInspector] public UnitController unit;
	[SerializeField] private List<AiState> aiStates;
	private AiState currentState;

	public float updateEvery = 0.2f;

	private void Awake() {
		unit = GetComponent<UnitController>();		
	}

	private void Start() {

		// duplicating state list so that each NPC controls their own states
		List<AiState> duplicateAiStates = new List<AiState>();
		foreach (AiState state in aiStates) {
			AiState duplicateState = Instantiate(state);
			duplicateAiStates.Add(duplicateState);
			duplicateState.Initialize(this);
		}
		aiStates = duplicateAiStates;


		StartCoroutine( SlowUpdate() );
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

	private void EndCurrentState() {
		if (currentState != null) {
			currentState.End();
			currentState = null;
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

}
