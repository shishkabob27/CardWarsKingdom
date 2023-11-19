using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Mobile.IOS
{
	internal class IOSFacebook : MobileFacebook
	{
		public enum FBInsightsFlushBehavior
		{
			FBInsightsFlushBehaviorAuto,
			FBInsightsFlushBehaviorExplicitOnly
		}

		private class NativeDict
		{
			public int NumEntries { get; set; }

			public string[] Keys { get; set; }

			public string[] Values { get; set; }

			public NativeDict()
			{
				NumEntries = 0;
				Keys = null;
				Values = null;
			}
		}

		private const string CancelledResponse = "{\"cancelled\":true}";

		private bool limitEventUsage;

		private IIOSWrapper iosWrapper;

		public override bool LimitEventUsage
		{
			get
			{
				return limitEventUsage;
			}
			set
			{
				limitEventUsage = value;
				iosWrapper.FBAppEventsSetLimitEventUsage(value);
			}
		}

		public override string SDKName
		{
			get
			{
				return "FBiOSSDK";
			}
		}

		public override string SDKVersion
		{
			get
			{
				return iosWrapper.FBSdkVersion();
			}
		}

		public IOSFacebook()
			: this(new IOSWrapper(), new CallbackManager())
		{
		}

		public IOSFacebook(IIOSWrapper iosWrapper, CallbackManager callbackManager)
			: base(callbackManager)
		{
			this.iosWrapper = iosWrapper;
		}

		public void Init(string appId, bool frictionlessRequests, HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			base.Init(hideUnityDelegate, onInitComplete);
			iosWrapper.Init(appId, frictionlessRequests, FacebookSettings.IosURLSuffix, Constants.UnitySDKUserAgentSuffixLegacy);
		}

		public override void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			iosWrapper.LogInWithReadPermissions(AddCallback(callback), permissions.ToCommaSeparateList());
		}

		public override void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			iosWrapper.LogInWithPublishPermissions(AddCallback(callback), permissions.ToCommaSeparateList());
		}

		public override void LogOut()
		{
			base.LogOut();
			iosWrapper.LogOut();
		}

		public override void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			ValidateAppRequestArgs(message, actionType, objectId, to, filters, excludeIds, maxRecipients, data, title, callback);
			string text = null;
			if (filters != null && filters.Any())
			{
				text = filters.First() as string;
			}
			iosWrapper.AppRequest(AddCallback(callback), message, (!actionType.HasValue) ? string.Empty : actionType.ToString(), (objectId == null) ? string.Empty : objectId, (to == null) ? null : to.ToArray(), (to != null) ? to.Count() : 0, (text == null) ? string.Empty : text, (excludeIds == null) ? null : excludeIds.ToArray(), (excludeIds != null) ? excludeIds.Count() : 0, maxRecipients.HasValue, maxRecipients.HasValue ? maxRecipients.Value : 0, data, title);
		}

		public override void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback)
		{
			string appLinkUrl2 = string.Empty;
			string previewImageUrl2 = string.Empty;
			if (appLinkUrl != null && !string.IsNullOrEmpty(appLinkUrl.AbsoluteUri))
			{
				appLinkUrl2 = appLinkUrl.AbsoluteUri;
			}
			if (previewImageUrl != null && !string.IsNullOrEmpty(previewImageUrl.AbsoluteUri))
			{
				previewImageUrl2 = previewImageUrl.AbsoluteUri;
			}
			iosWrapper.AppInvite(AddCallback(callback), appLinkUrl2, previewImageUrl2);
		}

		public override void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback)
		{
			iosWrapper.ShareLink(AddCallback(callback), contentURL.AbsoluteUrlOrEmptyString(), contentTitle, contentDescription, photoURL.AbsoluteUrlOrEmptyString());
		}

		public override void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback)
		{
			string link2 = ((!(link != null)) ? string.Empty : link.ToString());
			string picture2 = ((!(picture != null)) ? string.Empty : picture.ToString());
			iosWrapper.FeedShare(AddCallback(callback), toId, link2, linkName, linkCaption, linkDescription, picture2, mediaSource);
		}

		public override void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback)
		{
			iosWrapper.CreateGameGroup(AddCallback(callback), name, description, privacy);
		}

		public override void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback)
		{
			iosWrapper.JoinGameGroup(Convert.ToInt32(base.CallbackManager.AddFacebookDelegate(callback)), id);
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			NativeDict nativeDict = MarshallDict(parameters);
			if (valueToSum.HasValue)
			{
				iosWrapper.LogAppEvent(logEvent, valueToSum.Value, nativeDict.NumEntries, nativeDict.Keys, nativeDict.Values);
			}
			else
			{
				iosWrapper.LogAppEvent(logEvent, 0.0, nativeDict.NumEntries, nativeDict.Keys, nativeDict.Values);
			}
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters)
		{
			NativeDict nativeDict = MarshallDict(parameters);
			iosWrapper.LogPurchaseAppEvent(logPurchase, currency, nativeDict.NumEntries, nativeDict.Keys, nativeDict.Values);
		}

		public override void ActivateApp(string appId)
		{
			iosWrapper.FBSettingsActivateApp(appId);
		}

		public override void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			iosWrapper.FetchDeferredAppLink(AddCallback(callback));
		}

		public override void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			iosWrapper.GetAppLink(Convert.ToInt32(base.CallbackManager.AddFacebookDelegate(callback)));
		}

		public override void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback)
		{
			iosWrapper.RefreshCurrentAccessToken(Convert.ToInt32(base.CallbackManager.AddFacebookDelegate(callback)));
		}

		protected override void SetShareDialogMode(ShareDialogMode mode)
		{
			iosWrapper.SetShareDialogMode((int)mode);
		}

		private static NativeDict MarshallDict(Dictionary<string, object> dict)
		{
			NativeDict nativeDict = new NativeDict();
			if (dict != null && dict.Count > 0)
			{
				nativeDict.Keys = new string[dict.Count];
				nativeDict.Values = new string[dict.Count];
				nativeDict.NumEntries = 0;
				{
					foreach (KeyValuePair<string, object> item in dict)
					{
						nativeDict.Keys[nativeDict.NumEntries] = item.Key;
						nativeDict.Values[nativeDict.NumEntries] = item.Value.ToString();
						nativeDict.NumEntries++;
					}
					return nativeDict;
				}
			}
			return nativeDict;
		}

		private static NativeDict MarshallDict(Dictionary<string, string> dict)
		{
			NativeDict nativeDict = new NativeDict();
			if (dict != null && dict.Count > 0)
			{
				nativeDict.Keys = new string[dict.Count];
				nativeDict.Values = new string[dict.Count];
				nativeDict.NumEntries = 0;
				{
					foreach (KeyValuePair<string, string> item in dict)
					{
						nativeDict.Keys[nativeDict.NumEntries] = item.Key;
						nativeDict.Values[nativeDict.NumEntries] = item.Value;
						nativeDict.NumEntries++;
					}
					return nativeDict;
				}
			}
			return nativeDict;
		}

		private int AddCallback<T>(FacebookDelegate<T> callback) where T : IResult
		{
			string value = base.CallbackManager.AddFacebookDelegate(callback);
			return Convert.ToInt32(value);
		}
	}
}
