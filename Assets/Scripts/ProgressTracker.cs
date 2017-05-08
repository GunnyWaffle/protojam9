using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTracker : MonoBehaviour {

    public enum Zone
    {
        Space = 0,
        Sky,
        Clouds,
        Land
    }

    public ScrollScript background;
    public ScrollTransitionScript transition;
    public GUIManager gui;
    public float totalGameTime = 360f;

    float gameStartTime;
    float percentageCompletion = 0;
    Zone currentZone = 0;
    Zone percentZone = 0;

    // Use this for initialization
    void Start () {
        gameStartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePercentageCompletion();
        CheckPercentageCompletion();
        ValidateCurrentZone();
        TryWorldSwitch();
    }

    void UpdatePercentageCompletion()
    {
        percentageCompletion = (Time.time - gameStartTime) / totalGameTime;
        gui.UpdateProgressTracker(1 - percentageCompletion);

        if (percentageCompletion >= 1)
        {
            SceneTransition.Gameover();
        }
    }

    void CheckPercentageCompletion()
    {
        if (percentageCompletion >= 0.75)
        {
            percentZone = Zone.Land;
        }
        else if (percentageCompletion >= 0.5f)
        {
            percentZone = Zone.Clouds;
        }
        else if (percentageCompletion >= 0.25f)
        {
            percentZone = Zone.Sky;
        }
    }

    void ValidateCurrentZone()
    {
        if (currentZone != percentZone)
        {
            currentZone = percentZone;
            transition.InitiateTransition();
        }
    }

    void TryWorldSwitch()
    {
        if (transition.transform.position.y >= 0 && !transition.WorldSwitchDone)
        {
            background.ChangeWorlds();
            transition.WorldSwitchDone = true;
        }
    }
}
