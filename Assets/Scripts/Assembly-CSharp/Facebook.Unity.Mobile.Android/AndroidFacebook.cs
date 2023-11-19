using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Mobile.Android
{
	internal sealed class AndroidFacebook : MobileFacebook
	{
		private class JavaMethodCall<T> : MethodCall<T> where T : IResult
		{
			private AndroidFacebook androidImpl;

			public JavaMethodCall(AndroidFacebook androidImpl, string methodName)
				: base((FacebookBase)androidImpl, methodName)
			{
				this.androidImpl = androidImpl;
			}

			public override void Call(MethodArguments args = null)
			{
				MethodArguments methodArguments = ((args != null) ? new MethodArguments(args) : new MethodArguments());
				if (base.Callback != null)
				{
					methodArguments.AddString("callback_id", androidImpl.CallbackManager.AddFacebookDelegate(base.Callback));
				}
				androidImpl.CallFB(base.MethodName, methodArguments.ToJsonString());
			}
		}

		public const string LoginPermissionsKey = "scope";

		private bool limitEventUsage;

		private IAndroidJavaClass facebookJava;

		public string KeyHash { get; private set; }

		public override bool LimitEventUsage
		{
			get
			{
				return limitEventUsage;
			}
			set
			{
				limitEventUsage = value;
				CallFB("SetLimitEventUsage", value.ToString());
			}
		}

		public override string SDKName
		{
			get
			{
				return "FBAndroidSDK";
			}
		}

		public override string SDKVersion
		{
			get
			{
				return facebookJava.CallStatic<string>("GetSdkVersion");
			}
		}

		public AndroidFacebook()
			: this(new FBJavaClass(), new CallbackManager())
		{
		}

		public AndroidFacebook(IAndroidJavaClass facebookJavaClass, CallbackManager callbackManager)
			: base(callbackManager)
		{
			KeyHash = string.Empty;
			facebookJava = facebookJavaClass;
		}

		public void Init(string appId, HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			CallFB("SetUserAgentSuffix", string.Format("Unity.{0}", Constants.UnitySDKUserAgentSuffixLegacy));
			base.Init(hideUnityDelegate, onInitComplete);
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("appId", appId);
			JavaMethodCall<IResult> javaMethodCall = new JavaMethodCall<IResult>(this, "Init");
			javaMethodCall.Call(methodArguments);
		}

		public override void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddCommaSeparatedList("scope", permissions);
			JavaMethodCall<ILoginResult> javaMethodCall = new JavaMethodCall<ILoginResult>(this, "LoginWithReadPermissions");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddCommaSeparatedList("scope", permissions);
			JavaMethodCall<ILoginResult> javaMethodCall = new JavaMethodCall<ILoginResult>(this, "LoginWithPublishPermissions");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void LogOut()
		{
			JavaMethodCall<IResult> javaMethodCall = new JavaMethodCall<IResult>(this, "Logout");
			javaMethodCall.Call();
		}

		public override void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			ValidateAppRequestArgs(message, actionType, objectId, to, filters, excludeIds, maxRecipients, data, title, callback);
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("message", message);
			methodArguments.AddNullablePrimitive("action_type", actionType);
			methodArguments.AddString("object_id", objectId);
			methodArguments.AddCommaSeparatedList("to", to);
			if (filters != null && filters.Any())
			{
				string text = filters.First() as string;
				if (text != null)
				{
					methodArguments.AddString("filters", text);
				}
			}
			methodArguments.AddNullablePrimitive("max_recipients", maxRecipients);
			methodArguments.AddString("data", data);
			methodArguments.AddString("title", title);
			JavaMethodCall<IAppRequestResult> javaMethodCall = new JavaMethodCall<IAppRequestResult>(this, "AppRequest");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddUri("appLinkUrl", appLinkUrl);
			methodArguments.AddUri("previewImageUrl", previewImageUrl);
			JavaMethodCall<IAppInviteResult> javaMethodCall = new JavaMethodCall<IAppInviteResult>(this, "AppInvite");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddUri("content_url", contentURL);
			methodArguments.AddString("content_title", contentTitle);
			methodArguments.AddString("content_description", contentDescription);
			methodArguments.AddUri("photo_url", photoURL);
			JavaMethodCall<IShareResult> javaMethodCall = new JavaMethodCall<IShareResult>(this, "ShareLink");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("toId", toId);
			methodArguments.AddUri("link", link);
			methodArguments.AddString("linkName", linkName);
			methodArguments.AddString("linkCaption", linkCaption);
			methodArguments.AddString("linkDescription", linkDescription);
			methodArguments.AddUri("picture", picture);
			methodArguments.AddString("mediaSource", mediaSource);
			JavaMethodCall<IShareResult> javaMethodCall = new JavaMethodCall<IShareResult>(this, "FeedShare");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("name", name);
			methodArguments.AddString("description", description);
			methodArguments.AddString("privacy", privacy);
			JavaMethodCall<IGroupCreateResult> javaMethodCall = new JavaMethodCall<IGroupCreateResult>(this, "GameGroupCreate");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("id", id);
			JavaMethodCall<IGroupJoinResult> javaMethodCall = new JavaMethodCall<IGroupJoinResult>(this, "GameGroupJoin");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(methodArguments);
		}

		public override void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			JavaMethodCall<IAppLinkResult> javaMethodCall = new JavaMethodCall<IAppLinkResult>(this, "GetAppLink");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call();
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("logEvent", logEvent);
			methodArguments.AddNullablePrimitive("valueToSum", valueToSum);
			methodArguments.AddDictionary("parameters", parameters);
			JavaMethodCall<IResult> javaMethodCall = new JavaMethodCall<IResult>(this, "LogAppEvent");
			javaMethodCall.Call(methodArguments);
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddPrimative("logPurchase", logPurchase);
			methodArguments.AddString("currency", currency);
			methodArguments.AddDictionary("parameters", parameters);
			JavaMethodCall<IResult> javaMethodCall = new JavaMethodCall<IResult>(this, "LogAppEvent");
			javaMethodCall.Call(methodArguments);
		}

		public override void ActivateApp(string appId)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("app_id", appId);
			JavaMethodCall<IResult> javaMethodCall = new JavaMethodCall<IResult>(this, "ActivateApp");
			javaMethodCall.Call(methodArguments);
		}

		public override void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			MethodArguments args = new MethodArguments();
			JavaMethodCall<IAppLinkResult> javaMethodCall = new JavaMethodCall<IAppLinkResult>(this, "FetchDeferredAppLinkData");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call(args);
		}

		public override void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback)
		{
			JavaMethodCall<IAccessTokenRefreshResult> javaMethodCall = new JavaMethodCall<IAccessTokenRefreshResult>(this, "RefreshCurrentAccessToken");
			javaMethodCall.Callback = callback;
			javaMethodCall.Call();
		}

		protected override void SetShareDialogMode(ShareDialogMode mode)
		{
			CallFB("SetShareDialogMode", mode.ToString());
		}

		private void CallFB(string method, string args)
		{
			facebookJava.CallStatic(method, args);
		}
	}
}
