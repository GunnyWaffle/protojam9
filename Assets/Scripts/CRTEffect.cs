using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CRTEffect : MonoBehaviour {

	public Material crtMaterial;

	// Use this for initialization
	void Start () {

	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		crtMaterial.SetInt("_frameCount", Time.frameCount);
		Graphics.Blit(source, destination, crtMaterial);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
