using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    const int WeaponCount = 4;

    public PlayerBullet[] bulletPrefabs = new PlayerBullet[WeaponCount];
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

    public bool Died
    {
        get { return isDead; }
    }

    //Audio Stuff
    AudioSource Audio;
    public AudioClip playerExplosion;
    public AudioClip playerShoot;
	void Start()
    {
        explosion = transform.Find("explosion").GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();
        Audio = GetComponent<AudioSource>();
    }

    void OnValidate()
    {
        if (bulletPrefabs.Length != WeaponCount)
        {
            Debug.LogWarning("Don't change bulletPrefabs array size!");
            System.Array.Resize<PlayerBullet>(ref bulletPrefabs, WeaponCount);
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
                PlayerBullet ent = Instantiate(bulletPrefabs[(int)activeWeapon], transform.position, transform.rotation);
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

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "enemyBullet")
            return;

        if (isDead)
            return;

        KillPlayer();
    }

    public void KillPlayer()
    {
        isDead = true;

        GetComponent<SpriteRenderer>().enabled = false;
        Audio.PlayOneShot(playerExplosion);
        explosion.Play(true);
        EnemySpawner.instance.StopSpawning();
    }
}
