﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    public float speed;
    private AudioSource audioSource;
    public AudioClip playerExplosion;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 screenPos = Globals.ClampToScreen(transform.position);

        if ((transform.position - screenPos).magnitude > transform.localScale.magnitude)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            audioSource.PlayOneShot(playerExplosion);
            player.KillPlayer();
            Destroy(gameObject);
        }
    }
}
