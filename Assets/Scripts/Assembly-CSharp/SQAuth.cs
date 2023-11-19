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
		bool flag;
		int num;
		if (doFacebookAuth)
		{
			flag = KFFSendWWWRequestWithFormFunction != null;
		}
		else
			num = 0;
		PlatformAuth2(session, callback, doFacebookAuth, fbAccessToken);
	}

	private void PlatformAuth2(Session session, TFServer.JsonResponseHandler callback, bool doFacebookAuth, string fbAccessToken)
	{
		switch (platform)
		{
		case RuntimePlatform.Android:
			DoLoginAndroid(doFacebookAuth, session, fbAccessToken, callback);
			return;
		case RuntimePlatform.IPhonePlayer:
			if (doFacebookAuth)
			{
				AuthFromFbAccessToken(session, TFUtils.FacebookID, fbAccessToken, "AccessTokenExpiresAt", callback);
			}
			else
			{
				AuthFromGameCenter(session, TFUtils.FacebookID, TFUtils.FacebookID, callback);
			}
			session.Username = TFUtils.FacebookID;
			return;
		}
		if (ALLOW_DEBUG)
		{
			if (doFacebookAuth)
			{
				AuthFromFbAccessToken(session, TFUtils.FacebookID, fbAccessToken, "AccessTokenExpiresAt", callback);
			}
			else
			{
				AuthFromGameCenter(session, TFUtils.FacebookID, TFUtils.FacebookID, callback);
			}
			session.Username = TFUtils.FacebookID;
		}
	}

	private void AuthFromFbAccessToken(Session session, string playerId, string accessToken, string expDate, TFServer.JsonResponseHandler callback)
	{
		TFUtils.DebugLog("attempting auth to TF");
		session.Server.FbLogin(playerId, accessToken, expDate, currentNonce, callback);
	}

	private void AuthFromGameCenter(Session session, string playerId, string alias, TFServer.JsonResponseHandler callback)
	{
		TFUtils.DebugLog("attempting gc auth to TF");
		session.Server.GcLogin(playerId, alias, currentNonce, callback);
	}

	private void DoLoginIOS(Session session, bool doFacebookAuth, string fbAccessToken, bool doGcAuth, TFServer.JsonResponseHandler callback)
	{
		throw new InvalidOperationException("Unsupported platform for iOS login");
	}

	private void DoLoginAndroid(bool doFacebookAuth, Session session, string fbAccessToken, TFServer.JsonResponseHandler callback)
	{
		if (doFacebookAuth)
		{
			AuthFromFbAccessToken(session, TFUtils.FacebookID, fbAccessToken, "AccessTokenExpiresAt", callback);
		}
		else
		{
			AuthFromGameCenter(session, TFUtils.FacebookID, TFUtils.FacebookID, callback);
		}
		session.Username = TFUtils.FacebookID;
	}
}
