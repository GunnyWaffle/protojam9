using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

	public float deadzoneTolerance = 0.5f;
	public TextEffect playButton;
	public TextEffect creditsButton;
	public TextEffect exitButton;

	private int selectedIndex = 0;
	private TextEffect[] options;
	private bool waitForDeadzone = false;


	// Use this for initialization
	void Start () {
		options = new TextEffect[3];
		options [0] = playButton;
		options [1] = creditsButton;
		options [2] = exitButton;
	}
	
	// Update is called once per frame
	void Update () {
		HandleInput ();
	}

	private void HandleInput(){
		//Up down movement
		float movement = Input.GetAxis ("LeftJoyY");

		if (Mathf.Abs (movement) > deadzoneTolerance && !waitForDeadzone) {
			options [selectedIndex].SetRotationActive (false);
			selectedIndex = (selectedIndex + (int)Mathf.Sign (movement)) % options.Length;
			if (selectedIndex < 0) { //Cmon now unity... Mod not wrapping negatives around???
				selectedIndex = options.Length - 1;
			}
			options [selectedIndex].SetRotationActive (true);
			waitForDeadzone = true;
		}

		if (Mathf.Abs (movement) < deadzoneTolerance) {
			waitForDeadzone = false;
		}


		//Button press
		if (Input.GetButton ("A")) {
			switch (selectedIndex) {
				case 0:
					SceneManager.LoadScene ("Main");
					break;
				case 1:
					SceneManager.LoadScene ("Credits");
					break;
				case 2:
					Application.Quit ();
					break;
				default:
					break;
			}
		}
	}
}
