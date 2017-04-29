using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 3;
    public int damage = 4;
    public bool playerShot = false;
    public EnemySpawner.EnemyType type;

    public BulletMove move = BulletMove.Linear;
    public BulletRotate rotate = BulletRotate.None;
    public BulletFire fire = BulletFire.Single;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }

	void Update()
    {
        move.Move(gameObject);
        rotate.Rotate(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerShot && collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(damage);
            Destroy(gameObject);
        }

        if (playerShot && collision.tag == "enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy.type == type)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().UpdateScore();
                enemy.DestroyShip();
                Destroy(gameObject);
            }
        }
    }
}
