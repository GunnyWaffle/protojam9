using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScore : MonoBehaviour
{
    private Text scoreText;
    // Use this for initialization
    void Start()
    {
        scoreText = GetComponent<Text>();
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
