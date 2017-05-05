using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {

    public int health;
    public int scoreValue;
    public AudioClip deathSound;
    public Color damageColor;
    private Color defaultColor;
    public float timeForDamage;
    public DamagedByType type;
    private float currentTimeForDamage;
    private bool isDamaged = false;

    public UnityEvent customCallback;

    [HideInInspector]
    public bool isDead { get; private set; }
    private Player player;
    private AudioSource audio;
    private SpriteRenderer spr;
    private Collider2D collider;
    public enum DamagedByType
    {
        Blue = 0,
        Green = 1,
        Yellow = 2,
        Red = 3,
        Any = 4
    };

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spr = gameObject.GetComponent<SpriteRenderer>();
        audio = gameObject.GetComponent<AudioSource>();
        collider = gameObject.GetComponent<Collider2D>();
        defaultColor = spr.color;
        isDead = false;
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
        isDead = true;
        spr.enabled = false;
        collider.enabled = false;

        if (audio != null && deathSound != null)
        {
            audio.PlayOneShot(deathSound);
            Destroy(gameObject, deathSound.length);
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
            if (bullet.playerShot && (bullet.type == this.type || DamagedByType.Any == type))
            {
                DamageUnit(bullet.damage);
                if (bullet.shouldDestroyOnImpact)
                    Destroy(collision.gameObject);
            }
        }
    }
}
