using UnityEngine;
using System.Collections;

public class HeadScript : MonoBehaviour {
	
	// Editor variables.
	public BodyPieceScript bodyPiecePrefab;
	
	void Start(){
		GameObject headObject = this.gameObject;
		for(int n = 0; n < 10; n++){
			BodyPieceScript bodyPiece = (BodyPieceScript)GameObject.Instantiate(bodyPiecePrefab, this.transform.position - n * this.transform.forward, Quaternion.identity);
			bodyPiece.gameObject.layer = this.gameObject.layer;
			bodyPiece.head = headObject;
			headObject = bodyPiece.gameObject;
		}
	}

	
}
