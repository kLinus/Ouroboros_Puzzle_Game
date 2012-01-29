using UnityEngine;
using System.Collections;

public class BoxScript : MonoBehaviour {
	
	// Editor variables.
	public GameObject leftWall, rightWall, topWall, bottomWall;
	public BodyPieceScript bodyPiecePrefab;
	public int initialPieces;
	public float secondsPerPiece;
	
	// Private variables.
	float timeLeftUntilSpawn;
	
	// Use this for initialization
	void Start () {
		Vector3 topLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		leftWall.transform.position = new Vector3(topLeft.x, 0f, 0f);
		rightWall.transform.position = new Vector3(bottomRight.x, 0f, 0f);
		topWall.transform.position = new Vector3(0f, 0f, topLeft.z);
		bottomWall.transform.position = new Vector3(0f, 0f, bottomRight.z);
		
		for(int n = 0; n < initialPieces; n++){
			SpawnRandomPiece();
		}
		timeLeftUntilSpawn = secondsPerPiece;
	}
	
	void FixedUpdate(){
		timeLeftUntilSpawn -= Time.fixedDeltaTime;
		if(timeLeftUntilSpawn <= 0f){
			timeLeftUntilSpawn = secondsPerPiece;
			SpawnRandomPiece();
		}
	}
	
	void SpawnRandomPiece(){
		float border = 20f;
		Vector3 position = new Vector3(Random.Range(border, Screen.width - border), Random.Range(border, Screen.height - border), 0f);
		position = Camera.main.ScreenToWorldPoint(position);
		position.y = 0f;
		GameObject.Instantiate(bodyPiecePrefab, position, Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up));
	}
}
