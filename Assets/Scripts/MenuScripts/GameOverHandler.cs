using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    public TextMesh statusText; //The Victory or Defeat Text (Becomes "Leaderboards")
    public TextMesh flavorText; //The text below status text
    public TextMesh scoreText; //the text that tells the player their score
    public TextMesh nameInput; //the text that displays the players name as they input it
    public TextMesh scoresDisplay; //Originally the "Enter Your Initials" label. Becomes the leaderboard display
    public AudioClip victoryMusic; //The music to play on victory
    public AudioClip defeatMusic; //The music to play on defeat
    public AudioClip bossExplosion; //Boss explodes in the distance
    public AudioSource musicPlayer; //The audio source that plays the music
    public HighScore scoreManager; //the score manager that handles loading the leaderboards and saving the score
    public GameObject continueText; //the text telling the player to press A to continue from the leaderboards
    
    private bool waitForDeadzone = false; //Bool to force user to manually move selection each time
    private float deadzoneTolerance = 0.5f; //The threshold to move the selection
    private char[] alphabet; //the alphabet
    private int currentChar; //the current selected character
    private int currentSpot; //the current initial the player is inputting
    private char[] initials; //the initials
    private int scoreRank; //the rank of the players score
    private bool onLeaderboards = false; //is the player looking at the leaderboards or inputting their name?

    // Use this for initialization
    void Start()
    {
        //Fill alphabet array
        alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
        'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0',
        '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        //Start at -1 char, since it will be a prefilled character
        currentChar = -1;
        currentSpot = 0;

        //Start with prefills
        initials = new char[] { '■', '_', '_' };

        //Handle the win/lose conditions
        if (PlayerPrefs.GetInt("Victory") == 1)
        {
            SetupDefeatScreen();
        }
        else
        {
            SetupVictoryScreen();
        }

        //display the score
        SetupScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        //Handle player input for initials and pressing A
        HandleNameInput();
    }

    //Setup the victory display
    private void SetupVictoryScreen()
    {
        musicPlayer.Stop();
        musicPlayer.PlayOneShot(bossExplosion);
        musicPlayer.clip = victoryMusic;
        musicPlayer.Play();
        musicPlayer.PlayOneShot(bossExplosion);
        statusText.text = "VICTORY!";
        flavorText.text = "You Have Saved The Earth!";
        statusText.gameObject.GetComponent<TextEffect>().SetRotationActive(false);
    }

    //Setup the defeat display
    private void SetupDefeatScreen()
    {
        musicPlayer.Stop();
        musicPlayer.clip = defeatMusic;
        musicPlayer.Play();
        statusText.text = "DEFEAT!";
        flavorText.text = "You Failed to Defend The Earth!";
        statusText.gameObject.GetComponent<TextEffect>().SetRotationActive(false);
    }

    //Display the text
    private void SetupScoreText()
    {
        print(PlayerPrefs.GetInt("Score"));
        string score = PlayerPrefs.GetInt("Score").ToString();
        scoreText.text = "Score: " + score;
    }

    //Handles up and down and A for inputting name and continuing
    private void HandleNameInput()
    {
        //Up down movement
        float movement = Input.GetAxis("LeftJoyY");

        //If they are trying to move down, and are coming from a resting joystick state
        if (Mathf.Abs(movement) > deadzoneTolerance && !waitForDeadzone)
        {
            //If we went up, go backwards
            if(movement > 0)
            {
                currentChar -= 1;
            }
            //Down = forwards
            else
            {
                currentChar += 1;
            }

            //Loop the selections
            if(currentChar < 0)
            {
                currentChar = alphabet.Length - 1;
            }
            else if(currentChar >= alphabet.Length)
            {
                currentChar = 0;
            }

            //Update the displays and wait
            initials[currentSpot] = alphabet[currentChar];
            waitForDeadzone = true;
        }

        //If the joystick is reset
        if (Mathf.Abs(movement) < deadzoneTolerance)
        {
            waitForDeadzone = false;
        }

        nameInput.text = initials[0] + " " + initials[1] + " " + initials[2];

        //if A
        if (Input.GetButtonDown("A"))
        {
            //And not on leaderboards
            if (!onLeaderboards)
            {
                //lock in selection and move to next
                currentSpot += 1;
                currentChar = -1;

                //if all initials are filled
                if (currentSpot >= initials.Length)
                {
                    //Display high scores
                    string name = initials[0].ToString() + initials[1].ToString() + initials[2].ToString();
                    scoreRank = scoreManager.DetermineScoreRankingAndSave(name, PlayerPrefs.GetInt("Score"));
                    DisplayHighScores();
                    onLeaderboards = true;
                }
                else
                {
                    //Prefill empty selection with box to show player where they are
                    initials[currentSpot] = '■';
                }
            }
            else //if we are on the leaderboard
            {
                //load the credits
                SceneManager.LoadScene("Credits");
            }
        }
    }

    //Displays the leaderboards
    private void DisplayHighScores()
    {
        //update previous text displays
        statusText.text = "Leaderboards";
        statusText.gameObject.GetComponent<TextEffect>().UpdateText("Leaderboards");
        statusText.gameObject.GetComponent<TextEffect>().currentlyRotating = true;

        flavorText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(false);

        scoresDisplay.characterSize = 2.0f;


        //load scores and determine what to show
        KeyValuePair<string, int>[] sorted = scoreManager.GetSortedHighScores();

        string displayed = string.Empty;
      
        for (int i = 0; i < sorted.Length; i++)
        {
            if (i != 0 && Mathf.Abs(scoreRank - 1 - i) > 1)
            {
                continue;
            }

            displayed += (i + 1) + ": " + sorted[i].Key + " - " + sorted[i].Value + "\n";
            if (i == 0 && scoreRank > 2)
            {
                displayed += ". . .\n";
            }
        }

        //Show it
        scoresDisplay.text = displayed;
        continueText.SetActive(true);
    }
}
