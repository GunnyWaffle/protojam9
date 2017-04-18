using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private GameObject player;

    //Alters orbit speed along x and y
    float orbit_period_X = 1.0f;
    float orbit_period_Y = 1.0f;

    //Alters orbit length along x and y
    float orbit_radius_X = 1.0f;
    float orbit_radius_Y = 1.0f;

    public float speed = 1.0f;

    //Enemy shoot
    public EnemyBullet bullet;
    public float timeBetweenShots;
    private float lastShotTime;

    public EnemySpawner.EnemyType type;
    private Rigidbody2D myRB2d;

    // Use this for initialization
    void Start () {
        myRB2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().gameObject;
        lastShotTime = timeBetweenShots;
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
            myRB2d.velocity = dir.normalized * speed * Time.deltaTime;

            if (lastShotTime <= 0.0f)
            {
                EnemyBullet ent = Instantiate(bullet, transform.position, transform.rotation);
                lastShotTime = timeBetweenShots;
            }

            lastShotTime -= Time.deltaTime;
        }
    }

    public void DestroyShip()
    {
        EnemySpawner.instance.KilledEnemy(type);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            DestroyShip();
            collision.gameObject.GetComponent<Player>().KillPlayer();
        }
    }
}
