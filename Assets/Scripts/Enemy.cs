﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private GameObject player;

    //Alters orbit speed along x and y
    float orbit_period_X = 1.0f;
    float orbit_period_Y = 1.0f;

    //Alters orbit length along x and y
    float orbit_radius_X = 2.0f;
    float orbit_radius_Y = 1.0f;

    //Enemy shoot
    public GameObject bullet;

    public EnemySpawner.EnemyType type;
    private SpriteRenderer mySpriteRend;

    // Use this for initialization
    void Start () {
        mySpriteRend = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>().gameObject;
    }

    // Update is called once per frame
    void Update () {
        if (player != null)
        {
            //Enemy always faces player
            Vector3 dir = player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 270);

            //Enemy movement
            float time = Time.time;
            float x = orbit_radius_X * Mathf.Sin(time*orbit_period_X) + player.transform.position.x;
            float y = orbit_radius_Y * Mathf.Cos(time* orbit_period_Y) + player.transform.position.y;            
            transform.position = new Vector3(x, y, 0);

            //Enemy shooting
            if(time % 2 == 0 && !bullet.activeSelf)
            {
                //bullet.SetActive(true);
            }
        }
    }
}
