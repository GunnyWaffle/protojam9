using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : MonoBehaviour {

    public MissileBullet bullet;
    public float timeBetweenShot;
    private float lastShotFired;

    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    private AudioSource audioSource;

    private Player player;
    public GameObject fireLocation;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 270);

        // Are we in the deley between shots?
        if (lastShotFired <= 0.0f)
            FireMissile();
        else
            lastShotFired -= Time.deltaTime;
    }

    private void FireMissile()
    {
        var newBullet = bullet.fire.Fire(bullet.gameObject, fireLocation);
        newBullet[0].GetComponent<MissileBullet>().SetTarget(player.gameObject);
        //audioSource.PlayOneShot(enemyShoot);
        lastShotFired = timeBetweenShot;
    }
}
