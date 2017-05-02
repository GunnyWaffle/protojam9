using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {

    public int health;
    public int scoreValue;
    public AudioClip deathSound;
    public Color damageColor;
    public Color defaultColor;
    public float timeForDamage;
    public UnityEvent customCallback;
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
                spr.color = defaultColor;
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

        customCallback.Invoke();

        player.UpdateScore(scoreValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (bullet.playerShot)
            {
                DamageUnit(bullet.damage);
                Destroy(collision.gameObject);
            }
        }
    }
}
