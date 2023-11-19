using System;
using System.Collections.Generic;
using System.Globalization;
using Facebook.MiniJSON;

namespace Facebook.Unity.Canvas
{
	internal sealed class CanvasFacebook : FacebookBase, ICanvasFacebook, ICanvasFacebookCallbackHandler, ICanvasFacebookImplementation, IFacebook, IFacebookCallbackHandler
	{
		private class CanvasUIMethodCall<T> : MethodCall<T> where T : IResult
		{
			private CanvasFacebook canvasImpl;

			private string callbackMethod;

			public CanvasUIMethodCall(CanvasFacebook canvasImpl, string methodName, string callbackMethod)
				: base((FacebookBase)canvasImpl, methodName)
			{
				this.canvasImpl = canvasImpl;
				this.callbackMethod = callbackMethod;
			}

			public override void Call(MethodArguments args)
			{
				UI(base.MethodName, args, base.Callback);
			}

			private void UI(string method, MethodArguments args, FacebookDelegate<T> callback = null)
			{
				canvasImpl.canvasJSWrapper.DisableFullScreen();
				MethodArguments methodArguments = new MethodArguments(args);
				methodArguments.AddString("app_id", canvasImpl.appId);
				methodArguments.AddString("method", method);
				string text = canvasImpl.CallbackManager.AddFacebookDelegate(callback);
				canvasImpl.canvasJSWrapper.ExternalCall("FBUnity.ui", methodArguments.ToJsonString(), text, callbackMethod);
			}
		}

		internal const string MethodAppRequests = "apprequests";

		internal const string MethodFeed = "feed";

		internal const string MethodPay = "pay";

		internal const string MethodGameGroupCreate = "game_group_create";

		internal const string MethodGameGroupJoin = "game_group_join";

		internal const string CancelledResponse = "{\"cancelled\":true}";

		internal const string FacebookConnectURL = "https://connect.facebook.net";

		private const string AuthResponseKey = "authResponse";

		private const string ResponseKey = "response";

		private string appId;

		private string appLinkUrl;

		private ICanvasJSWrapper canvasJSWrapper;

		public override bool LimitEventUsage { get; set; }

		public override string SDKName
		{
			get
			{
				return "FBJSSDK";
			}
		}

		public override string SDKVersion
		{
			get
			{
				return canvasJSWrapper.GetSDKVersion();
			}
		}

		public override string SDKUserAgent
		{
			get
			{
				FacebookUnityPlatform currentPlatform = Constants.CurrentPlatform;
				string productName;
				if (currentPlatform == FacebookUnityPlatform.WebGL || currentPlatform == FacebookUnityPlatform.WebPlayer)
				{
					productName = string.Format(CultureInfo.InvariantCulture, "FBUnity{0}", Constants.CurrentPlatform.ToString());
				}
				else
				{
					FacebookLogger.Warn("Currently running on uknown web platform");
					productName = "FBUnityWebUnknown";
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", base.SDKUserAgent, Utilities.GetUserAgent(productName, FacebookSdkVersion.Build));
			}
		}

		public CanvasFacebook()
			: this(new CanvasJSWrapper(), new CallbackManager())
		{
		}

		public CanvasFacebook(ICanvasJSWrapper canvasJSWrapper, CallbackManager callbackManager)
			: base(callbackManager)
		{
			this.canvasJSWrapper = canvasJSWrapper;
		}

		public void Init(string appId, bool cookie, bool logging, bool status, bool xfbml, string channelUrl, string authResponse, bool frictionlessRequests, string jsSDKLocale, HideUnityDelegate hideUnityDelegate, InitDelegate onInitComplete)
		{
			if (canvasJSWrapper.IntegrationMethodJs == null)
			{
				throw new Exception("Cannot initialize facebook javascript");
			}
			base.Init(hideUnityDelegate, onInitComplete);
			canvasJSWrapper.ExternalEval(canvasJSWrapper.IntegrationMethodJs);
			this.appId = appId;
			bool flag = true;
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("appId", appId);
			methodArguments.AddPrimative("cookie", cookie);
			methodArguments.AddPrimative("logging", logging);
			methodArguments.AddPrimative("status", status);
			methodArguments.AddPrimative("xfbml", xfbml);
			methodArguments.AddString("channelUrl", channelUrl);
			methodArguments.AddString("authResponse", authResponse);
			methodArguments.AddPrimative("frictionlessRequests", frictionlessRequests);
			methodArguments.AddString("version", "v2.5");
			canvasJSWrapper.ExternalCall("FBUnity.init", flag ? 1 : 0, "https://connect.facebook.net", jsSDKLocale, Constants.DebugMode ? 1 : 0, methodArguments.ToJsonString(), status ? 1 : 0);
		}

		public override void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			canvasJSWrapper.DisableFullScreen();
			canvasJSWrapper.ExternalCall("FBUnity.login", permissions, base.CallbackManager.AddFacebookDelegate(callback));
		}

