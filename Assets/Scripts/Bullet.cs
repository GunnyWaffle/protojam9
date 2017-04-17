using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public Transform startMarker;
    public Transform endMarker;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time;
        Vector3 dir = endMarker.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 270);
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

    }
	
	// Update is called once per frame
	void Update ()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);

    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Hi");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Hi2");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hi");
            //gameObject.SetActive(false);
            //Reduce player health
        }
    }
}
