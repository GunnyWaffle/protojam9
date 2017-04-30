using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

	public float deadzoneTolerance = 0.5f; //The threshold to move the selection
	public TextEffect playButton; //The Play text on the menu
	public TextEffect creditsButton; //The credits text on the menu
	public TextEffect exitButton; //The exit text on the menu

	private int selectedIndex = 0; //The current user selection
	private TextEffect[] options; //The possible options (encapsulates the buttons above)
	private bool waitForDeadzone = false; //Bool to force user to manually move selection each time


	// Use this for initialization
	void Start () {
		//Put the buttons in the options array
		options = new TextEffect[3];
		options [0] = playButton;
		options [1] = creditsButton;
		options [2] = exitButton;

		//Highlight the play button (default selection)
		playButton.HighlightMenuText (true);
	}
	
	// Update is called once per frame
	void Update () {
		HandleInput ();
	}

	//Handles user input
	private void HandleInput(){
		//Up down movement
		float movement = Input.GetAxis ("LeftJoyY");

		//If they are trying to move down, and are coming from a resting joystick state
		if (Mathf.Abs (movement) > deadzoneTolerance && !waitForDeadzone) {
			//Deactivate the effects on the current selection
			options [selectedIndex].HighlightMenuText (false);
			options [selectedIndex].SetRotationActive (false);

			//Change the selection
			selectedIndex = (selectedIndex + (int)Mathf.Sign (movement)) % options.Length;
			if (selectedIndex < 0) { //Cmon now unity... Mod not wrapping negatives around???
				selectedIndex = options.Length - 1;
			}

			//Activate effects on new selection
			options [selectedIndex].HighlightMenuText (true);
			options [selectedIndex].SetRotationActive (true);

			//Force user to reset joystick before changing selection
			waitForDeadzone = true;
		}

		//If the joystick is reset
		if (Mathf.Abs (movement) < deadzoneTolerance) {
			waitForDeadzone = false;
		}


		//Button press
		if (Input.GetButton ("A")) {
			switch (selectedIndex) {
				case 0: //Play
					SceneManager.LoadScene ("Main");
					break;
				case 1: //Credits
					SceneManager.LoadScene ("Credits");
					break;
				case 2: //Quit
					Application.Quit ();
					break;
				default: //Bugged
					break;
			}
		}
	}
}
