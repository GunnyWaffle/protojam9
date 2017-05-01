using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private GameObject player;
    private AudioSource audioSource;
    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    public AudioClip playerExplosion;

    public float speed = 1.0f;
    public int score;

    //Enemy shoot
    public Bullet bullet;
    public float timeBetweenShots;
    private float lastShotTime;

    public EnemySpawner.EnemyType type;
    private Rigidbody2D myRB2d;
    private SpriteRenderer mySRD;

    private Vector3 currentTargetLocation;
    private bool flightLocked = false;

    private void Awake()
    {
        myRB2d = GetComponent<Rigidbody2D>();
        mySRD = GetComponent<SpriteRenderer>();
    }
    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>().gameObject;
        lastShotTime = (1 * timeBetweenShots) / 4;
        audioSource = GetComponent<AudioSource>();
        GenerateTargetLocation();
    }

    public void LockFlightPattern(bool flightLocked, string sortingLayer)
    {
        this.flightLocked = flightLocked;
        mySRD.sortingLayerName = sortingLayer;
    }

    // Update is called once per frame
    void Update () {
        if (player != null && !flightLocked)
        {
            PerformActions();
        }
    }

    void PerformActions()
    {
        // Move
        Move();

        // Try to fire at the player
        Attack();

        lastShotTime -= Time.deltaTime;
    }

    void Move()
    {
        //Enemy always faces player
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 normal = Vector3.Cross(dir.normalized, Vector3.forward);

        Vector3 moveDir = Vector3.zero;
        switch (type)
        {
            case EnemySpawner.EnemyType.Blue:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                moveDir = new Vector3(Mathf.Sin(2 * Time.time), -1, 0);
                moveDir.Normalize();
                break;
            case EnemySpawner.EnemyType.Green:
                transform.rotation = Quaternion.Euler(0, 0, angle + 270);
                normal.Normalize();
                Vector3 offset;
                if (dir.magnitude <= 1)
                {
                    offset = dir.normalized * -Mathf.Abs(Mathf.Sin(2 * Time.time));
                }
                else if (dir.magnitude <= 2)
                {
                    offset = Vector3.zero;
                }
                else
                {
                    offset = dir.normalized * Mathf.Abs(Mathf.Sin(2 * Time.time));
                }
                 
                offset.Normalize();
                moveDir = normal + offset;
                moveDir.Normalize();
                break;
            case EnemySpawner.EnemyType.Red:
                transform.rotation = Quaternion.Euler(0, 0, angle + 270);
                moveDir = dir.normalized;
                break;
            case EnemySpawner.EnemyType.Yellow:
                transform.rotation = Quaternion.Euler(0, 0, angle + 270);
                normal.Normalize();
                moveDir = normal;
                moveDir.Normalize();
                break;
            default:
                moveDir = dir.normalized;
                break;
        }
        ApplyTrajectory(moveDir, speed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        if (type == EnemySpawner.EnemyType.Blue)
            DestroyShip();
    }

    void Attack()
    {
        Vector3 dir = player.transform.position - transform.position;
        switch (type)
        {
            case EnemySpawner.EnemyType.Blue:
                //if ((transform.position - currentTargetLocation).magnitude < 0.02f)
                if (lastShotTime <= 0.0f)
                {
                    FireBullet();
                }
                //GenerateTargetLocation();
                //dir = currentTargetLocation - transform.position;
                break;
            case EnemySpawner.EnemyType.Green:
                if (lastShotTime <= 0.0f)
                    FireBullet();

                //if ((transform.position - currentTargetLocation).magnitude < 0.02f)
                //GenerateTargetLocation();

                //dir = currentTargetLocation - transform.position;
                break;
            case EnemySpawner.EnemyType.Red:
                if (lastShotTime <= 0.0f)
                {
                    FireBullet();
                }
                break;
            case EnemySpawner.EnemyType.Yellow:
                if (lastShotTime <= 0.0f)
                {
                    FireBullet();
                }
                break;
        }
    }

    public void ApplyTrajectory(Vector3 direction, float speed)
    {
        myRB2d.velocity = direction.normalized * speed;
    }

    public void DestroyShip()
    {
        if (type == EnemySpawner.EnemyType.Blue)
        {
            if (EnemySpawner.instance != null)
                EnemySpawner.instance.KilledEnemy(type);
        }
        else
        {
            if (BossPhaseOne.instance != null)
                BossPhaseOne.instance.KilledEnemy(type);
        }
        
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
