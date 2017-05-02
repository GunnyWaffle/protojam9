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
    None,
    Angle,
    LookAtTarget
}

public enum BulletFire
{
    Single
}

static class BulletMethods
{
    public static void Move(this BulletMove bm, Bullet bullet)
    {
        switch (bm)
        {
            case BulletMove.Linear:
                LinearMove(bullet);
                break;
            case BulletMove.None:
                break;
            default:
                break;
        }
    }

    public static void Rotate(this BulletRotate br, Bullet bullet)
    {
        switch (br)
        {
            case BulletRotate.Angle:
                AngleRotate(bullet);
                break;
            case BulletRotate.LookAtTarget:
                LookAtTarget(bullet);
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

    static void LinearMove(Bullet bullet)
    {
        bullet.Rigid.velocity = bullet.transform.up * bullet.speed;

        Vector3 screenPos = Globals.ClampToScreen(bullet.transform.position);

        if ((bullet.transform.position - screenPos).magnitude > bullet.transform.localScale.magnitude)
            GameObject.Destroy(bullet.gameObject);
    }

    static Bullet[] SingleFire(GameObject prefab, GameObject source)
    {
        Bullet[] bullets = new Bullet[1];

        bullets[0] = GameObject.Instantiate(prefab, source.transform.position, source.transform.rotation).GetComponent<Bullet>();

        return bullets;
    }

    static void AngleRotate(Bullet bullet)
    {
        bullet.transform.rotation = Quaternion.Euler(
            bullet.transform.rotation.eulerAngles.x,
            bullet.transform.rotation.eulerAngles.y,
            bullet.angle);
    }

    static void LookAtTarget(Bullet bullet)
    {
        Vector3 dir = bullet.target.transform.position - bullet.transform.position;
        bullet.angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        AngleRotate(bullet);
    }
}