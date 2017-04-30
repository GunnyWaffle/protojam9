using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int score;
    public Text scoreText;

    public int maxHealth = 12;
    public int health = 12;
    public int lives = 3;

    public float invincibilityTime = 2.5f;
    public float invincibilityPulseSpeed = 12.5f;
    public Color invincibilityColor = Color.red;
    float invincibilityCounter = 0;
    public bool IsInvincible { get { return invincibilityCounter > 0; } }

    SpriteRenderer sprite;

    const int WeaponCount = 4;

    public Bullet[] bulletPrefabs = new Bullet[WeaponCount];
    public float speed = 2.2f;
    public float focusPercentage = 0.4f;

    public float[] maxShotsPerSecond = new float[WeaponCount] { 4, 4, 4, 4 };
    float[] timeElapsed = new float[WeaponCount] { 0, 0, 0, 0 };

    public enum WeaponType { A, B, X, Y }
    WeaponType activeWeapon = WeaponType.A;
    public WeaponType ActiveWeapon { get { return activeWeapon; } }

    Animator anim;

    ParticleSystem explosion;
    bool isDead = false;
    public float timerAfterDeath;
    public bool IsDead { get { return isDead; } }

    //Audio Stuff
    AudioSource Audio;
    public AudioClip playerExplosion;
    public AudioClip playerShoot;
	void Start()
    {
        explosion = transform.Find("explosion").GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();
        Audio = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        PlayerPrefs.SetInt("Score", 0);
    }

    void OnValidate()
    {
        if (bulletPrefabs.Length != WeaponCount)
        {
            Debug.LogWarning("Don't change bulletPrefabs array size!");
            System.Array.Resize<Bullet>(ref bulletPrefabs, WeaponCount);
        }

        if (maxShotsPerSecond.Length != WeaponCount)
        {
            Debug.LogWarning("Don't change maxShotsPerSecond array size!");
            System.Array.Resize<float>(ref maxShotsPerSecond, WeaponCount);
        }
    }

    void Update()
    {
        if (isDead)
        {
            if (timerAfterDeath <= 0.0f)
                SceneTransition.Gameover();

            timerAfterDeath -= Time.deltaTime;
            return;
        }

        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
            if (invincibilityCounter > 0)
            {
                float t = invincibilityCounter / invincibilityTime;
                t *= invincibilityPulseSpeed;
                t *= 180 * Mathf.Deg2Rad;
                t = Mathf.Sin(t);
                t = (t + 1) / 2;

                Color lerpedColor = Color.Lerp(Color.white, invincibilityColor, t);
                sprite.color = lerpedColor;
            }
            else
            {
                invincibilityCounter = 0;
                sprite.color = Color.white;
            }
        }

        for (uint i = 0; i < WeaponCount; ++i)
            timeElapsed[i] += Time.deltaTime;

        ulong switchMask = (ulong)((Input.GetButton("A") ? 1 : 0) + (Input.GetButton("B") ? 2 : 0) + (Input.GetButton("X") ? 4 : 0) + (Input.GetButton("Y") ? 8 : 0));

        if (switchMask > 0)
            activeWeapon = (WeaponType)Globals.BitScanForward(switchMask);

        if (Input.GetAxis("RightTrigger") > 0 && timeElapsed[(int)activeWeapon] >=  1.0f / maxShotsPerSecond[(int)activeWeapon])
        {
            timeElapsed[(int)activeWeapon] = 0;
            Audio.PlayOneShot(playerShoot);
            if (bulletPrefabs[(int)activeWeapon] != null)
            {
                bulletPrefabs[(int)activeWeapon].fire.Fire(bulletPrefabs[(int)activeWeapon].gameObject, gameObject);
                Audio.PlayOneShot(playerShoot);
            }
            else
                Debug.Log("The bullet prefab for weapon " + activeWeapon + " is missing and/or null!");
        }

        Vector3 offset = new Vector3(Input.GetAxis("LeftJoyX"), -Input.GetAxis("LeftJoyY"), 0);
        Vector3 turn = new Vector3(Input.GetAxis("RightJoyX"), -Input.GetAxis("RightJoyY"), 0);

        Vector3 pos = Globals.ClampToScreen(transform.position);

        float speedLimiter = 1 - Input.GetAxis("LeftTrigger") * focusPercentage;

        if (transform.position == pos)
            transform.position += offset * speed * speedLimiter * Time.deltaTime;
        else
            transform.position = pos;

        Quaternion rot = transform.rotation;
        if (turn.magnitude > 0)
            rot = Quaternion.AngleAxis(Mathf.Atan2(-turn.x, turn.y) * Mathf.Rad2Deg, Vector3.forward);
        
        transform.rotation = rot;

        offset.x *= -1;
        float bank = (-(rot * offset).x + 1) / 2;
        anim.Play("PlayerShip", -1, bank);
    }

    public void DamagePlayer(int damage)
    {
        if (isDead || invincibilityCounter > 0)
            return;

        health -= damage;
        invincibilityCounter = invincibilityTime;
        // TODO sound

        if (health <= 0)
            KillPlayer();
    }

    public void KillPlayer()
    {
        if (--lives == 0)
            isDead = true;

        if (!isDead)
        {
            // TODO game state needs to be called here for player death, that is NOT game over
            health = maxHealth;
            return;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        Audio.PlayOneShot(playerExplosion);
        explosion.Play(true);
        EnemySpawner.instance.StopSpawning();

        PlayerPrefs.SetInt("Score", score);
    }

    public void UpdateScore()
    {
        score += 1;
        scoreText.text = "Score: " + score;    
    }
}
