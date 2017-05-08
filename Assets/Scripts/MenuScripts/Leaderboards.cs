using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leaderboards : MonoBehaviour
{
    public TextMesh standingsText; //The text containing all of the leaderboard data
    public HighScore scores; //the score interface
    public float scrollSpeed = 3.0f; //the scroll speed modifier

    private KeyValuePair<string, int>[] sortedScores; //The scores
    private float deadzoneTolerance = 0.5f; //the controller deadzone
    private Vector3 initialScrollPos; //the initial starting position
    private float scrollHeight; //the height of the scrolling text object
    private float scrollTimer; //a timer to make the scrolling speed increase after prolonged scrolling

    // Use this for initialization
    void Start()
    {
        //Set up the score display and prepare variables
        PopulateScores();
        scrollHeight = standingsText.gameObject.GetComponent<Renderer>().bounds.extents.y * 2;
        initialScrollPos = standingsText.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Handle user input
        HandleUserInput();
    }

    //Gets the scores and populates the on screen text object with them
    private void PopulateScores()
    {
        string text = string.Empty;
        sortedScores = scores.GetSortedHighScores();

        for (int i = 0; i < sortedScores.Length; i++)
        {
            text += (i + 1) + ": " + sortedScores[i].Key + " - " + sortedScores[i].Value + "\n\n";
        }

        standingsText.text = text;
    }

    //Handles scrolling and A to continue
    private void HandleUserInput()
    {
        float movement = Input.GetAxis("LeftJoyY");

        //If they are trying to move down, and are coming from a resting joystick state
        if (Mathf.Abs(movement) > deadzoneTolerance)
        {
            //Update the position, calculate the speed at which to scroll
            Vector3 newPos = standingsText.gameObject.transform.position;
            scrollTimer += Time.deltaTime * scrollSpeed;
            float scrollRate = Mathf.Clamp(Time.deltaTime * scrollSpeed * scrollTimer, 0, 1.5f);

            //If going up
            if (movement > 0)
            {
                if (newPos.y - scrollHeight < -2.0f)
                {
                    newPos.y += scrollRate;
                }
            }
            //If going down
            else
            {
                if (newPos.y > initialScrollPos.y)
                {
                    newPos.y -= scrollRate;
                }
            }

            standingsText.gameObject.transform.position = newPos;
        }

        //If not moving
        else
        {
            scrollTimer = 1;
        }

        //If they press A
        if (Input.GetButtonDown("A"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
