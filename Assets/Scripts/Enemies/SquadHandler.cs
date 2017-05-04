using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        try
        {
            transform.GetChild(0);
        }
        catch (UnityException e)
        {
            DestroySquad();
        }

	}

    public void DestroySquad()
    {
        EnemyFormationSpawner.instance.KilledSquad();
        Destroy(gameObject);
    }
}
