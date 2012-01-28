using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	// Editor variables.
	public int playerIndex;
	public HeadScript head;
	
	void Awake () {
		head.gameObject.layer = LayerMask.NameToLayer("Player" + playerIndex);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
