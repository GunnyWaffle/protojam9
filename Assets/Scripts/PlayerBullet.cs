using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed;

    void Start ()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
	
	void Update ()
    {
        Vector3 screenPos = Globals.ClampToScreen(transform.position);

        if ((transform.position - screenPos).magnitude > transform.localScale.magnitude)
            Destroy(gameObject);
	}
}
