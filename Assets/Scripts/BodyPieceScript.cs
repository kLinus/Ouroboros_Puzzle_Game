using UnityEngine;
using System.Collections.Generic;

public class BodyPieceScript : MonoBehaviour {
	
	// Editor variables.
	public float minDistance = 0;
	public float percentInterpolation;
	public float speed;
	public float rotationSpeed;
	public GameObject spinePrefab;
	public GameObject comboPrefab;
	public List<Material> materials;
	public PlayerScript player;
	public bool isBodyPiece = true;
	
	// Private variables.
	GameObject spine;
	Vector3 velocity;
	float rotationalVelocity;
	int type;//0-number of shapes
	private float defaultRotationSpeed;
	
	// Properties.
	public int BodyPieceType { get { return isBodyPiece ? type : -1; } }
	public GameObject Head { get { return player.GetHead(this.gameObject); } }
	public GameObject Tail { get { return player.GetTail(this.gameObject); } }
	public BodyPieceScript HeadPiece { get { return Head.GetComponent<BodyPieceScript>(); } }
	public BodyPieceScript TailPiece { get { return Tail.GetComponent<BodyPieceScript>(); } }
	
	void Awake(){
		spine = (GameObject)GameObject.Instantiate(this.spinePrefab, Vector3.zero, Quaternion.identity);
		if(isBodyPiece) {
			type = Random.Range(0, materials.Count);
			this.GetComponentInChildren<MeshRenderer>().material = materials[type];
		}
		defaultRotationSpeed = rotationSpeed;
	}

	void FixedUpdate () {
		spine.SetActiveRecursively(player != null);
	
		
		if(player == null){
			return;
		}
		GameObject head = this.Head;
		Vector3 deltaPos = (head.transform.position - minDistance * head.transform.forward) - this.transform.position;
		velocity = Vector3.Lerp(this.velocity, deltaPos, percentInterpolation);
		float deltaAngle = (Vector3.Cross(this.transform.forward, head.transform.forward).y < 0 ? -1 : 1) * Vector3.Angle(this.transform.forward, head.transform.forward);
		rotationalVelocity = rotationSpeed * Mathf.Lerp(this.rotationalVelocity, deltaAngle, percentInterpolation);
		this.transform.position += Time.fixedDeltaTime * speed * velocity;
		this.transform.rotation = Quaternion.AngleAxis(Time.fixedDeltaTime * rotationalVelocity, Vector3.up) * this.transform.rotation;
		this.spine.transform.position = 0.5f * (this.transform.position + head.transform.position) + 0.1f * Vector3.down;
		this.spine.transform.rotation = Quaternion.Slerp(this.transform.rotation, head.transform.rotation, 0.5f);
	}
	
	void OnDestroy(){
		Destroy(spine);
	}
	
	void OnTriggerStay(Collider other) {
		if(other.GetComponent<PlayerScript>() == null) return;
		other.GetComponent<PlayerScript>().GiveBodyPiece(this);
	}
	
	public void SpawnFirework(){
		GameObject.Instantiate(comboPrefab, this.transform.position, Quaternion.identity);
	}
	
	public void GiveToPlayer(PlayerScript player){
		this.player = player;
		this.gameObject.layer = LayerMask.NameToLayer("PlayerBody" + player.playerIndex);
		spine.SetActiveRecursively(true);
		//spine.GetComponentInChildren<MeshRenderer>().material.color = playerIndex == 1 ? Color.cyan : Color.yellow;
		spine.GetComponentInChildren<MeshRenderer>().material.color = player.PlayerColor;
		if(!isBodyPiece){
			this.GetComponentInChildren<MeshRenderer>().material.color = player.PlayerColor;
		}
	}
	
	public float RotationSpeed
	{
		set{ rotationSpeed = value; }
		get{ return rotationSpeed; }
	}
}
