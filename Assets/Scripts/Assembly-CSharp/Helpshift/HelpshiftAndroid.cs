using System;
using System.Collections.Generic;
using System.Linq;
using HSMiniJSON;
using UnityEngine;

namespace Helpshift
{
	public class HelpshiftAndroid
	{
		private AndroidJavaClass jc;

		private AndroidJavaObject currentActivity;

		private AndroidJavaObject application;

		private AndroidJavaObject hsPlugin;

		public HelpshiftAndroid()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				application = currentActivity.Call<AndroidJavaObject>("getApplication", new object[0]);
				hsPlugin = new AndroidJavaClass("com.helpshift.Helpshift");
			}
		}

		private AndroidJavaObject convertToJavaHashMap(Dictionary<string, object> configD)
		{
			return null;
		}

		private AndroidJavaObject convertMetadataToJavaHashMap(Dictionary<string, object> metaMap)
		{
			return null;
		}

		private void hsApiCall(string api, params object[] args)
		{
			//hsPlugin.CallStatic(api, args);
		}

		private void hsApiCall(string api)
		{
			//hsPlugin.CallStatic(api);
		}

		public void install(string apiKey, string domain, string appId, Dictionary<string, string> configMap)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, string> item in configMap)
			{
				dictionary.Add(item.Key, item.Value);
			}
			hsApiCall("install", application, apiKey, domain, appId, convertToJavaHashMap(dictionary));
		}

		public void install()
		{
			hsApiCall("install", application);
		}

		public int getNotificationCount(bool isAsync)
		{
			return hsPlugin.CallStatic<int>("getNotificationCount", new object[1] { isAsync });
		}

		public void setNameAndEmail(string userName, string email)
		{
			hsApiCall("setNameAndEmail", userName, email);
		}

		public void setUserIdentifier(string identifier)
		{
			hsApiCall("setUserIdentifier", identifier);
		}

		public void registerDeviceToken(string deviceToken)
		{
			hsApiCall("registerDeviceToken", currentActivity, deviceToken);
		}

		public void leaveBreadCrumb(string breadCrumb)
		{
			hsApiCall("leaveBreadCrumb", breadCrumb);
		}

		public void clearBreadCrumbs()
		{
			hsApiCall("clearBreadCrumbs");
		}

		public void login(string identifier, string userName, string email)
		{
			hsApiCall("login", identifier, userName, email);
		}

		public void logout()
		{
			hsApiCall("logout");
		}

		public void showConversation(Dictionary<string, object> configMap)
		{
			hsApiCall("showConversation", currentActivity, convertToJavaHashMap(configMap));
		}

		public void showFAQSection(string sectionPublishId, Dictionary<string, object> configMap)
		{
			hsApiCall("showFAQSection", currentActivity, sectionPublishId, convertToJavaHashMap(configMap));
		}

		public void showSingleFAQ(string questionPublishId, Dictionary<string, object> configMap)
		{
			hsApiCall("showSingleFAQ", currentActivity, questionPublishId, convertToJavaHashMap(configMap));
		}

		public void showFAQs(Dictionary<string, object> configMap)
		{
			hsApiCall("showFAQs", currentActivity, convertToJavaHashMap(configMap));
		}

		public void showConversation()
		{
			hsApiCall("showConversation");
		}

		public void showFAQSection(string sectionPublishId)
		{
			hsApiCall("showFAQSection", sectionPublishId);
		}

		public void showSingleFAQ(string questionPublishId)
		{
			hsApiCall("showSingleFAQ", questionPublishId);
		}

		public void showFAQs()
		{
			hsApiCall("showFAQs");
		}

		public void showConversationWithMeta(Dictionary<string, object> configMap)
		{
			hsApiCall("showConversationWithMeta", convertMetadataToJavaHashMap(configMap));
		}

		public void showFAQSectionWithMeta(string sectionPublishId, Dictionary<string, object> configMap)
		{
			hsApiCall("showFAQSectionWithMeta", sectionPublishId, convertMetadataToJavaHashMap(configMap));
		}

		public void showSingleFAQWithMeta(string questionPublishId, Dictionary<string, object> configMap)
		{
			hsApiCall("showSingleFAQWithMeta", questionPublishId, convertMetadataToJavaHashMap(configMap));
		}

		public void showFAQsWithMeta(Dictionary<string, object> configMap)
		{
			hsApiCall("showFAQsWithMeta", convertMetadataToJavaHashMap(configMap));
		}

		public void updateMetaData(Dictionary<string, object> metaData)
		{
			hsApiCall("setMetaData", Json.Serialize(metaData));
		}

		public void handlePushNotification(string issueId)
		{
			hsApiCall("handlePush", currentActivity, issueId);
		}

		public void showAlertToRateAppWithURL(string url)
		{
			hsApiCall("showAlertToRateApp", url);
		}

		public void registerSessionDelegates()
		{
			hsApiCall("registerSessionDelegates");
		}

		public void registerForPushWithGcmId(string gcmId)
		{
			hsApiCall("registerGcmKey", gcmId, currentActivity);
		}
	}
}
