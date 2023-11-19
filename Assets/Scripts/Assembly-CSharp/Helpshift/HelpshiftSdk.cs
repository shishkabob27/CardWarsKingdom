using System.Collections.Generic;

namespace Helpshift
{
	public class HelpshiftSdk
	{
		public const string HS_RATE_ALERT_CLOSE = "HS_RATE_ALERT_CLOSE";

		public const string HS_RATE_ALERT_FEEDBACK = "HS_RATE_ALERT_FEEDBACK";

		public const string HS_RATE_ALERT_SUCCESS = "HS_RATE_ALERT_SUCCESS";

		public const string HS_RATE_ALERT_FAIL = "HS_RATE_ALERT_FAIL";

		public const string HSTAGSKEY = "hs-tags";

		public const string HSCUSTOMMETADATAKEY = "hs-custom-metadata";

		public const string CONTACT_US_ALWAYS = "always";

		public const string CONTACT_US_NEVER = "never";

		public const string CONTACT_US_AFTER_VIEWING_FAQS = "after_viewing_faqs";

		public const string HSUserAcceptedTheSolution = "User accepted the solution";

		public const string HSUserRejectedTheSolution = "User rejected the solution";

		public const string HSUserSentScreenShot = "User sent a screenshot";

		public const string HSUserReviewedTheApp = "User reviewed the app";

		private static HelpshiftSdk instance;

		private static HelpshiftAndroid nativeSdk;

		private HelpshiftSdk()
		{
		}

		public static HelpshiftSdk getInstance()
		{
			if (instance == null)
			{
				instance = new HelpshiftSdk();
				nativeSdk = new HelpshiftAndroid();
			}
			return instance;
		}

		public void install(string apiKey, string domainName, string appId, Dictionary<string, string> config)
		{
			nativeSdk.install(apiKey, domainName, appId, config);
		}

		public void install(string apiKey, string domainName, string appId)
		{
			install(apiKey, domainName, appId, new Dictionary<string, string>());
		}

		public void install()
		{
			nativeSdk.install();
		}

		public int getNotificationCount(bool isAsync)
		{
			return nativeSdk.getNotificationCount(isAsync);
		}

		public void setNameAndEmail(string userName, string email)
		{
			nativeSdk.setNameAndEmail(userName, email);
		}

		public void setUserIdentifier(string identifier)
		{
			nativeSdk.setUserIdentifier(identifier);
		}

		public void login(string identifier, string name, string email)
		{
			nativeSdk.login(identifier, name, email);
		}

		public void logout()
		{
			nativeSdk.logout();
		}

		public void registerDeviceToken(string deviceToken)
		{
			nativeSdk.registerDeviceToken(deviceToken);
		}

		public void leaveBreadCrumb(string breadCrumb)
		{
			nativeSdk.leaveBreadCrumb(breadCrumb);
		}

		public void clearBreadCrumbs()
		{
			nativeSdk.clearBreadCrumbs();
		}

		public void showConversation(Dictionary<string, object> configMap)
		{
			nativeSdk.showConversation(configMap);
		}

		public void showConversation()
		{
			nativeSdk.showConversation();
		}

		public void showConversationWithMeta(Dictionary<string, object> configMap)
		{
			nativeSdk.showConversationWithMeta(configMap);
		}

		public void showFAQSection(string sectionPublishId, Dictionary<string, object> configMap)
		{
			nativeSdk.showFAQSection(sectionPublishId, configMap);
		}

		public void showFAQSection(string sectionPublishId)
		{
			nativeSdk.showFAQSection(sectionPublishId);
		}

		public void showFAQSectionWithMeta(string sectionPublishId, Dictionary<string, object> configMap)
		{
			nativeSdk.showFAQSectionWithMeta(sectionPublishId, configMap);
		}

		public void showSingleFAQ(string questionPublishId, Dictionary<string, object> configMap)
		{
			nativeSdk.showSingleFAQ(questionPublishId, configMap);
		}

		public void showSingleFAQ(string questionPublishId)
		{
			nativeSdk.showSingleFAQ(questionPublishId);
		}

		public void showSingleFAQWithMeta(string questionPublishId, Dictionary<string, object> configMap)
		{
			nativeSdk.showSingleFAQWithMeta(questionPublishId, configMap);
		}

		public void showFAQs(Dictionary<string, object> configMap)
		{
			nativeSdk.showFAQs(configMap);
		}

		public void showFAQs()
		{
			nativeSdk.showFAQs();
		}

		public void showFAQsWithMeta(Dictionary<string, object> configMap)
		{
			nativeSdk.showFAQsWithMeta(configMap);
		}

		public void updateMetaData(Dictionary<string, object> metaData)
		{
			nativeSdk.updateMetaData(metaData);
		}

		public void handlePushNotification(string issueId)
		{
			nativeSdk.handlePushNotification(issueId);
		}

		public void showAlertToRateAppWithURL(string url)
		{
			nativeSdk.showAlertToRateAppWithURL(url);
		}

		public void registerSessionDelegates()
		{
			nativeSdk.registerSessionDelegates();
		}

		public void registerForPush(string gcmId)
		{
			nativeSdk.registerForPushWithGcmId(gcmId);
		}
	}
}
