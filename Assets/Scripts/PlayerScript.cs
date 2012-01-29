using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	// Editor variables.
	public int playerIndex;
	public HeadScript head;
	public float rotationForce;
	public float moveForce;
	
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
	
	//Straighten ability variables
	private bool straightenEnabled = true;
	private bool straightenIsOnCooldown = false;
	private float straightenLastCastTime = 0;
	
	//Controls
	private KeyCode p1_BurstOfSpeedKey = KeyCode.RightShift;
	private KeyCode p1_RepelKey = KeyCode.Slash;
	private KeyCode p1_StraightenKey = KeyCode.Period;
	
	private KeyCode p2_BurstOfSpeedKey = KeyCode.LeftShift;
	private KeyCode p2_RepelKey = KeyCode.E;
	private KeyCode p2_StraightenKey = KeyCode.Q;
	
	void Awake () 
	{
		head.gameObject.layer = LayerMask.NameToLayer("Player" + playerIndex);
		defaultMoveForce = moveForce;
	}
	
	void FixedUpdate () 
	{

		//Burst of Speed Ability
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
		
		//Repel Ability
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
		
		//Straighten Ability
		
		
		//Movement
		bool left = (playerIndex == 1 && Input.GetKey(KeyCode.LeftArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.A));
		bool right = (playerIndex == 1 && Input.GetKey(KeyCode.RightArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.D));
		bool up = (playerIndex == 1 && Input.GetKey(KeyCode.UpArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.W));
		bool down = (playerIndex == 1 && Input.GetKey(KeyCode.DownArrow)) || (playerIndex == 2 && Input.GetKey(KeyCode.S));
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
		
	}
	
	public int PlayerIndex
	{
		get{ return playerIndex;}
	}
}