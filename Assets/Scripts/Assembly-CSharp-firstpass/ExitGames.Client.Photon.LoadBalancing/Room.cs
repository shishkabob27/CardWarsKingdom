using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class Room : RoomInfo
	{
		protected internal int PlayerTTL;

		protected internal int RoomTTL;

		private Dictionary<int, Player> players = new Dictionary<int, Player>();

		protected internal LoadBalancingClient LoadBalancingClient { get; set; }

		public new string Name
		{
			get
			{
				return name;
			}
			internal set
			{
				name = value;
			}
		}

		public new bool IsOpen
		{
			get
			{
				return isOpen;
			}
			set
			{
				if (!base.IsLocalClientInside)
				{
					LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
				}
				if (value != isOpen)
				{
					LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable { 
					{
						(byte)253,
						value
					} });
				}
				isOpen = value;
			}
		}

		public new bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				if (!base.IsLocalClientInside)
				{
					LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
				}
				if (value != isVisible)
				{
					LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable { 
					{
						(byte)254,
						value
					} });
				}
				isVisible = value;
			}
		}

		public new byte MaxPlayers
		{
			get
			{
				return maxPlayers;
			}
			set
			{
				if (!base.IsLocalClientInside)
				{
					LoadBalancingClient.DebugReturn(DebugLevel.WARNING, "Can't set room properties when not in that room.");
				}
				if (value != maxPlayers)
				{
					LoadBalancingClient.OpSetPropertiesOfRoom(new Hashtable { 
					{
						byte.MaxValue,
						value
					} });
				}
				maxPlayers = value;
			}
		}

		public new byte PlayerCount
		{
			get
			{
				if (Players == null)
				{
					return 0;
				}
				return (byte)Players.Count;
			}
		}

		public Dictionary<int, Player> Players
		{
			get
			{
				return players;
			}
			private set
			{
				players = value;
			}
		}

		public string[] ExpectedUsers
		{
			get
			{
				return expectedUsersField;
			}
		}

		public int MasterClientId { get; private set; }

		public string[] PropsListedInLobby
		{
			get
			{
				return propsListedInLobby;
			}
			private set
			{
				propsListedInLobby = value;
			}
		}

		protected internal Room()
			: base(null, null)
		{
		}

		protected internal Room(string roomName)
			: base(roomName, null)
		{
		}

		[Obsolete]
		protected internal Room(string roomName, Hashtable roomProperties, bool isVisible, bool isOpen, byte maxPlayers, string[] propsListedInLobby)
			: base(roomName, roomProperties)
		{
			base.isVisible = isVisible;
			base.isOpen = isOpen;
			base.maxPlayers = maxPlayers;
			PropsListedInLobby = propsListedInLobby;
		}

		protected internal Room(string roomName, RoomOptions options)
			: base(roomName, options.CustomRoomProperties)
		{
			isVisible = options.IsVisible;
			isOpen = options.IsOpen;
			maxPlayers = options.MaxPlayers;
			PropsListedInLobby = options.CustomRoomPropertiesForLobby;
			PlayerTTL = options.PlayerTtl;
			RoomTTL = options.EmptyRoomTtl;
		}

		public virtual void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, bool webForward = false)
		{
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			if (expectedProperties == null || expectedProperties.Count == 0)
			{
				base.CustomProperties.Merge(hashtable);
				base.CustomProperties.StripKeysWithNullValues();
			}
			if (base.IsLocalClientInside)
			{
				LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfRoom(hashtable, expectedProperties, webForward);
			}
		}

		public void SetPropertiesListedInLobby(string[] propsToListInLobby)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)250] = propsToListInLobby;
			if (LoadBalancingClient.OpSetPropertiesOfRoom(hashtable))
			{
				propsListedInLobby = propsToListInLobby;
			}
		}

		protected internal virtual void RemovePlayer(Player player)
		{
			Players.Remove(player.ID);
			player.RoomReference = null;
			if (player.ID == MasterClientId)
			{
				UpdateMasterClientId();
			}
		}

		protected internal virtual void RemovePlayer(int id)
		{
			RemovePlayer(GetPlayer(id));
		}

		protected internal virtual void MarkAsInactive(int id)
		{
			Player player = GetPlayer(id);
			if (player != null)
			{
				player.IsInactive = true;
			}
		}

		private void UpdateMasterClientId()
		{
			int num = int.MaxValue;
			foreach (int key in Players.Keys)
			{
				if (key < num)
				{
					num = key;
				}
			}
			if (players.Count == 0)
			{
				num = 0;
			}
			MasterClientId = num;
		}

		public virtual bool AddPlayer(Player player)
		{
			if (!Players.ContainsKey(player.ID))
			{
				StorePlayer(player);
				return true;
			}
			return false;
		}

		public virtual Player StorePlayer(Player player)
		{
			Players[player.ID] = player;
			player.RoomReference = this;
			if (MasterClientId == 0 || player.ID < MasterClientId)
			{
				UpdateMasterClientId();
			}
			return player;
		}

		public virtual Player GetPlayer(int id)
		{
			Player value = null;
			Players.TryGetValue(id, out value);
			return value;
		}

		public void ClearExpectedUsers()
		{
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)247] = null;
			LoadBalancingClient.OpSetPropertiesOfRoom(hashtable);
		}

		public override string ToString()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", name, (!isVisible) ? "hidden" : "visible", (!isOpen) ? "closed" : "open", maxPlayers, PlayerCount);
		}

		public new string ToStringFull()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", name, (!isVisible) ? "hidden" : "visible", (!isOpen) ? "closed" : "open", maxPlayers, PlayerCount, SupportClass.DictionaryToString(base.CustomProperties));
		}
	}
}
