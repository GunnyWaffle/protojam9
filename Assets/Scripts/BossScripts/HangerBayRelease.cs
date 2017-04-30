using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerBayRelease : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "enemy")
        {
            collision.gameObject.GetComponent<Enemy>().LockFlightPattern(false);
        }
    }
}
