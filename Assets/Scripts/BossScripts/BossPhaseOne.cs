using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseOne : MonoBehaviour {

    public static BossPhaseOne instance;
    public int health;
    public float spawnTimer;
    public int numEnemiesOnScreen;

    private Collider2D hitBox;
    private Rigidbody2D rgb2d;
    private HangerBaySpawn[] hangers;

    private float lastSpawn = 0.0f;
    private int enemiesOnScreen;

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        hitBox = gameObject.GetComponent<Collider2D>();
        rgb2d = gameObject.GetComponent<Rigidbody2D>();
        hangers = gameObject.GetComponentsInChildren<HangerBaySpawn>();
	}
	
	// Update is called once per frame
	void Update () {
        if (enemiesOnScreen < numEnemiesOnScreen && lastSpawn <= 0.0f)
        {
            HangerBaySpawn currentHander = hangers[Random.Range(0, hangers.Length)];
            currentHander.SpawnEnemy();
            lastSpawn = spawnTimer;
        }

        if (lastSpawn > 0.0f)
            lastSpawn -= Time.deltaTime;
    }

    public void DamageBoss(int damage)
    {
        health -= damage;
        // TODO sound

        if (health <= 0)
            TransitionPhaseTwo();
    }

    private void TransitionPhaseTwo()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(8);
        }
    }

    public void KilledEnemy(EnemySpawner.EnemyType type)
    {
        enemiesOnScreen--;
    }
}
