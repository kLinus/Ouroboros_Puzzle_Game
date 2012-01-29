using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {
	
	// Editor variables.
	public int playerIndex;
	public float rotationForce;
	public float rotationForcePerLevel;
	public float moveForce;
	public float moveForcePerLevel;
	public float eatCooldown;//seconds between eating a piece
	public float eatCooldownPerLevel;
	public BodyPieceScript bodyPiecePrefab;
	public BodyPieceScript tailPiecePrefab;
	public GameObject wingPiecePrefab;
	public int startingPieces;
	
	[System.Serializable]
	public class BurstOfSpeedProperties {
		public float maxForce;
		public float forceMultiplier;
		public float cooldownInSeconds;
	}public BurstOfSpeedProperties BurstOfSpeed;
	
	[System.Serializable]
	public class RepelProperties {
		public float maxForce;
		public float maxRange;
		public float forceDecayMultiplier;
		public float growthMultiplier;
		public float cooldown;
		public GameObject repelExplosionPrefab;
	}public RepelProperties Repel;
	
	[System.Serializable]
	public class StraightenProperties{
		public float cooldown;
	}public StraightenProperties Straighten;
	
	// Private variables.
	private LinkedList<GameObject> pieces;
	private GameObject wing1, wing2;
	private Color player1Color = new Color(0.23f, 0.96f, 0.64f);
	private Color player2Color = new Color(0.58f, 0.98f, 0.21f);
	private float eatCooldownTime;//seconds left before can eat a piece
	private int eatingSkill;
	private int speedSkill;
	private int rotatingSkill;
	
	//Burst of Speed abiltiy variables
	private bool burstOfSpeedEnabled = true;
	private bool burstOfSpeedInUse = false;
	private bool burstOfSpeedIsOnCooldown = false;
	private float defaultMoveForce;
	private float burstOfSpeedLastCastTime = 0;
	
	//Repel ability variables
	private bool repelEnabled = true;
	private bool repelIsOnCooldown = false;
	private float repelLastCastTime = 0;
	
	//Controls
	private KeyCode p1_BurstOfSpeedKey = KeyCode.RightShift;
	private KeyCode p1_RepelKey = KeyCode.Slash;
	private KeyCode p1_StraightenKey = KeyCode.Period;
	
	private KeyCode p2_BurstOfSpeedKey = KeyCode.LeftShift;
	private KeyCode p2_RepelKey = KeyCode.E;
	private KeyCode p2_StraightenKey = KeyCode.Q;
	
	public Color PlayerColor {
		get { return playerIndex == 1 ? player1Color : player2Color; }
	}
	
	public GameObject GetHead(GameObject gameObject){
		return pieces.Find(gameObject).Previous.Value;
	}
	
	public GameObject GetTail(GameObject gameObject){
		return pieces.Find(gameObject).Next.Value;
	}
	
	void Awake () {
		this.GetComponentInChildren<MeshRenderer>().material.color = PlayerColor;
		defaultMoveForce = moveForce;
	}
	
	void Start(){
		// Add starting body pieces.
		pieces = new LinkedList<GameObject>();
		pieces.AddLast(this.gameObject);
		for(int n = 0; n < startingPieces; n++){
			BodyPieceScript bodyPiece = (BodyPieceScript)GameObject.Instantiate(bodyPiecePrefab, this.transform.position - n * this.transform.forward, Quaternion.identity);
			pieces.AddLast(bodyPiece.gameObject);
			bodyPiece.GiveToPlayer(this);
		}
		
		// Add tail.
		BodyPieceScript tailBodyPiece = (BodyPieceScript)GameObject.Instantiate(tailPiecePrefab, this.transform.position - startingPieces * this.transform.forward, Quaternion.identity);
		pieces.AddLast(tailBodyPiece.gameObject);
		tailBodyPiece.GiveToPlayer(this);
		tailBodyPiece.gameObject.layer = LayerMask.NameToLayer("PlayerTail" + playerIndex);
		
		wing1 = (GameObject)GameObject.Instantiate(wingPiecePrefab, Vector3.zero, Quaternion.identity);
		wing1.GetComponentInChildren<MeshRenderer>().material.color = PlayerColor;
		wing2 = (GameObject)GameObject.Instantiate(wingPiecePrefab, Vector3.zero, Quaternion.identity);
		wing2.GetComponentInChildren<MeshRenderer>().material.color = PlayerColor;
		wing2.transform.localScale = new Vector3(-wing2.transform.localScale.x, wing2.transform.localScale.y, wing2.transform.localScale.z);
	}
	
	void FixedUpdate () {
		CheckForMatches();
		eatCooldownTime -= Time.fixedDeltaTime;
	
		//Burst of Speed Ability
		#region Burst of Speed
		if(burstOfSpeedEnabled || burstOfSpeedInUse)
		{
			bool shift = (playerIndex == 1 && Input.GetKey(p1_BurstOfSpeedKey)) || (playerIndex == 2 && Input.GetKey(p2_BurstOfSpeedKey));
			
			if( shift && !burstOfSpeedInUse)
			{
				burstOfSpeedInUse = true;
				burstOfSpeedEnabled = false;
				//Debug.Log("Burst of Speed Now in Use for" + playerIndex);
			}
			else if (burstOfSpeedInUse)
			{
				moveForce *= BurstOfSpeed.forceMultiplier;
				if(moveForce >= BurstOfSpeed.maxForce)
				{
					moveForce = defaultMoveForce;
					burstOfSpeedLastCastTime = Time.realtimeSinceStartup;
					//Debug.Log("Burst of Speed has completed for " +playerIndex);
					burstOfSpeedInUse = false;
					burstOfSpeedIsOnCooldown = true;
				}
			}
		}
		#endregion
		
		//Repel Ability
		#region Repel
		if(repelEnabled)
		{
			//Debug.Log("Repel is ready.");	
			bool repelButton = (playerIndex == 1 && Input.GetKey(p1_RepelKey)) || (playerIndex == 2 && Input.GetKey(p2_RepelKey));
			
			if (repelButton && !repelIsOnCooldown)
			{
				GameObject repelExplosion = Instantiate(Repel.repelExplosionPrefab , this.gameObject.transform.position, Quaternion.identity) as GameObject;
				repelExplosion.GetComponent<RepelScript>().SetRepelAttributes( Repel.maxRange, Repel.maxForce, 
																			   Repel.forceDecayMultiplier, Repel.growthMultiplier, playerIndex);
				repelExplosion.GetComponent<RepelScript>().RepelIsArmed = true;
				repelEnabled = false;
				repelIsOnCooldown = true;
				repelLastCastTime = Time.realtimeSinceStartup;
				//Debug.Log("Repel has been used");
			}
		}
		#endregion
		
		//Straighten Ability
		
		
		//Movement
		bool left = (playerIndex == 1 && Input.GetKey(KeyCode.LeftArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.A));
		bool right = (playerIndex == 1 && Input.GetKey(KeyCode.RightArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.D));
		bool up = (playerIndex == 1 && Input.GetKey(KeyCode.UpArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.W));
		bool down = (playerIndex == 1 && Input.GetKey(KeyCode.DownArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.S));
		float realMoveForce = moveForce + speedSkill * moveForcePerLevel;
		float realRotationForce = rotationForce + rotatingSkill * rotationForcePerLevel;
		if(left)
		{
			this.rigidbody.AddTorque(Time.fixedDeltaTime * realRotationForce * -this.transform.up);
		}
		if(right)
		{
			this.rigidbody.AddTorque(Time.fixedDeltaTime * realRotationForce * this.transform.up);
		}
		if(up)
		{
			this.rigidbody.AddForce(Time.fixedDeltaTime * realMoveForce * this.transform.forward);
		}
		if(down)
		{
			this.rigidbody.AddForce(Time.fixedDeltaTime * realMoveForce * -this.transform.forward);
		}
		
		#region Cooldown Checks
		//Check cooldowns
		if (burstOfSpeedIsOnCooldown){
			if (Time.realtimeSinceStartup - burstOfSpeedLastCastTime > BurstOfSpeed.cooldownInSeconds)
			{
				burstOfSpeedEnabled = true;
				burstOfSpeedIsOnCooldown = false;
				//Debug.Log("Burst of Speed is cooled down for" + playerIndex);
			}
		}
		
		if (repelIsOnCooldown){
			if (Time.realtimeSinceStartup - repelLastCastTime > Repel.cooldown)
			{
				repelEnabled = true;
				repelIsOnCooldown = false;
				//Debug.Log("Repel is cooled down for " + playerIndex);
			}
		}
		#endregion
		
		//Update wings.
		int middleIndex = pieces.Count / 2;
		LinkedListNode<GameObject> middleNode = pieces.First;
		for(int n = 0; n < middleIndex; n++) middleNode = middleNode.Next;
		wing1.transform.position = middleNode.Value.transform.position + middleNode.Value.transform.right;
		wing2.transform.position = middleNode.Value.transform.position - middleNode.Value.transform.right;
		wing1.transform.forward = middleNode.Value.transform.forward;
		wing2.transform.forward = middleNode.Value.transform.forward;
	}
	
	void RemoveBodyPiece(GameObject bodyPiece, bool destroyPiece){
		pieces.Remove(bodyPiece);
		if(destroyPiece) GameObject.Destroy(bodyPiece);
	}
	
	void CheckForMatches(){
		LinkedListNode<GameObject> currentBodyPiece = pieces.First.Next;
		LinkedListNode<GameObject> startingComboPiece = currentBodyPiece;
		int previousType = -1;
		int typeCount = 0;
		do {
			BodyPieceScript piece = currentBodyPiece.Value.GetComponent<BodyPieceScript>();
			if(piece.BodyPieceType == previousType){
				typeCount++;
			} else {
				if(typeCount >= 3){
					DoCombo(startingComboPiece, typeCount, previousType);
					return;
				}
				startingComboPiece = currentBodyPiece;
				typeCount = 1;
				previousType = piece.BodyPieceType;
			}
			currentBodyPiece = currentBodyPiece.Next;
		} while(currentBodyPiece != null);
		if(typeCount >= 3) DoCombo(startingComboPiece, typeCount, previousType);
	}
	
	void DoCombo(LinkedListNode<GameObject> startingComboPiece, int pieces, int type){
		Debug.Log("Doing combo for player " + playerIndex + " with " + pieces);
		// Find last combo piece and delete all pieces in between.
		
		for(int n = 0; n < pieces; n++){
			LinkedListNode<GameObject> next = startingComboPiece.Next;
			RemoveBodyPiece(startingComboPiece.Value, true);
			if(n != 0 && n != pieces - 1) startingComboPiece.Value.GetComponent<BodyPieceScript>().SpawnFirework();
			startingComboPiece = next;
		}
		
		if(type == 0) eatingSkill += pieces - 2; //Circle
		if(type == 1) speedSkill += pieces - 2; //Square
		if(type == 2) rotatingSkill += pieces - 2; //Triangle
		if(type == 3){ //Star
			foreach(Object gameObject in GameObject.FindObjectsOfType(typeof(PlayerScript))){
				PlayerScript player = gameObject as PlayerScript;
				if(player == this) continue;
				player.RemoveBodyPiece(player.pieces.First.Next.Value, true);
			}
		}
	}
	
	private void ToggleBurstOfSpeedEnabled() {
		burstOfSpeedEnabled = !burstOfSpeedEnabled;
	}
	
	private void ToggleBurstOfSpeedInUse() {
		burstOfSpeedInUse = !burstOfSpeedInUse;
	}
	
	public void GiveBodyPiece(BodyPieceScript bodyPiece){
		if(eatCooldownTime > 0f) return;
		eatCooldownTime = eatCooldown - eatCooldownPerLevel * eatingSkill;
		
		if(bodyPiece.player != null) bodyPiece.player.RemoveBodyPiece(bodyPiece.gameObject, false);
		pieces.AddAfter(pieces.First, bodyPiece.gameObject);

		bodyPiece.GiveToPlayer(this);
	}
}
