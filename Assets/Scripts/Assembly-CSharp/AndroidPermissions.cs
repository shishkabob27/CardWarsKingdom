using System;
using System.Collections;
using UnityEngine;

public class AndroidPermissions : MonoBehaviour
{
	public GameObject initialWindow;

	public GameObject tryAgainWindow;

	public GameObject finalWindow;

	public UILabel InitHeaderTxt;

	public UILabel InitBodyTxt;

	public UILabel InitOkTxt;

	public UILabel TryAgainHeaderTxt;

	public UILabel TryAgainBodyTxt;

	public UILabel RetryTxt;

	public UILabel FinalRequestHeaderTxt;

	public UILabel FinalRequestBodyTxt;

	public UILabel FinalRequestFooterTxt;

	public UILabel FinalRequestTypeTxt;

	public UILabel FinalRequestOkTxt;

	private static string ANDROIDPERMISSION_REQUEST_CONTEXT_EN = "To Install and Play Card Wars Kingdom we will need to access the following:";

	private static string ANDROIDPERMISSION_REQUEST_EN = "Photos/Media/Files";

	private static string ANDROIDPERMISSION_REQUEST_DENIED_EN = "Card Wars Kingdom can not be installed until you allow access to the following:";

	private static string RETRY_EN = "RETRY";

	private static string ANDROIDPERMISSION_DONOTASKAGAIN_HEADER_EN = "To play Card Wars Kingdom go to";

	private static string ANDROIDPERMISSION_DONOTASKAGAIN_LOCATION_EN = "Settings - Apps - Card Wars Kingdom - Permissions";

	private static string ANDROIDPERMISSION_DONOTASKAGAIN_FOOTER_EN = "and allow access to the following:";

	private static string ANDROIDPERMISSION_DONOTASKAGAIN_TYPE_EN = "Storage";

	private static string OK_EN = "OK";

	private bool waitingForResponse;

	private bool permissionGranted;

	private bool shouldShowReasoning = true;

	private void Awake()
	{
		SetupLabels();
		initialWindow.SetActive(false);
		tryAgainWindow.SetActive(false);
		finalWindow.SetActive(false);
	}

