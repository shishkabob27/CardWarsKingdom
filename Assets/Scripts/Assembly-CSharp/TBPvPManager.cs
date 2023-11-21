using System;
using System.Collections.Generic;
using System.Net;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;

public class TBPvPManager : Singleton<TBPvPManager>
{
	public enum GameState
	{
		None,
		Login,
		InGame
	}

	public enum tbPvPEventCode
	{
		Connected,
		Disconnected,
		ErrorDisconnected,
		JoinedToExist,
		NotFound,
		NewPlayerJoined,
		CreatedNewRoom,
		IncomingMessages,
		PlayerLeft,
		PropertiesChanged,
		Disabled
	}

	public class IncomingGameMessageObject
	{
		public byte MessageType;

		public string Message;
	}

	public delegate void TBPvPEvent(tbPvPEventCode e, object obj);

	private const bool OnGuiShortcut = true;

	public PhotonInterface GameClientInstance;

	public GameState CurrentState;

	public string PvPCompatibilityVersion = "110";

	private string UnrankedMatch = "U";

	private int savegameListStartIndex;

	private bool mFriendMatch;

	private string myCountryCode = "..";

	private string myIP = string.Empty;

	private string myLocation = string.Empty;

	private bool visible;

	public bool FriendMatch
	{
		get
		{
			return mFriendMatch;
		}
		set
		{
			mFriendMatch = value;
		}
	}

	public string CountryCode
	{
		get
		{
			return myCountryCode;
		}
	}

	public string LocationCode
	{
		get
		{
			return myLocation;
		}
	}

