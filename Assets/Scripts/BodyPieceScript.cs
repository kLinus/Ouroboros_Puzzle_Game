using UnityEngine;
using System.Collections;

public class BodyPieceScript : MonoBehaviour {
	
	// Editor variables.
	public float minDistance;
	public float percentInterpolation;
	public GameObject head;

	void FixedUpdate () {
		Vector3 toHead = head.transform.position - this.transform.position;
		float distanceCorrection = toHead.magnitude - minDistance;
		//rigidbody.AddForce(Time.fixedDeltaTime * springStrength * distanceCorrection * toHead.normalized);
		this.transform.position =
			(1f - percentInterpolation) * this.transform.position + 
			percentInterpolation * (head.transform.position - minDistance * head.transform.forward);
	}
}
