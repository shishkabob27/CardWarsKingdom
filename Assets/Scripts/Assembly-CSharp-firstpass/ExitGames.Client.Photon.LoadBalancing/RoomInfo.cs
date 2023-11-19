namespace ExitGames.Client.Photon.LoadBalancing
{
	public class RoomInfo
	{
		protected internal bool removedFromList;

		private Hashtable customProperties = new Hashtable();

		protected byte maxPlayers;

		protected string[] expectedUsersField;

		protected bool isOpen = true;

		protected bool isVisible = true;

		protected string name;

		protected internal int masterClientIdField;

		protected string[] propsListedInLobby;

		protected internal bool serverSideMasterClient { get; private set; }

		public Hashtable CustomProperties
		{
			get
			{
				return customProperties;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int PlayerCount { get; private set; }

		public bool IsLocalClientInside { get; set; }

		public byte MaxPlayers
		{
			get
			{
				return maxPlayers;
			}
		}

		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
		}

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
		}

		protected internal RoomInfo(string roomName, Hashtable roomProperties)
		{
			CacheProperties(roomProperties);
			name = roomName;
		}

		public override bool Equals(object other)
		{
			RoomInfo roomInfo = other as RoomInfo;
			return roomInfo != null && name.Equals(roomInfo.name);
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", name, (!isVisible) ? "hidden" : "visible", (!isOpen) ? "closed" : "open", maxPlayers, PlayerCount);
		}

		public string ToStringFull()
		{
			return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", name, (!isVisible) ? "hidden" : "visible", (!isOpen) ? "closed" : "open", maxPlayers, PlayerCount, SupportClass.DictionaryToString(customProperties));
		}

		protected internal virtual void CacheProperties(Hashtable propertiesToCache)
		{
			if (propertiesToCache == null || propertiesToCache.Count == 0 || customProperties.Equals(propertiesToCache))
			{
				return;
			}
			if (propertiesToCache.ContainsKey((byte)251))
			{
				removedFromList = (bool)propertiesToCache[(byte)251];
				if (removedFromList)
				{
					return;
				}
			}
			if (propertiesToCache.ContainsKey(byte.MaxValue))
			{
				maxPlayers = (byte)propertiesToCache[byte.MaxValue];
			}
			if (propertiesToCache.ContainsKey((byte)253))
			{
				isOpen = (bool)propertiesToCache[(byte)253];
			}
			if (propertiesToCache.ContainsKey((byte)254))
			{
				isVisible = (bool)propertiesToCache[(byte)254];
			}
			if (propertiesToCache.ContainsKey((byte)252))
			{
				PlayerCount = (byte)propertiesToCache[(byte)252];
			}
			if (propertiesToCache.ContainsKey((byte)248))
			{
				serverSideMasterClient = true;
				bool flag = masterClientIdField != 0;
				masterClientIdField = (int)propertiesToCache[(byte)248];
			}
			if (propertiesToCache.ContainsKey((byte)250))
			{
				propsListedInLobby = propertiesToCache[(byte)250] as string[];
			}
			if (propertiesToCache.ContainsKey((byte)247))
			{
				expectedUsersField = (string[])propertiesToCache[(byte)247];
			}
			customProperties.MergeStringKeys(propertiesToCache);
		}
	}
}
