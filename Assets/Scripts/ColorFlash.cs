using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFlash : MonoBehaviour {
	
	public Color Color;
	public float Speed;
	public float Duration;
	public Material FlashMaterial;

	private Material _oldMaterial;
	private SpriteRenderer _renderer;
	private IEnumerator _flashCoroutine;
	private bool _isFlashing;

	private float _flashTimer = 0.0f, _lastFlashTime = 0.0f;

	private void Start()
	{
		_isFlashing = false;
		_renderer = gameObject.GetComponent<SpriteRenderer>();
		_oldMaterial = _renderer.material;
	}

	private void Update()
	{
		if (_isFlashing )
		{
			if (_flashTimer >= Duration)
			{
				_isFlashing = false;
				_renderer.material = _oldMaterial;
				_flashTimer = 0.0f;
				_lastFlashTime = 0.0f;
			}
			else
			{
				if (_flashTimer - _lastFlashTime >= 1 / Speed)
				{
					FlipMaterial();
					_lastFlashTime = _flashTimer;
				}
				_flashTimer += Time.deltaTime;
			}
		}
	}

	private void FlipMaterial()
	{
		_renderer.material = (_renderer.material == _oldMaterial ? FlashMaterial : _oldMaterial);
	}

	public void Flash()
	{
		if (!_isFlashing)
		{
			FlashMaterial.SetColor("_Color", Color);
			FlipMaterial();
			_isFlashing = true;
		}
	}
	
}
