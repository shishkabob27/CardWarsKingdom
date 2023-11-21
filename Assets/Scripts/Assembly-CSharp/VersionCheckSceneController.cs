using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class VersionCheckSceneController : MonoBehaviour
{
	public UITweenController showRetryPopup;

	public UITweenController hideRetryPopup;

	public UILabel messageLabel;

	public GameObject retryButton;

	public GameObject closeButton;

	public GameObject updateButton;

	public GameObject appCloseButton;

	public UITexture updateButtonIcon;

	public GameObject Background;

	public UITexture LoadingLogo;

	private string updateURL;

	private bool clickable;

	private string iconURL;

	private void Start()
	{
		Background.SetActive(true);
		LoadingScreenController.LoadLoadingScreenLogo(LoadingLogo);
		CheckClientVersion();
	}

	private void CheckClientVersion()
	{
		KFFNetwork.deserializeJSONCallback = DeserializeJSON;
		string url = SQSettings.SERVER_URL + "static/version.txt";
		KFFNetwork.GetInstance().SendWWWRequestWithForm(null, url, checkClientVersionCallback, null, true);
	}

	private object DeserializeJSON(string json)
	{
		return Json.Deserialize(json);
	}

	private void checkClientVersionCallback(KFFNetwork.WWWInfo wwwinfo, object resultObj, string err, object param)
	{
		string text = null;
		if (!string.IsNullOrEmpty(err))
		{
			text = KFFLocalization.Get("!!ERROR_REQUIRESINTERNETCONNECTION");
		}
		else if (wwwinfo == null || wwwinfo.www == null)
		{
			text = KFFLocalization.Get("!!GAME_ERROR_CONTACTING");
		}
		else if (!string.IsNullOrEmpty(wwwinfo.www.error))
		{
			text = KFFLocalization.Get("!!ERROR_REQUIRESINTERNETCONNECTION");
		}
		else if (string.IsNullOrEmpty(wwwinfo.www.text))
		{
			text = KFFLocalization.Get("!!GAME_ERROR_CONTACTING");
		}
		else
		{
			string text2 = wwwinfo.www.text;
			object obj = DeserializeJSON(text2);
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				text = KFFLocalization.Get("!!GAME_ERROR_CONTACTING");
			}
			else
			{
				string text3 = null;
				string text4 = null;
				string text5 = TFUtils.TryLoadString(dictionary, "maintenance_mode");
				string msg = TFUtils.TryLoadString(dictionary, "message");
				string icon = TFUtils.TryLoadString(dictionary, "icon");
				bool canClick = TFUtils.TryLoadString(dictionary, "clickable") == "yes";
				if (text5 == "yes")
				{
					ShowUpdateMessage(msg, canClick, null, icon);
					return;
				}
				if (Application.platform == RuntimePlatform.Android)
				{
					text3 = TFUtils.TryLoadString(dictionary, "android_version");
					text4 = TFUtils.TryLoadString(dictionary, "android_url");
				}
				if (text3 == null)
				{
					text3 = TFUtils.TryLoadString(dictionary, "version");
				}
				if (text3 != null)
				{
					Version version = new Version(text3);
					if (version > new Version(Application.version))
					{
						msg = KFFLocalization.Get("!!GAME_OUTDATED");
						ShowUpdateMessage(msg, canClick, text4, icon);
						return;
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			ShowRetryPopup(text);
		}
		else
		{
			DetachedSingleton<SceneFlowManager>.Instance.LoadAssetBundleSceneDirect();
		}
	}

	private void ShowUpdateMessage(string msg, bool canClick, string url, string icon)
	{
		updateURL = url;
		clickable = canClick;
		iconURL = icon;
		if (icon != null && updateButtonIcon != null)
		{
			StartCoroutine(DownloadIcon());
		}
		if (messageLabel != null)
		{
			messageLabel.text = msg;
		}
		if (updateButton != null)
		{
			updateButton.SetActive(true);
			if (!clickable)
			{
				if ((bool)updateButton.GetComponent<BoxCollider>())
				{
					updateButton.GetComponent<BoxCollider>().enabled = false;
				}
			}
			else if ((bool)updateButton.GetComponent<BoxCollider>())
			{
				updateButton.GetComponent<BoxCollider>().enabled = true;
			}
		}
		if (retryButton != null)
		{
			retryButton.SetActive(false);
		}
		if (closeButton != null)
		{
			closeButton.SetActive(false);
		}
		if (showRetryPopup != null)
		{
			showRetryPopup.Play();
		}
	}

	private void ShowRetryPopup(string msg)
	{
		if (messageLabel != null)
		{
			messageLabel.text = msg;
		}
		if (updateButton != null)
		{
			updateButton.SetActive(false);
		}
		if (retryButton != null)
		{
			retryButton.SetActive(true);
		}
		if (closeButton != null)
		{
			closeButton.SetActive(false);
		}
		if (appCloseButton != null)
		{
			appCloseButton.SetActive(false);
		}
		if (showRetryPopup != null)
		{
			showRetryPopup.Play();
		}
	}

	public void RetryClicked()
	{
		if (hideRetryPopup != null)
		{
			hideRetryPopup.PlayWithCallback(CheckClientVersion);
		}
		else
		{
			CheckClientVersion();
		}
	}

	public void UpdateClicked()
	{
		if (!string.IsNullOrEmpty(updateURL))
		{
			Application.OpenURL(updateURL);
		}
	}

	private void ShowAppSignErrMessage(string msg)
	{
		if (messageLabel != null)
		{
			messageLabel.text = msg;
		}
		if (updateButton != null)
		{
			updateButton.SetActive(false);
		}
		if (retryButton != null)
		{
			retryButton.SetActive(false);
		}
		if (closeButton != null)
		{
			closeButton.SetActive(false);
		}
		if (appCloseButton != null)
		{
			appCloseButton.SetActive(true);
		}
		if (showRetryPopup != null)
		{
			showRetryPopup.Play();
		}
	}

	public void AppCloseClicked()
	{
		Application.Quit();
	}

	public IEnumerator DownloadIcon()
	{
		WWW www = new WWW(iconURL);
		yield return www;
		updateButtonIcon.mainTexture = www.texture;
	}

	private void OnDestroy()
	{
		LoadingLogo.UnloadTexture();
	}
}
