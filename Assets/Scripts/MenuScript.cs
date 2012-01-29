using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	
	public static MenuScript singleton;
	
	public Texture transparentBlack;
	public Texture tutorial, tutorial2;
	public Texture player1Won, player2Won;
	public int tutorialScreen = 0;
	public bool gameStarted = false;
	public int winningPlayer = -1;
	
	void Awake(){
		singleton = this;
	}
	
	void Update(){
		if(gameStarted && winningPlayer >= 0){
			gameStarted = false;
		}
		if(!gameStarted && Input.GetKeyDown(KeyCode.Space)){
			if(tutorialScreen == 0){
				tutorialScreen++;
			} else {
				
				if(winningPlayer >= 0) Application.LoadLevel(Application.loadedLevel);
				gameStarted = true;
			}
		}
		if(Input.GetKeyUp(KeyCode.Escape)){
			Application.Quit();
		}
	}
	
	void OnGUI(){
		if(!gameStarted){
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), transparentBlack);
			float centerX = Screen.width / 2f, centerY = Screen.height / 2f;
			if(winningPlayer < 0){
				float minSize = Mathf.Min(Screen.width / 2f, Screen.height / 2f);
				GUI.DrawTexture(new Rect(centerX - minSize, centerY - minSize, 2f * minSize, 2f * minSize), tutorialScreen == 0 ? tutorial : tutorial2);
			} else {
				float sizeX = player1Won.width / 2f, sizeY = player1Won.height / 2f;
				GUI.DrawTexture(new Rect(centerX - sizeX, centerY - sizeY, 2f * sizeX, 2f * sizeY), winningPlayer == 1 ? player1Won : player2Won);
			}
		}
	}
}
