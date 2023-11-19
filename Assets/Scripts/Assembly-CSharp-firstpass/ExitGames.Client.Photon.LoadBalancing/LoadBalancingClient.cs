using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class LoadBalancingClient : IPhotonPeerListener
	{
		public enum ServerConnection
		{
			MasterServer,
			GameServer,
			NameServer
		}

		public LoadBalancingPeer loadBalancingPeer;

		public string NameServerHost = "ns.exitgames.com";

		public string NameServerHttp = "http://ns.exitgames.com:80/photon/n";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
		{
			{
				ConnectionProtocol.Udp,
				5058
			},
			{
				ConnectionProtocol.Tcp,
				4533
			},
			{
				ConnectionProtocol.WebSocket,
				9093
			},
			{
				ConnectionProtocol.WebSocketSecure,
				19093
			}
		};

		private ClientState state;

		private bool inLobby;

		private bool autoJoinLobby = true;

		public bool EnableLobbyStatistics;

		private List<TypedLobbyInfo> lobbyStatistics = new List<TypedLobbyInfo>();

		public Dictionary<string, RoomInfo> RoomInfoList = new Dictionary<string, RoomInfo>();

		public Room CurrentRoom;

		private Player localPlayer;

		private JoinType lastJoinType;

		private LoadBalancingPeer.EnterRoomParams enterRoomParamsCache;

		private bool didAuthenticate;

		private string[] friendListRequested;

		private int friendListTimestamp;

		private bool isFetchingFriendList;

		public string AppVersion { get; set; }

		public string AppId { get; set; }

		public AuthenticationValues AuthValues { get; set; }

		public bool IsUsingNameServer { get; private set; }

		public string NameServerAddress
		{
			get
			{
				return GetNameServerAddress();
			}
		}

		public string CurrentServerAddress
		{
			get
			{
				return loadBalancingPeer.ServerAddress;
			}
		}

		public string MasterServerAddress { get; protected internal set; }

		private string GameServerAddress { get; set; }

		public ServerConnection Server { get; private set; }

		public ClientState State
		{
			get
			{
				return state;
			}
			protected internal set
			{
				state = value;
				if (OnStateChangeAction != null)
				{
					OnStateChangeAction(state);
				}
			}
		}

		public bool IsConnected
		{
			get
			{
				return loadBalancingPeer != null && State != 0 && State != ClientState.Disconnected;
			}
		}

		public bool IsConnectedAndReady
		{
			get
			{
				if (loadBalancingPeer == null)
				{
					return false;
				}
				switch (State)
				{
				case ClientState.Uninitialized:
				case ClientState.ConnectingToMasterserver:
				case ClientState.ConnectingToGameserver:
				case ClientState.Joining:
				case ClientState.Leaving:
				case ClientState.Disconnecting:
				case ClientState.Disconnected:
				case ClientState.ConnectingToNameServer:
				case ClientState.Authenticating:
					return false;
				default:
					return true;
				}
			}
		}

		public Action<ClientState> OnStateChangeAction { get; set; }

		public Action<EventData> OnEventAction { get; set; }

		public Action<OperationResponse> OnOpResponseAction { get; set; }

		public DisconnectCause DisconnectedCause { get; protected set; }

		public TypedLobby CurrentLobby { get; protected internal set; }

		[Obsolete("Use CurrentLobby.Name")]
		public string CurrentLobbyName
		{
			get
			{
				return CurrentLobby.Name;
			}
		}

		[Obsolete("Use CurrentLobby.Type")]
		public LobbyType CurrentLobbyType
		{
			get
			{
				return CurrentLobby.Type;
			}
		}

		public bool AutoJoinLobby
		{
			get
			{
				return autoJoinLobby;
			}
			set
			{
				autoJoinLobby = value;
			}
		}

		private bool requestLobbyStatistics
		{
			get
			{
				return EnableLobbyStatistics && Server == ServerConnection.MasterServer;
			}
		}

		public List<TypedLobbyInfo> LobbyStatistics
		{
			get
			{
				return lobbyStatistics;
			}
			private set
			{
				lobbyStatistics = value;
			}
		}

		[Obsolete("Use NickName instead.")]
		public string PlayerName
		{
			get
			{
				return NickName;
			}
			set
			{
				NickName = value;
			}
		}

		public string NickName
		{
			get
			{
				return LocalPlayer.NickName;
			}
			set
			{
				if (LocalPlayer != null)
				{
					LocalPlayer.NickName = value;
				}
			}
		}

		public string UserId
		{
			get
			{
				if (AuthValues != null)
				{
					return AuthValues.UserId;
				}
				return null;
			}
			set
			{
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.UserId = value;
			}
		}

		public Player LocalPlayer
		{
			get
			{
				if (localPlayer == null)
				{
					localPlayer = CreatePlayer(string.Empty, -1, true, null);
				}
				return localPlayer;
			}
			set
			{
				localPlayer = value;
			}
		}

		public int PlayersOnMasterCount { get; set; }

		public int PlayersInRoomsCount { get; set; }

		public int RoomsCount { get; set; }

		public List<FriendInfo> FriendList { get; private set; }

		public int FriendListAge
		{
			get
			{
				return (!isFetchingFriendList && friendListTimestamp != 0) ? (Environment.TickCount - friendListTimestamp) : 0;
			}
		}

		protected bool IsAuthorizeSecretAvailable
		{
			get
			{
				return AuthValues != null && !string.IsNullOrEmpty(AuthValues.Token);
			}
		}

		public string[] AvailableRegions { get; private set; }

		public string[] AvailableRegionsServers { get; private set; }

		public string CloudRegion { get; private set; }

		public LoadBalancingClient(ConnectionProtocol protocol = ConnectionProtocol.Udp)
		{
			loadBalancingPeer = new LoadBalancingPeer(this, protocol);
		}

		public LoadBalancingClient(string masterAddress, string appId, string gameVersion, ConnectionProtocol protocol = ConnectionProtocol.Udp)
			: this(protocol)
		{
			MasterServerAddress = masterAddress;
			AppId = appId;
			AppVersion = gameVersion;
		}

		private string GetNameServerAddress()
		{
			ConnectionProtocol usedProtocol = loadBalancingPeer.UsedProtocol;
			int value = 0;
			ProtocolToNameServerPort.TryGetValue(usedProtocol, out value);
			string arg = string.Empty;
			switch (usedProtocol)
			{
			case ConnectionProtocol.WebSocket:
				arg = "ws://";
				break;
			case ConnectionProtocol.WebSocketSecure:
				arg = "wss://";
				break;
			}
			return string.Format("{0}{1}:{2}", arg, NameServerHost, value);
		}

		public bool Connect(string masterServerAddress, string appId, string appVersion, string nickName, AuthenticationValues authValues)
		{
			if (!string.IsNullOrEmpty(masterServerAddress))
			{
				MasterServerAddress = masterServerAddress;
			}
			if (!string.IsNullOrEmpty(appId))
			{
				AppId = appId;
			}
			if (!string.IsNullOrEmpty(appVersion))
			{
				AppVersion = appVersion;
			}
			if (!string.IsNullOrEmpty(nickName))
			{
				NickName = nickName;
			}
			AuthValues = authValues;
			return Connect();
		}

		public virtual bool Connect()
		{
			loadBalancingPeer.QuickResendAttempts = 3;
			loadBalancingPeer.SentCountAllowance = 9;
			DisconnectedCause = DisconnectCause.None;
			if (loadBalancingPeer.Connect(MasterServerAddress, AppId))
			{
				State = ClientState.ConnectingToMasterserver;
				return true;
			}
			return false;
		}

		public bool ConnectToNameServer()
		{
			IsUsingNameServer = true;
			CloudRegion = null;
			if (!loadBalancingPeer.Connect(NameServerAddress, "NameServer"))
			{
				return false;
			}
			State = ClientState.ConnectingToNameServer;
			return true;
		}

		public bool ConnectToRegionMaster(string region)
		{
			IsUsingNameServer = true;
			if (State == ClientState.ConnectedToNameServer)
			{
				CloudRegion = region;
				return loadBalancingPeer.OpAuthenticate(AppId, AppVersion, AuthValues, region, requestLobbyStatistics);
			}
			CloudRegion = region;
			if (!loadBalancingPeer.Connect(NameServerAddress, "NameServer"))
			{
				return false;
			}
			State = ClientState.ConnectingToNameServer;
			return true;
		}

		public void Disconnect()
		{
			if (State != ClientState.Disconnected)
			{
				State = ClientState.Disconnecting;
				loadBalancingPeer.Disconnect();
				if (loadBalancingPeer.PeerState == PeerStateValue.Disconnected || loadBalancingPeer.PeerState == PeerStateValue.InitializingApplication)
				{
					State = ClientState.Disconnected;
				}
			}
		}

		public void Service()
		{
			if (loadBalancingPeer != null)
			{
				loadBalancingPeer.Service();
			}
		}

		private void DisconnectToReconnect()
		{
			switch (Server)
			{
			case ServerConnection.NameServer:
				State = ClientState.DisconnectingFromNameServer;
				break;
			case ServerConnection.MasterServer:
				State = ClientState.DisconnectingFromMasterserver;
				break;
			case ServerConnection.GameServer:
				State = ClientState.DisconnectingFromGameserver;
				break;
			}
			loadBalancingPeer.Disconnect();
		}

		private bool ConnectToGameServer()
		{
			if (loadBalancingPeer.Connect(GameServerAddress, AppId))
			{
				State = ClientState.ConnectingToGameserver;
				return true;
			}
			return false;
		}

		public bool OpGetRegions()
		{
			if (Server != ServerConnection.NameServer)
			{
				return false;
			}
			bool flag = loadBalancingPeer.OpGetRegions(AppId);
			if (flag)
			{
				AvailableRegions = null;
			}
			return flag;
		}

		public bool OpJoinLobby(TypedLobby lobby)
		{
			if (lobby == null)
			{
				lobby = TypedLobby.Default;
			}
			bool flag = loadBalancingPeer.OpJoinLobby(lobby);
			if (flag)
			{
				CurrentLobby = lobby;
			}
			return flag;
		}

		public bool OpLeaveLobby()
		{
			return loadBalancingPeer.OpLeaveLobby();
		}

		public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, string[] expectedUsers = null)
		{
			return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, TypedLobby.Default, null, expectedUsers);
		}

		public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode)
		{
			return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchmakingMode, TypedLobby.Default, null);
		}

		public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode, TypedLobby lobby, string sqlLobbyFilter, string[] expectedUsers = null)
		{
			if (lobby == null)
			{
				lobby = TypedLobby.Default;
			}
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinRandomRoom;
			CurrentLobby = lobby;
			enterRoomParamsCache = new LoadBalancingPeer.EnterRoomParams();
			enterRoomParamsCache.Lobby = lobby;
			LoadBalancingPeer.OpJoinRandomRoomParams opJoinRandomRoomParams = new LoadBalancingPeer.OpJoinRandomRoomParams();
			opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
			opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
			opJoinRandomRoomParams.MatchingType = matchmakingMode;
			opJoinRandomRoomParams.TypedLobby = lobby;
			opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
			opJoinRandomRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
		}

		public bool OpJoinRoom(string roomName, string[] expectedUsers = null)
		{
			return OpJoinRoom(roomName, 0, expectedUsers);
		}

		public bool OpJoinRoom(string roomName, int actorNumber, string[] expectedUsers = null)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinRoom;
			bool onGameServer = Server == ServerConnection.GameServer;
			LoadBalancingPeer.EnterRoomParams enterRoomParams = (enterRoomParamsCache = new LoadBalancingPeer.EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.ActorNumber = actorNumber;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpJoinRoom(enterRoomParams);
		}

		public bool OpJoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, string[] expectedUsers = null)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinOrCreateRoom;
			CurrentLobby = lobby;
			bool onGameServer = Server == ServerConnection.GameServer;
			LoadBalancingPeer.EnterRoomParams enterRoomParams = (enterRoomParamsCache = new LoadBalancingPeer.EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = lobby;
			enterRoomParams.CreateIfNotExists = true;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpJoinRoom(enterRoomParams);
		}

		public bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, string[] expectedUsers = null)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.CreateRoom;
			CurrentLobby = lobby;
			bool onGameServer = Server == ServerConnection.GameServer;
			LoadBalancingPeer.EnterRoomParams enterRoomParams = (enterRoomParamsCache = new LoadBalancingPeer.EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = lobby;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpCreateRoom(enterRoomParams);
		}

		public bool OpLeaveRoom()
		{
			return OpLeaveRoom(false);
		}

		public bool OpLeaveRoom(bool becomeInactive)
		{
			if (CurrentRoom == null || Server != ServerConnection.GameServer || State == ClientState.DisconnectingFromGameserver)
			{
				return false;
			}
			if (becomeInactive)
			{
				State = ClientState.DisconnectingFromGameserver;
				loadBalancingPeer.Disconnect();
			}
			else
			{
				State = ClientState.Leaving;
				loadBalancingPeer.OpLeaveRoom(false);
			}
			return true;
		}

		public bool OpFindFriends(string[] friendsToFind)
		{
			if (loadBalancingPeer == null)
			{
				return false;
			}
			if (isFetchingFriendList || Server == ServerConnection.GameServer)
			{
				return false;
			}
			isFetchingFriendList = true;
			friendListRequested = friendsToFind;
			return loadBalancingPeer.OpFindFriends(friendsToFind);
		}

		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties = null, bool webForward = false)
		{
			if (CurrentRoom == null)
			{
				if (expectedProperties == null && !webForward && LocalPlayer != null && LocalPlayer.ID == actorNr)
				{
					LocalPlayer.SetCustomProperties(propertiesToSet);
					return true;
				}
				if ((int)loadBalancingPeer.DebugOut >= 1)
				{
					DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. To use expectedProperties or webForward, you have to be in a room. State: " + State);
				}
				return false;
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(propertiesToSet);
			return OpSetPropertiesOfActor(actorNr, hashtable, expectedProperties, webForward);
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
		{
			if (CurrentRoom == null)
			{
				if ((int)loadBalancingPeer.DebugOut >= 1)
				{
					DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfActor() failed because this client is not in a room currently. State: " + State);
				}
				return false;
			}
			if (expectedProperties == null || expectedProperties.Count == 0)
			{
				Player player = CurrentRoom.GetPlayer(actorNr);
				if (player != null)
				{
					player.CacheProperties(actorProperties);
				}
			}
			return loadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webForward);
		}

		public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties = null, bool webForward = false)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(propertiesToSet);
			return OpSetPropertiesOfRoom(hashtable, expectedProperties, webForward);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
		{
			if (CurrentRoom == null)
			{
				if ((int)loadBalancingPeer.DebugOut >= 1)
				{
					DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfRoom() failed because this client is not in a room currently. State: " + State);
				}
				return false;
			}
			if (expectedProperties == null || expectedProperties.Count == 0)
			{
				CurrentRoom.CacheProperties(gameProperties);
			}
			return loadBalancingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webForward);
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
		{
			if (loadBalancingPeer == null)
			{
				return false;
			}
			return loadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
		}

		public bool OpWebRpc(string uriPath, object parameters)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(209, uriPath);
			dictionary.Add(208, parameters);
			return loadBalancingPeer.OpCustom(219, dictionary, true);
		}

		private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
		{
			if (CurrentRoom != null && gameProperties != null)
			{
				CurrentRoom.CacheProperties(gameProperties);
			}
			if (actorProperties == null || actorProperties.Count <= 0)
			{
				return;
			}
			if (targetActorNr > 0)
			{
				Player player = CurrentRoom.GetPlayer(targetActorNr);
				if (player != null)
				{
					player.CacheProperties(ReadoutPropertiesForActorNr(actorProperties, targetActorNr));
				}
				return;
			}
			foreach (object key in actorProperties.Keys)
			{
				int num = (int)key;
				Hashtable hashtable = (Hashtable)actorProperties[key];
				string actorName = (string)hashtable[byte.MaxValue];
				Player player2 = CurrentRoom.GetPlayer(num);
				if (player2 == null)
				{
					player2 = CreatePlayer(actorName, num, false, hashtable);
					CurrentRoom.StorePlayer(player2);
				}
				else
				{
					player2.CacheProperties(hashtable);
				}
			}
		}

		private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
		{
			if (actorProperties.ContainsKey(actorNr))
			{
				return (Hashtable)actorProperties[actorNr];
			}
			return actorProperties;
		}

		protected internal void ChangeLocalID(int newID)
		{
			if (LocalPlayer == null)
			{
				DebugReturn(DebugLevel.WARNING, string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", LocalPlayer, CurrentRoom.Players == null, newID));
			}
			if (CurrentRoom == null)
			{
				LocalPlayer.ChangeLocalID(newID);
				LocalPlayer.RoomReference = null;
				return;
			}
			CurrentRoom.RemovePlayer(LocalPlayer);
			LocalPlayer.ChangeLocalID(newID);
			CurrentRoom.StorePlayer(LocalPlayer);
			LocalPlayer.LoadBalancingClient = this;
		}

		private void CleanCachedValues()
		{
			ChangeLocalID(-1);
			isFetchingFriendList = false;
			if (Server == ServerConnection.GameServer || State == ClientState.Disconnecting || State == ClientState.Uninitialized)
			{
				CurrentRoom = null;
			}
			if (Server == ServerConnection.MasterServer || State == ClientState.Disconnecting || State == ClientState.Uninitialized)
			{
				RoomInfoList.Clear();
			}
		}

		private void GameEnteredOnGameServer(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode != 0)
			{
				switch (operationResponse.OperationCode)
				{
				case 227:
					DebugReturn(DebugLevel.ERROR, "Create failed on GameServer. Changing back to MasterServer. ReturnCode: " + operationResponse.ReturnCode);
					break;
				case 225:
				case 226:
					DebugReturn(DebugLevel.ERROR, "Join failed on GameServer. Changing back to MasterServer.");
					if (operationResponse.ReturnCode == 32758)
					{
						DebugReturn(DebugLevel.INFO, "Most likely the game became empty during the switch to GameServer.");
					}
					break;
				}
				DisconnectToReconnect();
				return;
			}
			CurrentRoom = CreateRoom(enterRoomParamsCache.RoomName, enterRoomParamsCache.RoomOptions);
			CurrentRoom.LoadBalancingClient = this;
			CurrentRoom.IsLocalClientInside = true;
			State = ClientState.Joined;
			if (operationResponse.Parameters.ContainsKey(252))
			{
				int[] actorsInGame = (int[])operationResponse.Parameters[252];
				UpdatedActorList(actorsInGame);
			}
			int newID = (int)operationResponse[254];
			ChangeLocalID(newID);
			Hashtable actorProperties = (Hashtable)operationResponse[249];
			Hashtable gameProperties = (Hashtable)operationResponse[248];
			ReadoutProperties(gameProperties, actorProperties, 0);
			switch (operationResponse.OperationCode)
			{
			case 227:
				break;
			case 225:
			case 226:
				break;
			}
		}

		private void UpdatedActorList(int[] actorsInGame)
		{
			if (actorsInGame == null)
			{
				return;
			}
			foreach (int num in actorsInGame)
			{
				Player player = CurrentRoom.GetPlayer(num);
				if (player == null)
				{
					CurrentRoom.StorePlayer(CreatePlayer(string.Empty, num, false, null));
				}
			}
		}

		protected internal virtual Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
		{
			return new Player(actorName, actorNumber, isLocal, actorProperties);
		}

		protected internal virtual Room CreateRoom(string roomName, RoomOptions opt)
		{
			if (opt == null)
			{
				opt = new RoomOptions();
			}
			return new Room(roomName, opt);
		}

		public virtual void DebugReturn(DebugLevel level, string message)
		{
		}

		public virtual void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.Parameters.ContainsKey(221))
			{
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.Token = operationResponse[221] as string;
			}
			switch (operationResponse.OperationCode)
			{
			case 222:
			{
				if (operationResponse.ReturnCode != 0)
				{
					DebugReturn(DebugLevel.ERROR, "OpFindFriends failed: " + operationResponse.ToStringFull());
					isFetchingFriendList = false;
					break;
				}
				bool[] array = operationResponse[1] as bool[];
				string[] array2 = operationResponse[2] as string[];
				FriendList = new List<FriendInfo>(friendListRequested.Length);
				for (int i = 0; i < friendListRequested.Length; i++)
				{
					FriendInfo friendInfo = new FriendInfo();
					friendInfo.Name = friendListRequested[i];
					friendInfo.Room = array2[i];
					friendInfo.IsOnline = array[i];
					FriendList.Insert(i, friendInfo);
				}
				friendListRequested = null;
				isFetchingFriendList = false;
				friendListTimestamp = Environment.TickCount;
				if (friendListTimestamp == 0)
				{
					friendListTimestamp = 1;
				}
				break;
			}
			case 230:
				if (operationResponse.ReturnCode != 0)
				{
					DebugReturn(DebugLevel.ERROR, string.Concat(operationResponse.ToStringFull(), " Server: ", Server, " Address: ", loadBalancingPeer.ServerAddress));
					switch (operationResponse.ReturnCode)
					{
					case short.MaxValue:
						DisconnectedCause = DisconnectCause.InvalidAuthentication;
						break;
					case 32755:
						DisconnectedCause = DisconnectCause.CustomAuthenticationFailed;
						break;
					case 32756:
						DisconnectedCause = DisconnectCause.InvalidRegion;
						break;
					case 32757:
						DisconnectedCause = DisconnectCause.MaxCcuReached;
						break;
					case -3:
						DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
						break;
					}
					State = ClientState.Disconnecting;
					Disconnect();
					break;
				}
				if (Server == ServerConnection.NameServer || Server == ServerConnection.MasterServer)
				{
					if (operationResponse.Parameters.ContainsKey(225))
					{
						string text2 = (string)operationResponse.Parameters[225];
						if (!string.IsNullOrEmpty(text2))
						{
							UserId = text2;
							DebugReturn(DebugLevel.INFO, string.Format("Setting UserId sent by Server:{0}", UserId));
						}
					}
					if (operationResponse.Parameters.ContainsKey(202))
					{
						NickName = (string)operationResponse.Parameters[202];
						DebugReturn(DebugLevel.INFO, string.Format("Setting Nickname sent by Server:{0}", NickName));
					}
				}
				if (Server == ServerConnection.NameServer)
				{
					MasterServerAddress = operationResponse[230] as string;
					DisconnectToReconnect();
				}
				else if (Server == ServerConnection.MasterServer)
				{
					State = ClientState.ConnectedToMaster;
					if (AutoJoinLobby)
					{
						loadBalancingPeer.OpJoinLobby(CurrentLobby);
					}
				}
				else if (Server == ServerConnection.GameServer)
				{
					State = ClientState.Joining;
					enterRoomParamsCache.PlayerProperties = LocalPlayer.AllProperties;
					enterRoomParamsCache.OnGameServer = true;
					if (lastJoinType == JoinType.JoinRoom || lastJoinType == JoinType.JoinRandomRoom || lastJoinType == JoinType.JoinOrCreateRoom)
					{
						loadBalancingPeer.OpJoinRoom(enterRoomParamsCache);
					}
					else if (lastJoinType == JoinType.CreateRoom)
					{
						loadBalancingPeer.OpCreateRoom(enterRoomParamsCache);
					}
				}
				break;
			case 220:
				AvailableRegions = operationResponse[210] as string[];
				AvailableRegionsServers = operationResponse[230] as string[];
				break;
			case 254:
				State = ClientState.DisconnectingFromGameserver;
				loadBalancingPeer.Disconnect();
				break;
			case 229:
				State = ClientState.JoinedLobby;
				inLobby = true;
				break;
			case 228:
				State = ClientState.ConnectedToMaster;
				inLobby = false;
				break;
			case 225:
			case 226:
			case 227:
			{
				if (Server == ServerConnection.GameServer)
				{
					GameEnteredOnGameServer(operationResponse);
					break;
				}
				if (operationResponse.ReturnCode != 0)
				{
					State = ((!inLobby) ? ClientState.ConnectedToMaster : ClientState.JoinedLobby);
					if ((int)loadBalancingPeer.DebugOut >= 1)
					{
						DebugReturn(DebugLevel.ERROR, string.Format("Getting into game failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
					}
					break;
				}
				GameServerAddress = (string)operationResponse[230];
				string text = operationResponse[byte.MaxValue] as string;
				if (!string.IsNullOrEmpty(text))
				{
					enterRoomParamsCache.RoomName = text;
				}
				DisconnectToReconnect();
				break;
			}
			}
			if (OnOpResponseAction != null)
			{
				OnOpResponseAction(operationResponse);
			}
		}

		public virtual void OnStatusChanged(StatusCode statusCode)
		{
			switch (statusCode)
			{
			case StatusCode.Connect:
				inLobby = false;
				if (State == ClientState.ConnectingToNameServer)
				{
					if ((int)loadBalancingPeer.DebugOut >= 5)
					{
						DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
					}
					Server = ServerConnection.NameServer;
					if (AuthValues != null)
					{
						AuthValues.Token = null;
					}
				}
				if (State == ClientState.ConnectingToGameserver)
				{
					if ((int)loadBalancingPeer.DebugOut >= 5)
					{
						DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
					}
					Server = ServerConnection.GameServer;
				}
				if (State == ClientState.ConnectingToMasterserver)
				{
					if ((int)loadBalancingPeer.DebugOut >= 5)
					{
						DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
					}
					Server = ServerConnection.MasterServer;
				}
				loadBalancingPeer.EstablishEncryption();
				if (IsAuthorizeSecretAvailable)
				{
					didAuthenticate = loadBalancingPeer.OpAuthenticate(AppId, AppVersion, AuthValues, CloudRegion, requestLobbyStatistics);
					if (didAuthenticate)
					{
						State = ClientState.Authenticating;
					}
					else
					{
						DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticateWithToken! Check log output, AuthValues and if you're connected. State: " + State);
					}
				}
				break;
			case StatusCode.EncryptionEstablished:
				if (Server == ServerConnection.NameServer)
				{
					State = ClientState.ConnectedToNameServer;
				}
				if (!didAuthenticate && (!IsUsingNameServer || CloudRegion != null))
				{
					didAuthenticate = loadBalancingPeer.OpAuthenticate(AppId, AppVersion, AuthValues, CloudRegion, requestLobbyStatistics);
					if (didAuthenticate)
					{
						State = ClientState.Authenticating;
					}
					else
					{
						DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + State);
					}
				}
				break;
			case StatusCode.Disconnect:
				CleanCachedValues();
				didAuthenticate = false;
				inLobby = false;
				switch (State)
				{
				case ClientState.Uninitialized:
				case ClientState.Disconnecting:
					if (AuthValues != null)
					{
						AuthValues.Token = null;
					}
					State = ClientState.Disconnected;
					break;
				case ClientState.DisconnectingFromGameserver:
				case ClientState.DisconnectingFromNameServer:
					Connect();
					break;
				case ClientState.DisconnectingFromMasterserver:
					ConnectToGameServer();
					break;
				default:
				{
					string empty = string.Empty;
					DebugReturn(DebugLevel.WARNING, string.Concat("Got a unexpected Disconnect in LoadBalancingClient State: ", State, ". ", empty));
					if (AuthValues != null)
					{
						AuthValues.Token = null;
					}
					State = ClientState.Disconnected;
					break;
				}
				}
				break;
			case StatusCode.DisconnectByServerUserLimit:
				DebugReturn(DebugLevel.ERROR, "The Photon license's CCU Limit was reached. Server rejected this connection. Wait and re-try.");
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.DisconnectByServerUserLimit;
				State = ClientState.Disconnected;
				break;
			case StatusCode.SecurityExceptionOnConnect:
			case StatusCode.ExceptionOnConnect:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.ExceptionOnConnect;
				State = ClientState.Disconnected;
				break;
			case StatusCode.DisconnectByServer:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.DisconnectByServer;
				State = ClientState.Disconnected;
				break;
			case StatusCode.TimeoutDisconnect:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.TimeoutDisconnect;
				State = ClientState.Disconnected;
				break;
			case StatusCode.Exception:
			case StatusCode.ExceptionOnReceive:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.Exception;
				State = ClientState.Disconnected;
				break;
			}
		}

		public virtual void OnEvent(EventData photonEvent)
		{
			switch (photonEvent.Code)
			{
			case 229:
			case 230:
			{
				if (photonEvent.Code == 230)
				{
					RoomInfoList = new Dictionary<string, RoomInfo>();
				}
				Hashtable hashtable = (Hashtable)photonEvent[222];
				foreach (string key in hashtable.Keys)
				{
					RoomInfo roomInfo = new RoomInfo(key, (Hashtable)hashtable[key]);
					if (roomInfo.removedFromList)
					{
						RoomInfoList.Remove(key);
					}
					else
					{
						RoomInfoList[key] = roomInfo;
					}
				}
				break;
			}
			case byte.MaxValue:
			{
				int num2 = (int)photonEvent[254];
				Hashtable hashtable3 = (Hashtable)photonEvent[249];
				Player player = CurrentRoom.GetPlayer(num2);
				if (player == null)
				{
					Player player2 = CreatePlayer(string.Empty, num2, false, hashtable3);
					CurrentRoom.StorePlayer(player2);
				}
				else
				{
					player.CacheProperties(hashtable3);
					player.IsInactive = false;
				}
				if (LocalPlayer.ID == num2)
				{
					int[] actorsInGame = (int[])photonEvent[252];
					UpdatedActorList(actorsInGame);
				}
				break;
			}
			case 254:
			{
				int id = (int)photonEvent[254];
				bool flag = false;
				if (photonEvent.Parameters.ContainsKey(233))
				{
					flag = (bool)photonEvent.Parameters[233];
				}
				if (flag)
				{
					CurrentRoom.MarkAsInactive(id);
				}
				else
				{
					CurrentRoom.RemovePlayer(id);
				}
				break;
			}
			case 253:
			{
				int num = 0;
				if (photonEvent.Parameters.ContainsKey(253))
				{
					num = (int)photonEvent[253];
				}
				Hashtable hashtable2 = (Hashtable)photonEvent[251];
				if (num > 0)
				{
					ReadoutProperties(null, hashtable2, num);
				}
				else
				{
					ReadoutProperties(hashtable2, null, 0);
				}
				break;
			}
			case 226:
				PlayersInRoomsCount = (int)photonEvent[229];
				RoomsCount = (int)photonEvent[228];
				PlayersOnMasterCount = (int)photonEvent[227];
				break;
			case 224:
			{
				string[] array = photonEvent[213] as string[];
				byte[] array2 = photonEvent[212] as byte[];
				int[] array3 = photonEvent[229] as int[];
				int[] array4 = photonEvent[228] as int[];
				lobbyStatistics.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					TypedLobbyInfo typedLobbyInfo = new TypedLobbyInfo();
					typedLobbyInfo.Name = array[i];
					typedLobbyInfo.Type = (LobbyType)array2[i];
					typedLobbyInfo.PlayerCount = array3[i];
					typedLobbyInfo.RoomCount = array4[i];
					lobbyStatistics.Add(typedLobbyInfo);
				}
				break;
			}
			}
			if (OnEventAction != null)
			{
				OnEventAction(photonEvent);
			}
		}

		public virtual void OnMessage(object message)
		{
			DebugReturn(DebugLevel.ALL, string.Format("got OnMessage {0}", message));
		}
	}
}
