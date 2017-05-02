using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBullet : Bullet {

    private HealthManager myHealth;

    new void Start()
    {
        base.Start();
        myHealth = gameObject.GetComponent<HealthManager>();
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
