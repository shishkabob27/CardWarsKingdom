using System.Collections.Generic;
using Helpshift;

public class HelpShiftManager : Singleton<HelpShiftManager>
{
	public string apiKey;

	public string Domain;

	public string iOS_AppID;

	public string Android_AppID;

	public string userID;

	private HelpshiftSdk HelpShiftSDKInstance;

	private void Start()
	{
		HelpShiftSDKInstance = HelpshiftSdk.getInstance();
		HelpShiftSDKInstance.install(apiKey, Domain, Android_AppID);
	}

	private void OnDisable()
	{
	}

	public void ShowFAQ()
	{
		HelpShiftSDKInstance.showFAQs();
	}

	public void ShowConversation(Dictionary<string, object> configMap)
	{
		userID = Singleton<PlayerInfoScript>.Instance.GetPlayerCode();
		HelpShiftSDKInstance.setUserIdentifier(userID);
		HelpShiftSDKInstance.showConversation(configMap);
		TFUtils.DebugLog("The HelpShiftUserID:" + userID);
	}

	public void PopupRateThisApp()
	{
		if (Singleton<PurchaseManager>.Instance.IsAmazon)
		{
			HelpShiftSDKInstance.showAlertToRateAppWithURL("amzn://apps/android?p=com.turner.cardwars2");
		}
		else
		{
			HelpShiftSDKInstance.showAlertToRateAppWithURL("market://details?id=" + MiscParams.marketStoreURL);
		}
	}

	public void alertToRateAppAction(string message)
	{
		if (!(message == "HS_RATE_ALERT_CLOSE") && !(message == "HS_RATE_ALERT_SUCCESS"))
		{
		}
	}
}
