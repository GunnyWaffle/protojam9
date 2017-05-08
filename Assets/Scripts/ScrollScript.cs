using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScript : MonoBehaviour {

    public float speed = 0.1f;
    public List<Texture> worlds;

    Renderer rend;
    int currentWorld = 0;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update () {
		rend.material.mainTextureOffset = new Vector2(0f, -(Time.time * speed) % 1);
    }

    public void ChangeWorlds()
    {
        currentWorld = (currentWorld + 1) % worlds.Count;
        rend.material.SetTexture("_MainTex", worlds[currentWorld]);
    }
}
