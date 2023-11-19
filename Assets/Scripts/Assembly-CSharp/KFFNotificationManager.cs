using System;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class KFFNotificationManager : Singleton<KFFNotificationManager>
{
	private int _DayToSec = 86400;

	private bool RanOnce;

	private string androidNotID = string.Empty;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			cancelAllLocalNotifications();
			AddNotifications();
			AddSpecialNotification();
		}
	}

	private void AddNotifications()
	{
		List<NotificationData> database = NotificationDataManager.Instance.GetDatabase();
		foreach (NotificationData item in database)
		{
			switch (item.nType)
			{
			case NotificationType.LastLogin:
				LastLoginNotificationHandler(item);
				break;
			case NotificationType.Stamina:
				StaminaNotificationHandler(item);
				break;
			case NotificationType.DateTime:
				DateTimeNotificationHandler(item);
				break;
			case NotificationType.EventStart:
				EventStartNotificationHandler(item, true);
				break;
			case NotificationType.EventEnd:
				EventStartNotificationHandler(item, false);
				break;
			}
		}
	}

	private void LastLoginNotificationHandler(NotificationData n)
	{
		if (n.HoursAfterLastLogin != 0)
		{
			scheduleLocalNotification(n.HoursAfterLastLogin * 60 * 60, KFFLocalization.Get(n.TextID), string.Empty, string.Empty);
		}
	}

	private void StaminaNotificationHandler(NotificationData n)
	{
		int num = DetachedSingleton<StaminaManager>.Instance.SecondsUntilFull(n.TargetStaminaType);
		if (num > 0)
		{
			scheduleLocalNotification(num, KFFLocalization.Get(n.TextID), string.Empty, string.Empty);
		}
	}

	private void DateTimeNotificationHandler(NotificationData n)
	{
		TimeSpan timeSpan = n.nDateTime - TFUtils.ServerTime;
		if (timeSpan.TotalSeconds > 0.0)
		{
			scheduleLocalNotification((int)timeSpan.TotalSeconds, KFFLocalization.Get(n.TextID), string.Empty, string.Empty);
		}
	}

	private void EventStartNotificationHandler(NotificationData n, bool isStart)
	{
		if (n.nEventType != 0)
		{
			return;
		}
		List<LeagueData> notificationOnLeagues = LeagueDataManager.Instance.GetNotificationOnLeagues();
		foreach (LeagueData item in notificationOnLeagues)
		{
			List<LeagueData.StartEndTime> availabilityTimes = item.AvailabilityTimes;
			foreach (LeagueData.StartEndTime item2 in availabilityTimes)
			{
				DateTime dateTime = ((!isStart) ? item2.EndTime.AddDays(-1 * n.DaysBefore).AddHours(-1 * n.HoursBefore).AddMinutes(-1 * n.MinutesBefore) : item2.StartTime.AddDays(-1 * n.DaysBefore).AddHours(-1 * n.HoursBefore).AddMinutes(-1 * n.MinutesBefore));
				TimeSpan timeSpan = dateTime - TFUtils.ServerTime;
				if (timeSpan.TotalSeconds > 0.0)
				{
					scheduleLocalNotification((int)timeSpan.TotalSeconds, KFFLocalization.Get(n.TextID), string.Empty, string.Empty);
				}
			}
		}
	}

	public void scheduleAdventureCompleteNotification(int completionTime, string notId)
	{
		List<NotificationData> database = NotificationDataManager.Instance.GetDatabase();
		foreach (NotificationData item in database)
		{
			if (item.nType == NotificationType.Adventure)
			{
				scheduleLocalNotification(completionTime, KFFLocalization.Get(item.TextID), string.Empty, notId);
			}
		}
	}

	public void scheduleLocalNotification(int secondsFromNow, string text, string action, string notId = "")
	{
		if (!Singleton<PlayerInfoScript>.Instance.SaveData.NotificationEnabled || secondsFromNow <= 0)
		{
			return;
		}
		AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(secondsFromNow, KFFLocalization.Get("!!CREDITS_TITLE"), text, string.Empty);
		androidNotificationConfiguration.smallIcon = "android_small";
		androidNotificationConfiguration.largeIcon = "android_large";
		int num = EtceteraAndroid.scheduleNotification(androidNotificationConfiguration);
		if (notId != string.Empty)
		{
			PlayerPrefs.SetString(notId, num.ToString());
			return;
		}
		if (androidNotID == string.Empty)
		{
			androidNotID = num.ToString();
		}
		else
		{
			androidNotID = androidNotID + "," + num;
		}
		PlayerPrefs.SetString("NotificationIDs", androidNotID);
	}

	public void cancelLocalNotification(string notId)
	{
		int result;
		if (PlayerPrefs.HasKey(notId) && int.TryParse(PlayerPrefs.GetString(notId), out result))
		{
			EtceteraAndroid.cancelNotification(result);
		}
	}

	private void cancelAllLocalNotifications()
	{
		if (PlayerPrefs.HasKey("NotificationIDs"))
		{
			androidNotID = PlayerPrefs.GetString("NotificationIDs");
		}
		if (androidNotID == string.Empty)
		{
			return;
		}
		string[] array = androidNotID.Split(new string[1] { "," }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			int result;
			if (int.TryParse(array[i], out result))
			{
				EtceteraAndroid.cancelNotification(result);
			}
		}
		PlayerPrefs.SetString("NotificationIDs", string.Empty);
		androidNotID = string.Empty;
	}

	private void CancelSpecialNotification(string notId)
	{
		if (!PlayerPrefs.HasKey("specialNotificationID"))
		{
			return;
		}
		string @string = PlayerPrefs.GetString("specialNotificationID");
		if (@string == string.Empty || @string == null)
		{
			return;
		}
		string[] array = @string.Split(new string[1] { "," }, StringSplitOptions.None);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != notId)
			{
				text = text + array[i] + ",";
			}
			else if (PlayerPrefs.HasKey(array[i]))
			{
				PlayerPrefs.DeleteKey(array[i]);
			}
		}
		if (text.Length > 0)
		{
			text = text.Remove(text.LastIndexOf(","));
		}
		PlayerPrefs.SetString("specialNotificationID", text);
	}

	private void AddSpecialNotification()
	{
		if (!PlayerPrefs.HasKey("specialNotificationID"))
		{
			return;
		}
		string @string = PlayerPrefs.GetString("specialNotificationID");
		if (@string == string.Empty || @string == null)
		{
			return;
		}
		string[] array = @string.Split(new string[1] { "," }, StringSplitOptions.None);
		string empty = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (!PlayerPrefs.HasKey(array[i]))
			{
				continue;
			}
			string[] array2 = PlayerPrefs.GetString(array[i]).Split(new string[1] { "||" }, StringSplitOptions.None);
			if (array2.Length > 1)
			{
				uint num = uint.Parse(array2[0]) - TFUtils.ServerTime.UnixTimestamp();
				string text = string.Empty;
				string action = string.Empty;
				if (array2.Length >= 2)
				{
					text = array2[1];
				}
				if (array2.Length >= 3)
				{
					action = array2[2];
				}
				if (num != 0)
				{
					scheduleLocalNotification((int)num, text, action, string.Empty);
				}
			}
		}
	}
}
