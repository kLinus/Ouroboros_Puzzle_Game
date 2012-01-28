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
	
	//Burst of Speed abiltiy variables
	private bool burstOfSpeedEnabled = true;
	private bool burstOfSpeedInUse = false;
	private bool burstOfSpeedOnCooldown = false;
	private float defaultMoveForce;
	private float burstOfSpeedLastCastTime;
	
	//Repel ability variables
	
	//Straighten ability variables
	
	void Awake () {
		head.gameObject.layer = LayerMask.NameToLayer("Player" + playerIndex);
		defaultMoveForce = moveForce;
	}
	
	void FixedUpdate () {
	
		//Burst of Speed Ability
		if((burstOfSpeedEnabled || burstOfSpeedInUse) && !burstOfSpeedOnCooldown)
		{
			bool shift = (playerIndex == 1 && Input.GetKey(KeyCode.LeftShift));
			if( shift && !burstOfSpeedInUse)
			{
				burstOfSpeedInUse = true;
				burstOfSpeedEnabled = false;
			}
			else if (burstOfSpeedInUse)
			{
				moveForce *= burstOfSpeedMultiplier;
				if(moveForce >= burstOfSpeedMaxForce)
				{
					moveForce = defaultMoveForce;
					burstOfSpeedInUse = false;
					burstOfSpeedEnabled = true;
					burstOfSpeedLastCastTime = 
				}
			}
			else
			{
				moveForce = defaultMoveForce;
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
		
	}
	
	private void ToggleBurstOfSpeedEnabled() {
		burstOfSpeedEnabled = !burstOfSpeedEnabled;
	}
	
	private void ToggleBurstOfSpeedInUse() {
		burstOfSpeedInUse = !burstOfSpeedInUse;
	}
}