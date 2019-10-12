using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AOEGenerator : NetworkBehaviour {
	private float reappliesEvery;
	private StatusEffect[] statusEffects;
	private UnitController sourceUnit;
	private Ability sourceAbility;
	private float reappliesEveryTimer = 0f;
	private float durationTimer;
	private bool destroysTrees;
	private NetworkHelper networkHelper;
	private float delay = 0f;
	private float delayTimer = 0f;
	private bool began = false;
	private bool initialized = false;
	private GameObject particlePrefab;

	//public ParticleSystem particleSystem;

	// Start is called before the first frame update
    public void Start() {
        reappliesEveryTimer = reappliesEvery;
    }

	public void Initialize(UnitController sourceUnit, Ability sourceAbility) {
		this.sourceUnit = sourceUnit;
		this.networkHelper = sourceUnit.networkHelper;
		this.sourceAbility = sourceAbility;
		durationTimer = sourceAbility.duration;

		IAoeGeneratorAbility aoeGenAbility = GetIAoeGeneratorAbility(sourceAbility);
		if (aoeGenAbility == null) return;

		this.particlePrefab = aoeGenAbility.GetAoeGenParticlePrefab();
		this.statusEffects = aoeGenAbility.GetStatusEffects();
		this.reappliesEvery = aoeGenAbility.GetReappliesEvery();
		this.destroysTrees = aoeGenAbility.GetDestroysTrees();
		this.delay = aoeGenAbility.GetDelay();
		this.delayTimer = this.delay;

		if (delay <= 0) began = true;
		initialized = true;

	}

	// update is only called on the server
    private void Update() {
		if (!isServer || !initialized) return;

		// DELAY BEFORE BEGINNING
		if (delayTimer > 0) {
			delayTimer -= Time.deltaTime;
			return;
		}

		if (delayTimer <= 0 && !began) {
			began = true;
			Begin();
		}

		// REAPPLIES EVERY (PULSE)
        reappliesEveryTimer -= Time.deltaTime;
		if (reappliesEveryTimer <= 0) {
			Pulse();
			reappliesEveryTimer = reappliesEvery;
		}

		// TOTAL DURATION
		durationTimer -= Time.deltaTime;
		if (durationTimer <= 0) {
			End();
		}
    }

	private void Begin() {
		NetworkParticle np = new NetworkParticle(particlePrefab.name, transform.position, transform.rotation, sourceAbility.duration, sourceAbility.aoeRadius);
		networkHelper.InstantiateParticle(np);
		Pulse();
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
				networkHelper.DestroyTree(tree, transform.position, rng);
			}
		}
	}

	private void ReapplyStatusEffects(UnitController unit) {
		foreach (StatusEffect effect in statusEffects) {
			networkHelper.ApplyStatusEffectTo(unit, effect, sourceAbility);
			//unit.ApplyStatusEffect(effect, sourceAbility);
		}
	}

	private void End() {
		NetworkServer.Destroy(this.gameObject);
	}

	private static IAoeGeneratorAbility GetIAoeGeneratorAbility(Ability abi) {
		if (abi is IAoeGeneratorAbility)
			return (IAoeGeneratorAbility)abi;
		else {
			Debug.Log(abi.abilityName + " is not an IAoeGeneratorAbility.");
			return null;
		}
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if (!initialized || !began) return;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, sourceAbility.aoeRadius);
	}
	#endif
}
