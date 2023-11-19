namespace Facebook.Unity.Mobile.IOS
{
	internal class IOSWrapper : IIOSWrapper
	{
		public void Init(string appId, bool frictionlessRequests, string urlSuffix, string unityUserAgentSuffix)
		{
			IOSInit(appId, frictionlessRequests, urlSuffix, unityUserAgentSuffix);
		}

		public void LogInWithReadPermissions(int requestId, string scope)
		{
			IOSLogInWithReadPermissions(requestId, scope);
		}

		public void LogInWithPublishPermissions(int requestId, string scope)
		{
			IOSLogInWithPublishPermissions(requestId, scope);
		}

		public void LogOut()
		{
			IOSLogOut();
		}

		public void SetShareDialogMode(int mode)
		{
			IOSSetShareDialogMode(mode);
		}

		public void ShareLink(int requestId, string contentURL, string contentTitle, string contentDescription, string photoURL)
		{
			IOSShareLink(requestId, contentURL, contentTitle, contentDescription, photoURL);
		}

		public void FeedShare(int requestId, string toId, string link, string linkName, string linkCaption, string linkDescription, string picture, string mediaSource)
		{
			IOSFeedShare(requestId, toId, link, linkName, linkCaption, linkDescription, picture, mediaSource);
		}

		public void AppRequest(int requestId, string message, string actionType, string objectId, string[] to = null, int toLength = 0, string filters = "", string[] excludeIds = null, int excludeIdsLength = 0, bool hasMaxRecipients = false, int maxRecipients = 0, string data = "", string title = "")
		{
			IOSAppRequest(requestId, message, actionType, objectId, to, toLength, filters, excludeIds, excludeIdsLength, hasMaxRecipients, maxRecipients, data, title);
		}

		public void AppInvite(int requestId, string appLinkUrl, string previewImageUrl)
		{
			IOSAppInvite(requestId, appLinkUrl, previewImageUrl);
		}

		public void CreateGameGroup(int requestId, string name, string description, string privacy)
		{
			IOSCreateGameGroup(requestId, name, description, privacy);
		}

		public void JoinGameGroup(int requestId, string groupId)
		{
			IOSJoinGameGroup(requestId, groupId);
		}

		public void FBSettingsActivateApp(string appId)
		{
			IOSFBSettingsActivateApp(appId);
		}

		public void LogAppEvent(string logEvent, double valueToSum, int numParams, string[] paramKeys, string[] paramVals)
		{
			IOSFBAppEventsLogEvent(logEvent, valueToSum, numParams, paramKeys, paramVals);
		}

		public void LogPurchaseAppEvent(double logPurchase, string currency, int numParams, string[] paramKeys, string[] paramVals)
		{
			IOSFBAppEventsLogPurchase(logPurchase, currency, numParams, paramKeys, paramVals);
		}

		public void FBAppEventsSetLimitEventUsage(bool limitEventUsage)
		{
			IOSFBAppEventsSetLimitEventUsage(limitEventUsage);
		}

		public void GetAppLink(int requestId)
		{
			IOSGetAppLink(requestId);
		}

		public string FBSdkVersion()
		{
			return IOSFBSdkVersion();
		}

		public void FetchDeferredAppLink(int requestId)
		{
			IOSFetchDeferredAppLink(requestId);
		}

		public void RefreshCurrentAccessToken(int requestId)
		{
			IOSRefreshCurrentAccessToken(requestId);
		}

		private static void IOSInit(string appId, bool frictionlessRequests, string urlSuffix, string unityUserAgentSuffix)
		{
		}

		private static void IOSLogInWithReadPermissions(int requestId, string scope)
		{
		}

		private static void IOSLogInWithPublishPermissions(int requestId, string scope)
		{
		}

		private static void IOSLogOut()
		{
		}

		private static void IOSSetShareDialogMode(int mode)
		{
		}

		private static void IOSShareLink(int requestId, string contentURL, string contentTitle, string contentDescription, string photoURL)
		{
		}

		private static void IOSFeedShare(int requestId, string toId, string link, string linkName, string linkCaption, string linkDescription, string picture, string mediaSource)
		{
		}

		private static void IOSAppRequest(int requestId, string message, string actionType, string objectId, string[] to = null, int toLength = 0, string filters = "", string[] excludeIds = null, int excludeIdsLength = 0, bool hasMaxRecipients = false, int maxRecipients = 0, string data = "", string title = "")
		{
		}

		private static void IOSAppInvite(int requestId, string appLinkUrl, string previewImageUrl)
		{
		}

		private static void IOSCreateGameGroup(int requestId, string name, string description, string privacy)
		{
		}

		private static void IOSJoinGameGroup(int requestId, string groupId)
		{
		}

		private static void IOSFBSettingsActivateApp(string appId)
		{
		}

		private static void IOSFBAppEventsLogEvent(string logEvent, double valueToSum, int numParams, string[] paramKeys, string[] paramVals)
		{
		}

		private static void IOSFBAppEventsLogPurchase(double logPurchase, string currency, int numParams, string[] paramKeys, string[] paramVals)
		{
		}

		private static void IOSFBAppEventsSetLimitEventUsage(bool limitEventUsage)
		{
		}

		private static void IOSGetAppLink(int requestId)
		{
		}

		private static string IOSFBSdkVersion()
		{
			return "NONE";
		}

		private static void IOSFetchDeferredAppLink(int requestId)
		{
		}

		private static void IOSRefreshCurrentAccessToken(int requestId)
		{
		}
	}
}
