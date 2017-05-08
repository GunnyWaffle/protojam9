using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseOne : MonoBehaviour {

    public static BossPhaseOne instance;
    public BossPhaseTwo bossTwoPrefab;
    public float spawnTimer;
    public int numEnemiesOnScreen;
    public int criticalAreas;
    public int score;

    public MissileTurret missileTurretOne;
    public void DeactivateMissleOne() { missileTurretOne.enabled = false; }
    public MissileTurret missileTurretTwo;
    public void DeactivateMissleTwo() { missileTurretTwo.enabled = false; }
    public ScatterTurret scatterTurretOne;
    public void DeactivateScatterOne() { scatterTurretOne.enabled = false; }
    public ScatterTurret scatterTurretTwo;
    public void DeactivateScatterTwo() { scatterTurretTwo.enabled = false; }
    private Collider2D hitBox;
    private HangerBaySpawn[] hangers;
    private Player player;
    public float explosionTimer;

    public ExplosionArea onDeathExplosion;
    private ColorFlash myColorFlash;
    private float timeBetweenFlashes = 0.2f;
    private float lastFlash;

    private float lastSpawn = 0.0f;
    private int enemiesOnScreen;

    // Use this for initialization
    void Start() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        hitBox = gameObject.GetComponent<Collider2D>();
        hangers = gameObject.GetComponentsInChildren<HangerBaySpawn>();
        player = FindObjectOfType<Player>();
        myColorFlash = gameObject.GetComponent<ColorFlash>();
        lastSpawn = spawnTimer;
        lastFlash = timeBetweenFlashes;
    }

    // Update is called once per frame
    void Update() {
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
        player.UpdateScore(200);
        instance.criticalAreas -= 1;
    }

    public void DestroyBoss()
    {
        player.UpdateScore(1000);

        TransitionPhaseTwo();
    }

    private void TransitionPhaseTwo()
    {
        StartCoroutine(PlayDeathAnimation());

        FlashBoss();
    }

    private void FlashBoss()
    {
        if (lastFlash <= 0.0f)
        {
            myColorFlash.Flash();
            lastFlash = timeBetweenFlashes;
        }
        else
        {
            lastFlash -= Time.deltaTime;
        }
    }

    private IEnumerator PlayDeathAnimation()
    {
        onDeathExplosion.TurnOnExplosions();
        yield return new WaitForSeconds(explosionTimer);
        onDeathExplosion.TurnOffExplosions();
        BossPhaseTwo newBossPhase = Instantiate(bossTwoPrefab, transform.position, transform.rotation);
        newBossPhase.InitializePhase(missileTurretOne, missileTurretTwo, scatterTurretOne, scatterTurretTwo, enemiesOnScreen);
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
