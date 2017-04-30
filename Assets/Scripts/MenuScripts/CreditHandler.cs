using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditHandler : MonoBehaviour {

	public GameObject creditRoll;
	public float scrollingSpeed;

	private Vector3 startingPos;
	private List<TextEffect> headers;

	// Use this for initialization
	void Start () {
		startingPos = creditRoll.transform.position;
		headers = new List<TextEffect> ();

		for (int i = 0; i < GameObject.Find ("Headers").transform.childCount; i++) {
			headers.Add (GameObject.Find ("Headers").transform.GetChild (i).gameObject.GetComponent<TextEffect>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		ScrollCredits ();
		if(Input.GetButton("A")){
			SceneManager.LoadScene("MainMenu");
		}
	}

	private void ScrollCredits(){
		Vector3 newPos = creditRoll.transform.position;
		newPos.y += Time.deltaTime * scrollingSpeed;
		creditRoll.transform.position = newPos;

		foreach (TextEffect text in headers) {
			text.MoveText (new Vector3 (0, Time.deltaTime * scrollingSpeed, 0));
		}

		if (creditRoll.transform.position.y > Mathf.Abs(startingPos.y)) {
			SceneManager.LoadScene("MainMenu");
		}
	}
}
