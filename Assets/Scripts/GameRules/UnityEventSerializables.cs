using Tree = HVH.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventSerializables : MonoBehaviour
{ }

[System.Serializable]
public class UnityEventTree : UnityEvent <Tree> {}

[System.Serializable]
public class UnityEventCollision : UnityEvent <Collision> {}