using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	// Editor variables.
	public int playerIndex;
	public HeadScript head;
	public float rotationForce;
	public float moveForce;
	
	public float burstOfSpeedMaxForce;
	public float burstOfSpeedMultiplier;
	public float burstOfSpeedCooldown;
	public float realtime;
	
	//Burst of Speed abiltiy variables
	private bool burstOfSpeedEnabled = true;
	private bool burstOfSpeedInUse = false;
	private bool burstOfSpeedOnCooldown = false;
	private float defaultMoveForce;
	private float burstOfSpeedLastCastTime = 0;
	
	//Repel ability variables
	
	//Straighten ability variables
	
	void Awake () {
		head.gameObject.layer = LayerMask.NameToLayer("Player" + playerIndex);
		defaultMoveForce = moveForce;
	}
	
	void FixedUpdate () {
		realtime = Time.realtimeSinceStartup;
	
		//Burst of Speed Ability
		if(burstOfSpeedEnabled || burstOfSpeedInUse)
		{
			bool shift = (playerIndex == 1 && Input.GetKey(KeyCode.RightShift)) || (playerIndex == 2 && Input.GetKey(KeyCode.LeftShift));
			
			if( shift && !burstOfSpeedInUse)
			{
				burstOfSpeedInUse = true;
				burstOfSpeedEnabled = false;
				Debug.Log("Burst of Speed Now in Use for" + playerIndex);
			}
			else if (burstOfSpeedInUse)
			{
				moveForce *= burstOfSpeedMultiplier;
				if(moveForce >= burstOfSpeedMaxForce)
				{
					moveForce = defaultMoveForce;
					burstOfSpeedLastCastTime = Time.realtimeSinceStartup;
					Debug.Log("Burst of Speed has completed for " +playerIndex);
					burstOfSpeedInUse = false;
					burstOfSpeedOnCooldown = true;
				}
			}
		}
		
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
		if (burstOfSpeedOnCooldown){
			if (Time.realtimeSinceStartup - burstOfSpeedLastCastTime > burstOfSpeedCooldown)
			{
				burstOfSpeedEnabled = true;
				Debug.Log("Burst of Speed is cooled down for" + playerIndex);
			}
		}
		
	}
	
	private void ToggleBurstOfSpeedEnabled() {
		burstOfSpeedEnabled = !burstOfSpeedEnabled;
	}
	
	private void ToggleBurstOfSpeedInUse() {
		burstOfSpeedInUse = !burstOfSpeedInUse;
	}
}