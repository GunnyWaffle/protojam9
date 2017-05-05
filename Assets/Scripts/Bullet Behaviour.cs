using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for all behaviours, please keep 'None' as the first option, so that default behaviour is predictable

// available movement behaviours
public enum BulletMove
{
    None, // do not move
    Linear, // move along the forward angular direction linearlly
    LinearGrowth, // move along the forward angular direction quadratically
    SpinMove // move in the direction initially released.
}

// available rotation behaviours
public enum BulletRotate
{
    None, // do not rotate
    Angle, // rotate to the set angle property
    LookAtTarget, // set the angle to the target, then call the Angle method
    Spin // spin in a single direction for the life of a bullet
}

// available firing behaviours
public enum BulletFire
{
    Single // a single shot is fired in the facing direction of the firing object
}

// the everything
// for each enum you add, provide a case in it's appropriate switch statement
// also write a method for implementing the behaviour, and call it in the switch case
// it is good design to keep rotation and movement separate
// IE: if you want to make a projectile go crazy, have a "random spin" and "random move" behaviour
// this way you can mix and match with other behaviours, while keeping your code relevant to it's purpose
static class BulletMethods
{
    // movement behaviours
    public static void Move(this BulletMove bm, Bullet bullet)
    {
        switch (bm)
        {
            case BulletMove.Linear:
                LinearMove(bullet);
                break;
            case BulletMove.LinearGrowth:
                LinearGrowthMove(bullet);
                break;
            case BulletMove.SpinMove:
                SpinMove(bullet);
                break;
            case BulletMove.None:
                break;
            default:
                break;
        }
    }

    // rotation behaviours
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
            case BulletRotate.Spin:
                Spin(bullet);
                break;
            case BulletRotate.None:
                break;
            default:
                break;
        }
    }

    // firing behaviours
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

    /* ~~~~~~~~~~~~~~~~~ movement methods ~~~~~~~~~~~~~~~~~ */

    // move in the forward direction of the bullet
    static void LinearMove(Bullet bullet)
    {
        bullet.Rigid.velocity = bullet.transform.up * bullet.speed;
        Cull(bullet);
    }

    // move in the forward direction of the bullet, but speed grows over time.
    static void LinearGrowthMove(Bullet bullet)
    {
        bullet.timeAlive += Time.deltaTime;
        bullet.Rigid.velocity = bullet.transform.up * bullet.timeAlive * bullet.speed;
    }

    // Move in the direction of original release. Allows the bullet to spin as an effect.
    static void SpinMove(Bullet bullet)
    {
        bullet.Rigid.velocity = bullet.releaseDirection * bullet.speed;
    }

    // always call this at the end of each movement behaviour to destroy the bullet when it leaves the screen
    static void Cull(Bullet bullet)
    {
        Vector3 screenPos = Globals.ClampToScreen(bullet.transform.position);

        if ((bullet.transform.position - screenPos).magnitude > bullet.transform.localScale.magnitude)
            GameObject.Destroy(bullet.gameObject);
    }

    /* ~~~~~~~~~~~~~~~~~ rotation methods ~~~~~~~~~~~~~~~~~ */

    // apply the angle to the bullet
    static void AngleRotate(Bullet bullet)
    {
        bullet.transform.rotation = Quaternion.Euler(
            bullet.transform.rotation.eulerAngles.x,
            bullet.transform.rotation.eulerAngles.y,
            bullet.angle);
    }

    // get the angle to the target, then apply it to the bullet
    static void LookAtTarget(Bullet bullet)
    {
        Vector3 dir = bullet.target.transform.position - bullet.transform.position;
        bullet.angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        AngleRotate(bullet);
    }

    // Spin in a single direction for the life of the bullet
    static void Spin(Bullet bullet)
    {
        bullet.angle = (bullet.angle + (bullet.spinSpeed * Time.deltaTime)) % 360;
        AngleRotate(bullet);
    }

    /* ~~~~~~~~~~~~~~~~~ firing methods ~~~~~~~~~~~~~~~~~ */

    // fire a single bullet, in the facing direction of the source
    static Bullet[] SingleFire(GameObject prefab, GameObject source)
    {
        Bullet[] bullets = new Bullet[1];

        bullets[0] = GameObject.Instantiate(prefab, source.transform.position, source.transform.rotation).GetComponent<Bullet>();

        return bullets;
    }
}
 