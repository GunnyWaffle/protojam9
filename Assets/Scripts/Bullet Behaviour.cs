using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletMove
{
    None,
    Linear
}

public enum BulletRotate
{
    Angle,
    FollowUnit,
    None
}

public enum BulletFire
{
    Single
}

static class BulletMethods
{
    public static void Move(this BulletMove bm, GameObject go)
    {
        switch (bm)
        {
            case BulletMove.Linear:
                LinearMove(go);
                break;
            case BulletMove.None:
                break;
            default:
                break;
        }
    }

    public static void Rotate(this BulletRotate br, GameObject go, float angle = 0.0f, GameObject unitToFollow = null)
    {
        switch (br)
        {
            case BulletRotate.Angle:
                AngleRotation(go, angle);
                break;
            case BulletRotate.FollowUnit:
                FollowUnit(go, unitToFollow);
                break;
            case BulletRotate.None:
                break;
            default:
                break;
        }
    }

    public static Bullet[] Fire(this BulletFire bf, GameObject prefab, GameObject source)
    {
        switch (bf)
        {
            case BulletFire.Single:
                return SingleFire(prefab, source);
            default:
                return new Bullet[0];
        }
    }

    static void LinearMove(GameObject go)
    {
        Vector3 screenPos = Globals.ClampToScreen(go.transform.position);

        if ((go.transform.position - screenPos).magnitude > go.transform.localScale.magnitude)
            GameObject.Destroy(go.gameObject);
    }

    static Bullet[] SingleFire(GameObject prefab, GameObject source)
    {
        Bullet[] bullets = new Bullet[1];

        bullets[0] = GameObject.Instantiate(prefab, source.transform.position, source.transform.rotation).GetComponent<Bullet>();

        return bullets;
    }

    static void AngleRotation(GameObject go, float angle)
    {
        go.transform.rotation = Quaternion.Euler(go.transform.rotation.eulerAngles.x, go.transform.rotation.eulerAngles.y, go.transform.rotation.eulerAngles.z + angle);
    }

    static void FollowUnit(GameObject you, GameObject other)
    {
        Vector3 dir = other.transform.position - you.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        you.transform.rotation = Quaternion.Euler(0, 0, angle + 270);
    }
}