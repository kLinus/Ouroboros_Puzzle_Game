using UnityEngine;
using System.Collections;

public class BoxScript : MonoBehaviour {
	
	// Editor variables.
	public GameObject leftWall, rightWall, topWall, bottomWall;
	
	// Use this for initialization
	void Start () {
		Vector3 topLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, Screen.height));
		leftWall.transform.position = new Vector3(topLeft.x, 0f, 0f);
		rightWall.transform.position = new Vector3(bottomRight.x, 0f, 0f);
		topWall.transform.position = new Vector3(0f, 0f, topLeft.z);
		bottomWall.transform.position = new Vector3(0f, 0f, bottomRight.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