	public string IPAddress
	{
		get
		{
			return myIP;
		}
	}

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			OnVisibleChanged();
		}
	}

	public bool IsConnected()
	{
		if (GameClientInstance != null)
		{
			return GameClientInstance.IsConnected;
		}
		return false;
	}

	public void Awake()
	{
		Application.runInBackground = true;
		GameClientInstance = new PhotonInterface();
		GameClientInstance.photonManager = this;
		GameClientInstance.AppId = SQSettings.PHOTON_PUN_APP_ID;
	}

	public void Start()
	{
	}

	public void OnEnable()
	{
	}

	public void CheckIP()
	{
		Session theSession = SessionManager.Instance.theSession;
		TFServer.JsonResponseHandler handler = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			myIP = string.Empty;
			myCountryCode = "US";
			if (status == HttpStatusCode.OK)
			{
				if (data.ContainsKey("ip"))
				{
					myIP = Convert.ToString(data["ip"]);
				}
				if (data.ContainsKey("country"))
				{
					myCountryCode = Convert.ToString(data["country"]);
				}
			}
		};
		theSession.Server.GetCC(handler);
	}

	public string GetCountryCode(int side)
	{
		return GameClientInstance.GetCountryCode(side);
	}

	public void ConnectToServer(string playerName, bool ranked, TBPvPEvent callback)
	{
		if (!MiscParams.PvpEnable)
		{
			if (callback != null)
			{
				callback(tbPvPEventCode.Disabled, null);
			}
			return;
		}
		if (GameClientInstance.IsConnected)
		{
			if (callback != null)
			{
				callback(tbPvPEventCode.Connected, null);
			}
			return;
		}
		GameClientInstance.init();
		GameClientInstance.AppVersion = GetCompatibilityVersion(ranked);
		GameClientInstance.NickName = playerName;
		PhotonInterface gameClientInstance = GameClientInstance;
		gameClientInstance.OnStateChangeAction = (Action<ClientState>)Delegate.Combine(gameClientInstance.OnStateChangeAction, new Action<ClientState>(OnStateChanged));
		GameClientInstance.ConnectToRegionMaster("US");
		GameClientInstance.pvpEventCallback = callback;
		Screen.sleepTimeout = -1;
	}

	public string GetCompatibilityVersion(bool ranked)
	{
		int pvPCompatibilityVersion = MiscParams.PvPCompatibilityVersion;
		if (Convert.ToInt32(PvPCompatibilityVersion) < 0)
		{
			return PvPCompatibilityVersion;
		}
		string text3;
		if (pvPCompatibilityVersion >= 0)
		{
			string text = ((!Singleton<AnalyticsManager>.Instance.JailBroken && Singleton<PlayerInfoScript>.Instance.SaveData.PvpSpecialDomainNumber == 0) ? "y" : "x");
			string text2 = Convert.ToString(pvPCompatibilityVersion) + "D" + text;
			text3 = text2;
		}
		else
		{
			text3 = Convert.ToString(pvPCompatibilityVersion);
		}
		if (!ranked)
		{
			text3 += UnrankedMatch;
		}
		return text3;
	}

	public void Disconnect()
	{
		FriendMatch = false;
		Visible = false;
		CurrentState = GameState.Login;
		GameClientInstance.Disconnect();
		PhotonInterface gameClientInstance = GameClientInstance;
		gameClientInstance.OnStateChangeAction = (Action<ClientState>)Delegate.Remove(gameClientInstance.OnStateChangeAction, new Action<ClientState>(OnStateChanged));
		Screen.sleepTimeout = -2;
	}

	public void OnApplicationQuit()
	{
		if (GameClientInstance != null && GameClientInstance.loadBalancingPeer != null)
		{
			GameClientInstance.Disconnect();
			GameClientInstance.loadBalancingPeer.StopThread();
		}
		GameClientInstance = null;
	}

	private void onPushNotificationsReceived(string payload)
	{
		if (GameClientInstance.Server == LoadBalancingClient.ServerConnection.MasterServer)
		{
			GetRoomsList();
		}
	}

	private void OnStateChanged(ClientState state)
	{
		switch (state)
		{
		case ClientState.ConnectedToMaster:
			Visible = true;
			if (GameClientInstance.pvpEventCallback != null)
			{
				GameClientInstance.pvpEventCallback(tbPvPEventCode.Connected, null);
			}
			break;
		}
	}

	private void OnVisibleChanged()
	{
		if (!visible)
		{
		}
	}

	public void Update()
	{
		GameClientInstance.Service();
	}

	private void GetRoomsList()
	{
		savegameListStartIndex = 0;
		GameClientInstance.OpWebRpc("GetGameList", null);
	}

	public void GameListUpdate()
	{
	}

	public void SaveGameList()
	{
		GameListUpdate();
	}

	public void LeaveGame()
	{
		LeaveGame(false);
	}

	public void AbandonGame()
	{
		LeaveGame(true);
	}

	public void LeaveGame(bool doAbandon)
	{
		GameClientInstance.OpLeaveRoom(!doAbandon);
		CurrentState = GameState.Login;
		Visible = false;
	}

	public void SearchGame(int mylevel, int range, int range2nd, int range3rd)
	{
		Visible = false;
		CurrentState = GameState.InGame;
		GameClientInstance.JoinRandomRoom(mylevel, range, range2nd, range3rd);
	}

	public void ConnectGame(string targetname)
	{
		Visible = false;
		CurrentState = GameState.InGame;
		GameClientInstance.JoinTheGame(targetname);
	}

	public void LoadGame(object[] parameters)
	{
		string roomName = parameters[0] as string;
		int actorNumber = (int)parameters[1];
		Visible = false;
		CurrentState = GameState.InGame;
		GameClientInstance.OpJoinRoom(roomName, actorNumber);
	}

	public void CreateMyRoom(string playerId)
	{
		Visible = false;
		CurrentState = GameState.InGame;
		GameClientInstance.OpCreateRoom(playerId);
	}

	public void SendMessage(byte messagetype, string messagebody)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[messagetype] = messagebody;
		GameClientInstance.loadBalancingPeer.OpRaiseEvent(1, hashtable, true, null);
	}
}
