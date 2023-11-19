using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;

public class OnlinePvPManager : Singleton<OnlinePvPManager>
{
	public enum OnlinePvPEventCode
	{
		Connected,
		Disconnected,
		Error,
		JoinedToExist,
		NotFound,
		NewPlayerJoined,
		CreatedNewGame,
		IncomingMessages,
		PlayerLeft,
		Disabled,
		SendSuccess,
		SendTimeout
	}

	private enum GameState
	{
		None,
		Initialized,
		LoggingInServer,
		LoggedInServer,
		SearchingGame,
		LoggingInGame,
		JoiningGame,
		HostingGame,
		InGameHost,
		InGameClient,
		LeavingHost,
		LeavingClient,
		LeavingServer,
		ErrorState
	}

	private enum NextAction
	{
		None,
		CreateGame,
		ConnectGame,
		SearchGame,
		DisconnectServer
	}

	private class StateController
	{
		public GameState currentState { get; set; }

		public List<TBPvPManager.tbPvPEventCode> expectedEvents { get; set; }

		public List<TBPvPManager.tbPvPEventCode> possibleEvents { get; set; }

		public NextAction nextAction { get; set; }

		public StateController(GameState state, List<TBPvPManager.tbPvPEventCode> targetEvents, NextAction action, List<TBPvPManager.tbPvPEventCode> events)
		{
			currentState = state;
			expectedEvents = targetEvents;
			possibleEvents = events;
			nextAction = action;
		}

		public StateController()
		{
			currentState = GameState.None;
			expectedEvents = null;
			possibleEvents = null;
			nextAction = NextAction.None;
		}

		public bool validate(TBPvPManager.tbPvPEventCode e)
		{
			foreach (TBPvPManager.tbPvPEventCode possibleEvent in possibleEvents)
			{
				if (e == possibleEvent)
				{
					return true;
				}
			}
			return false;
		}

		public bool compare(TBPvPManager.tbPvPEventCode e)
		{
			foreach (TBPvPManager.tbPvPEventCode expectedEvent in expectedEvents)
			{
				if (e == expectedEvent)
				{
					return true;
				}
			}
			return false;
		}
	}

	public delegate void OnlinePvPEvent(OnlinePvPEventCode e, object obj);

	private const byte ACK_BIT = 128;

	private const int IndexRoundOver = 16;

	private const byte HEARTBEAT_TYPE = byte.MaxValue;

	private TBPvPManager pvpManager;

	private string UserName = string.Empty;

	private string TargetRoom = string.Empty;

	private int Mylevel;

	private int TargetRange;

	private int TargetRange2nd;

	private int TargetRange3rd;

	private int MyPlayerID;

	private string CurrentGameRoom = string.Empty;

	private DateTime ReconnectStart;

	private bool WaitingAck;

	private float WaitingAckTimeout;

	private bool NeedToResend;

	private int CurrentMessageSendIndex;

	private int CurrentMessageRecvIndex;

	private float HeartBeat;

	private float ResendDelay;

	private string LastMessage;

	private OnlinePvPEvent targetCallback;

	private StateController onlineStateController = new StateController();

	private void Awake()
	{
		pvpManager = Singleton<TBPvPManager>.Instance;
	}

	private void Start()
	{
		onlineStateController.currentState = GameState.Initialized;
	}

	private void Update()
	{
		if (WaitingAck)
		{
			WaitingAckTimeout -= Time.deltaTime;
			if (WaitingAckTimeout <= 0f)
			{
				if (targetCallback != null)
				{
					targetCallback(OnlinePvPEventCode.SendTimeout, null);
				}
				ResetValue();
			}
		}
		if (NeedToResend)
		{
			ResendDelay -= Time.deltaTime;
			if (ResendDelay <= 0f)
			{
				SendMessage(LastMessage, false);
				NeedToResend = false;
			}
		}
		if (onlineStateController.currentState == GameState.InGameHost || onlineStateController.currentState == GameState.InGameClient)
		{
			HeartBeat -= Time.deltaTime;
			if (HeartBeat <= 0f)
			{
				SendMessageHeatBeat();
			}
		}
	}

