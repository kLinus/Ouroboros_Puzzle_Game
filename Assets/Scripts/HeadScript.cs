using UnityEngine;
using System.Collections;

public class HeadScript : MonoBehaviour {
	
	// Editor variables.
	public float rotationForce;
	public float moveForce;
	public BodyPieceScript bodyPiecePrefab;
	
	void Start(){
		GameObject headObject = this.gameObject;
		for(int n = 0; n < 10; n++){
			BodyPieceScript bodyPiece = (BodyPieceScript)GameObject.Instantiate(bodyPiecePrefab, this.transform.position - n * this.transform.forward, Quaternion.identity);
			bodyPiece.head = headObject;
			headObject = bodyPiece.gameObject;
		}
	}

	void FixedUpdate () {
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			this.rigidbody.AddTorque(Time.fixedDeltaTime * rotationForce * -this.transform.up);
		}
		if(Input.GetKey(KeyCode.RightArrow))
		{
			this.rigidbody.AddTorque(Time.fixedDeltaTime * rotationForce * this.transform.up);
		}
		if(Input.GetKey(KeyCode.UpArrow))
		{
			this.rigidbody.AddForce(Time.fixedDeltaTime * moveForce * this.transform.forward);
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			this.rigidbody.AddForce(Time.fixedDeltaTime * moveForce * -this.transform.forward);
		}
	}
}
