using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private GameObject player;
    private AudioSource audioSource;
    public AudioClip enemyShoot;

    public float speed = 1.0f;

    //Enemy shoot
    public Bullet bullet;
    public float timeBetweenShots;
    private float lastShotTime;

    [HideInInspector]
    public HealthManager myHealth;
    private Rigidbody2D myRB2d;
    private SpriteRenderer mySRD;

    private Vector3 currentTargetLocation;
    private bool flightLocked = false;

    private void Awake()
    {
        myRB2d = GetComponent<Rigidbody2D>();
        mySRD = GetComponent<SpriteRenderer>();
        myHealth = GetComponent<HealthManager>();
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
            CheckIfOnScreen();
        }
    }

    void PerformActions()
    {
        // Move
        Move();

        // Try to fire at the player
        Attack();

        if (!player.GetComponent<Player>().IsDead)
            lastShotTime -= Time.deltaTime;
    }

    void Move()
    {
        //Enemy always faces player
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 normal = Vector3.Cross(dir.normalized, Vector3.forward);

        Vector3 moveDir = Vector3.zero;
        switch (myHealth.type)
        {
            case HealthManager.DamagedByType.Blue:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                moveDir = new Vector3(Mathf.Sin(2 * Time.time), -1, 0);
                moveDir.Normalize();
                break;
            case HealthManager.DamagedByType.Green:
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
            case HealthManager.DamagedByType.Red:
                transform.rotation = Quaternion.Euler(0, 0, angle + 270);
                //moveDir = dir.normalized;


                if (transform.position.y > -1.8f)
                {

                    Vector3 lerpPos = Vector3.Lerp(transform.position, new Vector3(0, -1.8f, 0.0f), 0.1f);
                    moveDir = lerpPos - transform.position;
                }
                else
                {
                    //lerpPos = Vector3.Lerp(transform.position, new Vector3(Mathf.Sin(Time.time) + Mathf.PingPong(Time.time, 3), -1.8f, 0.0f), 0.1f);
                    moveDir = new Vector3(Mathf.Sin(Time.time), 0, 0.0f);
                }

                break;
            case HealthManager.DamagedByType.Yellow:
                transform.rotation = Quaternion.Euler(0, 0, angle + 270);
                if (dir.magnitude <= 0.75f)
                {
                    GetComponent<EnemySpecial>().TryDashing();
                    return;
                }
                else
                {
                    normal.Normalize();
                    moveDir = normal + dir.normalized;
                    moveDir.Normalize();
                }
                break;
            default:
                moveDir = dir.normalized;
                break;
        }
        ApplyTrajectory(moveDir, speed * Time.deltaTime);
    }

    void CheckIfOnScreen()
    {
        if (myHealth.type == HealthManager.DamagedByType.Blue)
        {
            Vector3 bottom = Camera.main.ScreenToWorldPoint(Vector3.down);
            if (transform.position.y < bottom.y)
            {
                UpdateSpawners();
                Destroy(gameObject);
            }
        }
    }

    void Attack()
    {
        if (player.GetComponent<Player>().IsDead)
            return;

        FireBullet();
    }

    public void ApplyTrajectory(Vector3 direction, float speed)
    {
        myRB2d.velocity = direction.normalized * speed;
    }

    public void UpdateSpawners()
    {
        if (myHealth.type == HealthManager.DamagedByType.Blue)
        {
            if (EnemySpawner.instance != null)
                EnemySpawner.instance.KilledEnemy(myHealth.type);
        }
        else
        {
            if (BossPhaseOne.instance != null)
                BossPhaseOne.instance.KilledEnemy(myHealth.type);
            if (BossPhaseTwo.instance != null)
                BossPhaseTwo.instance.KilledEnemy(myHealth.type);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(10);
            UpdateSpawners();
        }
    }

    private void FireBullet()
    {
        if (lastShotTime <= 0.0f && !myHealth.isDead)
        {
            bullet.fire.Fire(bullet.gameObject, gameObject);
            audioSource.PlayOneShot(enemyShoot);
            lastShotTime = timeBetweenShots;
        }
    }

    private void GenerateTargetLocation()
    {
        float randomXPos = Random.Range(0.05f, 0.95f);
        float randomYPos = Random.Range(0.05f, 0.95f);
        currentTargetLocation = Camera.main.ViewportToWorldPoint(new Vector3(randomXPos, randomYPos, 0.0f));
        currentTargetLocation.z = 0.0f;
    }
}
