using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {

    public int StartingHealth;
    private int remainingHealth;
    public int scoreValue;
    public AudioClip deathSound;
    public DamagedByType type;

    public bool shouldFlashWithDamage = false;
    private const float firstFlashingThreshold = 0.5f; // Begin flashing at 50 percent dead.
    private const float firstFlashThresholdTime = 2.0f; // Time between flashes at first threshold
    private const float secondFlashingThreshhold = 0.25f; // Begin flashing more regularily at 75 percent dead.
    private const float secondFlashingThreshholdTime = 1.0f; // Time between flashes at second threshold
    private const float thirdFlashingThreshhold = 0.15f; // Begin flashing more regularily at 85 percent dead.
    private const float thirdFlashingThreshholdTime = 0.2f; // Time between flashes at second threshold
    private float timeBetweenFlashes = -1.0f;
    private float lastFlash;

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
        remainingHealth = StartingHealth;
        isDead = false;
    }

    private void Update()
    {
        if (shouldFlashWithDamage)
            FlashUnitRegularily();
    }

    // Controls the rate of flashing for an enemy
    private void FlashUnitRegularily()
    {
        if (timeBetweenFlashes > 0.0f)
        {
            if (lastFlash <= 0.0f)
            {
                flashController.Flash();
                lastFlash = timeBetweenFlashes;
            }
            else
            {
                lastFlash -= Time.deltaTime;
            }
        }
    }

    public void DamageUnit(int damage)
    {
        remainingHealth -= damage;

        // Have we crossed a new threshold for health to flash?
        float percentageHealthRemaining = (float)remainingHealth / (float)StartingHealth;
        if (percentageHealthRemaining <= thirdFlashingThreshhold)
        {
            timeBetweenFlashes = thirdFlashingThreshholdTime;
            lastFlash = timeBetweenFlashes;
        }
        else if (percentageHealthRemaining <= secondFlashingThreshhold)
        {
            timeBetweenFlashes = secondFlashingThreshholdTime;
            lastFlash = timeBetweenFlashes;
        }
        else if (percentageHealthRemaining <= firstFlashingThreshold)
        {
            timeBetweenFlashes = firstFlashThresholdTime;
            lastFlash = timeBetweenFlashes;
        }

        flashController.Flash();
        // TODO sound

        if (remainingHealth <= 0 && !isDead)
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
