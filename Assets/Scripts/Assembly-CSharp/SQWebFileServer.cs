using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SQWebFileServer : TFWebFileServer
{
	private const string PERSISTENCE_PATH = "persist";

	private const string STATIC_PATH = "static";

	private const string GAME_DATA_FIELD = "game";

	private string eTagFile;

	private string playerGameUri;

	private string messageUri;

	private string messagesListUri;

	private string messageSendUri;

	private string messageDeleteUri;

	private string serverVersionUri;

	private string friendsAllRequestsSentUri;

	private string friendsAllRequestsRcvdUri;

	private string friendsListUri;

	private string friendsGetExplorersUri;

	private string friendsConfirmUserUri;

	private string friendsDenyUserUri;

	private string friendsRequestUri;

	private string friendsRemoveUri;

	private string friendsUserInfoUri;

	private string scoreUri;

	private string username;

	public DateTime LastSuccessfulSave;

	public string Username
	{
		set
		{
			username = value;
		}
	}

	public SQWebFileServer(CookieContainer cookies)
		: base(cookies)
	{
	}

	public void SetPlayerInfo(Player player)
	{
		eTagFile = player.CacheFile("lastETag");
		string arg = string.Format("{0}/{1}", SQSettings.SERVER_URL, "persist");
		serverVersionUri = string.Format("{0}/{1}/version.txt", SQSettings.SERVER_URL, "static");
		playerGameUri = string.Format("{0}/{1}/{2}", arg, player.playerId, "game");
		messageUri = string.Format("{0}/messages_get", arg);
		messagesListUri = string.Format("{0}/messages_received_ids", arg);
		messageSendUri = string.Format("{0}/messages_send", arg);
		messageDeleteUri = string.Format("{0}/messages_delete", arg);
		friendsAllRequestsRcvdUri = string.Format("{0}/friends_all_requests_received", arg);
		friendsListUri = string.Format("{0}/friends", arg);
		friendsGetExplorersUri = string.Format("{0}/friends_get_explorers", arg);
		friendsConfirmUserUri = string.Format("{0}/friends_confirm_request", arg);
		friendsDenyUserUri = string.Format("{0}/friends_deny_request", arg);
		friendsRequestUri = string.Format("{0}/friends_request", arg);
		friendsRemoveUri = string.Format("{0}/friends_remove", arg);
		friendsUserInfoUri = string.Format("{0}/user_info", arg);
	}

	public void GetGameData(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting game data from " + playerGameUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		string text = ReadETag();
		if (text != null)
		{
			webHeaderCollection.Add(HttpRequestHeader.IfNoneMatch, text);
		}
		try
		{
			GetFile(playerGameUri, webHeaderCollection, GetGameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void DeleteGameData(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Deleting game data from " + playerGameUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			DeleteFile(playerGameUri, webHeaderCollection, DeleteGameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void AssignFacebookIDToUser(FileCallbackHandler callback, string facebookID, object userData = null)
	{
		Debug.Log("Assigning facebook ID to user from " + playerGameUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			string arg = string.Format("{0}/{1}", SQSettings.SERVER_URL, "persist");
			string uri = string.Format("{0}/{1}/{2}", arg, "user_add_facebook", facebookID);
			GetFile(uri, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetServerVersion(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting Info from " + serverVersionUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(serverVersionUri, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetUserInfo(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting User Info from " + friendsUserInfoUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(friendsUserInfoUri, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void SendMessage(FileCallbackHandler callback, string to_id, string subject, string message, object userData = null)
	{
		string text = string.Format("{0}/{1}?subject={2}&message={3}", messageSendUri, to_id, Uri.EscapeUriString(subject), Uri.EscapeUriString(message));
		Debug.Log("Sending Message to " + text);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(text, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message2)
		{
			Debug.Log(message2);
		}
	}

	public void DeleteMessage(FileCallbackHandler callback, string msg_id, object userData = null)
	{
		string text = string.Format("{0}/{1}", messageDeleteUri, msg_id);
		Debug.Log("Deleting Message ID " + text);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(text, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetMessage(FileCallbackHandler callback, string id, object userData = null)
	{
		string text = string.Format("{0}/{1}", messageUri, id);
		Debug.Log("Getting Message from " + text);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(text, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetMessageList(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting Messages List from " + messagesListUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(messagesListUri, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetFriendRequests(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting Friends Requests from " + friendsAllRequestsRcvdUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(friendsAllRequestsRcvdUri, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetFriends(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting Friends List from " + friendsListUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(friendsListUri + "/" + PlayerInfoScript.Instance.GetPlayerCode(), webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void GetExplorers(FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Getting Explorers List from " + friendsGetExplorersUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(friendsGetExplorersUri, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	private void FriendOp(string uri, FileCallbackHandler callback, string id, object userData = null)
	{
		string text = string.Format("{0}/{1}", uri, id);
		Debug.Log("Friend opearation: " + text);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		try
		{
			GetFile(text, webHeaderCollection, GameCallbackWrapper(callback, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public void ConfirmFriendRequest(FileCallbackHandler callback, string id, object userData = null)
	{
		FriendOp(friendsConfirmUserUri, callback, id, userData);
	}

	public void DenyFriendRequest(FileCallbackHandler callback, string id, object userData = null)
	{
		FriendOp(friendsDenyUserUri, callback, id, userData);
	}

	public void RequestFriend(FileCallbackHandler callback, string id, object userData = null)
	{
		FriendOp(friendsRequestUri, callback, id, userData);
	}

	public void RemoveFriend(FileCallbackHandler callback, string id, object userData = null)
	{
		FriendOp(friendsRemoveUri, callback, id, userData);
	}

	public void SetDefaultHeaders(WebHeaderCollection wc)
	{
	}

	public void SaveGameData(string gameData, FileCallbackHandler callback, object userData = null)
	{
		Debug.Log("Saving game data from " + playerGameUri);
		WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
		SetDefaultHeaders(webHeaderCollection);
		webHeaderCollection.Add(HttpRequestHeader.ContentType, "application/octet-stream");
		webHeaderCollection.Add("x-nick-description", WWW.EscapeURL(TFUtils.DeviceName).Replace("+", "%20"));
		string text = ReadETag();
		if (text != null)
		{
			webHeaderCollection.Add(HttpRequestHeader.IfMatch, text);
		}
		try
		{
			SaveFile(playerGameUri, gameData, webHeaderCollection, SaveGameCallbackWrapper(callback, gameData, webHeaderCollection, true, userData), userData);
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	public FileCallbackHandler SaveGameCallbackWrapper(FileCallbackHandler callback, string gameData, WebHeaderCollection headers, bool retry, object userData = null)
	{
		return delegate(TFWebFileResponse response)
		{
			LastSuccessfulSave = DateTime.Now;
			Debug.Log(string.Concat("Server returned with ", response.StatusCode, " save at :", LastSuccessfulSave));
			if (retry && SetAuthHeader(response, headers, "PUT"))
			{
				try
				{
					SaveFile(playerGameUri, gameData, headers, SaveGameCallbackWrapper(callback, gameData, headers, false, userData), userData);
					return;
				}
				catch (Exception message)
				{
					Debug.Log(message);
					return;
				}
			}
			if (response.headers != null)
			{
				string text = response.headers[HttpResponseHeader.ETag];
				if (text != null)
				{
					SaveETag(text);
				}
			}
			handleResponse(response, callback);
		};
	}

	public FileCallbackHandler GetGameCallbackWrapper(FileCallbackHandler callback, WebHeaderCollection headers, bool retry, object userData = null)
	{
		return delegate(TFWebFileResponse response)
		{
			if (retry && SetAuthHeader(response, headers, "GET"))
			{
				try
				{
					GetFile(playerGameUri, headers, GameCallbackWrapper(callback, headers, false, userData), userData);
					return;
				}
				catch (Exception message)
				{
					Debug.Log(message);
					return;
				}
			}
			if (response.headers != null)
			{
				string text = response.headers[HttpResponseHeader.ETag];
				if (text != null)
				{
					SaveETag(text);
				}
			}
			handleResponse(response, callback);
		};
	}

	public FileCallbackHandler GameCallbackWrapper(FileCallbackHandler callback, WebHeaderCollection headers, bool retry, object userData = null)
	{
		return delegate(TFWebFileResponse response)
		{
			if (retry && SetAuthHeader(response, headers, "GET"))
			{
				try
				{
					GetFile(playerGameUri, headers, GameCallbackWrapper(callback, headers, false, userData), userData);
					return;
				}
				catch (Exception message)
				{
					Debug.Log(message);
					return;
				}
			}
			handleResponse(response, callback);
		};
	}

	public FileCallbackHandler DeleteGameCallbackWrapper(FileCallbackHandler callback, WebHeaderCollection headers, bool retry, object userData = null)
	{
		return delegate(TFWebFileResponse response)
		{
			Debug.Log(string.Concat("Server returned with ", response.StatusCode, " for delete"));
			if (retry && SetAuthHeader(response, headers, "DELETE"))
			{
				try
				{
					DeleteFile(playerGameUri, headers, DeleteGameCallbackWrapper(callback, headers, false, userData), userData);
					return;
				}
				catch (Exception message)
				{
					Debug.Log(message);
					return;
				}
			}
			handleResponse(response, callback);
		};
	}

	private bool SetAuthHeader(TFWebFileResponse response, WebHeaderCollection headers, string method)
	{
		string text = null;
		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			string[] values = response.headers.GetValues("WWW-Authenticate");
			if (values != null && values.Length == 1)
			{
				text = GenerateAuthHeader(values[0], username, "xc0u18^g0!ac3k%0+2vgglmnr1)x^!o(n6@$m3t^(7l!(#kv!-", method, playerGameUri);
				headers.Add(HttpRequestHeader.Authorization, text);
			}
		}
		return text != null;
	}

	private void handleResponse(TFWebFileResponse response, FileCallbackHandler callback)
	{
		callback(response);
	}

	private string GenerateAuthHeader(string serverAuthHeader, string username, string password, string method, string uri)
	{
		if (!serverAuthHeader.StartsWith("Digest "))
		{
			return null;
		}
		serverAuthHeader = serverAuthHeader.Substring("Digest ".Length);
		string[] array = serverAuthHeader.Split(',');
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('=');
			dictionary.Add(array3[0].Trim().ToLower(), array3[1].Trim());
		}
		Uri result;
		if (Uri.TryCreate(uri, UriKind.Absolute, out result))
		{
			uri = new Uri(uri).AbsolutePath;
		}
		string text2 = Unquote(dictionary["nonce"]);
		string text3 = Unquote(dictionary["opaque"]);
		string text4 = "auth";
		string text5 = string.Format("{0:x8}", 1);
		System.Random random = new System.Random();
		byte[] array4 = new byte[8];
		random.NextBytes(array4);
		string text6 = BitConverter.ToString(array4).Replace("-", string.Empty);
		string val = string.Format("{0}:{1}", username, password);
		string text7 = Digest(val);
		string val2 = string.Format("{0}:{1}", method, uri);
		string text8 = Digest(val2);
		string val3 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", text7, text2, text5, text6, text4, text8);
		string text9 = Digest(val3);
		return string.Format("Digest username=\"{0}\", nonce=\"{1}\", uri=\"{2}\", qop=\"{3}\", nc={4}, cnonce=\"{5}\", response=\"{6}\", opaque=\"{7}\"", username, text2, uri, text4, text5, text6, text9, text3);
	}

	private string Unquote(string val)
	{
		if (val.Length < 2 || val[0] != '"' || val[val.Length - 1] != '"')
		{
			return val;
		}
		return val.Substring(1, val.Length - 2);
	}

	private string Digest(string val)
	{
		MD5 mD = MD5.Create();
		Encoding aSCII = Encoding.ASCII;
		byte[] array = mD.ComputeHash(aSCII.GetBytes(val));
		return BitConverter.ToString(array).Replace("-", string.Empty).ToLower();
	}

	public string ReadETag()
	{
		//Discarded unreachable code: IL_002e
		if (File.Exists(eTagFile))
		{
			lock (eTagFile)
			{
				return File.ReadAllText(eTagFile);
			}
		}
		return null;
	}

	public void SaveETag(string newETag)
	{
		lock (eTagFile)
		{
			File.WriteAllText(eTagFile, newETag);
		}
	}

	public void DeleteETagFile()
	{
		File.Delete(eTagFile);
	}
}
