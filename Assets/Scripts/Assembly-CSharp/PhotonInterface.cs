using System;
using System.Collections;
using System.Collections.Generic;
using DW_Leaderboards;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;

public class PhotonInterface : LoadBalancingClient
{
	public const string WEBRPC_GET_DATA = "dw_leaderboard/webrpc_getdata";

	public const string PropC0 = "C0";

	public const string PropHC = "HC";

	public const string PropCC = "CC";

	public const string PropNames = "Names";

	private const byte MaxPlayers = 2;

	private ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();

	public TBPvPManager photonManager;

	public string LobbyName = "myLobby";

	public string RoomName = string.Empty;

	public int PlayerNr;

	private int myFilterC0;

	public string myFilterName = string.Empty;

	private bool hosting;

	public int TurnNumber = 1;

	public int PlayerIdToMakeThisTurn;

	public byte MyPoints;

	public byte OthersPoints;

	public List<SaveTBGameInfo> SavedGames = new List<SaveTBGameInfo>();

	private RpcGetDataCallback rpcCallback;

	private bool AutoCreateRoom = true;

	private int NumAttempts;

	private int SearchRange;

	private int SearchRange2nd;

	private int SearchRange3rd;

	private int SearchLevel;

	public TBPvPManager.TBPvPEvent pvpEventCallback;

	public bool IsMyTurn
	{
		get
		{
			return PlayerIdToMakeThisTurn == base.LocalPlayer.ID;
		}
	}

	public bool GameIsLoaded
	{
		get
		{
			return CurrentRoom != null && CurrentRoom.CustomProperties != null && CurrentRoom.CustomProperties.ContainsKey("pt");
		}
	}

	public bool GameCanStart
	{
		get
		{
			return CurrentRoom != null && CurrentRoom.Players.Count == 2;
		}
	}

	public bool GameWasAbandoned
	{
		get
		{
			return CurrentRoom != null && CurrentRoom.Players.Count < 2 && CurrentRoom.CustomProperties.ContainsKey("flips");
		}
	}

	public bool IsMyScoreHigher
	{
		get
		{
			return MyPoints > OthersPoints;
		}
	}

	public bool IsScoreTheSame
	{
		get
		{
			return MyPoints == OthersPoints;
		}
	}

	public bool IsHosting
	{
		get
		{
			return hosting;
		}
	}

	public ExitGames.Client.Photon.LoadBalancing.Player Opponent
	{
		get
		{
			return base.LocalPlayer.GetNext();
		}
	}

	public void SetServerAddress(string address)
	{
		base.MasterServerAddress = address;
	}

	public void init()
	{
		hosting = false;
	}

	public string GetCountryCode(int side)
	{
		string empty = string.Empty;
		if (CurrentRoom == null)
		{
			return empty;
		}
		empty = ((side == 0) ? ((!hosting) ? "CC" : "HC") : ((!hosting) ? "HC" : "CC"));
		string text = (string)CurrentRoom.CustomProperties[empty];
		Singleton<PlayerInfoScript>.Instance.SaveData.LastKnownLocation = text;
		return text;
	}

	public override void OnOperationResponse(OperationResponse operationResponse)
	{
		base.OnOperationResponse(operationResponse);
		switch (operationResponse.OperationCode)
		{
		case 219:
			if (operationResponse.ReturnCode == 0)
			{
				OnWebRpcResponse(new WebRpcResponse(operationResponse));
			}
			break;
		case 226:
		case 227:
			if (operationResponse.ReturnCode != 0)
			{
			}
			if (base.Server == ServerConnection.GameServer && operationResponse.ReturnCode == 0)
			{
				LoadTBGameFromProperties(false);
			}
			if (pvpEventCallback == null)
			{
				break;
			}
			if (operationResponse.OperationCode == 226)
			{
				if (operationResponse.ReturnCode != 0)
				{
					pvpEventCallback(TBPvPManager.tbPvPEventCode.NotFound, null);
				}
				else if (Opponent != null)
				{
					RoomName = CurrentRoom.Name;
					pvpEventCallback(TBPvPManager.tbPvPEventCode.JoinedToExist, Opponent.Name);
				}
				else
				{
					pvpEventCallback(TBPvPManager.tbPvPEventCode.JoinedToExist, null);
				}
				hosting = false;
				PlayerNr = 2;
			}
			if (operationResponse.OperationCode == 227)
			{
				pvpEventCallback(TBPvPManager.tbPvPEventCode.CreatedNewRoom, null);
				hosting = true;
				PlayerNr = 1;
				if (CurrentRoom != null)
				{
					RoomName = CurrentRoom.Name;
				}
			}
			break;
		case 225:
			if (AutoCreateRoom && operationResponse.ReturnCode == 32760)
			{
				if (SearchRange2nd == 0)
				{
					CreateTurnbasedRoom();
				}
				else if (NumAttempts < 2 && (NumAttempts != 1 || SearchRange2nd != SearchRange3rd))
				{
					NumAttempts++;
					JoinRandomRoom(SearchLevel, SearchRange, SearchRange2nd, SearchRange3rd, false);
				}
				else
				{
					CreateTurnbasedRoom();
				}
			}
			else if (operationResponse.ReturnCode != 0)
			{
			}
			break;
		}
	}

