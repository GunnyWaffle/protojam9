using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTurret : MonoBehaviour {

    public Bullet bullet;
    public int shotsPerBarrage;
    public float timeBetweenBarrages;
    private float lastBarrageFired;
    public float timeBetweenOneShot;
    private float lastShotFired;
    private float barrageAngle;

    private int shotsFired = 0;

    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    private AudioSource audioSource;

    private Player player;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        lastBarrageFired = timeBetweenBarrages;
        barrageAngle = Random.Range(0, 361);
    }

    // Update is called once per frame
    void Update()
    {
        // Should we fire  a new barrage?
        if (lastBarrageFired <= 0.0f)
        {
            //Have we completed firing a baragge?
            if (shotsFired == shotsPerBarrage)
            {
                shotsFired = 0;
                lastBarrageFired = timeBetweenBarrages;
                barrageAngle = Random.Range(0, 361);
            }
            else
            {
                // Are we in the deley between shots?
                if (lastShotFired <= 0.0f)
                    FireBarrage();
                else
                    lastShotFired -= Time.deltaTime;
            }
        }
        else
        {
            lastBarrageFired -= Time.deltaTime;
        }
    }

    private void FireBarrage()
    {
        // Find direction to the player
        //Vector3 dir = player.transform.position - transform.position;
        //float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        //float angleToFire = 0;

        var newBullet = bullet.fire.Fire(bullet.gameObject, gameObject); // Create and fire the bullet
        newBullet[0].angle = barrageAngle; // Set the initial angle for the bullet to face.
        //newBullet[0].transform.rotation = Quaternion.Euler(newBullet[0].transform.rotation.x, newBullet[0].transform.rotation.y, newBullet[0].angle); // Set the initial rotation so the up direction is correct
        lastShotFired = timeBetweenOneShot;

        shotsFired++;
    }
}
