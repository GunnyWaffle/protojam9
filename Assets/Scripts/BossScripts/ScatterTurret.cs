using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterTurret : MonoBehaviour {

    public Bullet bullet;
    public int shotsPerBarrage;
    public float timeBetweenBarrages;
    private float lastBarrageFired;
    public float timeBetweenOneShot;
    private float lastShotFired;
    public int shotSpread;

    public float rotationSpeed;

    private int shotsFired = 0;

    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    private AudioSource audioSource;
    private HealthManager myHealth;

    private Player player;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        myHealth = gameObject.GetComponent<HealthManager>();
        lastBarrageFired = timeBetweenBarrages;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!myHealth.isDead)
        {
            RotateTurret();

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
                        FireBullet();
                    else
                        lastShotFired -= Time.deltaTime;
                }
            }
            else
            {
                lastBarrageFired -= Time.deltaTime;
            }
        }
    }

    private void RotateTurret()
    {
        // Find direction to the player
        Vector3 dir = player.transform.position - transform.position;
        // Get the angle for vector and convert to Unity rotation, 0 is at the top of screen.
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 270) % 360;

        float angleDif = transform.rotation.eulerAngles.z - angle;
        float currentRotationAmount = rotationSpeed * Time.deltaTime;

        if (transform.rotation.eulerAngles.z > angle && angleDif < 180)
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - currentRotationAmount);
        }
        else if (transform.rotation.eulerAngles.z < angle && angleDif < 180)
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + currentRotationAmount);
        }
        else if (transform.rotation.eulerAngles.z > angle && angleDif > 180)
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + currentRotationAmount);
        }
        else if (transform.rotation.eulerAngles.z < angle && angleDif < 180)
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - currentRotationAmount);
        }
    }

    private void FireBullet()
    {
        float randomRotation = Random.Range(-shotSpread, shotSpread);
        shotsFired++;
        var newBullet = bullet.fire.Fire(bullet.gameObject, gameObject);
        newBullet[0].rotate = BulletRotate.Angle;
        newBullet[0].angle = randomRotation + gameObject.transform.rotation.eulerAngles.z;
        bullet.rotate.Rotate(newBullet[0]);
        //audioSource.PlayOneShot(enemyShoot);
        lastShotFired = timeBetweenOneShot;
    }
}
