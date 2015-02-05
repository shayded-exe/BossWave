﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PopupMessageInstance : MonoBehaviour 
{
	public float time = 1f;
	public float distance = 1f;

	[HideInInspector]
	public bool followPlayer = false;

	private Vector3 startingPosition;
	private float yOffset = 0f;

	private CanvasGroup canvasGroup;

	private void Awake()
	{
		startingPosition = transform.position;

		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		Appear();
	}

	private void FixedUpdate()
	{
		if (followPlayer)
		{
			transform.position = PlayerControl.Instance.PopupMessagePoint + new Vector3(0f, yOffset, 0f);
		}

		else
		{
			transform.position = startingPosition + new Vector3(0f, yOffset, 0f);
		}
	}

	public void Appear()
	{
		canvasGroup.alpha = 0f;
		yOffset = 0f;

		canvasGroup.DOFade(1f, time * 0.15f)
			.SetEase(Ease.InQuad);
		DOTween.To(() => yOffset, x => yOffset = x, distance, time)
			.SetEase(Ease.OutQuad);
		canvasGroup.DOFade(0f, time * 0.25f)
			.SetEase(Ease.InQuad)
			.SetDelay(time * 0.75f);

		Destroy(gameObject, time);
	}
}
