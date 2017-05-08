using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpecial : MonoBehaviour {

    public float dashSpeed = 10;
    public float dashActivationTime = 4;
    [Range(0.0f, 9.0f)]
    public float speedDriftLevel = 1.0f;

    Enemy enemyScript;
    Rigidbody2D rgb2d;
    float speedDrift = 0.9f;
    bool isDashing = false;
    float nextDashTime;

    // Use this for initialization
    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        rgb2d = GetComponent<Rigidbody2D>();
        nextDashTime = dashActivationTime;
        speedDrift = 0.9f + speedDriftLevel / 100;
    }

    // Update is called once per frame
    void Update()
    {

        if (isDashing)
        {
            rgb2d.velocity *= speedDrift;

            if (rgb2d.velocity.magnitude <= 0.1f)
            {
                rgb2d.velocity *= 0;
                isDashing = false;

                enemyScript.LockFlightPattern(false, GetComponent<SpriteRenderer>().sortingLayerName);
            }
        }
        else
        {
            TryDashing();
        }
        

    }

    public void TryDashing()
    {
        if (nextDashTime <= 0.0f)
        {
            Vector3 dir = Vector3.zero; ;
            if (enemyScript.myHealth.type == HealthManager.DamagedByType.Yellow)
                dir = transform.rotation * Vector3.up;
            if (enemyScript.myHealth.type == HealthManager.DamagedByType.Red)
            {
                Vector3 playerDir = transform.rotation * Vector3.up;
                dir = Vector3.Cross(playerDir, Vector3.forward);
            }
            Dash(dir);
        }
        else
        {
            nextDashTime -= Time.deltaTime;
        }
    }

    void Dash(Vector3 dir)
    {
        isDashing = true;
        enemyScript.LockFlightPattern(true, GetComponent<SpriteRenderer>().sortingLayerName);

        enemyScript.ApplyTrajectory(dir, dashSpeed);
        nextDashTime = dashActivationTime;
    }
}
