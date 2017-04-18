using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public static EnemySpawner instance;

    public List<Enemy> typesOfEnemies = new List<Enemy>();
    public int numEnemiesOnScreen;
    public float spawnTimer;

    private float lastSpawn = 0.0f;
    private float minSpawnLocation = .1f;
    private float maxSpawnLocation = .9f;
    private float spawnDeadZone = 120.0f;
    private float spawnBuffer = 10.0f;
    private int enemiesOnScreen = 0;
    private Camera myCamera;


    public enum EnemyType
    {
        Blue = 0,
        Green = 1,
        Yellow = 2,
        Red = 3
    };

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        myCamera = GetComponent<Camera>();
    }
    private void Update()
    {
        if (enemiesOnScreen < numEnemiesOnScreen && lastSpawn <= 0.0f)
        {
            SpawnEnemy();
            lastSpawn = spawnTimer;
        }

        if (lastSpawn > 0.0f)
            lastSpawn -= Time.deltaTime;      
    }

    void SpawnEnemy()
    {
        float a, b, c, d;
        int side = Random.Range(1, 5);
        int enemyType = Random.Range(0, typesOfEnemies.Count);
        Enemy enemy = typesOfEnemies[enemyType];
        float startingAngle;
        Vector3 spawnLocation;

        // Spawn on bottom
        if (side == 1)
        {
            // Four boundaries for the two spawn zones
            a = spawnBuffer;
            b = (180 - spawnDeadZone) / 2;
            c = (spawnDeadZone + b);
            d = (180 - spawnBuffer);

            spawnLocation = new Vector3(Random.Range(minSpawnLocation, maxSpawnLocation), 0, 0);
        }
        // Spawn on left side
        else if (side == 2)
        {
            // Four boundaries for the two spawn zones
            a = -90 + spawnBuffer;
            b = -90 + (180 - spawnDeadZone) / 2;
            c = (spawnDeadZone + b);
            d = (90 - spawnBuffer);

            spawnLocation = new Vector3(0, Random.Range(minSpawnLocation, maxSpawnLocation), 0);
        }
        // Spawn on Top
        else if (side == 3)
        {
            // Four boundaries for the two spawn zones
            a = 180 + spawnBuffer;
            b = 180 + (180 - spawnDeadZone) / 2;
            c = (spawnDeadZone + b);
            d = (360 - spawnBuffer);

            spawnLocation = new Vector3(Random.Range(minSpawnLocation, maxSpawnLocation), 1.0f, 0);
        }
        // Spawn on Right side
        else
        {
            // Four boundaries for the two spawn zones
            a = 90 + spawnBuffer;
            b = 90 + (180 - spawnDeadZone) / 2;
            c = (spawnDeadZone + b);
            d = (270 - spawnBuffer);

            spawnLocation = new Vector3(1.0f, Random.Range(minSpawnLocation, maxSpawnLocation), 0);
        }

        int direction = Random.Range(1, 3);
        if (direction == 1)
        {
            // Spawn shooting right
            startingAngle = Random.Range(a, b);
        }
        else
        {
            // Spawn shooting left
            startingAngle = Random.Range(c, d);
        }

        spawnLocation = myCamera.ViewportToWorldPoint(spawnLocation);
        spawnLocation.z = 0;
   
        Enemy newEnemy = Instantiate(enemy, spawnLocation, Quaternion.Euler(0, 180, 0));
        // May need to init the enemy
        enemiesOnScreen++;
    }

    public void KilledEnemy(EnemyType type)
    {
        enemiesOnScreen--;
    }
}
