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
    public float bulletSpeed;
    public float shotSpread;

    private int shotsFired = 0;

    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    private AudioSource audioSource;

    private Player player;
    public GameObject spawnAreaOne;
    public GameObject spawnAreaTwo;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 270);

        if (lastBarrageFired <= 0.0f)
        {
            if (shotsFired == shotsPerBarrage)
            {
                shotsFired = 0;
                lastBarrageFired = timeBetweenBarrages;
            }
            else
            {
                if (lastShotFired <= 0.0f)
                    FireBullet(spawnAreaOne);
                else
                    lastShotFired -= Time.deltaTime;
            }
        }
        else
        {
            lastBarrageFired -= Time.deltaTime;
        }
	}

    private void FireBullet(GameObject positionToFireFrom)
    {
        shotsFired++;
        bullet.fire.Fire(bullet.gameObject, gameObject);
        //audioSource.PlayOneShot(enemyShoot);
        lastShotFired = timeBetweenOneShot;
    }
}
