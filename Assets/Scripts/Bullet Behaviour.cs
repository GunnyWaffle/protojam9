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
            default:
                break;
        }
    }

    public static void Rotate(this BulletRotate br, GameObject go)
    {
        switch (br)
        {
            case BulletRotate.None:
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
}