	private void tbpvpcallback(TBPvPManager.tbPvPEventCode e, object obj)
	{
		if (!onlineStateController.validate(e))
		{
		}
		switch (onlineStateController.currentState)
		{
		case GameState.Initialized:
			break;
		case GameState.LoggedInServer:
			if (onlineStateController.compare(e))
			{
				if (onlineStateController.nextAction == NextAction.CreateGame)
				{
					onlineStateController.currentState = GameState.LoggingInGame;
					List<TBPvPManager.tbPvPEventCode> list9 = new List<TBPvPManager.tbPvPEventCode>();
					list9.Add(TBPvPManager.tbPvPEventCode.CreatedNewRoom);
					onlineStateController.expectedEvents = list9;
					List<TBPvPManager.tbPvPEventCode> list10 = new List<TBPvPManager.tbPvPEventCode>();
					list10.Add(TBPvPManager.tbPvPEventCode.CreatedNewRoom);
					list10.Add(TBPvPManager.tbPvPEventCode.Disconnected);
					list10.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
					onlineStateController.possibleEvents = list10;
					pvpManager.CreateMyRoom(UserName);
				}
				else if (onlineStateController.nextAction == NextAction.ConnectGame)
				{
					onlineStateController.currentState = GameState.JoiningGame;
					List<TBPvPManager.tbPvPEventCode> list11 = new List<TBPvPManager.tbPvPEventCode>();
					list11.Add(TBPvPManager.tbPvPEventCode.JoinedToExist);
					list11.Add(TBPvPManager.tbPvPEventCode.NotFound);
					onlineStateController.expectedEvents = list11;
					List<TBPvPManager.tbPvPEventCode> list12 = new List<TBPvPManager.tbPvPEventCode>();
					list12.Add(TBPvPManager.tbPvPEventCode.JoinedToExist);
					list12.Add(TBPvPManager.tbPvPEventCode.NotFound);
					list12.Add(TBPvPManager.tbPvPEventCode.Connected);
					list12.Add(TBPvPManager.tbPvPEventCode.Disconnected);
					list12.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
					onlineStateController.possibleEvents = list12;
					pvpManager.ConnectGame(TargetRoom);
				}
				else if (onlineStateController.nextAction == NextAction.SearchGame)
				{
					onlineStateController.currentState = GameState.SearchingGame;
					List<TBPvPManager.tbPvPEventCode> list13 = new List<TBPvPManager.tbPvPEventCode>();
					list13.Add(TBPvPManager.tbPvPEventCode.JoinedToExist);
					list13.Add(TBPvPManager.tbPvPEventCode.CreatedNewRoom);
					onlineStateController.expectedEvents = list13;
					List<TBPvPManager.tbPvPEventCode> list14 = new List<TBPvPManager.tbPvPEventCode>();
					list14.Add(TBPvPManager.tbPvPEventCode.JoinedToExist);
					list14.Add(TBPvPManager.tbPvPEventCode.CreatedNewRoom);
					list14.Add(TBPvPManager.tbPvPEventCode.NotFound);
					list14.Add(TBPvPManager.tbPvPEventCode.Disconnected);
					list14.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
					onlineStateController.possibleEvents = list14;
					pvpManager.SearchGame(Mylevel, TargetRange, TargetRange2nd, TargetRange3rd);
				}
				else
				{
					targetCallback(OnlinePvPEventCode.Error, null);
				}
				break;
			}
			switch (e)
			{
			case TBPvPManager.tbPvPEventCode.Disabled:
				if (targetCallback != null)
				{
					targetCallback(OnlinePvPEventCode.Disabled, null);
				}
				break;
			case TBPvPManager.tbPvPEventCode.ErrorDisconnected:
				if (targetCallback != null)
				{
					targetCallback(OnlinePvPEventCode.Error, null);
				}
				break;
			}
			break;
		case GameState.SearchingGame:
			if (onlineStateController.compare(e))
			{
				switch (e)
				{
				case TBPvPManager.tbPvPEventCode.CreatedNewRoom:
					if (targetCallback != null)
					{
						onlineStateController.currentState = GameState.HostingGame;
						targetCallback(OnlinePvPEventCode.CreatedNewGame, obj);
						List<TBPvPManager.tbPvPEventCode> list3 = new List<TBPvPManager.tbPvPEventCode>();
						list3.Add(TBPvPManager.tbPvPEventCode.NewPlayerJoined);
						onlineStateController.expectedEvents = list3;
						List<TBPvPManager.tbPvPEventCode> list4 = new List<TBPvPManager.tbPvPEventCode>();
						list4.Add(TBPvPManager.tbPvPEventCode.NewPlayerJoined);
						list4.Add(TBPvPManager.tbPvPEventCode.CreatedNewRoom);
						list4.Add(TBPvPManager.tbPvPEventCode.Disconnected);
						list4.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
						onlineStateController.possibleEvents = list4;
					}
					break;
				case TBPvPManager.tbPvPEventCode.JoinedToExist:
				{
					CurrentGameRoom = pvpManager.GameClientInstance.CurrentRoom.Name;
					MyPlayerID = 2;
					Dictionary<int, ExitGames.Client.Photon.LoadBalancing.Player> players = pvpManager.GameClientInstance.CurrentRoom.Players;
					foreach (ExitGames.Client.Photon.LoadBalancing.Player value in players.Values)
					{
					}
					if (targetCallback != null)
					{
						onlineStateController.currentState = GameState.InGameClient;
						targetCallback(OnlinePvPEventCode.JoinedToExist, obj);
						List<TBPvPManager.tbPvPEventCode> list = new List<TBPvPManager.tbPvPEventCode>();
						list.Add(TBPvPManager.tbPvPEventCode.IncomingMessages);
						list.Add(TBPvPManager.tbPvPEventCode.PlayerLeft);
						onlineStateController.expectedEvents = list;
						List<TBPvPManager.tbPvPEventCode> list2 = new List<TBPvPManager.tbPvPEventCode>();
						list2.Add(TBPvPManager.tbPvPEventCode.IncomingMessages);
						list2.Add(TBPvPManager.tbPvPEventCode.PlayerLeft);
						list2.Add(TBPvPManager.tbPvPEventCode.Disconnected);
						list2.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
						onlineStateController.possibleEvents = list2;
					}
					break;
				}
				}
			}
			else if (e == TBPvPManager.tbPvPEventCode.ErrorDisconnected && targetCallback != null)
			{
				targetCallback(OnlinePvPEventCode.Error, null);
			}
			break;
		case GameState.LoggingInGame:
			if (onlineStateController.compare(e))
			{
				if (e == TBPvPManager.tbPvPEventCode.CreatedNewRoom && targetCallback != null)
				{
					onlineStateController.currentState = GameState.HostingGame;
					targetCallback(OnlinePvPEventCode.CreatedNewGame, obj);
					List<TBPvPManager.tbPvPEventCode> list15 = new List<TBPvPManager.tbPvPEventCode>();
					list15.Add(TBPvPManager.tbPvPEventCode.NewPlayerJoined);
					onlineStateController.expectedEvents = list15;
					List<TBPvPManager.tbPvPEventCode> list16 = new List<TBPvPManager.tbPvPEventCode>();
					list16.Add(TBPvPManager.tbPvPEventCode.NewPlayerJoined);
					list16.Add(TBPvPManager.tbPvPEventCode.CreatedNewRoom);
					list16.Add(TBPvPManager.tbPvPEventCode.Disconnected);
					list16.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
					onlineStateController.possibleEvents = list16;
				}
			}
			else if ((e == TBPvPManager.tbPvPEventCode.ErrorDisconnected || e == TBPvPManager.tbPvPEventCode.Disconnected) && targetCallback != null)
			{
				targetCallback(OnlinePvPEventCode.Error, null);
			}
			break;
		case GameState.JoiningGame:
			if (onlineStateController.compare(e))
			{
				switch (e)
				{
				case TBPvPManager.tbPvPEventCode.JoinedToExist:
					CurrentGameRoom = Singleton<TBPvPManager>.Instance.GameClientInstance.RoomName;
					MyPlayerID = 2;
					if (targetCallback != null)
					{
						onlineStateController.currentState = GameState.InGameClient;
						targetCallback(OnlinePvPEventCode.JoinedToExist, obj);
						List<TBPvPManager.tbPvPEventCode> list5 = new List<TBPvPManager.tbPvPEventCode>();
						list5.Add(TBPvPManager.tbPvPEventCode.IncomingMessages);
						list5.Add(TBPvPManager.tbPvPEventCode.PlayerLeft);
						onlineStateController.expectedEvents = list5;
						List<TBPvPManager.tbPvPEventCode> list6 = new List<TBPvPManager.tbPvPEventCode>();
						list6.Add(TBPvPManager.tbPvPEventCode.IncomingMessages);
						list6.Add(TBPvPManager.tbPvPEventCode.PlayerLeft);
						list6.Add(TBPvPManager.tbPvPEventCode.Disconnected);
						list6.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
						onlineStateController.possibleEvents = list6;
					}
					break;
				case TBPvPManager.tbPvPEventCode.NotFound:
					if (targetCallback != null)
					{
						targetCallback(OnlinePvPEventCode.NotFound, obj);
						DeActivateOnlinePvP();
					}
					break;
				}
			}
			else if ((e == TBPvPManager.tbPvPEventCode.ErrorDisconnected || e == TBPvPManager.tbPvPEventCode.Disconnected) && targetCallback != null)
			{
				targetCallback(OnlinePvPEventCode.Error, null);
			}
			break;
		case GameState.HostingGame:
			if (onlineStateController.compare(e))
			{
				if (e == TBPvPManager.tbPvPEventCode.NewPlayerJoined)
				{
					CurrentGameRoom = Singleton<TBPvPManager>.Instance.GameClientInstance.RoomName;
					MyPlayerID = 1;
					if (targetCallback != null)
					{
						onlineStateController.currentState = GameState.InGameHost;
						targetCallback(OnlinePvPEventCode.NewPlayerJoined, obj);
						List<TBPvPManager.tbPvPEventCode> list7 = new List<TBPvPManager.tbPvPEventCode>();
						list7.Add(TBPvPManager.tbPvPEventCode.IncomingMessages);
						list7.Add(TBPvPManager.tbPvPEventCode.PlayerLeft);
						onlineStateController.expectedEvents = list7;
						List<TBPvPManager.tbPvPEventCode> list8 = new List<TBPvPManager.tbPvPEventCode>();
						list8.Add(TBPvPManager.tbPvPEventCode.IncomingMessages);
						list8.Add(TBPvPManager.tbPvPEventCode.PlayerLeft);
						list8.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
						onlineStateController.possibleEvents = list8;
					}
				}
			}
			else if ((e == TBPvPManager.tbPvPEventCode.ErrorDisconnected || e == TBPvPManager.tbPvPEventCode.Disconnected) && targetCallback != null)
			{
				targetCallback(OnlinePvPEventCode.Error, null);
			}
			break;
		case GameState.InGameHost:
		case GameState.InGameClient:
			if (onlineStateController.compare(e))
			{
				switch (e)
				{
				case TBPvPManager.tbPvPEventCode.IncomingMessages:
				{
					if (targetCallback == null)
					{
						break;
					}
					TBPvPManager.IncomingGameMessageObject incomingGameMessageObject = obj as TBPvPManager.IncomingGameMessageObject;
					byte messageType = incomingGameMessageObject.MessageType;
					if (messageType == byte.MaxValue)
					{
						break;
					}
					if ((messageType & 0x80u) != 0)
					{
						int num = Convert.ToInt32(messageType & -129);
						if (CurrentMessageSendIndex == num)
						{
							targetCallback(OnlinePvPEventCode.SendSuccess, null);
						}
						else
						{
							targetCallback(OnlinePvPEventCode.Error, null);
						}
						WaitingAck = false;
						break;
					}
					int num2 = Convert.ToInt32(messageType);
					int num3 = CurrentMessageRecvIndex + 1;
					if (num3 >= 16)
					{
						num3 = 1;
					}
					if (num2 == CurrentMessageRecvIndex)
					{
						SendMessageACK(Convert.ToByte(num2));
					}
					else if (num2 == num3)
					{
						SendMessageACK(Convert.ToByte(num2));
						CurrentMessageRecvIndex = num2;
						targetCallback(OnlinePvPEventCode.IncomingMessages, obj);
					}
					else
					{
						targetCallback(OnlinePvPEventCode.Error, null);
					}
					break;
				}
				case TBPvPManager.tbPvPEventCode.PlayerLeft:
					if (targetCallback != null)
					{
						targetCallback(OnlinePvPEventCode.PlayerLeft, obj);
					}
					if (onlineStateController.currentState == GameState.InGameHost)
					{
					}
					DeActivateOnlinePvP();
					break;
				}
			}
			else if ((e == TBPvPManager.tbPvPEventCode.ErrorDisconnected || e == TBPvPManager.tbPvPEventCode.Disconnected) && targetCallback != null)
			{
				targetCallback(OnlinePvPEventCode.Error, null);
			}
			break;
		case GameState.LeavingHost:
			break;
		case GameState.LeavingClient:
			break;
		case GameState.LeavingServer:
			if (onlineStateController.compare(e))
			{
				if (e == TBPvPManager.tbPvPEventCode.Disconnected && targetCallback != null)
				{
					onlineStateController.currentState = GameState.None;
					targetCallback(OnlinePvPEventCode.Disconnected, obj);
				}
			}
			else if (e == TBPvPManager.tbPvPEventCode.ErrorDisconnected && targetCallback != null)
			{
				targetCallback(OnlinePvPEventCode.Error, null);
			}
			break;
		case GameState.ErrorState:
			break;
		case GameState.LoggingInServer:
			break;
		}
	}

