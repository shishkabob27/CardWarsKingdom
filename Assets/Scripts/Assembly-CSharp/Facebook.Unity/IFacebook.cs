using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	internal interface IFacebook
	{
		bool LoggedIn { get; }

		bool LimitEventUsage { get; set; }

		string SDKName { get; }

		string SDKVersion { get; }

		string SDKUserAgent { get; }

		bool Initialized { get; }

		void LogInWithPublishPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback);

		void LogInWithReadPermissions(IEnumerable<string> permissions, FacebookDelegate<ILoginResult> callback);

		void LogOut();

		[Obsolete]
		void AppRequest(string message, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback);

		void AppRequest(string message, OGActionType? actionType, string objectId, IEnumerable<string> to, IEnumerable<object> filters, IEnumerable<string> excludeIds, int? maxRecipients, string data, string title, FacebookDelegate<IAppRequestResult> callback);

		void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, FacebookDelegate<IShareResult> callback);

		void FeedShare(string toId, Uri link, string linkName, string linkCaption, string linkDescription, Uri picture, string mediaSource, FacebookDelegate<IShareResult> callback);

		void GameGroupCreate(string name, string description, string privacy, FacebookDelegate<IGroupCreateResult> callback);

		void GameGroupJoin(string id, FacebookDelegate<IGroupJoinResult> callback);

		void API(string query, HttpMethod method, IDictionary<string, string> formData, FacebookDelegate<IGraphResult> callback);

		void API(string query, HttpMethod method, WWWForm formData, FacebookDelegate<IGraphResult> callback);

		void ActivateApp(string appId = null);

		void GetAppLink(FacebookDelegate<IAppLinkResult> callback);

		void AppEventsLogEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters);

		void AppEventsLogPurchase(float logPurchase, string currency, Dictionary<string, object> parameters);
	}
}