	public void SetupLabels()
	{
		string text = KFFLocalization.Get("!!ANDROIDPERMISSION_REQUEST_CONTEXT");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_REQUEST_CONTEXT_EN;
		}
		InitHeaderTxt.text = text;
		text = KFFLocalization.Get("!!ANDROIDPERMISSION_REQUEST");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_REQUEST_EN;
		}
		InitBodyTxt.text = text;
		TryAgainBodyTxt.text = text;
		text = KFFLocalization.Get("!!OK");
		if (text.StartsWith("!!"))
		{
			text = OK_EN;
		}
		InitOkTxt.text = text;
		FinalRequestOkTxt.text = text;
		text = KFFLocalization.Get("!!ANDROIDPERMISSION_REQUEST_DENIED");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_REQUEST_DENIED_EN;
		}
		TryAgainHeaderTxt.text = text;
		text = KFFLocalization.Get("!!RETRY");
		if (text.StartsWith("!!"))
		{
			text = RETRY_EN;
		}
		RetryTxt.text = text;
		text = KFFLocalization.Get("!!ANDROIDPERMISSION_DONOTASKAGAIN_HEADER");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_DONOTASKAGAIN_HEADER_EN;
		}
		FinalRequestHeaderTxt.text = text;
		text = KFFLocalization.Get("!!ANDROIDPERMISSION_DONOTASKAGAIN_LOCATION");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_DONOTASKAGAIN_LOCATION_EN;
		}
		FinalRequestBodyTxt.text = text;
		text = KFFLocalization.Get("!!ANDROIDPERMISSION_DONOTASKAGAIN_FOOTER");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_DONOTASKAGAIN_FOOTER_EN;
		}
		FinalRequestFooterTxt.text = text;
		text = KFFLocalization.Get("!!ANDROIDPERMISSION_DONOTASKAGAIN_TYPE");
		if (text.StartsWith("!!"))
		{
			text = ANDROIDPERMISSION_DONOTASKAGAIN_TYPE_EN;
		}
		FinalRequestTypeTxt.text = text;
	}

	public IEnumerator StartAndroidPermissionsRoutine()
	{
		yield return null;
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidPermissionGranter.Init();
			if (AndroidPermissionGranter.instance != null)
			{
				AndroidPermissionGranter instance = AndroidPermissionGranter.instance;
				instance.PermissionCheckCallback = (Action<bool>)Delegate.Combine(instance.PermissionCheckCallback, new Action<bool>(HandlePermissionsCheckResponse));
				AndroidPermissionGranter instance2 = AndroidPermissionGranter.instance;
				instance2.PermissionRequestCallback = (Action<bool>)Delegate.Combine(instance2.PermissionRequestCallback, new Action<bool>(HandlePermissionsResponse));
				AndroidPermissionGranter instance3 = AndroidPermissionGranter.instance;
				instance3.ShouldShowPermissionRationalCallback = (Action<bool>)Delegate.Combine(instance3.ShouldShowPermissionRationalCallback, new Action<bool>(HandleShouldShowPermissionReasoningResponse));
				waitingForResponse = true;
				permissionGranted = false;
				AndroidPermissionGranter.CheckPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE);
				while (waitingForResponse)
				{
					yield return null;
				}
				if (permissionGranted)
				{
					PlayerPrefs.SetInt("REQUESTED_WRITE_EXTERNAL_STORAGE", 1);
					AndroidPermissionGranter instance4 = AndroidPermissionGranter.instance;
					instance4.PermissionCheckCallback = (Action<bool>)Delegate.Remove(instance4.PermissionCheckCallback, new Action<bool>(HandlePermissionsCheckResponse));
					AndroidPermissionGranter instance5 = AndroidPermissionGranter.instance;
					instance5.PermissionRequestCallback = (Action<bool>)Delegate.Remove(instance5.PermissionRequestCallback, new Action<bool>(HandlePermissionsResponse));
					AndroidPermissionGranter instance6 = AndroidPermissionGranter.instance;
					instance6.ShouldShowPermissionRationalCallback = (Action<bool>)Delegate.Remove(instance6.ShouldShowPermissionRationalCallback, new Action<bool>(HandleShouldShowPermissionReasoningResponse));
					LoadNextScene();
					yield break;
				}
				yield return new WaitForEndOfFrame();
				bool isFirstTime = ((PlayerPrefs.GetInt("REQUESTED_WRITE_EXTERNAL_STORAGE") == 0) ? true : false);
				AndroidPermissionGranter.ShouldShowRequestPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE, "Install the game");
				shouldShowReasoning = true;
				waitingForResponse = true;
				while (waitingForResponse)
				{
					yield return null;
				}
				if (!isFirstTime)
				{
					ShowBreadCrumbWindow();
					bool stayInLoop4 = true;
					while (stayInLoop4)
					{
						waitingForResponse = true;
						AndroidPermissionGranter.CheckPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE);
						while (waitingForResponse)
						{
							yield return null;
						}
						if (permissionGranted)
						{
							finalWindow.SetActive(false);
							stayInLoop4 = false;
						}
						yield return new WaitForSeconds(1f);
					}
					LoadNextScene();
					yield break;
				}
				if (isFirstTime)
				{
					bool stayInLoop3 = true;
					initialWindow.SetActive(true);
					while (stayInLoop3)
					{
						if (!initialWindow.activeSelf)
						{
							stayInLoop3 = false;
						}
						yield return null;
					}
					AndroidPermissionGranter.GrantPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE);
					waitingForResponse = true;
					permissionGranted = false;
					while (waitingForResponse)
					{
						yield return null;
					}
					PlayerPrefs.SetInt("REQUESTED_WRITE_EXTERNAL_STORAGE", 1);
				}
				while (!permissionGranted)
				{
					tryAgainWindow.SetActive(true);
					bool stayInLoop2 = true;
					while (stayInLoop2)
					{
						if (!tryAgainWindow.activeSelf)
						{
							stayInLoop2 = false;
						}
						yield return null;
					}
					waitingForResponse = true;
					permissionGranted = false;
					AndroidPermissionGranter.GrantPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE);
					while (waitingForResponse)
					{
						yield return null;
					}
					if (!permissionGranted)
					{
						AndroidPermissionGranter.ShouldShowRequestPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE, "Install the game");
						shouldShowReasoning = true;
						waitingForResponse = true;
						while (waitingForResponse)
						{
							yield return null;
						}
						if (!shouldShowReasoning)
						{
							ShowBreadCrumbWindow();
							stayInLoop2 = true;
							while (stayInLoop2)
							{
								waitingForResponse = true;
								AndroidPermissionGranter.CheckPermission(AndroidPermissionGranter.AndroidPermission.WRITE_EXTERNAL_STORAGE);
								while (waitingForResponse)
								{
									yield return null;
								}
								if (permissionGranted)
								{
									finalWindow.SetActive(false);
									stayInLoop2 = false;
								}
								yield return new WaitForSeconds(1f);
							}
						}
					}
					yield return null;
				}
				AndroidPermissionGranter instance7 = AndroidPermissionGranter.instance;
				instance7.PermissionCheckCallback = (Action<bool>)Delegate.Remove(instance7.PermissionCheckCallback, new Action<bool>(HandlePermissionsCheckResponse));
				AndroidPermissionGranter instance8 = AndroidPermissionGranter.instance;
				instance8.PermissionRequestCallback = (Action<bool>)Delegate.Remove(instance8.PermissionRequestCallback, new Action<bool>(HandlePermissionsResponse));
				AndroidPermissionGranter instance9 = AndroidPermissionGranter.instance;
				instance9.ShouldShowPermissionRationalCallback = (Action<bool>)Delegate.Remove(instance9.ShouldShowPermissionRationalCallback, new Action<bool>(HandleShouldShowPermissionReasoningResponse));
				yield return new WaitForSeconds(0.1f);
			}
		}
		LoadNextScene();
	}

	private void LoadNextScene()
	{
		if (KFFLODManager.IsLowEndDevice() || !MoviePlayer.NeedToPlayMovies)
		{
			DetachedSingleton<SceneFlowManager>.Instance.LoadVersionCheckSceneDirect();
		}
		else
		{
			DetachedSingleton<SceneFlowManager>.Instance.LoadBootVideoSceneDirect();
		}
	}

	private void ShowBreadCrumbWindow()
	{
		finalWindow.SetActive(true);
	}

	private void HandlePermissionsCheckResponse(bool granted)
	{
		waitingForResponse = false;
		permissionGranted = granted;
	}

	private void HandlePermissionsResponse(bool granted)
	{
		waitingForResponse = false;
		permissionGranted = granted;
	}

	private void HandleShouldShowPermissionReasoningResponse(bool shouldShow)
	{
		waitingForResponse = false;
		shouldShowReasoning = shouldShow;
	}

	public void OnClickedInitialGotIt()
	{
		initialWindow.SetActive(false);
	}

	public void OnClickedTryAgain()
	{
		tryAgainWindow.SetActive(false);
	}

	public void OnClickAppSetting()
	{
		KFFAndroidPlugin.OpenAppSettingDialog();
	}
}
