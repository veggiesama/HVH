using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityController : MonoBehaviour {

	[HideInInspector] public UnitController caster, allyTarget, enemyTarget;
	[HideInInspector] public UnitInfo unitInfo;
	[HideInInspector] public Vector3 targetLocation;

	[HideInInspector] public float cooldownTimeRemaining = 0.0f;

	public AbilityTargetTypes targetType;
	public AbilityTargetTeams targetTeam;
	public DamageTypes damageType;
	public float damage = 0;
	public float cooldown = 0;
	public float castRange = 0;
	public float castTime = 0;
	//public float channelTime = 0;
	public float duration = 0;
	//public float aoeRadius = 0;

	// Use this for initialization
	protected virtual void Start () {
		caster = GetComponentInParent<UnitController>();
		unitInfo = GetComponentInParent<UnitInfo>();
	}
	public virtual void Update() {
		if (cooldownTimeRemaining > 0) 
			cooldownTimeRemaining -= Time.deltaTime;
	}

	public virtual bool Cast() {
		if (!IsCooldownReady()) return false;

		targetLocation = GetMouseLocation();
		allyTarget = caster.GetTarget(AbilityTargetTeams.ALLY);
		enemyTarget = caster.GetTarget(AbilityTargetTeams.ENEMY);

		if (!IsTargetInRange()) return false;

		cooldownTimeRemaining = cooldown;

		return true;
	}

	private Vector3 GetMouseLocation() {
		if (targetType != AbilityTargetTypes.POINT) {
			return Util.GetNullVector();
		}

		RaycastHit hit;
		Ray ray = (Ray)Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 100))
			return hit.point;
		else
			return Util.GetNullVector();
	}

	public bool IsCooldownReady() {
		if (cooldownTimeRemaining <= 0)
			return true;
		else {
			print("Ability on cooldown.");
			return false;
		}
	}

	public bool IsTargetInRange() {
		if (castRange == 0) return true;

		switch (targetType)
		{
			case AbilityTargetTypes.NONE:
				break;
			case AbilityTargetTypes.UNIT:
				switch (targetTeam)
				{
					case AbilityTargetTeams.ALLY:
						if (!allyTarget) {
							print("No ally selected.");
							return false;
						}
						else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), allyTarget.GetBodyPosition()) < castRange) {
							print("Target ally out of cast range.");
							return false;
						}
						break;

					case AbilityTargetTeams.ENEMY:
						if (!enemyTarget) {
							print("No enemy selected.");
							return false;
						}
						else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), enemyTarget.GetBodyPosition()) > castRange) {
							print("Target enemy out of cast range.");
							return false;
						}
						break;
					
					case AbilityTargetTeams.BOTH:
						if (!allyTarget && !enemyTarget) {
							print("Neither ally nor enemy selected.");
							return false;
						}
						// nothing checks for range yet
						break;
	
					default:
						break;
				}
				break;
			case AbilityTargetTypes.POINT:
				if (Util.IsNullVector(targetLocation)) {
					print("Invalid target location");
					return false;
				}
				else if (Util.GetDistanceIn2D(caster.GetBodyPosition(), targetLocation) > castRange) {
					print("Target location out of cast range.");
					return false;
				}
				break;
			case AbilityTargetTypes.PASSIVE:
				break;
			case AbilityTargetTypes.CHANNELLED:
				break;
			case AbilityTargetTypes.SHAPE:
				break;
			default:
				print("Unexpected AbilityTargetType value.");
				return false;
		}

		return true;
	}

}
