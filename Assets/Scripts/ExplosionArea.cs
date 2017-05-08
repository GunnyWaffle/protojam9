using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionArea : MonoBehaviour {

    public GameObject[] explosionObjects;
    private Animator[] explosionsAnimators;
        
    private BoxCollider2D explosionSpace;
    private Vector3 interiorExtend;
    private const string defaultAnimationState = "Default";
    private bool stopPlaying = false;

    private void Start()
    {
        explosionSpace = gameObject.GetComponent<BoxCollider2D>();
        interiorExtend = explosionSpace.bounds.extents;
        explosionsAnimators = new Animator[explosionObjects.Length];
        int i = 0;
        foreach (GameObject explosion in explosionObjects)
        {
            explosionsAnimators[i] = explosion.GetComponent<Animator>();
            i++;
        }
        gameObject.SetActive(false);
    }

    public void TurnOnExplosions()
    {
        gameObject.SetActive(true);
    }

    public void TurnOffExplosions()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
       CreateExplosions();
    }

    private void CreateExplosions()
    {
        int i = Random.Range(0, explosionsAnimators.Length);
        Animator explosionAnimator = explosionsAnimators[i];
        GameObject explosionObject = explosionObjects[i];

        if (!explosionAnimator.GetBool("IsDead"))
        {
            float randomXLoc = Random.Range(-interiorExtend.x, interiorExtend.x);
            float randomYLoc = Random.Range(-interiorExtend.y, interiorExtend.y);

            Vector3 newExplosionLoc = new Vector3(gameObject.transform.position.x  + randomXLoc, gameObject.transform.position.y  + randomYLoc, gameObject.transform.position.z);
            explosionObjects[i].transform.position = newExplosionLoc;
            var audio = explosionObject.GetComponent<AudioSource>();
            if (audio != null)
                audio.Play();
            explosionAnimator.SetTrigger("IsDead");
        }
    }
}
