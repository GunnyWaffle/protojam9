using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed;
    public EnemySpawner.EnemyType type;

    void Start ()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }
	
	void Update ()
    {
        Vector3 screenPos = Globals.ClampToScreen(transform.position);

        if ((transform.position - screenPos).magnitude > transform.localScale.magnitude)
            Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "enemy")
        {
            Enemy currentEnemy = collision.gameObject.GetComponent<Enemy>();
            if (currentEnemy.type == type)
            {
                currentEnemy.DestroyShip();
                Destroy(gameObject);
            }
        }
    }
}
