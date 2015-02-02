﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DG.Tweening;

public class GhostTrailEffect : MonoBehaviour 
{
	public bool trailActive = false;
	public float ghostSpawnTime = 0.1f;
	public float ghostLifetime = 0.25f;
	public AnimationCurve fadeCurve;

	private float ghostSpawnTimer = 0f;
	private List<SpriteRenderer> spriteRenderers;

	private ReadOnlyCollection<SpriteRenderer> SpriteRenderers
	{
		get
		{
			if (gameObject.tag == "Player")
			{
				return PlayerControl.Instance.SpriteRenderers;
			}
			else
			{
				return spriteRenderers.AsReadOnly();
			}
		}
	}

	private void Awake()
	{
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
	}

	private void FixedUpdate()
	{
		if (trailActive)
		{
			ghostSpawnTimer += Time.deltaTime;

			if (ghostSpawnTimer >= ghostSpawnTime)
			{
				GameObject currentParent = new GameObject();
				currentParent.HideInHiearchy();
				transform.CopyTo(currentParent.transform);
				currentParent.name = gameObject.name + " Ghost";

				foreach (SpriteRenderer spriteRenderer in SpriteRenderers)
				{
					GameObject currentGhost = new GameObject();
					spriteRenderer.transform.CopyTo(currentGhost.transform);
					currentGhost.name = spriteRenderer.name + " Ghost";
					currentGhost.transform.parent = currentParent.transform;

					SpriteRenderer ghostSpriteRenderer = currentGhost.AddComponent<SpriteRenderer>();
					ghostSpriteRenderer.sprite = spriteRenderer.sprite;
					ghostSpriteRenderer.color = spriteRenderer.color;
					ghostSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
					ghostSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

					ghostSpriteRenderer.DOFade(0f, ghostLifetime)
						.SetEase(fadeCurve)
						.OnUpdate(() =>
						{
							if (this == null)
							{
								ghostSpriteRenderer.DOKill(true);
							}
						})
						.OnComplete(() =>
						{
							if (currentParent != null)
							{
								Destroy(currentParent);
							}
						});
				}

				ghostSpawnTimer = 0f;
			}
		}
	}
}