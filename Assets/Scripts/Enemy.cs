using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private GameObject player;
    private AudioSource audioSource;
    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    public AudioClip playerExplosion;

    //Alters orbit speed along x and y
    float orbit_period_X = 1.0f;
    float orbit_period_Y = 1.0f;

    //Alters orbit length along x and y
    float orbit_radius_X = 1.0f;
    float orbit_radius_Y = 1.0f;

    public float speed = 1.0f;

    //Enemy shoot
    public Bullet bullet;
    public float timeBetweenShots;
    private float lastShotTime;

    public EnemySpawner.EnemyType type;
    private Rigidbody2D myRB2d;

    private Vector3 currentTargetLocation;

    // Use this for initialization
    void Start () {
        myRB2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().gameObject;
        lastShotTime = timeBetweenShots;
        audioSource = GetComponent<AudioSource>();
        GenerateTargetLocation();
    }

    // Update is called once per frame
    void Update () {
        if (player != null)
        {
            //Enemy always faces player
            Vector3 dir = player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 270);

            // Follow the player
            switch (type)
            {
                case EnemySpawner.EnemyType.Blue:
                    if ((transform.position - currentTargetLocation).magnitude < 0.02f)
                    {
                        FireBullet();
                        GenerateTargetLocation();
                    }
                    dir = currentTargetLocation - transform.position;
                    break;
                case EnemySpawner.EnemyType.Green:
                    if (lastShotTime <= 0.0f)
                        FireBullet();

                    if ((transform.position - currentTargetLocation).magnitude < 0.02f)
                        GenerateTargetLocation();

                    dir = currentTargetLocation - transform.position;
                    break;
                case EnemySpawner.EnemyType.Red:
                    if (lastShotTime <= 0.0f)
                        FireBullet();
                    break;
                case EnemySpawner.EnemyType.Yellow:
                    if (lastShotTime <= 0.0f)
                        FireBullet();
                    break;
            }

            myRB2d.velocity = dir.normalized * speed * Time.deltaTime;
            lastShotTime -= Time.deltaTime;
        }
    }

    public void DestroyShip()
    {
        EnemySpawner.instance.KilledEnemy(type);
        audioSource.PlayOneShot(enemyExplosion);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(8);
            DestroyShip();
        }
    }

    private void FireBullet()
    {
        bullet.fire.Fire(bullet.gameObject, gameObject);
        audioSource.PlayOneShot(enemyShoot);
        lastShotTime = timeBetweenShots;
    }

    private void GenerateTargetLocation()
    {
        float randomXPos = Random.Range(0.05f, 0.95f);
        float randomYPos = Random.Range(0.05f, 0.95f);
        currentTargetLocation = Camera.main.ViewportToWorldPoint(new Vector3(randomXPos, randomYPos, 0.0f));
        currentTargetLocation.z = 0.0f;
    }
}