	private void ActivateOnlinePvP(string name, bool ranked, OnlinePvPEvent callback)
	{
		onlineStateController.currentState = GameState.LoggedInServer;
		List<TBPvPManager.tbPvPEventCode> list = new List<TBPvPManager.tbPvPEventCode>();
		list.Add(TBPvPManager.tbPvPEventCode.Connected);
		onlineStateController.expectedEvents = list;
		List<TBPvPManager.tbPvPEventCode> list2 = new List<TBPvPManager.tbPvPEventCode>();
		list2.Add(TBPvPManager.tbPvPEventCode.Connected);
		list2.Add(TBPvPManager.tbPvPEventCode.Disconnected);
		list2.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
		list2.Add(TBPvPManager.tbPvPEventCode.Disabled);
		onlineStateController.possibleEvents = list2;
		if (callback != null)
		{
			targetCallback = callback;
		}
		UserName = name;
		pvpManager.ConnectToServer(name, ranked, tbpvpcallback);
	}

	private void DeActivateOnlinePvP()
	{
		onlineStateController.currentState = GameState.LeavingServer;
		List<TBPvPManager.tbPvPEventCode> list = new List<TBPvPManager.tbPvPEventCode>();
		list.Add(TBPvPManager.tbPvPEventCode.Disconnected);
		onlineStateController.expectedEvents = list;
		List<TBPvPManager.tbPvPEventCode> list2 = new List<TBPvPManager.tbPvPEventCode>();
		list2.Add(TBPvPManager.tbPvPEventCode.Disconnected);
		list2.Add(TBPvPManager.tbPvPEventCode.ErrorDisconnected);
		onlineStateController.possibleEvents = list2;
		pvpManager.Disconnect();
	}