	public override void OnEvent(EventData photonEvent)
	{
		base.OnEvent(photonEvent);
		switch (photonEvent.Code)
		{
		case 253:
			return;
		case byte.MaxValue:
			if (CurrentRoom.Players.Count == 2 && CurrentRoom.IsOpen)
			{
				CurrentRoom.IsOpen = false;
				CurrentRoom.IsVisible = false;
				SavePlayersInProps();
				if (pvpEventCallback != null)
				{
					pvpEventCallback(TBPvPManager.tbPvPEventCode.NewPlayerJoined, Opponent.Name);
				}
			}
			return;
		case 254:
			if (!GameWasAbandoned && pvpEventCallback != null)
			{
				pvpEventCallback(TBPvPManager.tbPvPEventCode.PlayerLeft, null);
			}
			return;
		}
		foreach (KeyValuePair<byte, object> parameter in photonEvent.Parameters)
		{
			if (parameter.Key != 245)
			{
				continue;
			}
			foreach (DictionaryEntry item in (ExitGames.Client.Photon.Hashtable)parameter.Value)
			{
				if (pvpEventCallback != null)
				{
					TBPvPManager.IncomingGameMessageObject incomingGameMessageObject = new TBPvPManager.IncomingGameMessageObject();
					incomingGameMessageObject.MessageType = (byte)item.Key;
					incomingGameMessageObject.Message = (string)item.Value;
					pvpEventCallback(TBPvPManager.tbPvPEventCode.IncomingMessages, incomingGameMessageObject);
				}
			}
		}
	}

	public override void DebugReturn(DebugLevel level, string message)
	{
		base.DebugReturn(level, message);
	}

	public override void OnStatusChanged(StatusCode statusCode)
	{
		base.OnStatusChanged(statusCode);
		switch (statusCode)
		{
		case StatusCode.Exception:
		case StatusCode.ExceptionOnReceive:
		case StatusCode.DisconnectByServer:
		case StatusCode.DisconnectByServerLogic:
			if (pvpEventCallback != null)
			{
				pvpEventCallback(TBPvPManager.tbPvPEventCode.ErrorDisconnected, null);
			}
			break;
		case StatusCode.ExceptionOnConnect:
			if (pvpEventCallback != null)
			{
				pvpEventCallback(TBPvPManager.tbPvPEventCode.ErrorDisconnected, null);
			}
			break;
		case StatusCode.Disconnect:
			SavedGames.Clear();
			if (pvpEventCallback != null)
			{
				pvpEventCallback(TBPvPManager.tbPvPEventCode.Disconnected, null);
			}
			break;
		}
	}

