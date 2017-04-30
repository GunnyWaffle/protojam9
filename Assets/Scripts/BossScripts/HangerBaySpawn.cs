using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerBaySpawn : MonoBehaviour {

    public List<Enemy> typesOfEnemies = new List<Enemy>();
    public Quaternion startingAngle = Quaternion.Euler(0, 180, 0);
    public GameObject exitHangerBay;
    public float speedExitHangerBay;

    private bool shouldSpawn = true;

    public enum EnemyType
    {
        Blue = 0,
        Green = 1,
        Yellow = 2,
        Red = 3
    };

    public void SpawnEnemy()
    {
        int enemyType = Random.Range(0, typesOfEnemies.Count);
        Enemy enemy = typesOfEnemies[enemyType];

        Enemy newEnemy = Instantiate(enemy, gameObject.transform.position, startingAngle);
        newEnemy.LockFlightPattern(true);
        newEnemy.ApplyTrajectory(exitHangerBay.transform.position - gameObject.transform.position, speedExitHangerBay);
        // May need to init the enemy
    }

    public void StopSpawning()
    {
        shouldSpawn = false;
    }
}