	public void CreateGame(string name, OnlinePvPEvent callback)
	{
		ResetValue();
		onlineStateController.nextAction = NextAction.CreateGame;
		ActivateOnlinePvP(name, true, callback);
	}

	public void JoinGame(string targetName, string myName, OnlinePvPEvent callback)
	{
		ResetValue();
		TargetRoom = targetName;
		UserName = myName;
		onlineStateController.nextAction = NextAction.ConnectGame;
		ActivateOnlinePvP(myName, true, callback);
	}

	public void SearchGame(string name, int level, int range, int range2nd, int range3rd, bool ranked, OnlinePvPEvent callback)
	{
		ResetValue();
		Mylevel = level;
		TargetRange = range;
		TargetRange2nd = range2nd;
		TargetRange3rd = range3rd;
		UserName = name;
		onlineStateController.nextAction = NextAction.SearchGame;
		ActivateOnlinePvP(name, ranked, callback);
	}

	private void ResetValue()
	{
		NeedToResend = false;
		WaitingAck = false;
		WaitingAckTimeout = -1f;
		CurrentMessageSendIndex = 1;
		CurrentMessageRecvIndex = 1;
		HeartBeat = MiscParams.PvPHeartBeat;
		ResendDelay = MiscParams.PvPResendDelay;
		LastMessage = string.Empty;
	}

