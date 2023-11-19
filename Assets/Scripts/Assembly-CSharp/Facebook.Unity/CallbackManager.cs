using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class CallbackManager
	{
		private IDictionary<string, object> facebookDelegates = new Dictionary<string, object>();

		private int nextAsyncId;

		public string AddFacebookDelegate<T>(FacebookDelegate<T> callback) where T : IResult
		{
			if (callback == null)
			{
				return null;
			}
			nextAsyncId++;
			facebookDelegates.Add(nextAsyncId.ToString(), callback);
			return nextAsyncId.ToString();
		}

		public void OnFacebookResponse(IInternalResult result)
		{
			object value;
			if (result != null && result.CallbackId != null && facebookDelegates.TryGetValue(result.CallbackId, out value))
			{
				CallCallback(value, result);
				facebookDelegates.Remove(result.CallbackId);
			}
		}

		private static void CallCallback(object callback, IResult result)
		{
			if (callback == null || result == null || TryCallCallback<IAppRequestResult>(callback, result) || TryCallCallback<IShareResult>(callback, result) || TryCallCallback<IGroupCreateResult>(callback, result) || TryCallCallback<IGroupJoinResult>(callback, result) || TryCallCallback<IPayResult>(callback, result) || TryCallCallback<IAppInviteResult>(callback, result) || TryCallCallback<IAppLinkResult>(callback, result) || TryCallCallback<ILoginResult>(callback, result) || TryCallCallback<IAccessTokenRefreshResult>(callback, result))
			{
				return;
			}
			throw new NotSupportedException("Unexpected result type: " + callback.GetType().FullName);
		}

		private static bool TryCallCallback<T>(object callback, IResult result) where T : IResult
		{
			FacebookDelegate<T> facebookDelegate = callback as FacebookDelegate<T>;
			if (facebookDelegate != null)
			{
				facebookDelegate((T)result);
				return true;
			}
			return false;
		}
	}
}