	public void WebRpcGetData(string user_id, RpcGetDataCallback callback)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("userid", user_id);
		OpWebRpc("dw_leaderboard/webrpc_getdata", dictionary);
		rpcCallback = callback;
	}

	private void OnWebRpcResponse(WebRpcResponse response)
	{
		KeyValuePair<string, object> returndata = default(KeyValuePair<string, object>);
		if (response.ReturnCode == 0)
		{
			if (response.Parameters == null)
			{
				if (rpcCallback != null)
				{
					rpcCallback(returndata, ResponseFlag.Error);
				}
				return;
			}
			if (response.Name.Equals("Data"))
			{
				foreach (KeyValuePair<string, object> parameter in response.Parameters)
				{
				}
				if (rpcCallback != null)
				{
					rpcCallback(returndata, ResponseFlag.Success);
				}
				return;
			}
		}
		if (rpcCallback != null)
		{
			rpcCallback(returndata, ResponseFlag.Error);
		}
	}

	public void SaveTBGameToProperties()
	{
	}

	public void SavePlayersInProps()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (CurrentRoom == null || CurrentRoom.CustomProperties == null || CurrentRoom.CustomProperties.ContainsKey("C0"))
		{
			if (!hosting)
			{
				hashtable["CC"] = photonManager.CountryCode;
				OpSetCustomPropertiesOfRoom(hashtable);
			}
		}
		else
		{
			hashtable["Names"] = string.Format("{0};{1}", base.LocalPlayer.Name, Opponent.Name);
			OpSetCustomPropertiesOfRoom(hashtable);
		}
	}

	public void LoadTBGameFromProperties(bool calledByEvent)
	{
		ExitGames.Client.Photon.Hashtable customProperties = CurrentRoom.CustomProperties;
		if (customProperties.Count == 0)
		{
			SaveTBGameToProperties();
		}
		if (CurrentRoom.CustomProperties.ContainsKey("t#"))
		{
			TurnNumber = (int)CurrentRoom.CustomProperties["t#"];
		}
		else
		{
			TurnNumber = 1;
		}
		if (CurrentRoom.CustomProperties.ContainsKey("pt"))
		{
			PlayerIdToMakeThisTurn = (int)CurrentRoom.CustomProperties["pt"];
		}
		else
		{
			PlayerIdToMakeThisTurn = 0;
		}
		if (PlayerIdToMakeThisTurn == 0)
		{
			PlayerIdToMakeThisTurn = CurrentRoom.MasterClientId;
		}
		MyPoints = GetPlayerPointsFromProps(base.LocalPlayer);
		OthersPoints = GetPlayerPointsFromProps(Opponent);
	}

	private string GetPlayerPointsPropKey(int id)
	{
		return string.Format("pt{0}", id);
	}

	private byte GetPlayerPointsFromProps(ExitGames.Client.Photon.LoadBalancing.Player player)
	{
		if (player == null || player.ID < 1)
		{
			return 0;
		}
		string playerPointsPropKey = GetPlayerPointsPropKey(player.ID);
		if (CurrentRoom.CustomProperties.ContainsKey(playerPointsPropKey))
		{
			return (byte)CurrentRoom.CustomProperties[playerPointsPropKey];
		}
		return 0;
	}

	public void CreateTurnbasedRoom()
	{
		RoomName = string.Format("{0}-{1}", base.PlayerName, UnityEngine.Random.Range(0, 1000).ToString("D4"));
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (Singleton<TBPvPManager>.Instance.FriendMatch)
		{
			int num = myFilterName.IndexOf('_');
			string value = myFilterName.Substring(0, num);
			string value2 = myFilterName.Substring(num + 1);
			int num2 = Convert.ToInt32(value) * 10;
			int num3 = Convert.ToInt32(value2);
			hashtable.Add("C0", num2 + num3);
			LobbyName = "myFriend";
		}
		else if (myFilterC0 != 0)
		{
			hashtable.Add("C0", myFilterC0);
		}
		hashtable.Add("HC", photonManager.CountryCode);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 2;
		roomOptions.CustomRoomProperties = hashtable;
		roomOptions.CustomRoomPropertiesForLobby = new string[4] { "Names", "C0", "HC", "CC" };
		roomOptions.PlayerTtl = 0;
		roomOptions.EmptyRoomTtl = 0;
		RoomOptions roomOptions2 = roomOptions;
		TypedLobby lobby = new TypedLobby(LobbyName, LobbyType.SqlLobby);
		OpCreateRoom(RoomName, roomOptions2, lobby);
	}

	public void OpCreateRoom(string playerId)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		int num = playerId.IndexOf('_');
		string value = playerId.Substring(0, num);
		string value2 = playerId.Substring(num + 1);
		int num2 = Convert.ToInt32(value) * 10;
		int num3 = Convert.ToInt32(value2);
		hashtable.Add("C0", num2 + num3);
		hashtable.Add("HC", photonManager.CountryCode);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 2;
		roomOptions.CustomRoomProperties = hashtable;
		roomOptions.CustomRoomPropertiesForLobby = new string[4] { "Names", "C0", "HC", "CC" };
		roomOptions.PlayerTtl = 0;
		roomOptions.EmptyRoomTtl = 0;
		RoomOptions roomOptions2 = roomOptions;
		TypedLobby lobby = new TypedLobby("myFriend", LobbyType.SqlLobby);
		OpCreateRoom(playerId, roomOptions2, lobby);
		RoomName = playerId;
	}

	public void HandoverTurnToNextPlayer()
	{
		if (base.LocalPlayer != null)
		{
			ExitGames.Client.Photon.LoadBalancing.Player nextFor = base.LocalPlayer.GetNextFor(PlayerIdToMakeThisTurn);
			if (nextFor != null)
			{
				PlayerIdToMakeThisTurn = nextFor.ID;
				return;
			}
		}
		PlayerIdToMakeThisTurn = 0;
	}

	public bool JoinRandomRoom(int mylevel, int range, int range2nd, int range3rd, bool firsttime = true)
	{
		ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		string text = null;
		AutoCreateRoom = true;
		int num;
		if (!firsttime)
		{
			num = ((NumAttempts != 1) ? range3rd : range2nd);
		}
		else
		{
			NumAttempts = 0;
			SearchLevel = mylevel;
			SearchRange = range;
			SearchRange2nd = range2nd;
			SearchRange3rd = range3rd;
			num = range;
		}
		myFilterC0 = mylevel;
		int num2 = mylevel - num;
		int num3 = mylevel + num;
		if (num2 < 1)
		{
			num2 = 1;
		}
		byte expectedMaxPlayers = 2;
		if (Singleton<TBPvPManager>.Instance.FriendMatch)
		{
			int num4 = myFilterName.IndexOf('_');
			string value = myFilterName.Substring(0, num4);
			string value2 = myFilterName.Substring(num4 + 1);
			int num5 = Convert.ToInt32(value) * 10;
			int num6 = Convert.ToInt32(value2);
			text = string.Format("C0 = {0}", num5 + num6);
			LobbyName = "myFriend";
			expectedMaxPlayers = 0;
		}
		else
		{
			text = string.Format("C0 > {0} AND C0 < {1}", num2, num3);
			LobbyName = "myLobby";
		}
		TypedLobby lobby = new TypedLobby(LobbyName, LobbyType.SqlLobby);
		OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.RandomMatching, lobby, text);
		return true;
	}

	public bool JoinTheGame(string targetname)
	{
		ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		string text = null;
		AutoCreateRoom = false;
		myFilterName = targetname;
		int num = targetname.IndexOf('_');
		string value = targetname.Substring(0, num);
		string value2 = targetname.Substring(num + 1);
		int num2 = Convert.ToInt32(value) * 10;
		int num3 = Convert.ToInt32(value2);
		text = string.Format("C0 = {0}", num2 + num3);
		byte expectedMaxPlayers = 0;
		TypedLobby lobby = new TypedLobby("myFriend", LobbyType.SqlLobby);
		OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.RandomMatching, lobby, text);
		RoomName = targetname;
		return true;
	}

	public string OperationResponseCode(byte code)
	{
		switch (code)
		{
		case 230:
			return "Authenticate";
		case 229:
			return "JoinLobby";
		case 228:
			return "LeaveLobby";
		case 227:
			return "CreateGame";
		case 226:
			return "JoinGame";
		case 225:
			return "JoinRandomGame";
		case 254:
			return "Leave";
		case 253:
			return "RaiseEvent";
		case 252:
			return "SetProperties";
		case 251:
			return "GetProperties";
		case 248:
			return "ChangeGroups";
		case 222:
			return "FindFriends";
		case 221:
			return "GetLobbyStats";
		case 220:
			return "GetRegions";
		case 219:
			return "WebRpc";
		default:
			return "unknown" + code;
		}
	}
}
