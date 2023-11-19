using System;
using System.Collections.Generic;
using Facebook.MiniJSON;
using Facebook.Unity.Canvas;
using Facebook.Unity.Editor.Dialogs;
using Facebook.Unity.Mobile;

namespace Facebook.Unity.Editor
{
	internal class EditorFacebook : FacebookBase, ICanvasFacebook, ICanvasFacebookCallbackHandler, ICanvasFacebookImplementation, IFacebook, IFacebookCallbackHandler, IMobileFacebook, IMobileFacebookCallbackHandler, IMobileFacebookImplementation
	{
		private const string WarningMessage = "You are using the facebook SDK in the Unity Editor. Behavior may not be the same as when used on iOS, Android, or Web.";

		private const string AccessTokenKey = "com.facebook.unity.editor.accesstoken";

		public override bool LimitEventUsage { get; set; }

		public ShareDialogMode ShareDialogMode { get; set; }

		public override string SDKName
		{
			get
			{
				return "FBUnityEditorSDK";
			}
		}

		public override string SDKVersion
		{
			get
			{
				return FacebookSdkVersion.Build;
			}
		}

		private IFacebookCallbackHandler EditorGameObject
		{
			get
			{
				return ComponentFactory.GetComponent<EditorFacebookGameObject>();
			}
		}

		public EditorFacebook()
			: base(new CallbackManager())
		{
		}

		public override void Init(HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			FacebookLogger.Warn("You are using the facebook SDK in the Unity Editor. Behavior may not be the same as when used on iOS, Android, or Web.");
			base.Init(hideUnityDelegate, onInitComplete);
			EditorGameObject.OnInitComplete(string.Empty);
		}

		public override void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			LogInWithPublishPermissions(permissions, callback);
		}

		public override void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			MockLoginDialog component = ComponentFactory.GetComponent<MockLoginDialog>();
			component.Callback = EditorGameObject.OnLoginComplete;
			component.CallbackID = base.CallbackManager.AddFacebookDelegate(callback);
		}

		public override void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			ShowEmptyMockDialog(OnAppRequestsComplete, callback, "Mock App Request");
		}

		public override void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback)
		{
			ShowMockShareDialog("ShareLink", callback);
		}

		public override void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback)
		{
			ShowMockShareDialog("FeedShare", callback);
		}

		public override void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback)
		{
			ShowEmptyMockDialog(OnGroupCreateComplete, callback, "Mock Group Create");
		}

		public override void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback)
		{
			ShowEmptyMockDialog(OnGroupJoinComplete, callback, "Mock Group Join");
		}

		public override void ActivateApp(string appId)
		{
			FacebookLogger.Info("This only needs to be called for iOS or Android.");
		}

		public override void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["url"] = "mockurl://testing.url";
			dictionary["callback_id"] = base.CallbackManager.AddFacebookDelegate(callback);
			OnGetAppLinkComplete(Json.Serialize(dictionary));
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			FacebookLogger.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters)
		{
			FacebookLogger.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		public void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback)
		{
			ShowEmptyMockDialog(OnAppInviteComplete, callback, "Mock App Invite");
		}

		public void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["url"] = "mockurl://testing.url";
			dictionary["ref"] = "mock ref";
			dictionary["extras"] = new Dictionary<string, object> { { "mock extra key", "mock extra value" } };
			dictionary["target_url"] = "mocktargeturl://mocktarget.url";
			dictionary["callback_id"] = base.CallbackManager.AddFacebookDelegate(callback);
			OnFetchDeferredAppLinkComplete(Json.Serialize(dictionary));
		}

		public void Pay(string product, string action, int quantity, int? quantityMin, int? quantityMax, string requestId, string pricepointId, string testCurrency, FacebookDelegate<IPayResult> callback)
		{
			ShowEmptyMockDialog(OnPayComplete, callback, "Mock Pay Dialog");
		}

		public void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback)
		{
			if (callback != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("callback_id", base.CallbackManager.AddFacebookDelegate(callback));
				Dictionary<string, object> dictionary2 = dictionary;
				if (AccessToken.CurrentAccessToken == null)
				{
					dictionary2["error"] = "No current access token";
				}
				else
				{
					IDictionary<string, object> source = (IDictionary<string, object>)Json.Deserialize(AccessToken.CurrentAccessToken.ToJson());
					dictionary2.AddAllKVPFrom(source);
				}
				OnRefreshCurrentAccessTokenComplete(dictionary2.ToJson());
			}
		}

		public override void OnAppRequestsComplete(string message)
		{
			AppRequestResult result = new AppRequestResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGetAppLinkComplete(string message)
		{
			AppLinkResult result = new AppLinkResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGroupCreateComplete(string message)
		{
			GroupCreateResult result = new GroupCreateResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnGroupJoinComplete(string message)
		{
			GroupJoinResult result = new GroupJoinResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public override void OnLoginComplete(string message)
		{
			LoginResult result = new LoginResult(message);
			OnAuthResponse(result);
		}

		public override void OnShareLinkComplete(string message)
		{
			ShareResult result = new ShareResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnAppInviteComplete(string message)
		{
			AppInviteResult result = new AppInviteResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnFetchDeferredAppLinkComplete(string message)
		{
			AppLinkResult result = new AppLinkResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnPayComplete(string message)
		{
			PayResult result = new PayResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnRefreshCurrentAccessTokenComplete(string message)
		{
			AccessTokenRefreshResult result = new AccessTokenRefreshResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnFacebookAuthResponseChange(string message)
		{
			throw new NotSupportedException();
		}

		public void OnUrlResponse(string message)
		{
			throw new NotSupportedException();
		}

		private void ShowEmptyMockDialog<T>(EditorFacebookMockDialog.OnComplete callback, FacebookDelegate<T> userCallback, string title) where T : IResult
		{
			EmptyMockDialog component = ComponentFactory.GetComponent<EmptyMockDialog>();
			component.Callback = callback;
			component.CallbackID = base.CallbackManager.AddFacebookDelegate(userCallback);
			component.EmptyDialogTitle = title;
		}

		private void ShowMockShareDialog(string subTitle, FacebookDelegate<IShareResult> userCallback)
		{
			MockShareDialog component = ComponentFactory.GetComponent<MockShareDialog>();
			component.SubTitle = subTitle;
			component.Callback = EditorGameObject.OnShareLinkComplete;
			component.CallbackID = base.CallbackManager.AddFacebookDelegate(userCallback);
		}
	}
}
