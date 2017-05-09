using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
    //Public
    public Camera mainCam; //The main scene camera
    public SpriteRenderer titleBlock; //The graphic which blocks the menu visuals
    public GameObject bossShip; //The boss sprite
    public MenuHandler menuControl; //The menu handling script

    //Private
    private Color blockColor; //Used to set the color of the titleBlock
    private List<Vector4> cameraPosAndLerpTimes; //A list of camera positions, and the time to lerp between them (Outlined in start)
    private float timer; //A timer that runs the overall animation
    private int currentStep; //An index to track where we are in the animation
    private float flashTimer; //A timer that handles the red "alert" flashing in the animation
    private bool transitionToMenu; //A bool that tells us if the user has decided to skip the intro
    private bool doingIntro; //Are we currently animating the intro?
    private Vector3 cameraStartPos; //The starting position of the camera (used when making impromptu transition to menu)
    private TextMesh skipText; //Text that tells the player to press start to skip the intro sequence
    private bool displaySkip = false; //Do we display the skipText?

    // Use this for initialization
    void Start()
    {
        currentStep = 0;
        timer = 0;
        blockColor = Color.black;
        transitionToMenu = false;
        doingIntro = true;

        skipText = gameObject.transform.FindChild("SkipText").GetComponent<TextMesh>();

        if (PlayerPrefs.GetInt("WatchedIntro") == 1)
        {
            displaySkip = true;
        }

        cameraPosAndLerpTimes = new List<Vector4>();
        //Define the movement of the camera     X       Y      Z     TIME
        //Camera moves from point[0].xyx to point[1].xyz in point[1].w seconds
        cameraPosAndLerpTimes.Add(new Vector4(5.25f, -2.2f, -10.0f, 2.0f));
        cameraPosAndLerpTimes.Add(new Vector4(-6.25f, 2.45f, -10.0f, 7.0f));
        cameraPosAndLerpTimes.Add(new Vector4(-6.25f, 2.45f, -10.0f, 0.2f));
        cameraPosAndLerpTimes.Add(new Vector4(5.25f, -2.2f, -10.0f, 1.0f));
        cameraPosAndLerpTimes.Add(new Vector4(5.25f, -2.2f, -10.0f, 2.0f));
        cameraPosAndLerpTimes.Add(new Vector4(0, 0, -10.0f, 2.0f));
    }

    // Update is called once per frame
    void Update()
    {
        //if we are currently running the intro animation
        if (doingIntro)
        {
            //Increment timer, move the camera
            timer += Time.deltaTime;
            DoCameraLerp(currentStep, currentStep + 1);

            //Handle unique events for each stage
            switch (currentStep)
            {
                case 0: StageZero(); break;
                case 1: StageOne(); break;
                case 2: StageTwo(); break;
                case 3: StageThree(); break;
                case 4: StageFour(); break;
                case 5: StageFive(); break;
                default: break;
            }

            //If the user presses start (to skip the intro) and they have already seen it
            if (Input.GetButtonDown("Start") && displaySkip)
            {
                //Transition to the menu, move out of the current step loop, save the cameras position, reset timer
                transitionToMenu = true;
                currentStep = 10;
                doingIntro = false;
                cameraStartPos = mainCam.transform.position;
                timer = 0;
            }
        }

        //If the user has chosen to transition straight to the menu
        if (transitionToMenu)
        {
            TransitionToMain();
        }
    }

    //Fades in the scene
    private void StageZero()
    {
        blockColor.r = timer * .5f;
        blockColor.g = timer * .5f;
        blockColor.b = timer * .5f;
        titleBlock.color = blockColor;

        if (displaySkip == true && skipText.color.a <= .4f)
        {
            Color skipColor = skipText.color * .3f;
            skipColor.r = 1;
            skipColor.g = 1;
            skipColor.b = 1;
            skipColor.a += timer;
            skipText.color = skipColor;
        }
    }

    //Move the boss ship towards earth
    private void StageOne()
    {
        bossShip.transform.position = Vec3SmoothLerp(new Vector3(-11.58f, 5.31f, -7.33f),
            new Vector3(1.75f, -1.3f, -7.33f), timer, cameraPosAndLerpTimes[2].w + cameraPosAndLerpTimes[3].w);
    }

    //Make the boss ship get smaller and move towards earth. Flash the scene red (an alarm in the ship)
    private void StageTwo()
    {
        bossShip.transform.position = Vec3SmoothLerp(new Vector3(-11.58f, 5.31f, -7.33f),
            new Vector3(1.75f, -1.3f, -7.33f), timer + cameraPosAndLerpTimes[2].w, cameraPosAndLerpTimes[2].w + cameraPosAndLerpTimes[3].w);

        bossShip.transform.position = Vec3SmoothLerp(bossShip.transform.position, new Vector3(4.85f, -3.25f, -7.33f), timer, cameraPosAndLerpTimes[3].w);

        bossShip.transform.localScale = Vec3SmoothLerp(bossShip.transform.localScale,
            new Vector3(0, 0, 0), timer * .5f, cameraPosAndLerpTimes[2].w + cameraPosAndLerpTimes[3].w);
    }

    //Continue the red flashing. Pause for a second
    private void StageThree()
    {
        flashTimer += Time.deltaTime * 5;
        blockColor.g = Mathf.Cos(flashTimer) / 2 + 0.5f;
        blockColor.b = Mathf.Cos(flashTimer) / 2 + 0.5f;
        titleBlock.color = blockColor;
    }

    //Continue flashing, scale up the camera as we move to center screen
    private void StageFour()
    {
        flashTimer += Time.deltaTime * 5;
        blockColor.g = Mathf.Cos(flashTimer) / 2 + 0.5f;
        blockColor.b = Mathf.Cos(flashTimer) / 2 + 0.5f;
        titleBlock.color = blockColor;
        mainCam.GetComponent<Camera>().orthographicSize = Mathf.Lerp(2, 5, Smootherstep(timer * .5f));

        if (displaySkip == true && skipText.color.a > 0.0f)
        {
            Color skipColor = skipText.color;
            skipColor.a -= timer;
            skipText.color = skipColor;
        }
    }

    //Safely finish flashing the camera, fade out the menu blocker, give the user ability to select menu options
    private void StageFive()
    {
        if (!(blockColor.g - .99f > 0))
        {
            flashTimer += Time.deltaTime * 5;
            blockColor.g = Mathf.Cos(flashTimer) / 2 + 0.5f;
            blockColor.b = Mathf.Cos(flashTimer) / 2 + 0.5f;
        }

        blockColor.a = Mathf.Lerp(1, 0, timer * 2);
        titleBlock.color = blockColor;
        menuControl.SetPlayerControlAbility(true);
        PlayerPrefs.SetInt("WatchedIntro", 1);
    }

    //For when the user wants to skip the intro. Safely transition to the main menu and give the user control.
    private void TransitionToMain()
    {
        timer += Time.deltaTime;

        bossShip.GetComponent<SpriteRenderer>().color = blockColor;

        mainCam.transform.position = Vec3SmoothLerp(cameraStartPos, new Vector3(0, 0, -10.0f), timer, 1);

        if (displaySkip == true && skipText.color.a > 0.0f)
        {
            Color skipColor = skipText.color;
            skipColor.a -= timer;
            skipText.color = skipColor;
        }

        if (timer >= 0.2f)
        {
            mainCam.GetComponent<Camera>().orthographicSize = Mathf.Lerp(mainCam.GetComponent<Camera>().orthographicSize, 5, Smootherstep((timer - .2f) * .65f));
            if (timer >= 1.0f)
            {
                blockColor.r += Time.deltaTime * 2.0f;
                blockColor.g += Time.deltaTime * 2.0f;
                blockColor.b += Time.deltaTime * 2.0f;
                blockColor.a -= Time.deltaTime * 2.0f;
                titleBlock.color = blockColor;

                if (blockColor.r >= 1.0 && blockColor.g >= 1.0 && blockColor.b >= 1.0 && blockColor.a <= .3f)
                {
                    menuControl.SetPlayerControlAbility(true);
                }

                if (timer >= 2.0f)
                {
                    transitionToMenu = false;
                }
            }
        }
    }

    //Lerp the camera from defined position 1 to position 2
    private void DoCameraLerp(int startIndex, int endIndex)
    {
        if (startIndex >= 0 && startIndex < cameraPosAndLerpTimes.Count && endIndex >= 0 && endIndex < cameraPosAndLerpTimes.Count)
        {
            if (timer <= cameraPosAndLerpTimes[endIndex].w)
            {
                Vector3 newPos = Vec3SmoothLerp(cameraPosAndLerpTimes[startIndex], cameraPosAndLerpTimes[endIndex], timer, cameraPosAndLerpTimes[endIndex].w);
                mainCam.transform.position = newPos;
            }
            else
            {
                timer = 0;
                currentStep++;
            }
        }
    }

    //Lerp from a point to another point (with SmootherStep)
    private Vector3 Vec3SmoothLerp(Vector3 start, Vector3 end, float time, float totalTime)
    {
        Vector3 lerped = new Vector3(
            Mathf.Lerp(start.x, end.x, Smootherstep(time / totalTime)),
            Mathf.Lerp(start.y, end.y, Smootherstep(time / totalTime)),
            Mathf.Lerp(start.z, end.z, Smootherstep(time / totalTime))
            );
        return lerped;
    }


    //From: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
    //Smoothly lerp between two points with ease in and ease out
    private float Smootherstep(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}
