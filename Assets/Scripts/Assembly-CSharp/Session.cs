using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Allies;
using Messages;
using MiniJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Session
{
	public class FramerateWatcher
	{
		public float frequency = 0.5f;

		private float accum;

		private int frames;

		private float waitTime;

		private float prevWindowsFPS;

		public float Framerate
		{
			get
			{
				return prevWindowsFPS;
			}
		}

		public void Update()
		{
			accum += Time.timeScale / Time.deltaTime;
			frames++;
			waitTime += Time.deltaTime;
			if (waitTime > frequency)
			{
				waitTime = 0f;
				prevWindowsFPS = accum / (float)frames;
				accum = 0f;
				frames = 0;
			}
		}
	}

	public class Authorizing
	{
		private bool _finishedLogin;

		private bool _isFacebookAuth;

		public void OnEnter(Session session, bool doFacebookAuth, string fbAccessToken)
		{
			TFUtils.DebugLog("Starting to User login");
			_finishedLogin = false;
			_isFacebookAuth = doFacebookAuth;
			Player.LoadFromNetwork("userLogin", session, doFacebookAuth, fbAccessToken);
		}

		public void OnLeave(Session session)
		{
		}

		public void OnUpdate(Session session)
		{
			if (_finishedLogin)
			{
				return;
			}
			if (session.PlayerIsLoggedIn())
			{
				TFUtils.DebugLog("User logged In");
				_finishedLogin = true;
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)session.CheckAsyncRequest("userLogin");
			if (dictionary == null)
			{
				return;
			}
			bool flag = session.Server.IsNetworkError(dictionary);
			bool flag2 = true;
			if (dictionary.ContainsKey("success"))
			{
				flag2 = (bool)dictionary["success"];
			}
			if (flag || !flag2)
			{
				if (Session.OnSessionUserLoginFail != null)
				{
					Session.OnSessionUserLoginFail();
				}
				else
				{
					TFUtils.ErrorLog("User Login failed due network error");
				}
			}
			else
			{
				session.ThePlayer = Player.LoadFromDataDict(dictionary);
			}
			if (session.ThePlayer != null)
			{
				session.TheGame.SetPlayer(session.ThePlayer);
				session.ThePlayer.SaveLocally();
				session.WebFileServer.SetPlayerInfo(session.ThePlayer);
			}
		}

		public bool IsLoggedIn()
		{
			return _finishedLogin;
		}
	}

	public delegate void GameloopAction();

	public delegate void AsyncAction();

	private const string LOAD_GAME = "loadGame";

	private const string DELETE_GAME = "deleteGame";

	private const string GET_USERINFO = "getUserInfo";

	private const string GET_SERVERVERSION = "getServerVersion";

	private const string GET_MESSAGES_LIST = "getMessagesList";

	private const string GET_MESSAGE = "getMessage";

	private const string SEND_MESSAGE = "sendMessage";

	private const string DELETE_MESSAGE = "deleteMessage";

	private const string TEST_CONNECTIVITY = "testConnectivity";

	private const string GET_FRIENDS_LIST = "getFriendsList";

	private const string GET_EXPLORERS_LIST = "getExplorersList";

	private const string GET_FRIEND_REQUESTS = "getFriendRequests";

	private const string CONFIRM_FRIEND_REQUEST = "confirmFriendRequest";

	private const string DENY_FRIEND_REQUEST = "denyFriendRequest";

	private const string REQUEST_FRIEND = "requestFriend";

	private const string REMOVE_FRIEND = "removeFriend";

	private const string USER_LOGIN = "userLogin";

	private SessionManager.AssignFacebookIDToUserCallback assignFacebookIDToUserCallback;

	public string manifestVersion = "0";

	private AndroidJavaObject androidActivity;

	private Player player;

	private SQServer server;

	private SQWebFileServer webFileServer;

	private SQAuth auth;

	private Game game;

	private Authorizing authorizing;

	private int currentVersion;

	private bool messageListLoaded;

	private List<string> queuedResponses = new List<string>();

	private bool needsReload;

	private Dictionary<string, TFServer.JsonResponseHandler> externalRequests = new Dictionary<string, TFServer.JsonResponseHandler>();

	private Dictionary<string, object> asyncRequests = new Dictionary<string, object>();

	private Dictionary<string, TFWebFileResponse> asyncFileRequests = new Dictionary<string, TFWebFileResponse>();

	private SQContentPatcher contentPatcher;

	private bool _finishedPatching;

	private Thread _validationThread;

	private object _validationLock = new object();

	public SQServer Server
	{
		get
		{
			return server;
		}
	}

	public SQWebFileServer WebFileServer
	{
		get
		{
			return webFileServer;
		}
	}

	public string Username
	{
		set
		{
			webFileServer.Username = value;
		}
	}

	public SQAuth Auth
	{
		get
		{
			return auth;
		}
	}

	public Game TheGame
	{
		get
		{
			return game;
		}
		set
		{
			game = value;
		}
	}

	public Player ThePlayer
	{
		get
		{
			return player;
		}
		set
		{
			player = value;
		}
	}

	public string UpdateUrl { get; private set; }

	public string IosUpdateUrl { get; private set; }

	public string AndroidUpdateUrl { get; private set; }

	public string AmazonUpdateUrl { get; private set; }

	public string ChatSwitch { get; private set; }

	public bool ValidatingLastPatch
	{
		get
		{
			return _validationThread != null;
		}
	}

	public static event Action OnSessionUserLoginFail;

	public Session(int currentVersion, string fbid, bool doFacebookLogin, string fbAccessToken)
	{
		TFUtils.Init(fbid);
		TFUtils.DebugLog("Trying to create the session...");
		authorizing = new Authorizing();
		CookieContainer cookies = new CookieContainer();
		server = new SQServer(cookies);
		webFileServer = new SQWebFileServer(cookies);
		auth = new SQAuth(Application.platform);
		this.currentVersion = currentVersion;
		OnInit();
		authorizing.OnEnter(this, doFacebookLogin, fbAccessToken);
	}

	static Session()
	{
	}

	public void ProcessAsyncResponse(string key)
	{
		TFWebFileResponse tFWebFileResponse = CheckAsyncFileRequest(key);
		if (tFWebFileResponse == null)
		{
			return;
		}
		switch (key)
		{
		case "loadGame":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (gamedata). Loading from network response");
				Dictionary<string, object> asJSONDict = tFWebFileResponse.GetAsJSONDict();
				if (asJSONDict != null && asJSONDict.ContainsKey("PlayerName"))
				{
					try
					{
						string text = tFWebFileResponse.Data.ToString();
						int num = text.IndexOf("HasAuthenticated");
						num += "HasAuthenticated".Length + 2;
						text = text.Remove(num, 1);
						text = text.Insert(num, "1");
						game.SaveLocally(text);
					}
					catch
					{
						game.SaveLocally(tFWebFileResponse.Data);
					}
					SessionManager.loginCompletedWithoutError = true;
				}
				else
				{
					TFUtils.DebugLog("Server returned invalid gamedata: " + tFWebFileResponse.Data);
				}
				break;
			}
			TFUtils.DebugLog(string.Concat("Server returned status ", tFWebFileResponse.StatusCode, ". Loading from local data"));
			SessionManager.loginCompletedWithoutError = tFWebFileResponse.StatusCode == HttpStatusCode.NotFound || tFWebFileResponse.StatusCode == HttpStatusCode.NotModified;
			if (game.GameExists(player))
			{
				if (tFWebFileResponse.StatusCode == HttpStatusCode.NotAcceptable)
				{
					try
					{
						string text2 = game.LoadLocally();
						int num2 = text2.IndexOf("PaidHardCurrency");
						num2 += "PaidHardCurrency".Length + 2;
						int num3 = text2.IndexOf(',', num2);
						int count = num3 - num2;
						text2 = text2.Remove(num2, count);
						text2 = text2.Insert(num2, "0");
						num2 = text2.IndexOf("FreeHardCurrency");
						num2 += "FreeHardCurrency".Length + 2;
						num3 = text2.IndexOf(',', num2);
						count = num3 - num2;
						text2 = text2.Remove(num2, count);
						text2 = text2.Insert(num2, "5");
						num2 = text2.IndexOf("Zxcvbnm");
						num2 += "Zxcvbnm".Length + 2;
						num3 = text2.IndexOf(',', num2);
						count = num3 - num2;
						text2 = text2.Remove(num2, count);
						text2 = text2.Insert(num2, "1");
						game.SaveLocally(text2);
						TFUtils.DebugLog("Normal response, but it's a suspicious data - Reset HardCurrencies.");
					}
					catch
					{
						game.SaveLocally(tFWebFileResponse.Data);
					}
				}
				TFUtils.DebugLog("Creating game from local file");
			}
			else if (tFWebFileResponse.StatusCode == HttpStatusCode.NotFound)
			{
				TFUtils.DebugLog("Initializing new game");
				WebFileServer.DeleteETagFile();
			}
			else if (tFWebFileResponse.StatusCode == HttpStatusCode.NotModified)
			{
				TFUtils.DebugLog(string.Concat("What is going on? This is not an expected outcome: response status ", tFWebFileResponse.StatusCode, " Network down: ", tFWebFileResponse.NetworkDown));
				WebFileServer.DeleteETagFile();
			}
			else
			{
				TFUtils.DebugLog(string.Concat("What is going on? This is not an expected outcome: response status ", tFWebFileResponse.StatusCode, " Network down: ", tFWebFileResponse.NetworkDown));
			}
			break;
		case "deleteGame":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (delete game).");
			}
			else
			{
				TFUtils.DebugLog(string.Concat("Server returned status ", tFWebFileResponse.StatusCode, ". Nothing we can do...."));
			}
			break;
		case "getMessagesList":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (my messages). Loading from network response");
				if (game != null)
				{
					game.MyMessagesList = ProcessMessageListData(tFWebFileResponse.Data);
				}
				Message.ClearMessage();
			}
			else
			{
				messageListLoaded = true;
				Message.GotallMessagesCallback(null, tFWebFileResponse);
			}
			break;
		case "getServerVersion":
		case "testConnectivity":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				if (game != null)
				{
					game.MyServerVersion = ProcessVersionData(tFWebFileResponse.Data);
				}
			}
			else
			{
				game.MyServerVersion = new Version(0, 0);
			}
			break;
		case "getUserInfo":
		case "getMessage":
		case "getFriendsList":
		case "getExplorersList":
		case "getFriendRequests":
		case "confirmFriendRequest":
		case "denyFriendRequest":
		case "requestFriend":
		case "removeFriend":
			if (tFWebFileResponse.StatusCode == HttpStatusCode.OK)
			{
				TFUtils.DebugLog("Server returned success (" + key + "). Loading from network response");
				if (game != null)
				{
					TFUtils.DebugLog("Return = " + tFWebFileResponse.Data);
					switch (key)
					{
					case "getUserInfo":
						game.MyUserInfo = tFWebFileResponse.Data;
						break;
					case "getMessage":
						game.MyMessages.Add(tFWebFileResponse.Data);
						ProcessedMessageData();
						break;
					case "getFriendsList":
						game.MyFriendsList = tFWebFileResponse.Data;
						break;
					case "getExplorersList":
						game.MyExplorersList = tFWebFileResponse.Data;
						break;
					case "getFriendRequests":
						game.MyFriendRequests = tFWebFileResponse.Data;
						break;
					}
				}
			}
			else
			{
				TFUtils.DebugLog(string.Concat("Server returned status ", tFWebFileResponse.StatusCode, ". Nothing we can do...."));
				game.MyUserInfo = "{\"error\":\"no data\"}";
			}
			switch (key)
			{
			case "getFriendsList":
				game.MyFriendsList = tFWebFileResponse.Data;
				Ally.AlliesListCallback(ThePlayer.playerId, tFWebFileResponse);
				break;
			case "denyFriendRequest":
				Ally.DenyAllyRequestCallback(tFWebFileResponse);
				break;
			case "getFriendRequests":
				game.MyFriendRequests = tFWebFileResponse.Data;
				Ally.AllyRequestListCallback(ThePlayer.playerId, tFWebFileResponse);
				break;
			case "removeFriend":
				Ally.RemoveAllyCallback(tFWebFileResponse);
				break;
			}
			break;
		}
		game.AccessDone = true;
	}

	public void OnUpdate()
	{
		authorizing.OnUpdate(this);
		ProcessAsyncResponses();
	}

	public void ReloadGame()
	{
		needsReload = false;
		SceneManager.LoadScene("AppReloadScene");
	}

	public void LoadGameFromNetwork()
	{
		game.LoadFromNetwork("loadGame", this);
	}

	public void DeleteGameFromNetwork()
	{
		game.DeleteFromNetwork("deleteGame", this);
	}

	public void SendMessage(string to_id, string subject, string message)
	{
		game.SendMessage("sendMessage", to_id, subject, message, this);
	}

	public void DeleteMessage(string msg_id)
	{
		game.DeleteMessage("deleteMessage", msg_id, this);
	}

	public void GetMessagesList()
	{
		game.GetMessagesList("getMessagesList", this);
	}

	public void GetMessage(string id)
	{
		game.GetMessage("getMessage", id, this);
	}

	public void GetUserInfo()
	{
		game.GetUserInfo("getUserInfo", this);
	}

	public void GetServerVersion()
	{
		game.GetServerVersion("getServerVersion", this);
	}

	public void TestConnectivity()
	{
		game.GetServerVersion("testConnectivity", this);
	}

	public void GetFriendsList()
	{
		game.GetFriendsList("getFriendsList", this);
	}

	public void GetExplorersList()
	{
		game.GetExplorersList("getExplorersList", this);
	}

	public void GetFriendRequests()
	{
		game.GetFriendRequests("getFriendRequests", this);
	}

	public void ConfirmFriendRequest(string id)
	{
		game.ConfirmFriendRequest("confirmFriendRequest", id, this);
	}

	public void DenyFriendRequest(string id)
	{
		game.DenyFriendRequest("denyFriendRequest", id, this);
	}

	public void RequestFriend(string id)
	{
		game.RequestFriend("requestFriend", id, this);
	}

	public void RemoveFriend(string id)
	{
		game.RemoveFriend("removeFriend", id, this);
	}

	public void GetServerTime()
	{
		TFServer.JsonResponseHandler handler = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			if (status == HttpStatusCode.OK)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)data["data"];
				DateTime serverTime = DateTime.Parse(dictionary["server_time"].ToString());
				TFUtils.UpdateServerTime(serverTime);
			}
		};
		Server.GetTime(handler);
	}

	public bool IsLoggedIn()
	{
		return authorizing.IsLoggedIn();
	}

	public bool IsMessagelistLoaded()
	{
		return messageListLoaded;
	}

	public int GetLocalVersion()
	{
		return currentVersion;
	}

	public bool PlayerIsLoggedIn()
	{
		return player != null;
	}

	public void onExternalMessage(string msg)
	{
		TFUtils.DebugLog("decoding message: " + msg);
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(msg);
		string text = dictionary["requestId"] as string;
		if (externalRequests.ContainsKey(text))
		{
			TFServer.JsonResponseHandler jsonResponseHandler = externalRequests[text];
			externalRequests.Remove(text);
			if (dictionary["data"] is Dictionary<string, object>)
			{
				jsonResponseHandler(dictionary["data"] as Dictionary<string, object>, HttpStatusCode.OK);
			}
			else
			{
				TFUtils.ErrorLog("Callback result is not a Dictionary<string, object>");
			}
		}
		else
		{
			TFUtils.DebugLog("No handler found for id: " + text);
		}
	}

	public void registerExternalCallback(string requestId, TFServer.JsonResponseHandler callback)
	{
		externalRequests[requestId] = callback;
	}

	public AndroidJavaObject getAndroidActivity()
	{
		if (androidActivity == null)
		{
			int num = AndroidJNI.AttachCurrentThread();
			TFUtils.DebugLog("attach result: " + num);
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			androidActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		return androidActivity;
	}

	private List<string> ProcessMessageListData(string data)
	{
		List<object> list = (List<object>)Json.Deserialize(data);
		List<string> list2 = new List<string>();
		foreach (object item in list)
		{
			list2.Add((string)item);
		}
		GetNextMessage(list2);
		return list2;
	}

	private void ProcessedMessageData()
	{
		game.MyMessagesList.RemoveAt(0);
		GetNextMessage(game.MyMessagesList);
	}

	private void GetNextMessage(List<string> list)
	{
		if (list == null || list.Count == 0)
		{
			messageListLoaded = true;
			TFWebFileResponse tFWebFileResponse = new TFWebFileResponse();
			tFWebFileResponse.StatusCode = HttpStatusCode.OK;
			Message.GotallMessagesCallback(game.MyMessages, tFWebFileResponse);
		}
		else
		{
			GetMessage(list[0]);
		}
	}

	private Version ProcessVersionData(string response)
	{
		object obj = Json.Deserialize(response);
		if (obj != null)
		{
			Dictionary<string, object> data = (Dictionary<string, object>)obj;
			string text = null;
			IosUpdateUrl = TFUtils.TryLoadString(data, "ios_url");
			AndroidUpdateUrl = TFUtils.TryLoadString(data, "android_url");
			AmazonUpdateUrl = TFUtils.TryLoadString(data, "amazon_url");
			ChatSwitch = TFUtils.TryLoadString(data, "chat_switch");
			if (ChatSwitch == null || ChatSwitch == string.Empty)
			{
				ChatSwitch = "1";
			}
			if (TFUtils.AmazonDevice)
			{
				text = TFUtils.TryLoadString(data, "amazon_version");
				UpdateUrl = AmazonUpdateUrl;
			}
			else
			{
				text = TFUtils.TryLoadString(data, "android_version");
				UpdateUrl = AndroidUpdateUrl;
			}
			if (text == null)
			{
				text = TFUtils.TryLoadString((Dictionary<string, object>)obj, "version");
			}
			if (text != null)
			{
				return new Version(text);
			}
		}
		return new Version(1, 0);
	}

	protected void ProcessAsyncResponses()
	{
		if (queuedResponses.Count <= 0)
		{
			return;
		}
		List<string> list = new List<string>(queuedResponses);
		foreach (string item in list)
		{
			ProcessAsyncResponse(item);
		}
	}

	protected void QueueResponse(string key)
	{
		queuedResponses.Add(key);
	}

	public void AddAsyncResponse(string key, object val)
	{
		lock (asyncRequests)
		{
			if (asyncRequests.ContainsKey(key))
			{
				TFUtils.DebugLog("Warning: got second async response for " + key + "; Existing value was: " + asyncRequests[key]);
			}
			asyncRequests[key] = val;
		}
	}

	public object CheckAsyncRequest(string key)
	{
		object result = null;
		lock (asyncRequests)
		{
			if (asyncRequests.ContainsKey(key))
			{
				result = asyncRequests[key];
				asyncRequests.Remove(key);
				return result;
			}
			return result;
		}
	}

	public TFServer.JsonResponseHandler AsyncResponder(string key)
	{
		return delegate(Dictionary<string, object> response, HttpStatusCode status)
		{
			AddAsyncResponse(key, response);
		};
	}

	public void AddAsyncFileResponse(string key, TFWebFileResponse val)
	{
		lock (asyncFileRequests)
		{
			asyncFileRequests[key] = val;
			game.AccessDone = false;
			QueueResponse(key);
		}
	}

	public TFWebFileResponse CheckAsyncFileRequest(string key)
	{
		TFWebFileResponse result = null;
		lock (asyncFileRequests)
		{
			if (asyncFileRequests.ContainsKey(key))
			{
				result = asyncFileRequests[key];
				asyncFileRequests.Remove(key);
				return result;
			}
			return result;
		}
	}

	public TFWebFileServer.FileCallbackHandler AsyncFileResponder(string key)
	{
		return delegate(TFWebFileResponse response)
		{
			AddAsyncFileResponse(key, response);
		};
	}

	private void OnInit()
	{
		_validationThread = null;
		_finishedPatching = false;
	}

	public void PatchingEventListener(string patchingEvent)
	{
		if (patchingEvent == "patchingDone" || patchingEvent == "patchingNotNecessary")
		{
			_finishedPatching = true;
		}
	}

	private void OnDispose()
	{
		lock (_validationLock)
		{
			if (_validationThread != null)
			{
				_validationThread.Abort();
				_validationThread.Join();
				_validationThread = null;
			}
		}
	}

	public bool IsPatchDone()
	{
		return _finishedPatching;
	}

	public void ValidateLastPatch()
	{
		lock (_validationLock)
		{
			if (_validationThread != null)
			{
				return;
			}
			SQContentPatcher patcher = new SQContentPatcher();
			Session me = this;
			_validationThread = new Thread((ThreadStart)delegate
			{
				patcher.ValidateAndFixDownloadedManifests();
				lock (me._validationLock)
				{
					me._validationThread = null;
				}
			});
			_validationThread.Start();
		}
	}

	public bool UpdatePatching()
	{
		if (contentPatcher != null || ValidatingLastPatch)
		{
			return contentPatcher != null;
		}
		TFUtils.DebugLog("UpdatePatching - contentPatcher is null");
		contentPatcher = new SQContentPatcher();
		SQContentPatcher sQContentPatcher = contentPatcher;
		sQContentPatcher.AddListener(OnPatchingEvent);
		sQContentPatcher.ReadManifests();
		return true;
	}

	private void OnPatchingEvent(string eventStr)
	{
		switch (eventStr)
		{
		case "patchingNecessary":
			_finishedPatching = true;
			contentPatcher.StartDownloadingPatchedContent();
			break;
		case "patchingDone":
			_finishedPatching = true;
			if (contentPatcher != null && contentPatcher.ContentChanged && SessionManager.Instance.IsLoadDataDone())
			{
				needsReload = true;
			}
			contentPatcher = null;
			break;
		case "patchingNotNecessary":
			contentPatcher = null;
			break;
		}
	}

	public void StartPatch()
	{
		TFUtils.DebugLog("Starting to Patch content");
		_finishedPatching = false;
		UpdatePatching();
	}
}
