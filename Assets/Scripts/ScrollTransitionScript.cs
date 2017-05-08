using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTransitionScript : MonoBehaviour {

    public float speed;
    public List<Texture> transitions;
    public ScrollScript background;
    public bool WorldSwitchDone {
        get
        {
            return transitionDone;
        }

        set
        {
            transitionDone = value;
        }
    }

    Rigidbody rgb;
    Renderer rend;
    int transitionToUse = 0;
    Vector3 startPos;
    bool transitionDone = false;

	// Use this for initialization
	void Start () {
        rgb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        //if (transform.position.y >= 0 && !transitionDone)
        //{
        //    background.ChangeWorlds();
        //    transitionDone = true;
        //}
	}

    public void InitiateTransition()
    {
        rgb.velocity = new Vector3(0, speed, 0);
    }

    public void ResetPosition()
    {
        transform.position = startPos;
    }

    void OnBecameInvisible()
    {
        rgb.velocity = Vector3.zero;
        ResetPosition();
        UpdateTexture();
        transitionDone = false;
    }

    void UpdateTexture()
    {
        transitionToUse = (transitionToUse + 1) % transitions.Count;
        rend.material.SetTexture("_MainTex", transitions[transitionToUse]);
    }
}
