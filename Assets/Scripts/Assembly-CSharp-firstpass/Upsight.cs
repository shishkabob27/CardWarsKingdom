using System;
using System.Collections.Generic;
using UnityEngine;
using UpsightMiniJSON;

public class Upsight
{
	private static bool initialized;

	private static AndroidJavaObject _pluginBase;

	private static AndroidJavaObject _pluginPushExtension;

	private static AndroidJavaObject _pluginMarketingExtension;

	static Upsight()
	{
		init();
	}

	public static void init()
	{
		//Discarded unreachable code: IL_007c
		if (Application.platform != RuntimePlatform.Android || initialized)
		{
			return;
		}
		initialized = true;
		UpsightManager.init();
		try
		{
			_pluginBase = new AndroidJavaObject("com.upsight.android.unity.UpsightPlugin");
			if (_pluginBase == null || AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
			{
				AndroidJNI.ExceptionDescribe();
				_pluginBase = null;
				AndroidJNI.ExceptionClear();
				return;
			}
		}
		catch (Exception)
		{
			_pluginBase = null;
			return;
		}
		try
		{
			_pluginMarketingExtension = new AndroidJavaObject("com.upsight.android.unity.UpsightMarketingManager");
			if (_pluginMarketingExtension != null)
			{
				_pluginBase.Call("registerExtension", _pluginMarketingExtension);
			}
		}
		catch
		{
			_pluginMarketingExtension = null;
		}
		try
		{
			_pluginPushExtension = new AndroidJavaObject("com.upsight.android.unity.UpsightPushManager");
			if (_pluginPushExtension != null)
			{
				_pluginBase.Call("registerExtension", _pluginPushExtension);
			}
		}
		catch
		{
			_pluginPushExtension = null;
		}
	}

	public static string getAppToken()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return "UnityEditor-Token";
		}
		if (_pluginBase == null || !initialized)
		{
			return null;
		}
		return _pluginBase.Call<string>("getAppToken", new object[0]);
	}

	public static string getPublicKey()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return "UnityEditor-Key";
		}
		if (_pluginBase == null || !initialized)
		{
			return null;
		}
		return _pluginBase.Call<string>("getPublicKey", new object[0]);
	}

	public static string getSid()
	{
		if (!initSuccessful())
		{
			return null;
		}
		return _pluginBase.Call<string>("getSid", new object[0]);
	}

	public static void setLoggerLevel(UpsightLoggerLevel loggerLevel)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setLoggerLevel", loggerLevel.ToString().ToUpper());
		}
	}

	public static string getPluginVersion()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return "UnityEditor";
		}
		if (_pluginBase == null || !initialized)
		{
			return null;
		}
		return _pluginBase.Call<string>("getPluginVersion", new object[0]);
	}

	public static bool getOptOutStatus()
	{
		if (!initSuccessful())
		{
			return false;
		}
		return _pluginBase.Call<bool>("getOptOutStatus", new object[0]);
	}

	public static void setOptOutStatus(bool optOutStatus)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setOptOutStatus", optOutStatus);
		}
	}

	public static void setUserAttributeString(string key, string value)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setUserAttributesString", key, value);
		}
	}

	public static void setUserAttributeFloat(string key, float value)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setUserAttributesFloat", key, value);
		}
	}

	public static void setUserAttributeInt(string key, int value)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setUserAttributesInt", key, value);
		}
	}

	public static void setUserAttributeBool(string key, bool value)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setUserAttributesBool", key, value);
		}
	}

	public static void setUserAttributeDate(string key, DateTime value)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setUserAttributesDatetime", key, (long)value.ToUnixTimestamp());
		}
	}

	public static string getUserAttributeString(string key)
	{
		if (!initSuccessful())
		{
			return null;
		}
		return _pluginBase.Call<string>("getUserAttributesString", new object[1] { key });
	}

	public static float getUserAttributeFloat(string key)
	{
		if (!initSuccessful())
		{
			return 0f;
		}
		return _pluginBase.Call<float>("getUserAttributesFloat", new object[1] { key });
	}

	public static int getUserAttributeInt(string key)
	{
		if (!initSuccessful())
		{
			return 0;
		}
		return _pluginBase.Call<int>("getUserAttributesInt", new object[1] { key });
	}

	public static bool getUserAttributeBool(string key)
	{
		if (!initSuccessful())
		{
			return false;
		}
		return _pluginBase.Call<bool>("getUserAttributesBool", new object[1] { key });
	}

	public static DateTime getUserAttributeDate(string key)
	{
		if (!initSuccessful())
		{
			return default(DateTime);
		}
		return _pluginBase.Call<long>("getUserAttributesDatetime", new object[1] { key }).ToDateTime();
	}

	public static string getManagedString(string key)
	{
		if (!initSuccessful())
		{
			return null;
		}
		return _pluginBase.Call<string>("getManagedString", new object[1] { key });
	}

	public static float getManagedFloat(string key)
	{
		if (!initSuccessful())
		{
			return 0f;
		}
		return _pluginBase.Call<float>("getManagedFloat", new object[1] { key });
	}

	public static int getManagedInt(string key)
	{
		if (!initSuccessful())
		{
			return 0;
		}
		return _pluginBase.Call<int>("getManagedInt", new object[1] { key });
	}

	public static bool getManagedBool(string key)
	{
		if (!initSuccessful())
		{
			return false;
		}
		return _pluginBase.Call<bool>("getManagedBool", new object[1] { key });
	}

	public static void setLocation(double lat, double lon)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("setLocation", lat, lon);
		}
	}

	public static void purgeLocation()
	{
		if (initSuccessful())
		{
			_pluginBase.Call("purgeLocation");
		}
	}

	public static void recordCustomEvent(string eventName, Dictionary<string, object> properties = null)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("recordAnalyticsEvent", eventName, (properties == null) ? null : Json.Serialize(properties));
		}
	}

	public static void recordMilestoneEvent(string scope, Dictionary<string, object> properties = null)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("recordMilestoneEvent", scope, (properties == null) ? null : Json.Serialize(properties));
		}
	}

	public static bool isContentReadyForBillboardWithScope(string scope)
	{
		if (!initSuccessful() || _pluginMarketingExtension == null)
		{
			return false;
		}
		return _pluginMarketingExtension.Call<bool>("isContentReadyForBillboardWithScope", new object[1] { scope });
	}

	public static void prepareBillboard(string scope)
	{
		if (initSuccessful() && _pluginMarketingExtension != null)
		{
			_pluginMarketingExtension.Call("prepareBillboard", scope);
		}
	}

	public static void destroyBillboard(string scope)
	{
		if (initSuccessful() && _pluginMarketingExtension != null)
		{
			_pluginMarketingExtension.Call("destroyBillboard", scope);
		}
	}

	public static void recordMonetizationEvent(double totalPrice, string currency, UpsightPurchaseResolution resolution, string product = null, double price = -1.0, int quantity = -1, Dictionary<string, object> properties = null)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("recordMonetizationEvent", totalPrice, currency, product, price, resolution.ToString().ToLower(), quantity, (properties == null) ? null : Json.Serialize(properties));
		}
	}

	public static void recordGooglePlayPurchase(int quantity, string currency, double price, double totalPrice, string product, int responseCode, string inAppPurchaseData, string inAppDataSignature, Dictionary<string, object> properties = null)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("recordGooglePlayPurchase", quantity, currency, price, totalPrice, product, responseCode, inAppPurchaseData, inAppDataSignature, (properties == null) ? null : Json.Serialize(properties));
		}
	}

	public static void recordAppleStorePurchase(int quantity, string currency, double price, string transactionIdentifier, string product, UpsightPurchaseResolution resolution, Dictionary<string, object> properties = null)
	{
	}

	public static void recordAttributionEvent(string campaign, string creative, string source, Dictionary<string, object> properties = null)
	{
		if (initSuccessful())
		{
			_pluginBase.Call("recordAttributionEvent", campaign, creative, source, (properties == null) ? null : Json.Serialize(properties));
		}
	}

	public static void registerForPushNotifications()
	{
		if (initSuccessful() && _pluginPushExtension != null)
		{
			_pluginPushExtension.Call("registerForPushNotifications");
		}
	}

	public static void unregisterForPushNotifications()
	{
		if (initSuccessful() && _pluginPushExtension != null)
		{
			_pluginPushExtension.Call("unregisterForPushNotifications");
		}
	}

	public static void setShouldSynchronizeManagedVariables(bool shouldSynchronize)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.upsight.android.unity.UnitySessionCallbacks"))
		{
			androidJavaClass.CallStatic("setShouldSynchronizeManagedVariables", shouldSynchronize);
		}
	}

	public static void onPause()
	{
		if (initSuccessful())
		{
			_pluginBase.Call("onApplicationPaused");
		}
	}

	public static void onResume()
	{
		if (initSuccessful())
		{
			_pluginBase.Call("onApplicationResumed");
		}
	}

	private static bool initSuccessful()
	{
		return Application.platform == RuntimePlatform.Android && initialized && _pluginBase != null;
	}
}
