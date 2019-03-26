using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour {

	public Dropdown statusDropdown;
	public Button statusButton;
	private UnitController unit; 

	private void Start()
	{
		unit = GameController.GetLocalOwner().unit;

		statusButton.onClick.AddListener( delegate {
			ApplyStatus();
		});
	}

	private void ApplyStatus() {

		StatusEffect status;
		switch (statusDropdown.captionText.text)
		{
			case "Airborn":
				status = ScriptableObject.CreateInstance<Airborn>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Dead":
				status = ScriptableObject.CreateInstance<Dead>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, null);
				break;
			case "Immobilized":
				status = ScriptableObject.CreateInstance<Immobilized>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Invisible":
				status = ScriptableObject.CreateInstance<Invisible>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Invulnerable":
				status = ScriptableObject.CreateInstance<Invulnerable>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Revealed":
				status = ScriptableObject.CreateInstance<Revealed>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Silenced":
				status = ScriptableObject.CreateInstance<Silenced>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Slowed":
				status = ScriptableObject.CreateInstance<Slowed>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			case "Stunned":
				status = ScriptableObject.CreateInstance<Stunned>();
				status.Reset();
				unit.ApplyStatusEffect(status, null, unit);
				break;
			default:
				break;
		}
	}
}
