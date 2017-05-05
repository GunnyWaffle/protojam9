using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTurret : MonoBehaviour {

    public Bullet bullet;
    public int shotsPerBarrage;
    public float timeBetweenBarrages;
    private float lastBarrageFired;
    public float timeBetweenOneShot;
    private float lastShotFired;
    public float fanSpread;
    private float angleStep;

    public enum FanDirection
    {
        LeftToRight,
        RightToLeft
    }
    public FanDirection myDirection = FanDirection.RightToLeft;

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
        angleStep = (float)fanSpread / shotsPerBarrage;
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
        Vector3 dir = player.transform.position - transform.position;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270;
        float angleToFire = 0;
        // If we are faning RightToLeft, we want to start from the right and add angle.
        if (myDirection == FanDirection.RightToLeft)
        {
            baseAngle -= (fanSpread / 2.0f);
            baseAngle = baseAngle % 360;
            angleToFire = baseAngle + (shotsFired * angleStep);
        }
        // If we are fanning from LeftToRight we want to start from the left and subtract angle.
        else if (myDirection == FanDirection.LeftToRight)
        {
            baseAngle += (fanSpread / 2.0f);
            baseAngle = baseAngle % 360;
            angleToFire = baseAngle - (shotsFired * angleStep);
        }
        // Add a little randomness to fan so that the bullets should not nicely line up.
        int randomVariance = Random.Range(-7, 7);
        angleToFire += randomVariance;

        var newBullet = bullet.fire.Fire(bullet.gameObject, gameObject); // Create and fire the bullet
        newBullet[0].angle = angleToFire + gameObject.transform.rotation.eulerAngles.z; // Set the initial angle for the bullet to face.
        newBullet[0].transform.rotation = Quaternion.Euler(newBullet[0].transform.rotation.x, newBullet[0].transform.rotation.y, newBullet[0].angle); // Set the initial rotation so the up direction is correct
        newBullet[0].releaseDirection = newBullet[0].transform.up; // Save the initial up direction
        lastShotFired = timeBetweenOneShot;

        shotsFired++;
    }
}
