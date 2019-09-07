using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tree = HVH.Tree;

// might be more accurate to call physics tree?
public class DeadTree : MonoBehaviour {
	public float initialDelay = 5f;
	public float stopSinkingAfter = 10f;
	public float sinkSpeed = 0.85f;
	public float growthSpeed = 0.45f;
	private Rigidbody rb;
	private bool isSinking = false;
	private bool isRising = false;
	private float colliderHeight;
	private Vector3 startGrowth, targetGrowth;
	private float lerp;
	private Tree tree;
	public float rngDelayAdd, rngSpeedFactor;

	private void Start() {
		rb = GetComponent<Rigidbody>();
		colliderHeight = GetComponent<BoxCollider>().bounds.size.y;
		tree = GetComponentInParent<Tree>();
		rngDelayAdd = Random.Range(0f, 1f);
		rngSpeedFactor = Random.Range(0.8f, 1.2f);
	}

	public void StartSinking() {
		StartCoroutine( _StartSinking() );
		StartCoroutine( StopSinking() );
	}

	private IEnumerator _StartSinking () {
		yield return new WaitForSeconds(initialDelay+rngDelayAdd);
		rb.isKinematic = true;
		isSinking = true;
		yield return null;
	}

	private IEnumerator StopSinking () {
		yield return new WaitForSeconds(initialDelay+rngDelayAdd+stopSinkingAfter);
		isSinking = false;
		yield return null;
	}

	public IEnumerator StartRising(Vector3 pos, Quaternion rot, Vector3 scale) {
		yield return new WaitForSeconds(rngDelayAdd);
		isRising = true;
		startGrowth = pos + (Vector3.down * colliderHeight);
		targetGrowth = pos;
		lerp = 0;
		ResetTransform(startGrowth, rot, scale);
		yield return null;
	}

	public void ResetTransform(Vector3 pos, Quaternion rot, Vector3 scale) {
		this.gameObject.transform.position = pos;
		this.gameObject.transform.rotation = rot;
		this.gameObject.transform.localScale = scale;
	}

	private void StopRising() {
		isRising = false;
		rb.isKinematic = false;
		this.gameObject.transform.position = targetGrowth;

		tree.EnableTree(true);
		this.gameObject.SetActive(false);
	}

	private void FixedUpdate() {
		if (isSinking)
			this.gameObject.transform.position += (Vector3.down * (sinkSpeed * rngSpeedFactor * Time.fixedDeltaTime));

		if (isRising) {
			this.gameObject.transform.position = Vector3.Lerp(startGrowth, targetGrowth, lerp/1);
			lerp += (Time.fixedDeltaTime * growthSpeed * rngSpeedFactor);

			if (lerp/1 >= 1)
				StopRising();
		}
	}
}