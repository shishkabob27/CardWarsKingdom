using System;
using System.Collections.Generic;
using Facebook.MiniJSON;

namespace Facebook.Unity.Mobile
{
	internal abstract class MobileFacebook : FacebookBase, IFacebook, IFacebookCallbackHandler, IMobileFacebook, IMobileFacebookCallbackHandler, IMobileFacebookImplementation
	{
		private const string CallbackIdKey = "callback_id";

		private ShareDialogMode shareDialogMode;

		public ShareDialogMode ShareDialogMode
		{
			get
			{
				return shareDialogMode;
			}
			set
			{
				shareDialogMode = value;
				SetShareDialogMode(shareDialogMode);
			}
		}

		protected MobileFacebook(CallbackManager callbackManager)
			: base(callbackManager)
		{
		}

		public abstract void AppInvite(Uri appLinkUrl, Uri previewImageUrl, FacebookDelegate<IAppInviteResult> callback);

		public abstract void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback);

		public abstract void RefreshCurrentAccessToken(FacebookDelegate<IAccessTokenRefreshResult> callback);

		public override void OnLoginComplete(string message)
		{
			LoginResult result = new LoginResult(message);
			OnAuthResponse(result);
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

		public override void OnAppRequestsComplete(string message)
		{
			AppRequestResult result = new AppRequestResult(message);
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

		public override void OnShareLinkComplete(string message)
		{
			ShareResult result = new ShareResult(message);
			base.CallbackManager.OnFacebookResponse(result);
		}

		public void OnRefreshCurrentAccessTokenComplete(string message)
		{
			AccessTokenRefreshResult accessTokenRefreshResult = new AccessTokenRefreshResult(message);
			if (accessTokenRefreshResult.AccessToken != null)
			{
				AccessToken.CurrentAccessToken = accessTokenRefreshResult.AccessToken;
			}
			base.CallbackManager.OnFacebookResponse(accessTokenRefreshResult);
		}

		protected abstract void SetShareDialogMode(ShareDialogMode mode);

		private static IDictionary<string, object> DeserializeMessage(string message)
		{
			return (Dictionary<string, object>)Json.Deserialize(message);
		}

		private static string SerializeDictionary(IDictionary<string, object> dict)
		{
			return Json.Serialize(dict);
		}

		private static bool TryGetCallbackId(IDictionary<string, object> result, out string callbackId)
		{
			callbackId = null;
			object value;
			if (result.TryGetValue("callback_id", out value))
			{
				callbackId = value as string;
				return true;
			}
			return false;
		}

		private static bool TryGetError(IDictionary<string, object> result, out string errorMessage)
		{
			errorMessage = null;
			object value;
			if (result.TryGetValue("error", out value))
			{
				errorMessage = value as string;
				return true;
			}
			return false;
		}
	}
}