		public override void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback)
		{
			canvasJSWrapper.DisableFullScreen();
			canvasJSWrapper.ExternalCall("FBUnity.login", permissions, base.CallbackManager.AddFacebookDelegate(callback));
		}

		public override void LogOut()
		{
			base.LogOut();
			canvasJSWrapper.ExternalCall("FBUnity.logout");
		}

		public override void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			ValidateAppRequestArgs(message, actionType, objectId, to, filters, excludeIds, maxRecipients, data, title, callback);
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("message", message);
			methodArguments.AddCommaSeparatedList("to", to);
			methodArguments.AddString("action_type", (!actionType.HasValue) ? null : actionType.ToString());
			methodArguments.AddString("object_id", objectId);
			methodArguments.AddList("filters", filters);
			methodArguments.AddList("exclude_ids", excludeIds);
			methodArguments.AddNullablePrimitive("max_recipients", maxRecipients);
			methodArguments.AddString("data", data);
			methodArguments.AddString("title", title);
			CanvasUIMethodCall<IAppRequestResult> canvasUIMethodCall = new CanvasUIMethodCall<IAppRequestResult>(this, "apprequests", "OnAppRequestsComplete");
			canvasUIMethodCall.Callback = callback;
			canvasUIMethodCall.Call(methodArguments);
		}

		public override void ActivateApp(string appId)
		{
			canvasJSWrapper.ExternalCall("FBUnity.activateApp");
		}

		public override void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddUri("link", contentURL);
			methodArguments.AddString("name", contentTitle);
			methodArguments.AddString("description", contentDescription);
			methodArguments.AddUri("picture", photoURL);
			CanvasUIMethodCall<IShareResult> canvasUIMethodCall = new CanvasUIMethodCall<IShareResult>(this, "feed", "OnShareLinkComplete");
			canvasUIMethodCall.Callback = callback;
			canvasUIMethodCall.Call(methodArguments);
		}

		public override void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("to", toId);
			methodArguments.AddUri("link", link);
			methodArguments.AddString("name", linkName);
			methodArguments.AddString("caption", linkCaption);
			methodArguments.AddString("description", linkDescription);
			methodArguments.AddUri("picture", picture);
			methodArguments.AddString("source", mediaSource);
			CanvasUIMethodCall<IShareResult> canvasUIMethodCall = new CanvasUIMethodCall<IShareResult>(this, "feed", "OnShareLinkComplete");
			canvasUIMethodCall.Callback = callback;
			canvasUIMethodCall.Call(methodArguments);
		}

		public void Pay(string product, string action, int quantity, int? quantityMin, int? quantityMax, string requestId, string pricepointId, string testCurrency, FacebookDelegate<IPayResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("product", product);
			methodArguments.AddString("action", action);
			methodArguments.AddPrimative("quantity", quantity);
			methodArguments.AddNullablePrimitive("quantity_min", quantityMin);
			methodArguments.AddNullablePrimitive("quantity_max", quantityMax);
			methodArguments.AddString("request_id", requestId);
			methodArguments.AddString("pricepoint_id", pricepointId);
			methodArguments.AddString("test_currency", testCurrency);
			CanvasUIMethodCall<IPayResult> canvasUIMethodCall = new CanvasUIMethodCall<IPayResult>(this, "pay", "OnPayComplete");
			canvasUIMethodCall.Callback = callback;
			canvasUIMethodCall.Call(methodArguments);
		}

		public override void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("name", name);
			methodArguments.AddString("description", description);
			methodArguments.AddString("privacy", privacy);
			methodArguments.AddString("display", "async");
			CanvasUIMethodCall<IGroupCreateResult> canvasUIMethodCall = new CanvasUIMethodCall<IGroupCreateResult>(this, "game_group_create", "OnGroupCreateComplete");
			canvasUIMethodCall.Callback = callback;
			canvasUIMethodCall.Call(methodArguments);
		}

		public override void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback)
		{
			MethodArguments methodArguments = new MethodArguments();
			methodArguments.AddString("id", id);
			methodArguments.AddString("display", "async");
			CanvasUIMethodCall<IGroupJoinResult> canvasUIMethodCall = new CanvasUIMethodCall<IGroupJoinResult>(this, "game_group_join", "OnJoinGroupComplete");
			canvasUIMethodCall.Callback = callback;
			canvasUIMethodCall.Call(methodArguments);
		}

		public override void GetAppLink(FacebookDelegate<IAppLinkResult> callback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("url", appLinkUrl);
			Dictionary<string, object> obj = dictionary;
			callback(new AppLinkResult(Json.Serialize(obj)));
			appLinkUrl = string.Empty;
		}

		public override void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			canvasJSWrapper.ExternalCall("FBUnity.logAppEvent", logEvent, valueToSum, Json.Serialize(parameters));
		}

		public override void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters)
		{
			canvasJSWrapper.ExternalCall("FBUnity.logPurchase", logPurchase, currency, Json.Serialize(parameters));
		}

		public override void OnLoginComplete(string responseJsonData)
		{
			string response = FormatAuthResponse(responseJsonData);
			OnAuthResponse(new LoginResult(response));
		}

		public override void OnGetAppLinkComplete(string message)
		{
			throw new NotImplementedException();
		}

		public void OnFacebookAuthResponseChange(string responseJsonData)
		{
			string response = FormatAuthResponse(responseJsonData);
			LoginResult loginResult = new LoginResult(response);
			AccessToken.CurrentAccessToken = loginResult.AccessToken;
		}

		public void OnPayComplete(string responseJsonData)
		{
			string result = FormatResult(responseJsonData);
			PayResult result2 = new PayResult(result);
			base.CallbackManager.OnFacebookResponse(result2);
		}

		public override void OnAppRequestsComplete(string responseJsonData)
		{
			string result = FormatResult(responseJsonData);
			AppRequestResult result2 = new AppRequestResult(result);
			base.CallbackManager.OnFacebookResponse(result2);
		}

		public override void OnShareLinkComplete(string responseJsonData)
		{
			string result = FormatResult(responseJsonData);
			ShareResult result2 = new ShareResult(result);
			base.CallbackManager.OnFacebookResponse(result2);
		}

		public override void OnGroupCreateComplete(string responseJsonData)
		{
			string result = FormatResult(responseJsonData);
			GroupCreateResult result2 = new GroupCreateResult(result);
			base.CallbackManager.OnFacebookResponse(result2);
		}

		public override void OnGroupJoinComplete(string responseJsonData)
		{
			string result = FormatResult(responseJsonData);
			GroupJoinResult result2 = new GroupJoinResult(result);
			base.CallbackManager.OnFacebookResponse(result2);
		}

		public void OnUrlResponse(string url)
		{
			appLinkUrl = url;
		}

		private static string FormatAuthResponse(string result)
		{
			if (string.IsNullOrEmpty(result))
			{
				return result;
			}
			IDictionary<string, object> formattedResponseDictionary = GetFormattedResponseDictionary(result);
			IDictionary<string, object> value;
			if (formattedResponseDictionary.TryGetValue<IDictionary<string, object>>("authResponse", out value))
			{
				formattedResponseDictionary.Remove("authResponse");
				foreach (KeyValuePair<string, object> item in value)
				{
					formattedResponseDictionary[item.Key] = item.Value;
				}
			}
			return Json.Serialize(formattedResponseDictionary);
		}

		private static string FormatResult(string result)
		{
			if (string.IsNullOrEmpty(result))
			{
				return result;
			}
			return Json.Serialize(GetFormattedResponseDictionary(result));
		}

		private static IDictionary<string, object> GetFormattedResponseDictionary(string result)
		{
			IDictionary<string, object> dictionary = (IDictionary<string, object>)Json.Deserialize(result);
			IDictionary<string, object> value;
			if (dictionary.TryGetValue<IDictionary<string, object>>("response", out value))
			{
				object value2;
				if (dictionary.TryGetValue("callback_id", out value2))
				{
					value["callback_id"] = value2;
				}
				return value;
			}
			return dictionary;
		}
	}
}
