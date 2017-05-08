using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseThree : MonoBehaviour {

    public static BossPhaseThree instance;
    public int criticalAreas;
    public int score;
    public float timeBetweenMoves;
    private float lastMoveTime;
    private Vector3 currentTargetLocation;
    private bool reachedDestination;
    public float screenXBuffer;
    public float screenYBuffer;
    public float bossMoveSpeed;

    public ExplosionArea onDeathExplosion;
    private ColorFlash myColorFlash;
    public float transitionInFlashAmount;
    private float flashInTimePlayed;
    public float transitionOutFlashAmount;
    private float flashOutTimePlayed;
    public float timeBetweenFlashes = 0.2f;
    private float lastFlashTime;

    private Collider2D hitBox;
    private Rigidbody2D myRGB2D;
    private Player player;
    private bool isDead;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        myRGB2D = gameObject.GetComponent<Rigidbody2D>();
        hitBox = gameObject.GetComponent<Collider2D>();
        player = FindObjectOfType<Player>();
        lastMoveTime = timeBetweenMoves;
        reachedDestination = true;
        myColorFlash = gameObject.GetComponent<ColorFlash>();
        flashInTimePlayed = transitionInFlashAmount;
        flashOutTimePlayed = transitionOutFlashAmount;
        lastFlashTime = timeBetweenFlashes;
        isDead = false;
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

        if (criticalAreas == 0 && !isDead)
            DestroyBoss();
        else if (isDead)
            FlashBoss();

        if (lastMoveTime <= 0.0f && !isDead)
        {
            if (reachedDestination)
                SelectMoveLocation();

            if ((currentTargetLocation - transform.position).magnitude < 0.05f)
            {
                reachedDestination = true;
                lastMoveTime = timeBetweenMoves;
                myRGB2D.velocity = new Vector2(0.0f, 0.0f);
            }
            else
            {
                Vector3 dir = currentTargetLocation - transform.position;
                myRGB2D.velocity = dir.normalized * bossMoveSpeed * Time.deltaTime;
            }
        }
        else
        {
            lastMoveTime -= Time.deltaTime;
        }

    }

    private void SelectMoveLocation()
    {
        float randomXPos = Random.Range(screenXBuffer, 1.0f - screenXBuffer);
        float randomYPos = Random.Range(screenYBuffer, 1.0f - screenYBuffer);
        currentTargetLocation = Camera.main.ViewportToWorldPoint(new Vector3(randomXPos, randomYPos, 0.0f));
        currentTargetLocation.z = 0.0f;
        reachedDestination = false;
    }
    public void DecrementCriticalAreas()
    {
        player.UpdateScore(1000);
        instance.criticalAreas -= 1;
    }

    public void DestroyBoss()
    {
        player.UpdateScore(2000);
        myRGB2D.velocity = new Vector2(0.0f, 0.0f);
        isDead = true;
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        onDeathExplosion.TurnOnExplosions();
        yield return new WaitForSeconds(transitionOutFlashAmount);
        onDeathExplosion.TurnOffExplosions();
        EndBossFight();
    }

    private void EndBossFight()
    {
        PlayerPrefs.SetInt("Victory", 0);
        Destroy(gameObject);
        SceneTransition.Gameover();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(8);
        }
    }
}
