using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class LoadBalancingPeer : PhotonPeer
	{
		public class EnterRoomParams
		{
			public string RoomName;

			public RoomOptions RoomOptions;

			public TypedLobby Lobby;

			public Hashtable PlayerProperties;

			public bool OnGameServer = true;

			public bool CreateIfNotExists;

			public string[] ExpectedUsers;

			public int ActorNumber;
		}

		public class OpJoinRandomRoomParams
		{
			public Hashtable ExpectedCustomRoomProperties;

			public byte ExpectedMaxPlayers;

			public MatchmakingMode MatchingType;

			public TypedLobby TypedLobby;

			public string SqlLobbyFilter;

			public string[] ExpectedUsers;
		}

		private readonly Dictionary<byte, object> opParameters = new Dictionary<byte, object>();

		public LoadBalancingPeer(ConnectionProtocol protocolType)
			: base(protocolType)
		{
		}

		public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
			: base(listener, protocolType)
		{
		}

		public virtual bool OpGetRegions(string appId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[224] = appId;
			return OpCustom(220, dictionary, true, 0, true);
		}

		public virtual bool OpJoinLobby()
		{
			return OpJoinLobby(TypedLobby.Default);
		}

		public virtual bool OpJoinLobby(TypedLobby lobby)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
			}
			Dictionary<byte, object> dictionary = null;
			if (lobby != null && !lobby.IsDefault)
			{
				dictionary = new Dictionary<byte, object>();
				dictionary[213] = lobby.Name;
				dictionary[212] = (byte)lobby.Type;
			}
			return OpCustom(229, dictionary, true);
		}

		public virtual bool OpLeaveLobby()
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
			}
			return OpCustom(228, null, true);
		}

		private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
		{
			if (roomOptions == null)
			{
				roomOptions = new RoomOptions();
			}
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)253] = roomOptions.IsOpen;
			hashtable[(byte)254] = roomOptions.IsVisible;
			hashtable[(byte)250] = ((roomOptions.CustomRoomPropertiesForLobby != null) ? roomOptions.CustomRoomPropertiesForLobby : new string[0]);
			hashtable.MergeStringKeys(roomOptions.CustomRoomProperties);
			if (roomOptions.MaxPlayers > 0)
			{
				hashtable[byte.MaxValue] = roomOptions.MaxPlayers;
			}
			op[248] = hashtable;
			op[241] = roomOptions.CleanupCacheOnLeave;
			if (roomOptions.CheckUserOnJoin)
			{
				op[232] = true;
			}
			if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
			{
				op[235] = roomOptions.PlayerTtl;
			}
			if (roomOptions.EmptyRoomTtl > 0)
			{
				op[236] = roomOptions.EmptyRoomTtl;
			}
			if (roomOptions.SuppressRoomEvents)
			{
				op[237] = true;
			}
			if (roomOptions.Plugins != null)
			{
				op[204] = roomOptions.Plugins;
			}
			if (roomOptions.PublishUserId)
			{
				op[239] = true;
			}
		}

		public virtual bool OpCreateRoom(EnterRoomParams opParams)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (!string.IsNullOrEmpty(opParams.RoomName))
			{
				dictionary[byte.MaxValue] = opParams.RoomName;
			}
			if (opParams.Lobby != null && !string.IsNullOrEmpty(opParams.Lobby.Name))
			{
				dictionary[213] = opParams.Lobby.Name;
				dictionary[212] = (byte)opParams.Lobby.Type;
			}
			if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
			{
				dictionary[238] = opParams.ExpectedUsers;
			}
			if (opParams.OnGameServer)
			{
				if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
				{
					dictionary[249] = opParams.PlayerProperties;
					dictionary[250] = true;
				}
				RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
			}
			return OpCustom(227, dictionary, true);
		}

		public virtual bool OpJoinRoom(EnterRoomParams opParams)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (!string.IsNullOrEmpty(opParams.RoomName))
			{
				dictionary[byte.MaxValue] = opParams.RoomName;
			}
			if (opParams.CreateIfNotExists)
			{
				dictionary[215] = (byte)1;
				if (opParams.Lobby != null)
				{
					dictionary[213] = opParams.Lobby.Name;
					dictionary[212] = (byte)opParams.Lobby.Type;
				}
			}
			if (opParams.ActorNumber != 0)
			{
				dictionary[215] = (byte)3;
				dictionary[254] = opParams.ActorNumber;
			}
			if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
			{
				dictionary[238] = opParams.ExpectedUsers;
			}
			if (opParams.OnGameServer)
			{
				if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
				{
					dictionary[249] = opParams.PlayerProperties;
					dictionary[250] = true;
				}
				if (opParams.CreateIfNotExists)
				{
					RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
				}
			}
			return OpCustom(226, dictionary, true);
		}

		public virtual bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
			if (opJoinRandomRoomParams.ExpectedMaxPlayers > 0)
			{
				hashtable[byte.MaxValue] = opJoinRandomRoomParams.ExpectedMaxPlayers;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (hashtable.Count > 0)
			{
				dictionary[248] = hashtable;
			}
			if (opJoinRandomRoomParams.MatchingType != 0)
			{
				dictionary[223] = (byte)opJoinRandomRoomParams.MatchingType;
			}
			if (opJoinRandomRoomParams.TypedLobby != null && !string.IsNullOrEmpty(opJoinRandomRoomParams.TypedLobby.Name))
			{
				dictionary[213] = opJoinRandomRoomParams.TypedLobby.Name;
				dictionary[212] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
			}
			if (!string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter))
			{
				dictionary[245] = opJoinRandomRoomParams.SqlLobbyFilter;
			}
			if (opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length > 0)
			{
				dictionary[238] = opJoinRandomRoomParams.ExpectedUsers;
			}
			return OpCustom(225, dictionary, true);
		}

		public virtual bool OpLeaveRoom(bool becomeInactive)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (becomeInactive)
			{
				dictionary[233] = becomeInactive;
			}
			return OpCustom(254, dictionary, true);
		}

		public virtual bool OpFindFriends(string[] friendsToFind)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (friendsToFind != null && friendsToFind.Length > 0)
			{
				dictionary[1] = friendsToFind;
			}
			return OpCustom(222, dictionary, true);
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
			}
			if (actorNr <= 0 || actorProperties == null)
			{
				if ((int)base.DebugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(251, actorProperties);
			dictionary.Add(254, actorNr);
			dictionary.Add(250, true);
			if (expectedProperties != null && expectedProperties.Count != 0)
			{
				dictionary.Add(231, expectedProperties);
			}
			if (webForward)
			{
				dictionary[234] = true;
			}
			return OpCustom(252, dictionary, true, 0, false);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(251, gameProperties);
			dictionary.Add(250, true);
			if (expectedProperties != null && expectedProperties.Count != 0)
			{
				dictionary.Add(231, expectedProperties);
			}
			if (webForward)
			{
				dictionary[234] = true;
			}
			return OpCustom(252, dictionary, true, 0, false);
		}

		public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (getLobbyStatistics)
			{
				dictionary[211] = true;
			}
			if (authValues != null && authValues.Token != null)
			{
				dictionary[221] = authValues.Token;
				return OpCustom(230, dictionary, true, 0, false);
			}
			dictionary[220] = appVersion;
			dictionary[224] = appId;
			if (!string.IsNullOrEmpty(regionCode))
			{
				dictionary[210] = regionCode;
			}
			if (authValues != null)
			{
				if (!string.IsNullOrEmpty(authValues.UserId))
				{
					dictionary[225] = authValues.UserId;
				}
				if (authValues.AuthType != CustomAuthenticationType.None)
				{
					dictionary[217] = (byte)authValues.AuthType;
					if (!string.IsNullOrEmpty(authValues.Token))
					{
						dictionary[221] = authValues.Token;
					}
					else
					{
						if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
						{
							dictionary[216] = authValues.AuthGetParameters;
						}
						if (authValues.AuthPostData != null)
						{
							dictionary[214] = authValues.AuthPostData;
						}
					}
				}
			}
			return OpCustom(230, dictionary, true, 0, base.IsEncryptionAvailable);
		}

		public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			if ((int)base.DebugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (groupsToRemove != null)
			{
				dictionary[239] = groupsToRemove;
			}
			if (groupsToAdd != null)
			{
				dictionary[238] = groupsToAdd;
			}
			return OpCustom(248, dictionary, true, 0);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent)
		{
			return OpRaiseEvent(eventCode, customEventContent, sendReliable, null);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent, byte channelId, EventCaching cache, int[] targetActors, ReceiverGroup receivers, byte interestGroup)
		{
			return OpRaiseEvent(eventCode, sendReliable, customEventContent, channelId, cache, targetActors, receivers, interestGroup, false);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, bool sendReliable, object customEventContent, byte channelId, EventCaching cache, int[] targetActors, ReceiverGroup receivers, byte interestGroup, bool forwardToWebhook)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[244] = eventCode;
			if (customEventContent != null)
			{
				dictionary[245] = customEventContent;
			}
			if (cache != 0)
			{
				dictionary[247] = (byte)cache;
			}
			if (receivers != 0)
			{
				dictionary[246] = (byte)receivers;
			}
			if (interestGroup != 0)
			{
				dictionary[240] = interestGroup;
			}
			if (targetActors != null)
			{
				dictionary[252] = targetActors;
			}
			if (forwardToWebhook)
			{
				dictionary[234] = true;
			}
			return OpCustom(253, dictionary, sendReliable, channelId, false);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, byte interestGroup, Hashtable customEventContent, bool sendReliable)
		{
			return OpRaiseEvent(eventCode, sendReliable, customEventContent, 0, EventCaching.DoNotCache, null, ReceiverGroup.Others, 0, false);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId)
		{
			return OpRaiseEvent(eventCode, sendReliable, evData, channelId, EventCaching.DoNotCache, null, ReceiverGroup.Others, 0, false);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, int[] targetActors)
		{
			return OpRaiseEvent(eventCode, sendReliable, evData, channelId, EventCaching.DoNotCache, targetActors, ReceiverGroup.Others, 0, false);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, int[] targetActors, EventCaching cache)
		{
			return OpRaiseEvent(eventCode, sendReliable, evData, channelId, cache, targetActors, ReceiverGroup.Others, 0, false);
		}

		[Obsolete("Use overload with RaiseEventOptions to reduce parameter- and overload-clutter.")]
		public virtual bool OpRaiseEvent(byte eventCode, Hashtable evData, bool sendReliable, byte channelId, EventCaching cache, ReceiverGroup receivers)
		{
			return OpRaiseEvent(eventCode, sendReliable, evData, channelId, cache, null, receivers, 0, false);
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
		{
			opParameters.Clear();
			opParameters[244] = eventCode;
			if (customEventContent != null)
			{
				opParameters[245] = customEventContent;
			}
			if (raiseEventOptions == null)
			{
				raiseEventOptions = RaiseEventOptions.Default;
			}
			else
			{
				if (raiseEventOptions.CachingOption != 0)
				{
					opParameters[247] = (byte)raiseEventOptions.CachingOption;
				}
				if (raiseEventOptions.Receivers != 0)
				{
					opParameters[246] = (byte)raiseEventOptions.Receivers;
				}
				if (raiseEventOptions.InterestGroup != 0)
				{
					opParameters[240] = raiseEventOptions.InterestGroup;
				}
				if (raiseEventOptions.TargetActors != null)
				{
					opParameters[252] = raiseEventOptions.TargetActors;
				}
				if (raiseEventOptions.ForwardToWebhook)
				{
					opParameters[234] = true;
				}
			}
			return OpCustom(253, opParameters, sendReliable, raiseEventOptions.SequenceChannel, sendReliable);
		}
	}
}
