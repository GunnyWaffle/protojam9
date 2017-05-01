using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    public int health;
    public int scoreValue;
    public AudioClip deathSound;
    public Color damageColor;
    public float timeForDamage;
    private float currentTimeForDamage;
    private bool isDamaged = false;

    private bool isDead = false;
    private Player player;
    private AudioSource audio;
    private SpriteRenderer spr;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spr = gameObject.GetComponent<SpriteRenderer>();
        audio = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isDamaged)
        {
            if (currentTimeForDamage <= 0.0f)
            {
                spr.color = Color.white;
                isDamaged = false;
            }
            else
            {
                spr.color = damageColor;
                currentTimeForDamage -= Time.deltaTime;
            }
        }
    }

    public void DamageUnit(int damage)
    {
        health -= damage;

        isDamaged = true;
        currentTimeForDamage = timeForDamage;
        // TODO sound

        if (health <= 0)
            KillUnit();
    }

    public void KillUnit()
    {
        spr.enabled = false;

        if (audio != null && deathSound != null)
        {
            audio.PlayOneShot(deathSound);
            while (audio.isPlaying) { /*wait*/ }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        player.UpdateScore(scoreValue);
    }
}
