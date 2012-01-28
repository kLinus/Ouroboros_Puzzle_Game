using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	// Editor variables.
	public int playerIndex;
	public HeadScript head;
	public float rotationForce;
	public float moveForce;
	
	void Awake () {
		head.gameObject.layer = LayerMask.NameToLayer("Player" + playerIndex);
	}
	
	void FixedUpdate () {
		bool left = (playerIndex == 1 && Input.GetKey(KeyCode.LeftArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.A));
		bool right = (playerIndex == 1 && Input.GetKey(KeyCode.RightArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.D));
		bool up = (playerIndex == 1 && Input.GetKey(KeyCode.UpArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.W));
		bool down = (playerIndex == 1 && Input.GetKey(KeyCode.DownArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.D));
		if(left)
		{
			this.rigidbody.AddTorque(Time.fixedDeltaTime * rotationForce * -this.transform.up);
		}
		if(right)
		{
			this.rigidbody.AddTorque(Time.fixedDeltaTime * rotationForce * this.transform.up);
		}
		if(up)
		{
			this.rigidbody.AddForce(Time.fixedDeltaTime * moveForce * this.transform.forward);
		}
		if(down)
		{
			this.rigidbody.AddForce(Time.fixedDeltaTime * moveForce * -this.transform.forward);
		}
	}
}
