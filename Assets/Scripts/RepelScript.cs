using UnityEngine;
using System.Collections;

public class RepelScript : MonoBehaviour {
	
	public GameObject repelExplosion;
	public SphereCollider explosionCollider;
	public bool repelIsArmed = false;
	private float maxForce;
	private float currentForce;
	private float maxRange;
	private float forceDecayMultiplier;
	private float growthMultiplier;
	private int playerThatCastedRepel;
	
	void Start()
	{
		repelExplosion = this.gameObject;
	}
	
	void Awake ()
	{
		explosionCollider = gameObject.AddComponent("SphereCollider") as SphereCollider;
		explosionCollider.center = Vector3.zero;
		explosionCollider.radius = .1f;
		currentForce = maxForce;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(repelIsArmed)
		{
			explosionCollider.isTrigger = true;
			explosionCollider.radius += growthMultiplier;
			currentForce *= forceDecayMultiplier;
			
			if (explosionCollider.radius >= maxRange)
			{
				GameObject.Destroy(this.gameObject);
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<PlayerScript>().playerIndex != playerThatCastedRepel)
		{
			Debug.Log(playerThatCastedRepel + " casted repel on " + other.gameObject.GetComponent<PlayerScript>().playerIndex);
			other.gameObject.rigidbody.AddForce( -currentForce * Vector3.Normalize(other.transform.position - this.transform.position));
		}

	}

	public void SetRepelAttributes(float _maxRange, float _maxForce, float _forceDecayMultiplier, float _growthMultiplier, int _playerIndex)
	{
		this.maxForce = _maxForce;
		this.maxRange = _maxRange;
		this.forceDecayMultiplier = _forceDecayMultiplier;
		this.growthMultiplier = _growthMultiplier;
		this.playerThatCastedRepel = _playerIndex;
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
