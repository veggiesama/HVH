using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Airborn")]
public class Airborn : StatusEffect {

	[Header("Airborn")]
	public AirbornClippingTypes airbornClippingType;
	private List<Tree> alreadyTriggeredList = new List<Tree>();

	 // default field values; called by editor and serialized into asset before Initialize() is called
	public override void Reset()
	{
		statusName = "Airborn";
		type = StatusEffectTypes.AIRBORN;
		duration = 3f;
		airbornClippingType = AirbornClippingTypes.NO_CLIP;
	}
	
	// initializer
	public override void Initialize(GameObject obj, Ability ability) {
		base.Initialize(obj, ability);
	}

	public override void Apply()
	{
		base.Apply();
		unit.DetachFromNav();
		unit.SetVision(VisionType.FLYING);

		switch (airbornClippingType) {
			case AirbornClippingTypes.NO_CLIP:
				unit.body.SetNoclip();
				break;
			case AirbornClippingTypes.TREE_CLIP:
				break;
			case AirbornClippingTypes.TREE_DESTROY:
				unit.body.onCollidedTree.AddListener(OnCollidedTree); // sub
				break;
		}
	}

	void OnCollidedTree(Tree tree) {
		if (alreadyTriggeredList.Contains(tree)) return;
		alreadyTriggeredList.Add(tree);
		networkHelper.DestroyTree(tree, unit.GetBodyPosition(), 0f);
	}

	public override void Update() {
		base.Update(); // tracks duration
	}

	public override void FixedUpdate() {}

	public override void Stack(StatusEffect status) {
		//base.Stack(status);
	}

	public override void End() {
		unit.SetVision(VisionType.NORMAL);
		unit.body.onCollidedTree.RemoveListener(OnCollidedTree); // unsub

		if (!unit.HasStatusEffect(StatusEffectTypes.DEAD)) {
			unit.AttachToNav();
			unit.body.ResetBody();
		}

		base.End();
	}
}
