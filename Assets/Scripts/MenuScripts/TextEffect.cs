using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect : MonoBehaviour {

	public int numVariations; //Number of color variants following the text

	[Range(0.0f, 1.0f)]
	public float range; //The size of the circle the text lerps around

	public bool mainRotate; //Should the center text rotate?
	public bool currentlyRotating; //Is this currently rotating?

	private GameObject textClone; //A cloned version, which we will remove the TextEffect script from and base clones off (eventually recycled into the container for all the clones)
	private List<GameObject> variations; //The color variants
	private float timer; //A time tracker
	private Vector3 defaultPos; //The starting location of the center text
	private float timingOffset; //The lag of each variant compared to the previous
	private string defaultText; //The starting text;

	// Use this for initialization
	void Start () {
		//Save starting text
		defaultText = GetComponent<TextMesh>().text;

		//Clone the text, Strip it of the TextEffect script, make it's name something more fitting
		textClone = Instantiate (gameObject);
		Destroy (textClone.GetComponent<TextEffect> ());
		textClone.name = "VariationContainer " + GetComponent<TextMesh> ().text;

		//Calculate the offset
		timingOffset = 1.0f / numVariations;

		//Save the position of the main text
		defaultPos = transform.position;

		//Create the list of variants
		variations = new List<GameObject> ();

		//Populate that list
		for (int i = 0; i < numVariations - 1; i++) {
			variations.Add (Instantiate (textClone));
			Vector3 newPos = transform.position;
			newPos.z += .03f * (i + 1);
			variations [i].transform.position = newPos;
			variations [i].GetComponent<TextMesh> ().color = Random.ColorHSV ((1.0f / numVariations) * i, (1.0f / numVariations) * i, 1, 1, 1, 1);
		}

		//Destroy the text mesh on the clone, it's just a container now
		Destroy(textClone.GetComponent<TextMesh> ());

		//Set the parents of the variants to the container
		for (int i = 0; i < variations.Count; i++) {
			variations [i].transform.parent = textClone.transform;
		}

		//If the main one is rotating, add it to the list that gets rotated
		if (mainRotate) {
			variations.Insert (0, gameObject);
		} else { //If it doesnt rotate, keep the variants close
			range = .1f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (currentlyRotating) {
			//Update timer, negative to control the direction of the rotation
			timer -= Time.deltaTime;
			//For each one, lerp around a circle at an offset
			for (int i = 0; i < variations.Count; i++) {
				Vector3 newPos = new Vector3 ((Mathf.Sin (timer + (timingOffset * i) % 1) * range) + defaultPos.x, 
					                 (Mathf.Cos (timer + (timingOffset * i) % 1) * range) + defaultPos.y,
					                 variations [i].transform.position.z);
				variations [i].transform.position = newPos;
			}
		}
	}

	//Allows the activity of the rotation to be set
	public void SetRotationActive(bool toggle){
		currentlyRotating = toggle;

		//Deactivates all of the variants
		textClone.SetActive (currentlyRotating);

		//Reset the position of the main text
		if (!currentlyRotating) {
			this.transform.position = defaultPos;
		}
	}

	//Update the text of the string
	public void UpdateText(string text){
		GetComponent<TextMesh> ().text = text;
		for (int i = 0; i < variations.Count; i++) {
			variations [i].GetComponent<TextMesh> ().text = text;
		}
	}

	public void HighlightMenuText(bool toggle){
		if (toggle) {
			UpdateText ("- " + defaultText + " -");
		} else {
			UpdateText (defaultText);
		}
	}

	public void MoveText(Vector3 translation){
		defaultPos += translation;
	}

	public void AttachVariantsToParent(Transform parent){
		textClone.transform.parent = parent;
	}
}
