using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : MonoBehaviour {

    public Bullet bullet;
    public float timeBetweenShot;
    private float lastShotFired;

    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    private AudioSource audioSource;

    public float rotationSpeed;

    private Player player;
    private HealthManager myHealth;
    public GameObject fireLocation;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        myHealth = gameObject.GetComponent<HealthManager>();
        lastShotFired = timeBetweenShot;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myHealth.isDead)
        {
            RotateTurret();

            // Are we in the deley between shots?
            if (lastShotFired <= 0.0f)
                FireMissile();
            else
                lastShotFired -= Time.deltaTime;
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

    private void FireMissile()
    {
        var newBullet = bullet.fire.Fire(bullet.gameObject, fireLocation);
        newBullet[0].GetComponent<Bullet>().target = player.gameObject;
        audioSource.PlayOneShot(enemyShoot);
        lastShotFired = timeBetweenShot;
    }
}
