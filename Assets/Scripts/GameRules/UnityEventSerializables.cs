﻿using Tree = HVH.Tree;
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

[System.Serializable]
public class UnityEventDamage : UnityEvent <float> {}

[System.Serializable]
public class UnityEventHealth : UnityEvent <float> {}

[System.Serializable]
public class UnityEventChooseCharacter: UnityEvent <int> {}

[System.Serializable]
public class UnityEventNetworkStatusEffect: UnityEvent <NetworkStatusEffect> {}
