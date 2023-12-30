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
	}
}
