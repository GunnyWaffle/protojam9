using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Start"))
        {
            if (SceneManager.GetActiveScene().name == "Start")
                StartGame();
            else if (SceneManager.GetActiveScene().name == "Credits")
                Reset();
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public static void Gameover()
    {
        SceneManager.LoadScene("Credits");
    }

    void Reset()
    {
        SceneManager.LoadScene("Start");
    }
}
