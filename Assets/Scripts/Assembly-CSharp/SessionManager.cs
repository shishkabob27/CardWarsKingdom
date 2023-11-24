using System;
using System.IO;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
	public enum States
	{
		WAITING_FOR_USERID,
		LOGGING_IN,
		LOAD_DATA,
		LOADING,
		VALIDATE_PATCH,
		VERSION_CHECK,
		PATCHING,
		MESSAGE_FETCH,
		SYNCING,
		SAVING,
		QUERYING,
		SENDING,
		DELETING,
		READY
	}

	public delegate void AssignFacebookIDToUserCallback(bool success);

	public delegate void OnReadyDelegate();

	public delegate void OnSaveDelegate(bool success);

	private const int CurrentVersion = 1;

	public static bool loginCompletedWithoutError;

	public static bool loginOnece = true;

	private static SessionManager _instance;

	public GameObject BusyIcon;

	public string PlayerID;

	public string LoginID;

	public string NetState;

	private States state;

	private bool loadingDataFinished;

	private OnReadyDelegate myOnReadyCallback;

	private OnSaveDelegate saveToServerCallback;

	private string saveToServerData;

	private int? saveToServerResponse;

	private OnReadyDelegate attemptConnectionCallback;

	private int? attemptConnectionResponse;

	private bool checkedVersion;

	private Session session;

	public new static SessionManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool NeedsForcedUpdate { get; private set; }

	public bool HasNewMessagesReady
	{
		get
		{
			return session != null && session.TheGame != null && session.TheGame.MyMessages != null && session.TheGame.MyMessages.Count > 0;
		}
	}

	private States State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
			switch (state)
			{
			case States.WAITING_FOR_USERID:
				NetState = "WAITING_FOR_USERID";
				break;
			case States.LOGGING_IN:
				NetState = "LOGGING_IN";
				break;
			case States.LOAD_DATA:
				NetState = "LOAD_DATA";
				break;
			case States.LOADING:
				NetState = "LOADING";
				break;
			case States.PATCHING:
				NetState = "PATCHING";
				break;
			case States.SAVING:
				NetState = "SAVING";
				break;
			case States.SYNCING:
				NetState = "SYNCING";
				break;
			case States.QUERYING:
				NetState = "QUERYING";
				break;
			case States.SENDING:
				NetState = "SENDING";
				break;
			case States.DELETING:
				NetState = "DELETING";
				break;
			case States.READY:
				NetState = "READY";
				break;
			case States.VALIDATE_PATCH:
			case States.VERSION_CHECK:
			case States.MESSAGE_FETCH:
				break;
			}
		}
	}

	public OnReadyDelegate OnReadyCallback
	{
		get
		{
			return myOnReadyCallback;
		}
		set
		{
			myOnReadyCallback = value;
		}
	}

	public Session theSession
	{
		get
		{
			return session;
		}
	}

	public event Action OnUserLoginFail;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	private void Start()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("SessionMgr");
		if (array.Length > 1)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		session = null;
		State = States.WAITING_FOR_USERID;
		KFFNetwork.ToggleManualSleepControl(true);
		Screen.sleepTimeout = -1;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void OnSessionUserLoginFail()
	{
		if (this.OnUserLoginFail != null)
		{
			this.OnUserLoginFail();
		}
	}

	public void Login(bool doFacebookLogin, string fbAccessToken, string fbid)
	{
		Session.OnSessionUserLoginFail += OnSessionUserLoginFail;
		LoginID = ((fbid != null) ? fbid : "default");
		State = States.LOGGING_IN;
		session = new Session(1, fbid, doFacebookLogin, fbAccessToken);
		session.TheGame = new Game();
	}

	public void Logout()
	{
		Session.OnSessionUserLoginFail -= OnSessionUserLoginFail;
		session = null;
		State = States.WAITING_FOR_USERID;
	}

	public bool IsLoggedIn()
	{
		bool result = false;
		if (session != null)
		{
			result = session.IsLoggedIn();
		}
		return result;
	}

	public bool IsReady()
	{
		return IsLoggedIn() && State == States.READY;
	}

	public void StartSyncStreamingAssets()
	{
		session.StartPatch();
	}

	public bool IsPatchingSyncDone()
	{
		return session.IsPatchDone();
	}

	public bool IsMessageSyncDone()
	{
		return session.IsMessagelistLoaded();
	}

	private bool IsSaveDone()
	{
		if (session == null || session.TheGame == null)
		{
			return true;
		}
		return session.TheGame.IsDoneServerAccess();
	}

	public string GetStreamingAssetsPath(string fname)
	{
		string result = Path.Combine(Application.streamingAssetsPath, fname);
		string persistentAssetsPath = TFUtils.GetPersistentAssetsPath();
		if (!string.IsNullOrEmpty(persistentAssetsPath))
		{
			string text = Path.Combine(persistentAssetsPath, fname);
			if (File.Exists(text))
			{
				return text;
			}
		}
		return result;
	}

	public string GetPlayerDataPath(string fname)
	{
		return session.ThePlayer.CacheFile(fname);
	}

	public void SetGameStateJson(string gameData)
	{
		if (session != null && session.TheGame != null)
		{
			session.TheGame.SaveLocally(gameData);
		}
	}

	public void SaveToServer(string gameData, OnSaveDelegate callback)
	{
		if (session != null && session.TheGame != null)
		{
			if (callback == null)
			{
				session.TheGame.SaveToServer(session, gameData);
				return;
			}
			saveToServerData = gameData;
			saveToServerCallback = callback;
			AttemptSave();
		}
	}

	private void AttemptSave()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			saveToServerResponse = 408;
		}
		else
		{
			session.TheGame.SaveToServer(session, saveToServerData, RecordSaveResponse);
		}
	}

	public void RecordSaveResponse(TFWebFileResponse response)
	{
		saveToServerResponse = (int)response.StatusCode;
	}

	public void HandleSaveResponse()
	{
		int? num = saveToServerResponse;
		int num2 = (num.HasValue ? num.Value : 0);
		saveToServerResponse = null;
		bool success = num2 == 200 || num2 == 201;
		saveToServerCallback(success);
	}

	public void AttemptConnection(OnReadyDelegate callback)
	{
		if (session != null && session.TheGame != null && callback != null)
		{
			attemptConnectionCallback = callback;
			AttemptConnection();
		}
	}

	private void AttemptConnection()
	{
		if (Application.internetReachability != 0)
		{
			session.WebFileServer.GetServerVersion(RecordConnectionResponse);
		}
	}

	public void RecordConnectionResponse(TFWebFileResponse response)
	{
		attemptConnectionResponse = (int)response.StatusCode;
	}

	public void HandleConnectionResponse()
	{
		int? num = attemptConnectionResponse;
		int num2 = (num.HasValue ? num.Value : 0);
		attemptConnectionResponse = null;
		if (num2 != 200)
		{
			string text = string.Format("HTTP Status {0}: There was a problem accessing the server.", num2);
		}
		else
		{
			attemptConnectionCallback();
		}
	}

	public void LoadFromServer()
	{
		if (session != null && session.TheGame != null)
		{
			session.LoadGameFromNetwork();
		}
	}

	public void LoadPlayerFromFileSystem()
	{
		if (session != null)
		{
            Debug.LogWarning("User Login failed due network error");
		}
	}

	public void DeleteFromServer()
	{
		if (session != null && session.TheGame != null)
		{
			session.DeleteGameFromNetwork();
		}
	}

	public void DeleteLocal()
	{
		if (session != null && session.TheGame != null)
		{
			session.TheGame.DestroyCache(session.ThePlayer);
		}
	}

	public string GetGameStateJson()
	{
		if (session != null && session.TheGame != null)
		{
			return session.TheGame.LoadLocally();
		}
		return null;
	}

	public void RequestMyUserInfo()
	{
		if (State == States.READY)
		{
			State = States.QUERYING;
			if (session != null && session.TheGame != null)
			{
				session.GetUserInfo();
			}
		}
	}

	public Version GetServerVersion()
	{
		if (session != null && session.TheGame != null)
		{
			return session.TheGame.MyServerVersion;
		}
		return null;
	}

	public void TestConnectivity()
	{
		if (session != null && session.TheGame != null)
		{
			session.TestConnectivity();
		}
	}

	public string GetMyUserInfoJson()
	{
		if (session != null && session.TheGame != null)
		{
			return session.TheGame.MyUserInfo;
		}
		return null;
	}

	public void RequestSendMessage(string to_id, string subject, string message)
	{
		if (State == States.READY)
		{
			State = States.SENDING;
			if (session != null && session.TheGame != null)
			{
				session.SendMessage(to_id, subject, message);
			}
		}
	}

	public void RequestDeleteMessage(string msg_id)
	{
		if (State == States.READY)
		{
			State = States.DELETING;
			if (session != null && session.TheGame != null)
			{
				session.DeleteMessage(msg_id);
			}
		}
	}

	public void RequestMyMessagesList()
	{
		if (State == States.READY)
		{
			State = States.QUERYING;
			if (session != null && session.TheGame != null)
			{
				session.GetMessagesList();
			}
		}
	}

	public void RequestMyMessage(string id)
	{
		if (State == States.READY)
		{
			State = States.QUERYING;
			if (session != null && session.TheGame != null)
			{
				session.GetMessage(id);
			}
		}
	}

	public void GetFriendList()
	{
		session.GetFriendsList();
	}

	public void RequestFriend(string id)
	{
		session.RequestFriend(id);
	}

	public void GetFriendRequests()
	{
		session.GetFriendRequests();
	}

	public void ConfirmFriendRequest(string id)
	{
		session.ConfirmFriendRequest(id);
	}

	public void DenyFriendRequest(string id)
	{
		session.DenyFriendRequest(id);
	}

	public void RemoveFriend(string id)
	{
		session.RemoveFriend(id);
	}

	public string GetManifestVersion()
	{
		return session.manifestVersion;
	}

	private void StartLoadingData()
	{
		Singleton<BusyIconPanelController>.Instance.Show();
		loadingDataFinished = false;
		Language.ReloadLanguage();
		Singleton<LoadingManager>.Instance.LoadAll(FinishedLoadingData);
	}

	public void FinishedLoadingData()
	{
		Singleton<BusyIconPanelController>.Instance.Hide();
		loadingDataFinished = true;
	}

	public bool IsLoadDataDone()
	{
		return loadingDataFinished;
	}

	private void Update()
	{
		if (session != null)
		{
			session.OnUpdate();
		}
		if (State == States.LOGGING_IN && IsLoggedIn())
		{
			PlayerID = session.ThePlayer.playerId;
			State = States.VALIDATE_PATCH;
			session.ValidateLastPatch();
		}
		if (State == States.VALIDATE_PATCH && !session.ValidatingLastPatch)
		{
			State = States.VERSION_CHECK;
			StartSyncStreamingAssets();
		}
		if (State == States.VERSION_CHECK)
		{
			if (!checkedVersion)
			{
				checkedVersion = true;
				session.GetServerVersion();
			}
			if (GetServerVersion() != null)
			{
				if (GetServerVersion() > new Version((!string.IsNullOrEmpty(SQSettings.BundleVersion)) ? SQSettings.BundleVersion : "1.0"))
				{
					NeedsForcedUpdate = true;
					State = States.PATCHING;
				}
				else
				{
					State = States.PATCHING;
				}
				checkedVersion = false;
			}
		}
		if (State == States.MESSAGE_FETCH && IsMessageSyncDone())
		{
			State = States.PATCHING;
		}
		if (State == States.PATCHING && IsPatchingSyncDone())
		{
			State = States.LOAD_DATA;
			StartLoadingData();
		}
		if (State == States.LOAD_DATA && IsLoadDataDone())
		{
			State = States.LOADING;
			LoadFromServer();
		}
		if (State == States.LOADING && IsSaveDone())
		{
			PlayerInfoScript.Load();
			State = States.SYNCING;
			Singleton<PlayerInfoScript>.Instance.User_Sync(true);
		}
		if (State == States.SYNCING && Singleton<PlayerInfoScript>.Instance.ServerSyncDone)
		{
			State = States.SAVING;
			Singleton<PlayerInfoScript>.Instance.Save();
		}
		if (State == States.SAVING && IsSaveDone())
		{
			State = States.READY;
			if (myOnReadyCallback != null)
			{
				myOnReadyCallback();
			}
		}
		if (State == States.QUERYING && IsSaveDone())
		{
			State = States.READY;
			if (myOnReadyCallback != null)
			{
				myOnReadyCallback();
			}
		}
		if (State == States.SENDING && IsSaveDone())
		{
			State = States.READY;
			if (myOnReadyCallback != null)
			{
				myOnReadyCallback();
			}
		}
		if (State == States.DELETING && IsSaveDone())
		{
			State = States.READY;
			if (myOnReadyCallback != null)
			{
				myOnReadyCallback();
			}
		}
		if (State == States.READY && Screen.sleepTimeout == -1)
		{
			KFFNetwork.ToggleManualSleepControl(false);
			Screen.sleepTimeout = -2;
		}
		if (saveToServerResponse.HasValue)
		{
			HandleSaveResponse();
		}
		if (attemptConnectionResponse.HasValue)
		{
			HandleConnectionResponse();
		}
		if (session != null && session.TheGame != null)
		{
			if (session.TheGame.needsSaveSuccessfulDialog)
			{
				session.TheGame.needsSaveSuccessfulDialog = false;
			}
			if (session.TheGame.needsSaveFailedDialog)
			{
				session.TheGame.needsSaveFailedDialog = false;
			}
		}
	}

	public void OnApplicationFocus(bool focus)
	{
		//OnApplicationPauseFromFocus(!focus);
	}

	public void OnApplicationPauseFromFocus(bool paused)
	{
		bool inPurchaseProcess = Singleton<PurchaseManager>.Instance.InPurchaseProcess;
		Debug.Log("Application pausing : " + paused + ", " + inPurchaseProcess);
		if (!(Instance != this))
		{
			if (!paused && session != null && !inPurchaseProcess)
			{
				Debug.Log("H: Calling GetServerTime");
				session.GetServerTime();
			}
			if (!paused && IsReady() && !inPurchaseProcess)
			{
				StartSyncStreamingAssets();
			}
		}
	}

	private void onExternalMessage(string msg)
	{
		session.onExternalMessage(msg);
	}

	public void ClearMessages()
	{
		if (session != null && session.TheGame != null && session.TheGame.MyMessages != null)
		{
			session.TheGame.MyMessages.Clear();
		}
	}
}
