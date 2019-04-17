using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DebugMenu : MonoBehaviour {

	public Dropdown statusDropdown;
	public Button statusButton;
	private UnitController unit; 

	private void Start() {
		var dict = ResourceLibrary.Instance.statusEffectDictionary;
		statusDropdown.AddOptions(dict.Keys.ToList());

		unit = FindObjectOfType<UICanvas>().GetLocalPlayer().unit;

		statusButton.onClick.AddListener( delegate {
			ApplyStatus();
		});
	}

	private void ApplyStatus() {
		var dict = ResourceLibrary.Instance.statusEffectDictionary;
		string selectedOption = statusDropdown.captionText.text;
		if (dict.TryGetValue(selectedOption, out StatusEffect status)) {
			StatusEffect s = Instantiate(status);
			unit.networkHelper.ApplyStatusEffectTo(s);
		} 

	}
}
