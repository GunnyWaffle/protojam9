using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {

    public int health;
    public int scoreValue;
    public AudioClip deathSound;
    public float timeForDamage;
    public DamagedByType type;
    private float currentTimeForDamage;

    public UnityEvent customCallback;

    [HideInInspector]
    public bool isDead { get; private set; }
    private Player player;
    private ColorFlash flashController;
    private AudioSource audio;
    private SpriteRenderer spr;
    private Collider2D collider;
    private Animator animator;

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
        animator = gameObject.GetComponent<Animator>();
        flashController = gameObject.GetComponent<ColorFlash>();
        isDead = false;
    }

    public void DamageUnit(int damage)
    {
        health -= damage;

        flashController.Flash();
        // TODO sound

        if (health <= 0 && !isDead)
            KillUnit();
    }

    public void KillUnit()
    {
        isDead = true;
        collider.enabled = false;
        StartCoroutine(PlayDeathAnimation());

        if (audio != null && deathSound != null)
        {
            audio.PlayOneShot(deathSound);
        }

        customCallback.Invoke();

        player.UpdateScore(scoreValue);

        Destroy(gameObject, 3f);
    }

    private IEnumerator PlayDeathAnimation()
    {
        if (animator != null)
        {
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            animator.SetTrigger("isDead");
            yield return new WaitForSeconds(1f);
        }

        spr.enabled = false;
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
