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

    public float angle = 0;
    public GameObject target;

    private Rigidbody2D rigid;
    public Rigidbody2D Rigid { get { return rigid; } }

    protected void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

	void Update()
    {
        rotate.Rotate(this);
        move.Move(this);
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
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().UpdateScore(enemy.score);
                enemy.DestroyShip();
                Destroy(gameObject);
            }
        }
    }
}
