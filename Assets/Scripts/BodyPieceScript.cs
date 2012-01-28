using UnityEngine;
using System.Collections;

public class BodyPieceScript : MonoBehaviour {
	
	// Editor variables.
	public float minDistance;
	public float percentInterpolation;
	public float speed;
	public float rotationSpeed;
	public GameObject head, tail;
	
	// Private variables.
	Vector3 velocity;
	float rotationalVelocity;

	void FixedUpdate () {
		Vector3 deltaPos = (head.transform.position - minDistance * head.transform.forward) - this.transform.position;
		velocity = Vector3.Lerp(this.velocity, deltaPos, percentInterpolation);
		Vector3 forwardDelta = head.transform.forward - this.transform.forward;
		float deltaAngle = (Vector3.Cross(this.transform.forward, this.head.transform.forward).y < 0 ? -1 : 1) * Vector3.Angle(this.transform.forward, this.head.transform.forward);
		if(head.GetComponent<HeadScript>() != null)Debug.Log(deltaAngle);
		rotationalVelocity = rotationSpeed * Mathf.Lerp(this.rotationalVelocity, deltaAngle, percentInterpolation);
		this.transform.position += Time.fixedDeltaTime * speed * velocity;
		this.transform.rotation = Quaternion.AngleAxis(Time.fixedDeltaTime * rotationalVelocity, Vector3.up) * this.transform.rotation;
	}
}
