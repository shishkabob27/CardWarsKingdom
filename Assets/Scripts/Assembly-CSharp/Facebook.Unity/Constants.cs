using System;
using System.Globalization;
using UnityEngine;

namespace Facebook.Unity
{
	internal static class Constants
	{
		public const string CallbackIdKey = "callback_id";

		public const string AccessTokenKey = "access_token";

		public const string UrlKey = "url";

		public const string RefKey = "ref";

		public const string ExtrasKey = "extras";

		public const string TargetUrlKey = "target_url";

		public const string CancelledKey = "cancelled";

		public const string ErrorKey = "error";

		public const string OnPayCompleteMethodName = "OnPayComplete";

		public const string OnShareCompleteMethodName = "OnShareLinkComplete";

		public const string OnAppRequestsCompleteMethodName = "OnAppRequestsComplete";

		public const string OnGroupCreateCompleteMethodName = "OnGroupCreateComplete";

		public const string OnGroupJoinCompleteMethodName = "OnJoinGroupComplete";

		public const string GraphAPIVersion = "v2.5";

		public const string GraphUrlFormat = "https://graph.{0}/{1}/";

		public const string UserLikesPermission = "user_likes";

		public const string EmailPermission = "email";

		public const string PublishActionsPermission = "publish_actions";

		public const string PublishPagesPermission = "publish_pages";

		private static FacebookUnityPlatform? currentPlatform;

		public static Uri GraphUrl
		{
			get
			{
				string uriString = string.Format(CultureInfo.InvariantCulture, "https://graph.{0}/{1}/", FB.FacebookDomain, "v2.5");
				return new Uri(uriString);
			}
		}

		public static string GraphApiUserAgent
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", FB.FacebookImpl.SDKUserAgent, UnitySDKUserAgent);
			}
		}

		public static bool IsMobile
		{
			get
			{
				return CurrentPlatform == FacebookUnityPlatform.Android || CurrentPlatform == FacebookUnityPlatform.IOS;
			}
		}

		public static bool IsEditor
		{
			get
			{
				return false;
			}
		}

		public static bool IsWeb
		{
			get
			{
				return CurrentPlatform == FacebookUnityPlatform.WebGL || CurrentPlatform == FacebookUnityPlatform.WebPlayer;
			}
		}

		public static string UnitySDKUserAgentSuffixLegacy
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "Unity.{0}", FacebookSdkVersion.Build);
			}
		}

		public static string UnitySDKUserAgent
		{
			get
			{
				return Utilities.GetUserAgent("FBUnitySDK", FacebookSdkVersion.Build);
			}
		}

		public static bool DebugMode
		{
			get
			{
				return UnityEngine.Debug.isDebugBuild;
			}
		}

		public static FacebookUnityPlatform CurrentPlatform
		{
			get
			{
				if (!currentPlatform.HasValue)
				{
					currentPlatform = GetCurrentPlatform();
				}
				return currentPlatform.Value;
			}
			set
			{
				currentPlatform = value;
			}
		}

		private static FacebookUnityPlatform GetCurrentPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				return FacebookUnityPlatform.Android;
			case RuntimePlatform.IPhonePlayer:
				return FacebookUnityPlatform.IOS;
			case RuntimePlatform.WebGLPlayer:
				return FacebookUnityPlatform.WebGL;
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
				return FacebookUnityPlatform.WebPlayer;
			default:
				return FacebookUnityPlatform.Unknown;
			}
		}
	}
}
