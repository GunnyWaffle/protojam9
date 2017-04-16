using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    const int WeaponCount = 4;

    public GameObject[] bulletPrefabs = new GameObject[WeaponCount];
    public float speed = 35;
    public float rotateSpeed = 6;
    public float[] maxShotsPerSecond = new float[WeaponCount] { 4, 4, 4, 4 };

    Rigidbody2D rb;

    float shotDelay = 0.25f;
    float timeElapsed = 0;
    public enum WeaponType { A, B, X, Y }
    WeaponType activeWeapon = WeaponType.A;
    public WeaponType ActiveWeapon { get { return activeWeapon; } }


    Vector3 cannonOffset = new Vector3(0.5f, -0.1f, 0);

    ParticleSystem explosion;
    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    public bool Died
    {
        get { return isDead; }
    }

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        explosion = transform.Find("explosion").GetComponent<ParticleSystem>();
    }

    void OnValidate()
    {
        if (bulletPrefabs.Length != WeaponCount)
        {
            Debug.LogWarning("Don't change bulletPrefabs array size!");
            System.Array.Resize<GameObject>(ref bulletPrefabs, WeaponCount);
        }

        if (maxShotsPerSecond.Length != WeaponCount)
        {
            Debug.LogWarning("Don't change maxShotsPerSecond array size!");
            System.Array.Resize<float>(ref maxShotsPerSecond, WeaponCount);
        }
    }

    void Update()
    {
        if (isDead)
            return;

        timeElapsed += Time.deltaTime;

        ulong switchMask = (ulong)((Input.GetButton("A") ? 1 : 0) + (Input.GetButton("B") ? 2 : 0) + (Input.GetButton("X") ? 4 : 0) + (Input.GetButton("Y") ? 8 : 0));

        if (switchMask > 0)
            activeWeapon = (WeaponType)Globals.BitScanForward(switchMask);

        Debug.Log(activeWeapon);

        if (Input.GetAxis("RightTrigger") > 0 && timeElapsed >= shotDelay)
        {
            timeElapsed = 0;

            GameObject ent = (GameObject)GameObject.Instantiate(bulletPrefabs[(int)activeWeapon], transform.position + cannonOffset, Quaternion.identity);

            cannonOffset.x *= -1;
        }

        Vector3 offset = new Vector3(Input.GetAxis("LeftJoyX"), -Input.GetAxis("LeftJoyY"), 0);

        Vector3 pos = Globals.ClampToScreen(transform.position);

        if (transform.position == pos)
        {
            rb.AddForce(offset, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(Vector3.Normalize(-transform.position), ForceMode2D.Force);
        }
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
