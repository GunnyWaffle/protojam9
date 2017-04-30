using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBullet : Bullet {

    private HealthManager myHealth;
    private GameObject target;

    private void Start()
    {
        myHealth = gameObject.GetComponent<HealthManager>();
    }

    protected override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;

        base.Move();
    }

    protected override void Rotate()
    {
        rotate.Rotate(gameObject, 0.0f, target);
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (bullet.playerShot)
            {
                myHealth.DamageUnit(bullet.damage);
                Destroy(collision.gameObject);
            }
        }
    }
}
