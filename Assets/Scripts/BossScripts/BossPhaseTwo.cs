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
    private ColorFlash myColorFlash;
    public float transitionInFlashAmount;
    private float flashInTimePlayed;
    public float transitionOutFlashAmount;
    private float flashOutTimePlayed;
    public float timeBetweenFlashes = 0.2f;
    private float lastFlashTime;

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
        myColorFlash = gameObject.GetComponent<ColorFlash>();
        hangers = gameObject.GetComponentsInChildren<HangerBaySpawn>();
        player = FindObjectOfType<Player>();
        flashOutTimePlayed = transitionOutFlashAmount;
        flashInTimePlayed = transitionInFlashAmount;
        lastFlashTime = timeBetweenFlashes;
        lastSpawn = spawnTimer;
    }

    public void InitializePhase(MissileTurret missileOne, MissileTurret missileTwo, ScatterTurret scatterOne, ScatterTurret scatterTwo, int enemiesOnScreen)
    {
        // Is the first missile turret present? If not leave remains on the ship where it should be.
        if (missileOne == null)
        {
            missleTurretOne.GetComponent<HealthManager>().LeaveRemains();
            missleTurretOne.SetActive(false);
        }
        else
        {
            missleTurretOne.GetComponent<HealthManager>().SetHealth(missileOne.GetComponent<HealthManager>().GetHealth());
            missleTurretOne.transform.rotation = missileOne.transform.rotation;
        }

        // Is the second missle turret present? If not leave remains on the ship where it should be.
        if (missileTwo == null)
        {
            missleTurretTwo.GetComponent<HealthManager>().LeaveRemains();
            missleTurretTwo.SetActive(false);
        }
        else
        {
            missleTurretTwo.GetComponent<HealthManager>().SetHealth(missileTwo.GetComponent<HealthManager>().GetHealth());
            missleTurretTwo.transform.rotation = missileTwo.transform.rotation;
        }

        // Is the first scatter turret present? If not leave remains.
        if (scatterOne == null)
        {
            scatterTurretOne.GetComponent<HealthManager>().LeaveRemains();
            scatterTurretOne.SetActive(false);
        }
        else
        {
            scatterTurretOne.GetComponent<HealthManager>().SetHealth(scatterOne.GetComponent<HealthManager>().GetHealth());
            scatterTurretOne.transform.rotation = scatterOne.transform.rotation;
        }

        // Is the second scatter turret present? If nor leave remains.
        if (scatterTwo == null)
        {
            scatterTurretTwo.GetComponent<HealthManager>().LeaveRemains();
            scatterTurretTwo.SetActive(false);
        }
        else
        {
            scatterTurretTwo.GetComponent<HealthManager>().SetHealth(scatterTwo.GetComponent<HealthManager>().GetHealth());
            scatterTurretTwo.transform.rotation = scatterTwo.transform.rotation;
        }


        this.enemiesOnScreen = enemiesOnScreen;
    }

    private void FlashBoss()
    {
        if (lastFlashTime <= 0.0f)
        {
            myColorFlash.Flash();
            lastFlashTime = timeBetweenFlashes;
        }
        else
        {
            lastFlashTime -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (flashInTimePlayed > 0.0f)
        {
            FlashBoss();
            flashInTimePlayed -= Time.deltaTime;
        }

        if (enemiesOnScreen < numEnemiesOnScreen && lastSpawn <= 0.0f)
        {
            HangerBaySpawn currentHander = hangers[Random.Range(0, hangers.Length)];
            currentHander.SpawnEnemy();
            enemiesOnScreen++;
            lastSpawn = spawnTimer;
            player.UpdateScore(-10);
        }

        if (lastSpawn > 0.0f)
            lastSpawn -= Time.deltaTime;

        if (criticalAreas == 0)
            DestroyBoss();
    }

    public void DecrementCriticalAreas()
    {
        player.UpdateScore(500);
        instance.criticalAreas -= 1;
    }

    public void DestroyBoss()
    {
        player.UpdateScore(1000);

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
