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

    private Player player;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        lastBarrageFired = timeBetweenBarrages;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 270);

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
