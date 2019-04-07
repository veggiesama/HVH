using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEGenerator : MonoBehaviour {
	private float reappliesEvery;
	private StatusEffect[] statusEffects;
	private UnitController sourceUnit;
	private Ability sourceAbility;
	private float currentTimer = 0f;
	private float remainingDuration;
	private bool destroysTrees;

	//public ParticleSystem particleSystem;

	// Start is called before the first frame update
    public void Start()
    {
        currentTimer = reappliesEvery;
    }

	public void Initialize(UnitController sourceUnit, Ability sourceAbility, StatusEffect[] statusEffects, float reappliesEvery, bool destroysTrees) {
		this.sourceUnit = sourceUnit;
		this.sourceAbility = sourceAbility;
		this.statusEffects = statusEffects;
		this.reappliesEvery = reappliesEvery;
		this.destroysTrees = destroysTrees;
		remainingDuration = sourceAbility.duration;

		Pulse();
	}

    // Update is called once per frame
    private void Update() {
        currentTimer -= Time.deltaTime;
		if (currentTimer <= 0) {
			Pulse();
			currentTimer = reappliesEvery;
		}

		remainingDuration -= Time.deltaTime;
		if (remainingDuration <= 0) {
			End();
		}
    }

	private void Pulse() {
		Collider[] colliders = Physics.OverlapSphere(transform.position, sourceAbility.aoeRadius);
		foreach (Collider col in colliders) {
			if (Util.IsBody(col.gameObject)) {
				UnitController unit = col.gameObject.GetComponentInParent<UnitController>(); 
				if (!sourceUnit.SharesTeamWith(unit)) // TODO: fix to be based on sourceAbility.targetTeam
					ReapplyStatusEffects(unit);
			}
			else if (destroysTrees && Util.IsTree(col.gameObject)) {
				float rng = Random.Range(0, reappliesEvery * 0.2f); // destroy each tree at a random time before next Pulse()
				Tree tree = col.gameObject.GetComponent<Tree>();
				sourceUnit.GetPlayer().DestroyTree(tree, transform.position, rng);
			}
		}
	}

	private void ReapplyStatusEffects(UnitController unit) {
		foreach (StatusEffect effect in statusEffects) {
			unit.ApplyStatusEffect(effect, sourceAbility, sourceUnit);
		}
	}

	private void End() {
		Destroy(this.gameObject);
	}

}