	public void RestartGame()
	{
		ResetValue();
		if (MyPlayerID == 1)
		{
			onlineStateController.nextAction = NextAction.CreateGame;
			ActivateOnlinePvP(CurrentGameRoom, true, null);
			ReconnectStart = DateTime.Now;
		}
		else if (MyPlayerID == 2)
		{
			TargetRoom = CurrentGameRoom;
			onlineStateController.nextAction = NextAction.ConnectGame;
			ActivateOnlinePvP(UserName, true, null);
			ReconnectStart = DateTime.Now;
		}
	}

	public void LeaveGame()
	{
		DeActivateOnlinePvP();
	}

	public void SendMessage(string message, bool keepIt = true)
	{
		if (onlineStateController.currentState == GameState.InGameHost || onlineStateController.currentState == GameState.InGameClient)
		{
			CurrentMessageSendIndex++;
			if (CurrentMessageSendIndex >= 16)
			{
				CurrentMessageSendIndex = 1;
			}
			pvpManager.SendMessage(Convert.ToByte(CurrentMessageSendIndex), message);
			NeedToResend = false;
			if (keepIt)
			{
				LastMessage = message;
			}
			if (CurrentMessageSendIndex > 2)
			{
				WaitingAck = true;
				WaitingAckTimeout = MiscParams.PvPMessageAckTimeout;
			}
		}
	}

	private void SendMessageACK(byte type)
	{
		pvpManager.SendMessage(Convert.ToByte(type + 128), string.Empty);
	}

	public void ReSendMessage()
	{
		NeedToResend = true;
		ResendDelay = MiscParams.PvPResendDelay;
	}

	private void SendMessageHeatBeat()
	{
		pvpManager.SendMessage(Convert.ToByte(byte.MaxValue), string.Empty);
		HeartBeat = MiscParams.PvPHeartBeat;
	}

	public void Reset()
	{
		DeActivateOnlinePvP();
	}

	public string GetMyCountryCode()
	{
		return pvpManager.GetCountryCode(0);
	}

	public string GetOpponentCountryCode()
	{
		return pvpManager.GetCountryCode(1);
	}

	public string GetMyCountryTexture()
	{
		return Singleton<CountryFlagManager>.Instance.GetMyFlag();
	}

	public void SetFriendFilter(string myFilter)
	{
		pvpManager.GameClientInstance.myFilterName = myFilter;
	}
}
