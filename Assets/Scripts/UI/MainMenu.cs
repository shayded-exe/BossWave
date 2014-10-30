﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour 
{
	public EventSystem eventSystem;

	public float startDelay = 1f;
	public float fadeTime = 2f;
	public float nodeMoveSpeed = 2f;

	public Image logo;

	private Slider volumeSlider;

	#if !MOBILE_INPUT
	private Toggle fullscreenToggle;
	private ResolutionSelector resolutionSelector;
	#endif

	private CanvasGroup menu;
	private RectTransform rectTransform;

	void Awake()
	{
		volumeSlider = transform.FindSubChild("Volume").GetComponent<Slider>();

		#if !MOBILE_INPUT
		fullscreenToggle = transform.FindSubChild("Fullscreen").GetComponent<Toggle>();
		resolutionSelector = transform.FindSubChild("Resolution").GetComponent<ResolutionSelector>();
		#endif

		menu = GetComponent<CanvasGroup>();
		rectTransform = GetComponent<RectTransform>();

		LoadPrefs();
	}

	void Start()
	{
		Time.timeScale = 1f;

		StartCoroutine(ShowMenu());
	}

	public void GoToNode(string node)
	{
		iTween.ValueTo(gameObject, iTween.Hash("from", rectTransform.anchoredPosition.x,
											   "to", -transform.FindChild(node).GetComponent<RectTransform>().anchoredPosition.x,
											   "time", nodeMoveSpeed,
											   "easetype", iTween.EaseType.easeOutQuint,
											   "onupdate", "GoToUpdateX",
											   "ignoretimescale", true));

		iTween.ValueTo(gameObject, iTween.Hash("from", rectTransform.anchoredPosition.y,
											   "to", -transform.FindChild(node).GetComponent<RectTransform>().anchoredPosition.y,
											   "time", nodeMoveSpeed,
											   "easetype", iTween.EaseType.easeOutQuint,
											   "onupdate", "GoToUpdateY",
											   "ignoretimescale", true));
	}

	public void SelectObject(GameObject gameObject)
	{
		eventSystem.SetSelectedGameObject(gameObject, new BaseEventData(eventSystem));
	}

	public void LoadLevel(string levelName)
	{
		StartCoroutine(HideMenu(levelName));
	}

	public void SetVolume(float newVolume)
	{
		newVolume = Mathf.Abs(newVolume);

		PlayerPrefs.SetFloat("Settings/Volume", newVolume);
		AudioListener.volume = newVolume;
	}

	public void ApplySettings()
	{
		#if !MOBILE_INPUT
		resolutionSelector.SetResolution();
		PlayerPrefs.SetInt("Settings/Fullscreen", fullscreenToggle.isOn ? 1 : 0);
		Screen.fullScreen = fullscreenToggle.isOn;
		#endif
	}

	public void ResetPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	private void LoadPrefs()
	{
		if (PlayerPrefs.HasKey("Settings/Volume"))
		{
			AudioListener.volume = PlayerPrefs.GetFloat("Settings/Volume");
			volumeSlider.value = -AudioListener.volume;
		}

		#if !MOBILE_INPUT
		if (PlayerPrefs.HasKey("Settings/Fullscreen"))
		{
			Screen.fullScreen = PlayerPrefs.GetInt("Settings/Fullscreen") == 1 ? true : false;
			fullscreenToggle.isOn = Screen.fullScreen;
		}
		#endif
	}

	private IEnumerator ShowMenu()
	{
		yield return new WaitForSeconds(startDelay);

		logo.GetComponent<Animator>().SetTrigger("Start");
		CRTEffect.StartCRT(fadeTime);
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f,
											   "to", 1f,
											   "time", fadeTime,
											   "easetype", iTween.EaseType.easeOutQuint,
											   "onupdate", "UpdateMenuAlpha",
											   "ignoretimescale", true));
	}

	private IEnumerator HideMenu(string levelName = "none")
	{
		if (levelName != "none" && levelName != "Exit")
		{
			CRTEffect.AnimateScanlines(fadeTime, 0f, iTween.EaseType.easeOutSine);

			AsyncOperation async = Application.LoadLevelAsync(levelName);
			async.allowSceneActivation = false;

			yield return new WaitForSeconds(fadeTime);

			async.allowSceneActivation = true;
		}
		else if (levelName == "Exit")
		{
			Application.Quit();
		}
	}

	private void UpdateMenuAlpha(float newValue)
	{
		menu.alpha = newValue;
	}

	private void GoToUpdateX(float newValue)
	{
		rectTransform.anchoredPosition = new Vector2(newValue, rectTransform.anchoredPosition.y);
	}

	private void GoToUpdateY(float newValue)
	{
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newValue);
	}
}