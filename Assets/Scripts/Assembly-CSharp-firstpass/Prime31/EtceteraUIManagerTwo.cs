using UnityEngine;

namespace Prime31
{
	public class EtceteraUIManagerTwo : MonoBehaviourGUI
	{
		private int _fiveSecondNotificationId;

		private int _tenSecondNotificationId;

		private void OnGUI()
		{
			beginColumn();
			if (GUILayout.Button("Show Inline Web View"))
			{
				EtceteraAndroid.inlineWebViewShow("http://prime31.com/", 160, 430, Screen.width - 160, Screen.height - 100);
			}
			if (GUILayout.Button("Close Inline Web View"))
			{
				EtceteraAndroid.inlineWebViewClose();
			}
			if (GUILayout.Button("Set Url of Inline Web View"))
			{
				EtceteraAndroid.inlineWebViewSetUrl("http://google.com");
			}
			if (GUILayout.Button("Set Frame of Inline Web View"))
			{
				EtceteraAndroid.inlineWebViewSetFrame(80, 50, 300, 400);
			}
			if (GUILayout.Button("Get First 25 Contacts"))
			{
				EtceteraAndroid.loadContacts(0, 25);
			}
			GUILayout.Label("Request M Permissions");
			if (GUILayout.Button("Request Permission"))
			{
				EtceteraAndroid.requestPermissions(new string[1] { "android.permission.READ_PHONE_STATE" });
			}
			if (GUILayout.Button("Should Show Permission Rationale"))
			{
				bool flag = EtceteraAndroid.shouldShowRequestPermissionRationale("android.permission.READ_PHONE_STATE");
			}
			if (GUILayout.Button("Check Permission"))
			{
				bool flag2 = EtceteraAndroid.checkSelfPermission("android.permission.READ_PHONE_STATE");
			}
			endColumn(true);
			if (GUILayout.Button("Schedule Notification in 5s"))
			{
				AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(5L, "Notification Title - 5 Seconds", "The subtitle of the notification", "Ticker text gets ticked");
				androidNotificationConfiguration.extraData = "five-second-note";
				androidNotificationConfiguration.groupKey = "my-note-group";
				AndroidNotificationConfiguration androidNotificationConfiguration2 = androidNotificationConfiguration;
				androidNotificationConfiguration2.sound = false;
				androidNotificationConfiguration2.vibrate = false;
				_fiveSecondNotificationId = EtceteraAndroid.scheduleNotification(androidNotificationConfiguration2);
			}
			if (GUILayout.Button("Schedule Notification in 10s"))
			{
				AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(10L, "Notification Title - 10 Seconds", "The subtitle of the notification", "Ticker text gets ticked");
				androidNotificationConfiguration.extraData = "ten-second-note";
				androidNotificationConfiguration.groupKey = "my-note-group";
				AndroidNotificationConfiguration config = androidNotificationConfiguration;
				_tenSecondNotificationId = EtceteraAndroid.scheduleNotification(config);
			}
			if (GUILayout.Button("Schedule Group Summary Notification in 5s"))
			{
				AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(5L, "Group Summary Title", "Group Summary Subtitle - Stuff Happened", "Ticker text");
				androidNotificationConfiguration.extraData = "group-summary-note";
				androidNotificationConfiguration.groupKey = "my-note-group";
				androidNotificationConfiguration.isGroupSummary = true;
				AndroidNotificationConfiguration config2 = androidNotificationConfiguration;
				EtceteraAndroid.scheduleNotification(config2);
			}
			if (GUILayout.Button("Cancel 5s Notification"))
			{
				EtceteraAndroid.cancelNotification(_fiveSecondNotificationId);
			}
			if (GUILayout.Button("Cancel 10s Notification"))
			{
				EtceteraAndroid.cancelNotification(_tenSecondNotificationId);
			}
			if (GUILayout.Button("Check for Notifications"))
			{
				EtceteraAndroid.checkForNotifications();
			}
			if (GUILayout.Button("Cancel All Notifications"))
			{
				EtceteraAndroid.cancelAllNotifications();
			}
			if (GUILayout.Button("Quit App"))
			{
				Application.Quit();
			}
			endColumn();
			if (bottomRightButton("Previous Scene"))
			{
				MonoBehaviourGUI.loadLevel("EtceteraTestScene");
			}
		}
	}
}
