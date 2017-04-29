using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletMove
{
    None,
    Linear
}

public enum BulletRotate
{
    None
}

static class BulletMethods
{
    public static void Move(this BulletMove bm, GameObject go)
    {
        switch (bm)
        {
            case BulletMove.Linear:
                LinearMove(go);
                break;
            case BulletMove.None:
            default:
                break;
        }
    }

    public static void Rotate(this BulletRotate bm, GameObject go)
    {
        switch (bm)
        {
            case BulletRotate.None:
            default:
                break;
        }
    }

    static void LinearMove(GameObject go)
    {
        Vector3 screenPos = Globals.ClampToScreen(go.transform.position);

        if ((go.transform.position - screenPos).magnitude > go.transform.localScale.magnitude)
            GameObject.Destroy(go.gameObject);
    }
}

public class Bullet : MonoBehaviour
{
    public float speed;
    public bool playerShot;
    public EnemySpawner.EnemyType type;
    private AudioSource audioSource;
    public AudioClip explosion;

    public BulletMove move = BulletMove.Linear;
    public BulletRotate rotate = BulletRotate.None;

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
