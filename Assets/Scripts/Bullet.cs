using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public bool playerShot;
    public EnemySpawner.EnemyType type;
    private AudioSource audioSource;
    public AudioClip explosion;

    public BulletMove move = BulletMove.Linear;
    public BulletRotate rotate = BulletRotate.None;
    public BulletFire fire = BulletFire.Single;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        audioSource = GetComponent<AudioSource>();
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
            Player player = collision.gameObject.GetComponent<Player>();
            audioSource.PlayOneShot(explosion);
            player.KillPlayer();
            Destroy(gameObject);
        }

        if (playerShot && collision.tag == "enemy")
        {
            Enemy currentEnemy = collision.gameObject.GetComponent<Enemy>();
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            if (currentEnemy.type == type)
            {
                player.UpdateScore();
                audioSource.PlayOneShot(explosion);
                currentEnemy.DestroyShip();
                Destroy(gameObject);
            }
        }
    }
}
