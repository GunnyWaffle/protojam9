using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    const int WeaponCount = 4;

    public GameObject[] bulletPrefabs = new GameObject[WeaponCount];
    public float speed = 2.2f;

    public float[] maxShotsPerSecond = new float[WeaponCount] { 4, 4, 4, 4 };
    float[] timeElapsed = new float[WeaponCount] { 0, 0, 0, 0 };

    public enum WeaponType { A, B, X, Y }
    WeaponType activeWeapon = WeaponType.A;
    public WeaponType ActiveWeapon { get { return activeWeapon; } }

    Animator anim;

    ParticleSystem explosion;
    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    public bool Died
    {
        get { return isDead; }
    }

	void Start()
    {
        explosion = transform.Find("explosion").GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();
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

        for (uint i = 0; i < WeaponCount; ++i)
            timeElapsed[i] += Time.deltaTime;

        ulong switchMask = (ulong)((Input.GetButton("A") ? 1 : 0) + (Input.GetButton("B") ? 2 : 0) + (Input.GetButton("X") ? 4 : 0) + (Input.GetButton("Y") ? 8 : 0));

        if (switchMask > 0)
            activeWeapon = (WeaponType)Globals.BitScanForward(switchMask);

        if (Input.GetAxis("RightTrigger") > 0 && timeElapsed[(int)activeWeapon] >=  1.0f / maxShotsPerSecond[(int)activeWeapon])
        {
            timeElapsed[(int)activeWeapon] = 0;

            if (bulletPrefabs[(int)activeWeapon] != null)
            {
                GameObject ent = (GameObject)Instantiate(bulletPrefabs[(int)activeWeapon], transform.position, transform.rotation);
            }
            else
                Debug.Log("The bullet prefab for weapon " + activeWeapon + " is missing and/or null!");
        }

        Vector3 offset = new Vector3(Input.GetAxis("LeftJoyX"), -Input.GetAxis("LeftJoyY"), 0);
        Vector3 tilt = new Vector3(Input.GetAxis("RightJoyX"), -Input.GetAxis("RightJoyY"), 0);

        Vector3 pos = Globals.ClampToScreen(transform.position);

        if (transform.position == pos)
            transform.position += offset * speed * Time.deltaTime;
        else
            transform.position = pos;

        float bank = offset.x;
        bank = Mathf.Abs(bank) < Mathf.Abs(tilt.x) ? tilt.x : (Mathf.Abs(bank) - Mathf.Abs(tilt.x)) * Mathf.Sign(bank);
        bank += 1;
        bank /= 2;

        anim.Play("PlayerShip", -1, bank);

        if (tilt.magnitude > 0)
            transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(-tilt.x, tilt.y) * Mathf.Rad2Deg, Vector3.forward);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "enemyBullet")
            return;

        if (isDead)
            return;

        isDead = true;

        GetComponent<SpriteRenderer>().enabled = false;

        explosion.Play(true);
    }
}
