using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect : MonoBehaviour {

	public int numVariations;

	[Range(0.0f, 1.0f)]
	public float range;

	public bool mainRotate;
	public bool currentlyRotating;

	private GameObject textClone;
	private List<GameObject> variations;
	private float timer;
	private Vector3 defaultPos;
	private float timingOffset;

	// Use this for initialization
	void Start () {
		textClone = Instantiate (gameObject);
		Destroy (textClone.GetComponent<TextEffect> ());
		textClone.name = "VariationContainer " + GetComponent<TextMesh> ().text;

		timingOffset = 1.0f / numVariations;

		defaultPos = transform.position;
		variations = new List<GameObject> ();

		for (int i = 0; i < numVariations - 1; i++) {
			variations.Add (Instantiate (textClone));
			Vector3 newPos = transform.position;
			newPos.z += .1f * i + 1;
			variations [i].transform.position = newPos;
			variations [i].GetComponent<TextMesh> ().color = Random.ColorHSV ((1.0f / numVariations) * i, (1.0f / numVariations) * i, 1, 1, 1, 1);
		}

		Destroy(textClone.GetComponent<TextMesh> ());

		for (int i = 0; i < variations.Count; i++) {
			variations [i].transform.parent = textClone.transform;
		}

		if (mainRotate) {
			variations.Insert (0, gameObject);
		} else {
			range = .1f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (currentlyRotating) {
			timer -= Time.deltaTime;
			for (int i = 0; i < variations.Count; i++) {
				Vector3 newPos = new Vector3 ((Mathf.Sin (timer + (timingOffset * i) % 1) * range) + defaultPos.x, 
					                 (Mathf.Cos (timer + (timingOffset * i) % 1) * range) + defaultPos.y,
					                 variations [i].transform.position.z);
				variations [i].transform.position = newPos;
			}
		}
	}

	public void SetRotationActive(bool toggle){
		currentlyRotating = toggle;
		textClone.SetActive (currentlyRotating);
		if (!currentlyRotating) {
			this.transform.position = defaultPos;
		}
	}

	public void UpdateText(string text){
		GetComponent<TextMesh> ().text = text;
		for (int i = 0; i < variations.Count; i++) {
			variations [i].GetComponent<TextMesh> ().text = text;
		}
	}
}
