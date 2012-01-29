using UnityEngine;
using System.Collections;

public class RepelScript : MonoBehaviour {
	
	bool repelIsArmed = false;
	private float maxForce;
	private float currentForce;
	private float maxRange;
	private float forceDecayMultiplier;
	private float growthMultiplier;
	private float radius;
	private int playerThatCastedRepel;
	private PlayerScript targetPlayer;
	
	void Awake ()
	{
		radius = .1f;
		currentForce = maxForce;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(repelIsArmed)
		{
			radius += Time.fixedDeltaTime * growthMultiplier;
			
			if ((this.transform.position - targetPlayer.transform.position).magnitude <= radius)
			{
				Debug.Log(playerThatCastedRepel + " casted repel on " + targetPlayer.playerIndex);
				targetPlayer.gameObject.rigidbody.AddForce( currentForce * Vector3.Normalize(targetPlayer.transform.position - this.transform.position));
				GameObject.Destroy(this.gameObject);
			} else if (radius >= maxRange)
			{
				GameObject.Destroy(this.gameObject);
			}
		}
	}

	public void SetRepelAttributes(float _maxRange, float _maxForce, float _forceDecayMultiplier, float _growthMultiplier, int _playerIndex)
	{
		this.maxForce = _maxForce;
		this.currentForce = maxForce;
		this.maxRange = _maxRange;
		this.forceDecayMultiplier = _forceDecayMultiplier;
		this.growthMultiplier = _growthMultiplier;
		this.playerThatCastedRepel = _playerIndex;
		
		foreach(Object gameObject in GameObject.FindObjectsOfType(typeof(PlayerScript))){
			PlayerScript player = gameObject as PlayerScript;
			if(player.playerIndex == this.playerThatCastedRepel) continue;
			targetPlayer = player;
		}
	}
	
	public float MaxForce
	{
		set{ maxForce = value; }
		get{ return maxForce; }
	}
	
	public float ForceDecayMultiplier
	{
		set{ forceDecayMultiplier = value; }
		get{ return forceDecayMultiplier; }
	}
	
	public float MaxRange
	{
		set{ maxRange = value; }
		get{ return maxRange; }
	}
	
	public float GrowthMultiplier
	{
		set{growthMultiplier = value; }
		get{ return growthMultiplier; }
	}
	
	public int PlayerIndexThatCasted
	{
		set{ playerThatCastedRepel = value; }
		get{ return playerThatCastedRepel; }
	}
	
	public bool RepelIsArmed
	{
		set
		{
			if (maxForce != 0 && forceDecayMultiplier != 0  && growthMultiplier != 0 && maxRange != 0 && playerThatCastedRepel != 0)
			{
				repelIsArmed = value; 
			}
			else
			{
				Debug.Log("One or more variables are not set.");
			}
		}
		get{ return repelIsArmed; }
	}
}
