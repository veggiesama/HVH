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

		switch (airbornClippingType) {
			case AirbornClippingTypes.NO_CLIP:
				unit.body.SetNoclip();
				break;
			case AirbornClippingTypes.TREE_CLIP:
				//unit.body.SetTreeClipOnly();
				break;
			case AirbornClippingTypes.TREE_DESTROY:
				//unit.body.SetTreeClipOnly();
				//unit.body.SetTriggerable(true);
				unit.body.OnCollidedTreeEventHandler += OnCollidedTree; // event sub
				break;
		}
	}

	public void OnCollidedTree(Tree tree) {
		if (alreadyTriggeredList.Contains(tree)) return;

		Debug.Log("Collided with tree");
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
		unit.body.OnCollidedTreeEventHandler -= OnCollidedTree; // event unsub
		unit.AttachToNav();
		unit.body.ResetBody();
		base.End();
	}
}
