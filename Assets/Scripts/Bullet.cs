using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the core class/component for all bullets
// Make a prefab of an object with this component to configure it pre-fire
// The fire method will create the bullet(s) for you, no need to instantiate anything that is a bullet

public class Bullet : MonoBehaviour
{
    // various properties that can be used in behaviours
    public float speed = 3; // movement speed
    public int damage = 4; // damage applied to hit entity
    public bool playerShot = false; // true if the player shot this
    public EnemySpawner.EnemyType type; // the type of bullet this is

    // optional properties, add to this list for more exposed variables needed in behaviours
    public float angle = 0; // the forward facing angle
    public GameObject target; // the target

    // the current behaviour of the bullet
    public BulletMove move = BulletMove.Linear; // used each update
    public BulletRotate rotate = BulletRotate.None; // used each update
    public BulletFire fire = BulletFire.Single; // called externally to spawn bullets, set this via a prefab

    // apply movement to the rigid body
    private Rigidbody2D rigid;
    public Rigidbody2D Rigid { get { return rigid; } }

    // must call this base.Start() if you inherit Bullet
    // feel free to add more component grabs here for optional properties
    protected void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // apply bullet behaviours
	void Update()
    {
        rotate.Rotate(this);
        move.Move(this);
    }

    // can be expanded upon, fired when the bullet hits something
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
