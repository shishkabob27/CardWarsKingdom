using System;
using System.Collections.Generic;
using Facebook.Unity.Canvas;
using Facebook.Unity.Editor;
using Facebook.Unity.Mobile;
using Facebook.Unity.Mobile.Android;
using Facebook.Unity.Mobile.IOS;
using UnityEngine;

namespace Facebook.Unity
{
	public sealed class FB : ScriptableObject
	{
		public sealed class Canvas
		{
			private static ICanvasFacebook CanvasFacebookImpl
			{
				get
				{
					ICanvasFacebook canvasFacebook = FacebookImpl as ICanvasFacebook;
					if (canvasFacebook == null)
					{
						throw new InvalidOperationException("Attempt to call Canvas interface on non canvas platform");
					}
					return canvasFacebook;
				}
			}

			public static void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate<IPayResult> callback = null)
			{
				CanvasFacebookImpl.Pay(product, action, quantity, quantityMin, quantityMax, requestId, pricepointId, testCurrency, callback);
			}
		}

		public sealed class Mobile
		{
			public static ShareDialogMode ShareDialogMode
			{
				get
				{
					return MobileFacebookImpl.ShareDialogMode;
				}
				set
				{
					MobileFacebookImpl.ShareDialogMode = value;
				}
			}

			private static IMobileFacebook MobileFacebookImpl
			{
				get
				{
					IMobileFacebook mobileFacebook = FacebookImpl as IMobileFacebook;
					if (mobileFacebook == null)
					{
						throw new InvalidOperationException("Attempt to call Mobile interface on non mobile platform");
					}
					return mobileFacebook;
				}
			}

			public static void AppInvite(Uri appLinkUrl, Uri previewImageUrl = null, FacebookDelegate<IAppInviteResult> callback = null)
			{
				MobileFacebookImpl.AppInvite(appLinkUrl, previewImageUrl, callback);
			}

			public static void FetchDeferredAppLinkData(FacebookDelegate<IAppLinkResult> callback = null)
			{
				if (callback != null)
				{
					MobileFacebookImpl.FetchDeferredAppLink(callback);
				}
			}

