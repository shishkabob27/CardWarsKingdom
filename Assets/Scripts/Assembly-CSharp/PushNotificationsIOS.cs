using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PushNotificationsIOS : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void internalSendStringTags(string tagName, string[] tags);

	[DllImport("__Internal")]
	public static extern void registerForRemoteNotifications();

	[DllImport("__Internal")]
	public static extern void unregisterForRemoteNotifications();

	[DllImport("__Internal")]
	public static extern void setListenerName(string listenerName);

	[DllImport("__Internal")]
	public static extern IntPtr _getPushToken();

	[DllImport("__Internal")]
	public static extern IntPtr _getPushwooshHWID();

	[DllImport("__Internal")]
	public static extern void setIntTag(string tagName, int tagValue);

	[DllImport("__Internal")]
	public static extern void setStringTag(string tagName, string tagValue);

	[DllImport("__Internal")]
	public static extern void startLocationTracking();

	[DllImport("__Internal")]
	public static extern void stopLocationTracking();

	[DllImport("__Internal")]
	public static extern void clearNotificationCenter();

	public static void setListTag(string tagName, List<object> tagValues)
	{
		List<string> list = new List<string>();
		foreach (object tagValue in tagValues)
		{
			string text = tagValue.ToString();
			if (text != null)
			{
				list.Add(text);
			}
		}
		string[] tags = list.ToArray();
		internalSendStringTags(tagName, tags);
	}

	private void Start()
	{
	}

	public static string getPushToken()
	{
		return Marshal.PtrToStringAnsi(_getPushToken());
	}

	public static string getPushwooshHWID()
	{
		return Marshal.PtrToStringAnsi(_getPushwooshHWID());
	}

	private void onRegisteredForPushNotifications(string token)
	{
	}

	private void onFailedToRegisteredForPushNotifications(string error)
	{
	}

	private void onPushNotificationsReceived(string payload)
	{
	}
}
