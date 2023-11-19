namespace ExitGames.Client.Photon.LoadBalancing
{
	public class RoomOptions
	{
		private bool isVisible = true;

		private bool isOpen = true;

		public byte MaxPlayers;

		public int PlayerTtl;

		public int EmptyRoomTtl;

		private bool cleanupCacheOnLeave = true;

		public Hashtable CustomRoomProperties;

		public string[] CustomRoomPropertiesForLobby = new string[0];

		public string[] Plugins;

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				isVisible = value;
			}
		}

		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
			set
			{
				isOpen = value;
			}
		}

		public bool CheckUserOnJoin { get; set; }

		public bool CleanupCacheOnLeave
		{
			get
			{
				return cleanupCacheOnLeave;
			}
			set
			{
				cleanupCacheOnLeave = value;
			}
		}

		public bool SuppressRoomEvents { get; set; }

		public bool PublishUserId { get; set; }
	}
}