			public static void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback = null)
			{
				MobileFacebookImpl.RefreshCurrentAccessToken(callback);
			}
		}

		public sealed class Android
		{
			public static string KeyHash
			{
				get
				{
					AndroidFacebook androidFacebook = FacebookImpl as AndroidFacebook;
					return (androidFacebook == null) ? string.Empty : androidFacebook.KeyHash;
				}
			}
		}

		internal abstract class CompiledFacebookLoader : MonoBehaviour
		{
			protected abstract FacebookGameObject FBGameObject { get; }

			public void Start()
			{
				facebook = FBGameObject.Facebook;
				OnDLLLoadedDelegate();
				LogVersion();
				UnityEngine.Object.Destroy(this);
			}
		}

		private delegate void OnDLLLoaded();

		private const string DefaultJSSDKLocale = "en_US";

		private static IFacebook facebook;

		private static bool isInitCalled;

		private static string facebookDomain = "facebook.com";

		public static string AppId { get; private set; }

		public static bool IsLoggedIn
		{
			get
			{
				return facebook != null && FacebookImpl.LoggedIn;
			}
		}

		public static bool IsInitialized
		{
			get
			{
				return facebook != null && facebook.Initialized;
			}
		}

		public static bool LimitAppEventUsage
		{
			get
			{
				return facebook != null && facebook.LimitEventUsage;
			}
			set
			{
				if (facebook != null)
				{
					facebook.LimitEventUsage = value;
				}
			}
		}

		internal static IFacebook FacebookImpl
		{
			get
			{
				if (facebook == null)
				{
					throw new NullReferenceException("Facebook object is not yet loaded.  Did you call FB.Init()?");
				}
				return facebook;
			}
			set
			{
				facebook = value;
			}
		}

		internal static string FacebookDomain
		{
			get
			{
				return facebookDomain;
			}
			set
			{
				facebookDomain = value;
			}
		}

		private static OnDLLLoaded OnDLLLoadedDelegate { get; set; }

		public static void Init(InitDelegate onInitComplete = null, HideUnityDelegate onHideUnity = null, string authResponse = null)
		{
			//Init(FacebookSettings.AppId, FacebookSettings.Cookie, FacebookSettings.Logging, FacebookSettings.Status, FacebookSettings.Xfbml, FacebookSettings.FrictionlessRequests, authResponse, "en_US", onHideUnity, onInitComplete);
		}

		public static void Init(string appId, bool cookie = true, bool logging = true, bool status = true, bool xfbml = false, bool frictionlessRequests = true, string authResponse = null, string jsSDKLocale = "en_US", HideUnityDelegate onHideUnity = null, InitDelegate onInitComplete = null)
		{
			if (string.IsNullOrEmpty(appId))
			{
				throw new ArgumentException("appId cannot be null or empty!");
			}
			AppId = appId;
			if (!isInitCalled)
			{
				isInitCalled = true;
				if (Constants.IsEditor)
				{
					OnDLLLoadedDelegate = delegate
					{
						((EditorFacebook)facebook).Init(onHideUnity, onInitComplete);
					};
					ComponentFactory.GetComponent<EditorFacebookLoader>();
					return;
				}
				switch (Constants.CurrentPlatform)
				{
				case FacebookUnityPlatform.WebGL:
				case FacebookUnityPlatform.WebPlayer:
					OnDLLLoadedDelegate = delegate
					{
						((CanvasFacebook)facebook).Init(appId, cookie, logging, status, xfbml, FacebookSettings.ChannelUrl, authResponse, frictionlessRequests, jsSDKLocale, onHideUnity, onInitComplete);
					};
					ComponentFactory.GetComponent<CanvasFacebookLoader>();
					break;
				case FacebookUnityPlatform.IOS:
					OnDLLLoadedDelegate = delegate
					{
						((IOSFacebook)facebook).Init(appId, frictionlessRequests, onHideUnity, onInitComplete);
					};
					ComponentFactory.GetComponent<IOSFacebookLoader>();
					break;
				case FacebookUnityPlatform.Android:
					OnDLLLoadedDelegate = delegate
					{
						((AndroidFacebook)facebook).Init(appId, onHideUnity, onInitComplete);
					};
					ComponentFactory.GetComponent<AndroidFacebookLoader>();
					break;
				default:
					throw new NotImplementedException("Facebook API does not yet support this platform");
				}
			}
			else
			{
				FacebookLogger.Warn("FB.Init() has already been called.  You only need to call this once and only once.");
			}
		}

		public static void LogInWithPublishPermissions(IEnumerable<string> permissions = null, FacebookDelegate<ILoginResult> callback = null)
		{
			FacebookImpl.LogInWithPublishPermissions(permissions, callback);
		}

		public static void LogInWithReadPermissions(IEnumerable<string> permissions = null, FacebookDelegate<ILoginResult> callback = null)
		{
			FacebookImpl.LogInWithReadPermissions(permissions, callback);
		}

		public static void LogOut()
		{
			FacebookImpl.LogOut();
		}

		public static void AppRequest(string message, OGActionType actionType, string objectId, IEnumerable<string> to, string data = "", string title = "", FacebookDelegate<IAppRequestResult> callback = null)
		{
			FacebookImpl.AppRequest(message, actionType, objectId, to, null, null, null, data, title, callback);
		}

		public static void AppRequest(string message, OGActionType actionType, string objectId, IEnumerable<object> filters = null, IEnumerable<string> excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate<IAppRequestResult> callback = null)
		{
			FacebookImpl.AppRequest(message, actionType, objectId, null, filters, excludeIds, maxRecipients, data, title, callback);
		}

		public static void AppRequest(string message, IEnumerable<string> to = null, IEnumerable<object> filters = null, IEnumerable<string> excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate<IAppRequestResult> callback = null)
		{
			FacebookImpl.AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
		}

		public static void ShareLink(Uri contentURL = null, string contentTitle = "", string contentDescription = "", Uri photoURL = null, FacebookDelegate<IShareResult> callback = null)
		{
			FacebookImpl.ShareLink(contentURL, contentTitle, contentDescription, photoURL, callback);
		}

		public static void FeedShare(string toId = "", Uri link = null, string linkName = "", string linkCaption = "", string linkDescription = "", Uri picture = null, string mediaSource = "", FacebookDelegate<IShareResult> callback = null)
		{
			FacebookImpl.FeedShare(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, callback);
		}

		public static void API(string query, HttpMethod method, FacebookDelegate<IGraphResult> callback = null, IDictionary<string, string> formData = null)
		{
			if (string.IsNullOrEmpty(query))
			{
				throw new ArgumentNullException("query", "The query param cannot be null or empty");
			}
			FacebookImpl.API(query, method, formData, callback);
		}

		public static void API(string query, HttpMethod method, FacebookDelegate<IGraphResult> callback, WWWForm formData)
		{
			if (string.IsNullOrEmpty(query))
			{
				throw new ArgumentNullException("query", "The query param cannot be null or empty");
			}
			FacebookImpl.API(query, method, formData, callback);
		}

		public static void ActivateApp()
		{
			FacebookImpl.ActivateApp(AppId);
		}

		public static void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			if (callback != null)
			{
				FacebookImpl.GetAppLink(callback);
			}
		}

		public static void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate<IGroupCreateResult> callback = null)
		{
			FacebookImpl.GameGroupCreate(name, description, privacy, callback);
		}

		public static void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback = null)
		{
			FacebookImpl.GameGroupJoin(id, callback);
		}

		public static void LogAppEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			FacebookImpl.AppEventsLogEvent(logEvent, valueToSum, parameters);
		}

		public static void LogPurchase(float logPurchase, string currency = null, Dictionary<string, object> parameters = null)
		{
			if (string.IsNullOrEmpty(currency))
			{
				currency = "USD";
			}
			FacebookImpl.AppEventsLogPurchase(logPurchase, currency, parameters);
		}

		private static void LogVersion()
		{
			if (facebook != null)
			{
				FacebookLogger.Info(string.Format("Using Facebook Unity SDK v{0} with {1}", FacebookSdkVersion.Build, FacebookImpl.SDKUserAgent));
			}
			else
			{
				FacebookLogger.Info(string.Format("Using Facebook Unity SDK v{0}", FacebookSdkVersion.Build));
			}
		}
	}
}
