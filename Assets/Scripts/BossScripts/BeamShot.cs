using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamShot : MonoBehaviour {

    public Bullet bullet;
    public float beamTime;
    private float currentBeamDuration;
    public float timeBetweenBeamShots;
    private float lastBeamFired;
    public float segmentDelay = 0.1f;
    private float timeBetweenSegments = 0.0f;

    public AudioClip enemyShoot;
    public AudioClip enemyExplosion;
    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        lastBeamFired = timeBetweenBeamShots;
    }

    // Update is called once per frame
    void Update()
    {
        // Should we fire a new beam?
        if (lastBeamFired <= 0.0f)
        {

            // How long have we been firing the beam
            if (currentBeamDuration <= 0.0f)
            {
                currentBeamDuration = beamTime;
                lastBeamFired = timeBetweenBeamShots;
            }
            else
            {
                FireBeamSegment();
                currentBeamDuration -= Time.deltaTime;
            }
        }
        else
        {
            lastBeamFired -= Time.deltaTime;
        }
    }

    private void FireBeamSegment()
    {
        if (timeBetweenSegments <= 0.0f)
        {
            var newBullet = bullet.fire.Fire(bullet.gameObject, gameObject);
            newBullet[0].rotate = BulletRotate.Angle;
            newBullet[0].angle = gameObject.transform.rotation.eulerAngles.z;
            bullet.rotate.Rotate(newBullet[0]);
            timeBetweenSegments = segmentDelay;
        }

        timeBetweenSegments -= Time.deltaTime;
        //audioSource.PlayOneShot(enemyShoot); 
    }
}
