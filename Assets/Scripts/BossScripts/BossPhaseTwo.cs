using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTwo : MonoBehaviour {

    public static BossPhaseTwo instance;
    public BossPhaseThree bossPhaseThreePrefab;
    public GameObject bossPhaseThreeSpawnLoc;
    public float spawnTimer;
    public int numEnemiesOnScreen;
    public int criticalAreas;
    public int score;
    public GameObject missleTurretOne;
    public GameObject missleTurretTwo;
    public GameObject scatterTurretOne;
    public GameObject scatterTurretTwo;

    private Collider2D hitBox;
    private HangerBaySpawn[] hangers;
    private Player player;

    private float lastSpawn = 0.0f;
    private int enemiesOnScreen;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        hitBox = gameObject.GetComponent<Collider2D>();
        hangers = gameObject.GetComponentsInChildren<HangerBaySpawn>();
        player = FindObjectOfType<Player>();
        lastSpawn = spawnTimer;
    }

    public void InitializePhase(bool missleOne, bool missleTwo, bool scatterOne, bool scatterTwo, int enemiesOnScreen)
    {
        // Is the first missile turret present? If not leave remains on the ship where it should be.
        if (!missleOne)
            missleTurretOne.SetActive(false);
        else
            scatterTurretOne.GetComponent<HealthManager>().LeaveRemains();

        // Is the second missle turret present? If not leave remains on the ship where it should be.
        if (!missleTwo)
            missleTurretTwo.SetActive(false);
        else
            scatterTurretOne.GetComponent<HealthManager>().LeaveRemains();

        // Is the first scatter turret present? If not leave remains.
        if (!scatterOne)
            scatterTurretOne.SetActive(false);
        else
            scatterTurretOne.GetComponent<HealthManager>().LeaveRemains();

        // Is the second scatter turret present? If nor leave remains.
        if (!scatterTwo)
            scatterTurretTwo.SetActive(false);
        else
            scatterTurretOne.GetComponent<HealthManager>().LeaveRemains();


        this.enemiesOnScreen = enemiesOnScreen;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesOnScreen < numEnemiesOnScreen && lastSpawn <= 0.0f)
        {
            HangerBaySpawn currentHander = hangers[Random.Range(0, hangers.Length)];
            currentHander.SpawnEnemy();
            enemiesOnScreen++;
            lastSpawn = spawnTimer;
        }

        if (lastSpawn > 0.0f)
            lastSpawn -= Time.deltaTime;

        if (criticalAreas == 0)
            DestroyBoss();
    }

    public void DecrementCriticalAreas()
    {
        instance.criticalAreas -= 1;
    }

    public void DestroyBoss()
    {
        player.UpdateScore(score);

        TransitionPhaseThree();
    }

    private void TransitionPhaseThree()
    {
        BossPhaseThree newBossPhase = Instantiate(bossPhaseThreePrefab, bossPhaseThreeSpawnLoc.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(8);
        }
    }

    public void KilledEnemy(HealthManager.DamagedByType type)
    {
        enemiesOnScreen--;
        if (enemiesOnScreen < 0)
            enemiesOnScreen = 0;

        lastSpawn = spawnTimer;
    }
}
