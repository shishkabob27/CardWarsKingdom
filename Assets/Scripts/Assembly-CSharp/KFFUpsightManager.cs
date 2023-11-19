using System.Collections.Generic;
using System.Text;

public class KFFUpsightManager : Singleton<KFFUpsightManager>
{
	public static string androidAppToken;

	public static string androidAppSecret;

	public static string gcmProjectNumber;

	public static string iosAppToken;

	public static string iosAppSecret;

	private static int _moreGamesBadgeCount = -1;

	private static ContentRequester[] _Requesters;

	private static bool mDisplayOnce;

	private void Start()
	{
		Upsight.init();
		mDisplayOnce = true;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (mDisplayOnce)
		{
		}
	}

	private void OnApplicationQuit()
	{
		int currentStamina;
		int maxStamina;
		int secondsUntilNextStamina;
		DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Quests, out currentStamina, out maxStamina, out secondsUntilNextStamina);
		string upsightEvent = "Session.QuitApp";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("remainingHearts", currentStamina);
		dictionary.Add("timeUntilnextHeart", secondsUntilNextStamina);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
	}

	private void OnEnable()
	{
		if (mDisplayOnce)
		{
			UpsightManager.onBillboardAppearEvent += onBillboardAppearEvent;
			UpsightManager.onBillboardDismissEvent += onBillboardDismissEvent;
			UpsightManager.billboardDidReceiveRewardEvent += billboardDidReceiveRewardEvent;
			UpsightManager.billboardDidReceivePurchaseEvent += billboardDidReceivePurchaseEvent;
			UpsightManager.managedVariablesDidSynchronizeEvent += managedVariablesDidSynchronizeEvent;
		}
	}

	private void OnDisable()
	{
		if (mDisplayOnce)
		{
			UpsightManager.onBillboardAppearEvent -= onBillboardAppearEvent;
			UpsightManager.onBillboardDismissEvent -= onBillboardDismissEvent;
			UpsightManager.billboardDidReceiveRewardEvent -= billboardDidReceiveRewardEvent;
			UpsightManager.billboardDidReceivePurchaseEvent -= billboardDidReceivePurchaseEvent;
			UpsightManager.managedVariablesDidSynchronizeEvent -= managedVariablesDidSynchronizeEvent;
		}
	}

	private void onBillboardAppearEvent(string scope)
	{
	}

	private void onBillboardDismissEvent(string scope)
	{
	}

	private void billboardDidReceivePurchaseEvent(UpsightPurchase purchase)
	{
	}

	private void billboardDidReceiveRewardEvent(UpsightReward reward)
	{
	}

	private void managedVariablesDidSynchronizeEvent(List<string> tags)
	{
		if (tags == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string tag in tags)
		{
			stringBuilder.Append("\n").Append(tag);
		}
	}
}
