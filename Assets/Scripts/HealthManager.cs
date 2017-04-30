using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    public int health;

    private bool isDead = false;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void DamageUnit(int damage)
    {
        health -= damage;
        // TODO sound

        if (health <= 0)
            KillUnit();
    }

    public void KillUnit()
    {
        Destroy(gameObject);
        //Audio.PlayOneShot(playerExplosion);
        //explosion.Play(true);

        player.UpdateScore();
    }
}
