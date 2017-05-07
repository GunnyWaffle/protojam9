using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
    //Public
    public Camera mainCam;
    public SpriteRenderer titleBlock;
    public GameObject bossShip;
    public MenuHandler menuControl;

    //Private
    private Color blockColor;
    private List<Vector4> cameraPosAndLerpTimes;
    private float timer;
    private int currentStep;
    private float flashTimer;

    // Use this for initialization
    void Start()
    {
        currentStep = 0;
        timer = 0;
        blockColor = Color.black;

        cameraPosAndLerpTimes = new List<Vector4>();
        cameraPosAndLerpTimes.Add(new Vector4(5.25f, -2.2f, -10.0f, 2.0f));
        cameraPosAndLerpTimes.Add(new Vector4(-6.25f, 2.45f, -10.0f, 7.0f));
        cameraPosAndLerpTimes.Add(new Vector4(-6.25f, 2.45f, -10.0f, .2f));
        cameraPosAndLerpTimes.Add(new Vector4(5.25f, -2.2f, -10.0f, 1.0f));
        cameraPosAndLerpTimes.Add(new Vector4(5.25f, -2.2f, -10.0f, 2.0f));
        cameraPosAndLerpTimes.Add(new Vector4(0, 0, -10.0f, 2.0f));
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        DoCameraLerp(currentStep, currentStep + 1);

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
    }

    private void StageZero()
    {
        blockColor.r = timer * .5f;
        blockColor.g = timer * .5f;
        blockColor.b = timer * .5f;
        titleBlock.color = blockColor;
    }

    private void StageOne()
    {
        bossShip.transform.position = Vec3SmoothLerp(new Vector3(-11.58f, 5.31f, -7.33f), 
            new Vector3(1.75f, -1.3f, -7.33f), timer, cameraPosAndLerpTimes[2].w + cameraPosAndLerpTimes[3].w);
    }

    private void StageTwo()
    {
        flashTimer += Time.deltaTime * 5;
        bossShip.transform.position = Vec3SmoothLerp(new Vector3(-11.58f, 5.31f, -7.33f),
            new Vector3(1.75f, -1.3f, -7.33f), timer + cameraPosAndLerpTimes[2].w, cameraPosAndLerpTimes[2].w + cameraPosAndLerpTimes[3].w);

        bossShip.transform.position = Vec3SmoothLerp(bossShip.transform.position, new Vector3(4.85f, -3.25f, -7.33f), timer, cameraPosAndLerpTimes[3].w);

        bossShip.transform.localScale = Vec3SmoothLerp(bossShip.transform.localScale,
            new Vector3(0, 0, 0), timer * .5f, cameraPosAndLerpTimes[2].w + cameraPosAndLerpTimes[3].w);


        blockColor.g = Mathf.Cos(flashTimer);
        blockColor.b = Mathf.Cos(flashTimer);
        titleBlock.color = blockColor;

        bossShip.GetComponent<SpriteRenderer>().color = blockColor;
    }

    private void StageThree()
    {
        flashTimer += Time.deltaTime * 5;
        blockColor.g = Mathf.Cos(flashTimer);
        blockColor.b = Mathf.Cos(flashTimer);
        titleBlock.color = blockColor;
    }

    private void StageFour()
    {
        flashTimer += Time.deltaTime * 5;
        blockColor.g = Mathf.Cos(flashTimer);
        blockColor.b = Mathf.Cos(flashTimer);
        titleBlock.color = blockColor;
        mainCam.GetComponent<Camera>().orthographicSize = Mathf.Lerp(2, 5, Smootherstep(timer *.5f));
    }

    private void StageFive()
    {
        if(!(blockColor.g - .99f > 0))
        {
            flashTimer += Time.deltaTime * 5;
            blockColor.g = Mathf.Cos(flashTimer);
            blockColor.b = Mathf.Cos(flashTimer);
        }
        blockColor.a = Mathf.Lerp(1, 0, timer * 2);
        titleBlock.color = blockColor;
        menuControl.SetPlayerControlAbility(true);
    }

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
    private float Smootherstep(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}
