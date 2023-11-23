using System;
using System.Collections.Generic;
using System.Net;
using MiniJSON;
using UnityEngine;

public class SQAuth
{
	public delegate void OnUserDataFn(Session session, int userID);

	public delegate void KFFWWWRequestCallback(object wwwinfo, object obj, string str, object param);

	public delegate object KFFSendWWWRequestWithFormCallback(WWWForm form, string scriptNameAndParams, KFFWWWRequestCallback cb, object callbackParam);

	public delegate string LoadPlayerNameCallback();

	private const string AUTH_REQUEST = "authRequest";

	private const string DO_GC_AUTH = "do_gc_auth";

	private const string SETTINGS = "settings";

	public static bool g_reassignID;

	private bool ALLOW_DEBUG = true;

	public bool loggedIn;

	private string currentNonce;

	private RuntimePlatform platform;

	public static KFFSendWWWRequestWithFormCallback KFFSendWWWRequestWithFormFunction;

	public static LoadPlayerNameCallback LoadPlayerNameFunction;

	public SQAuth(RuntimePlatform platform)
	{
		this.platform = platform;
	}

	public void AuthUser(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		g_reassignID = false;
		CheckNonce(session, callback, doFacebookAuth, fbAccessToken);
	}

	private void CheckNonce(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		if (currentNonce == null)
		{
			TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
			{
				if (status != HttpStatusCode.OK)
				{
					callback((Dictionary<string, object>)Json.Deserialize(TFServer.NETWORK_ERROR_JSON), status);
				}
				else
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
					currentNonce = (string)dictionary["nonce"];
					PlatformAuth(session, callback, doFacebookAuth, fbAccessToken);
				}
			};
			session.Server.PreAuth(callback2);
		}
		else
		{
			PlatformAuth(session, callback, doFacebookAuth, fbAccessToken);
		}
	}

	private void PlatformAuth(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		session.Server.GcLogin(TFUtils.FacebookID, TFUtils.FacebookID, currentNonce, callback);
        session.Username = TFUtils.FacebookID;
    }
}
