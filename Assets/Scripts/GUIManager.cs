﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    //Public fields
    public Color textColor; //Color of all GUI text
    public Player player; //The player object in the game
    public List<Sprite> healthBarTextures; //A list of all health bar textures in order
    public GameObject lifeIconPrefab; //The icon prefab for a player life
    public Sprite emptyLifeSprite; //The sprite that displays when a life is missing

    //Private fields
    private TextMesh scoreText; //Text displaying the score
    private GameObject livesText; //Gameobject with Lives Text attached
    private TextMesh healthTextLabel; //Text next to health bar
    private SpriteRenderer healthMeterBar; //The sprite renderer of the visible health bar
    private Vector3 initialLiveTextPos; //The initial position of the life text
    private List<GameObject> lifeIcons; //List of icons displaying lives

    // Use this for initialization
    void Start()
    {
        //Get all components
        scoreText = transform.Find("ScoreText").GetComponent<TextMesh>();
        livesText = transform.Find("LivesText").gameObject;
        healthTextLabel = transform.Find("HealthBar").transform.Find("HealthLabel").GetComponent<TextMesh>();
        healthMeterBar = transform.Find("HealthBar").transform.Find("HealthBarMeter").GetComponent<SpriteRenderer>();
        initialLiveTextPos = livesText.transform.position;
        lifeIcons = new List<GameObject>();

        ResetGui(); //Reset the look of everything, since it wont update until player updates it otherwise
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Update the GUI Score Display
    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = "Score: " + score;
    }

    //Update the GUI Health Display
    public void UpdateHealthDisplay(int health, int maxHealth)
    {
        //Need to calculate the amount of health to display. 
        //We have an 8 cell health bar that can display arbitrary ranges
        int displayedHealth;
        //If at or below zero, draw zero
        if (health <= 0)
        {
            displayedHealth = 0;
        }
        //Otherwise, calculate an approximation in a factor of 8 to display the remaining health percentage
        else
        {
            //First, get the percentage of health left. Multiply that by the number of cells - 1, then add 1.
            //The (percent * cells - 1) + 1 accounts for float to int rounding problems.
            displayedHealth = (int)(((float)health / (float)maxHealth) * (float)(healthBarTextures.Count - 2)) + 1;
        }

        //update display texture
        healthMeterBar.sprite = healthBarTextures[displayedHealth];
    }

    //Updates the GUI Lives Display
    public void UpdateLivesDisplay(int livesRemaining)
    {
        //If we haven't created the icons yet
        if (lifeIcons.Count <= 0)
        {
            //calculate the position of the label text
            Vector3 newPos = initialLiveTextPos;
            newPos.x = newPos.x - ((livesRemaining) * emptyLifeSprite.bounds.extents.x * lifeIconPrefab.transform.localScale.x * 3.5f);
            livesText.transform.position = newPos;

            //Create the life icons, and properly position them
            for (int i = 0; i < livesRemaining; i++)
            {
                newPos = initialLiveTextPos;
                GameObject newLifeIcon = Instantiate(lifeIconPrefab);
                newPos.x = newPos.x - ((livesRemaining - i - 1) * emptyLifeSprite.bounds.extents.x * lifeIconPrefab.transform.localScale.x * 3.0f);
                newLifeIcon.transform.position = newPos;
                newLifeIcon.transform.parent = livesText.transform;
                lifeIcons.Add(newLifeIcon);
            }
        }

        //Replace any used lives with the empty sprite
        for (int i = livesRemaining; i < lifeIcons.Count; i++)
        {
            lifeIcons[i].GetComponent<SpriteRenderer>().sprite = emptyLifeSprite;
        }
    }

    //Resets the GUI to it's initial state at the start of the game
    private void ResetGui()
    {
        //Reset all displays to their default state
        UpdateScoreDisplay(0);
        UpdateHealthDisplay(player.health, player.maxHealth);
        UpdateLivesDisplay(player.lives);

        //Set the colors of all the text meshes to the inspector selected color
        scoreText.color = textColor;
        livesText.GetComponent<TextMesh>().color = textColor;
        healthTextLabel.color = textColor;
    }
}
