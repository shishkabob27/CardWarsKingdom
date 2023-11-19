using System;
using System.Collections.Generic;
using UnityEngine;
using UpsightMiniJSON;

public class UpsightManager : MonoBehaviour
{
	private static readonly string GameObjectName = "UpsightManager";

	private static bool initialized;

	private bool _destroyed;

	public static event Action<string> onBillboardAppearEvent;

	public static event Action<string> onBillboardDismissEvent;

	public static event Action<UpsightReward> billboardDidReceiveRewardEvent;

	public static event Action<UpsightPurchase> billboardDidReceivePurchaseEvent;

	public static event Action<List<string>> managedVariablesDidSynchronizeEvent;

	public static void init()
	{
		if (!initialized)
		{
			initialized = true;
			GameObject gameObject = GameObject.Find(GameObjectName) ?? new GameObject(GameObjectName);
			UpsightManager upsightManager = gameObject.GetComponent<UpsightManager>() ?? gameObject.AddComponent<UpsightManager>();
			if (upsightManager == null)
			{
			}
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			Upsight.init();
		}
	}

	private void Awake()
	{
		if (_destroyed)
		{
			return;
		}
		UpsightManager[] array = UnityEngine.Object.FindObjectsOfType<UpsightManager>();
		bool flag = false;
		if (array.Length > 1)
		{
			UpsightManager[] array2 = array;
			foreach (UpsightManager upsightManager in array2)
			{
				if (upsightManager.gameObject.name == GameObjectName && !flag)
				{
					flag = true;
				}
				else if (!upsightManager._destroyed)
				{
					upsightManager._destroyed = true;
					UnityEngine.Object.Destroy(upsightManager);
				}
			}
		}
		if (!flag)
		{
			init();
		}
		else
		{
			initialized = true;
		}
	}

	private void onBillboardAppear(string scope)
	{
		if (UpsightManager.onBillboardAppearEvent != null)
		{
			UpsightManager.onBillboardAppearEvent(scope);
		}
	}

	private void onBillboardDismiss(string scope)
	{
		if (UpsightManager.onBillboardDismissEvent != null)
		{
			UpsightManager.onBillboardDismissEvent(scope);
		}
	}

	private void billboardDidReceiveReward(string json)
	{
		if (UpsightManager.billboardDidReceiveRewardEvent != null)
		{
			UpsightManager.billboardDidReceiveRewardEvent(UpsightReward.rewardFromJson(json));
		}
	}

	private void billboardDidReceivePurchase(string json)
	{
		if (UpsightManager.billboardDidReceivePurchaseEvent != null)
		{
			UpsightManager.billboardDidReceivePurchaseEvent(UpsightPurchase.purchaseFromJson(json));
		}
	}

	private void managedVariablesDidSynchronize(string json)
	{
		if (UpsightManager.managedVariablesDidSynchronizeEvent == null)
		{
			return;
		}
		List<string> list = null;
		if (json != null && 0 < json.Length)
		{
			List<object> list2 = Json.Deserialize(json) as List<object>;
			list = new List<string>();
			foreach (object item in list2)
			{
				list.Add((string)item);
			}
		}
		UpsightManager.managedVariablesDidSynchronizeEvent(list);
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Upsight.onPause();
		}
		else
		{
			Upsight.onResume();
		}
	}
}
