﻿using System.Collections;
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
    public HealthManager.DamagedByType type; // the type of bullet this is
    public bool shouldDestroyOnImpact = true; // should we destoroy this bullet on impact?

    // optional properties, add to this list for more exposed variables needed in behaviours
    public float angle = 0; // the forward facing angle
    public GameObject target; // the target
    public float weaveSpeed = 25;
    public float weaveAmplitude = 5;
    public float weaveGrowthRate = 0.5f;
    public float speedGrowthRate = 1;

    // Recommended to pair these two together. Spinning a bullet while using linear move will have the bullet act like a bomerang.
    public float spinSpeed = 0; // speed that a bullet will spin at.
    [HideInInspector]
    public Vector3 releaseDirection; // direction that a bullet was released at.

    // the current behaviour of the bullet
    public BulletMove move = BulletMove.Linear; // used each update
    public BulletRotate rotate = BulletRotate.None; // used each update
    public BulletFire fire = BulletFire.Single; // called externally to spawn bullets, set this via a prefab

    // apply movement to the rigid body
    private Rigidbody2D rigid;
    public Rigidbody2D Rigid { get { return rigid; } }

    [HideInInspector]
    public float timeAlive = 0.0f;

    // must call this base.Start() if you inherit Bullet
    // feel free to add more component grabs here for optional properties
    protected void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // apply bullet behaviours
	void Update()
    {
        timeAlive += Time.deltaTime;
        rotate.Rotate(this);
        move.Move(this);
    }

    // can be expanded upon, fired when the bullet hits something
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerShot && collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(damage);
            if (shouldDestroyOnImpact)
                Destroy(gameObject);
        }
    }
}
