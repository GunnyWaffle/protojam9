using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;

    public float speed = 35;
    public float rotateSpeed = 6;

    Rigidbody2D rb;

    float shotDelay = 0.25f;
    float timeElapsed = 0;
    Vector3 cannonOffset = new Vector3(0.5f, -0.1f, 0);

    ParticleSystem explosion;
    bool isDead = false;

    public bool Died
    {
        get { return isDead; }
    }

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        explosion = transform.Find("explosion").GetComponent<ParticleSystem>();
    }
	
	void Update()
    {
        if (isDead)
            return;

        timeElapsed += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && timeElapsed >= shotDelay)
        {
            timeElapsed = 0;

            GameObject ent = (GameObject)GameObject.Instantiate(bulletPrefab, transform.position + cannonOffset, Quaternion.identity);

            cannonOffset.x *= -1;
        }

        Vector3 offset = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            offset.y += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            offset.x -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            offset.y -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            offset.x += speed * Time.deltaTime;

        Vector3 pos = Globals.ClampToScreen(transform.position);

        if (transform.position == pos)
        {
            rb.AddForce(offset, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(Vector3.Normalize(-transform.position), ForceMode2D.Force);
        }

        //transform.position = pos;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "enemyBullet")
            return;

        if (isDead)
            return;

        isDead = true;

        rb.velocity = Vector3.zero;

        GetComponent<SpriteRenderer>().enabled = false;

        explosion.Play(true);
    }
}
