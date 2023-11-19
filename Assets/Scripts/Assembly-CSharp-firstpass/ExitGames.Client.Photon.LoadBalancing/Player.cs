using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class Player
	{
		private int actorID = -1;

		public readonly bool IsLocal;

		private string nickName;

		public object Tag;

		protected internal LoadBalancingClient LoadBalancingClient { get; set; }

		protected internal Room RoomReference { get; set; }

		public int ID
		{
			get
			{
				return actorID;
			}
		}

		public string NickName
		{
			get
			{
				return nickName;
			}
			set
			{
				if (string.IsNullOrEmpty(nickName) || !nickName.Equals(value))
				{
					nickName = value;
					if (IsLocal && LoadBalancingClient != null && LoadBalancingClient.State == ClientState.Joined)
					{
						SetPlayerNameProperty();
					}
				}
			}
		}

		[Obsolete("Use NickName")]
		public string Name
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

		public bool IsMasterClient
		{
			get
			{
				if (RoomReference == null)
				{
					return false;
				}
				return ID == RoomReference.MasterClientId;
			}
		}

		public Hashtable CustomProperties { get; private set; }

		public Hashtable AllProperties
		{
			get
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Merge(CustomProperties);
				hashtable[byte.MaxValue] = nickName;
				return hashtable;
			}
		}

		public bool IsInactive { get; set; }

		protected internal Player(string nickName, int actorID, bool isLocal)
			: this(nickName, actorID, isLocal, null)
		{
		}

		protected internal Player(string nickName, int actorID, bool isLocal, Hashtable playerProperties)
		{
			IsLocal = isLocal;
			this.actorID = actorID;
			NickName = nickName;
			CustomProperties = new Hashtable();
			CacheProperties(playerProperties);
		}

		public Player Get(int id)
		{
			if (RoomReference == null)
			{
				return null;
			}
			return RoomReference.GetPlayer(id);
		}

		public Player GetNext()
		{
			return GetNextFor(ID);
		}

		public Player GetNextFor(Player currentPlayer)
		{
			if (currentPlayer == null)
			{
				return null;
			}
			return GetNextFor(currentPlayer.ID);
		}

		public Player GetNextFor(int currentPlayerId)
		{
			if (RoomReference == null || RoomReference.Players == null || RoomReference.Players.Count < 2)
			{
				return null;
			}
			Dictionary<int, Player> players = RoomReference.Players;
			int num = int.MaxValue;
			int num2 = currentPlayerId;
			foreach (int key in players.Keys)
			{
				if (key < num2)
				{
					num2 = key;
				}
				else if (key > currentPlayerId && key < num)
				{
					num = key;
				}
			}
			return (num == int.MaxValue) ? players[num2] : players[num];
		}

		public virtual void CacheProperties(Hashtable properties)
		{
			if (properties == null || properties.Count == 0 || CustomProperties.Equals(properties))
			{
				return;
			}
			if (properties.ContainsKey(byte.MaxValue))
			{
				string text = (string)properties[byte.MaxValue];
				if (text != null)
				{
					if (IsLocal)
					{
						if (!text.Equals(nickName))
						{
							SetPlayerNameProperty();
						}
					}
					else
					{
						NickName = text;
					}
				}
			}
			if (properties.ContainsKey((byte)254))
			{
				IsInactive = (bool)properties[(byte)254];
			}
			CustomProperties.MergeStringKeys(properties);
		}

		public override string ToString()
		{
			return NickName + " " + SupportClass.DictionaryToString(CustomProperties);
		}

		public override bool Equals(object p)
		{
			Player player = p as Player;
			return player != null && GetHashCode() == player.GetHashCode();
		}

		public override int GetHashCode()
		{
			return ID;
		}

		protected internal void ChangeLocalID(int newID)
		{
			if (IsLocal)
			{
				actorID = newID;
			}
		}

		public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedProperties = null, bool webForward = false)
		{
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			if (expectedProperties == null)
			{
				CustomProperties.Merge(hashtable);
				CustomProperties.StripKeysWithNullValues();
			}
			if (RoomReference != null && RoomReference.IsLocalClientInside)
			{
				LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfActor(actorID, hashtable, expectedProperties, webForward);
			}
		}

		private void SetPlayerNameProperty()
		{
			Hashtable hashtable = new Hashtable();
			hashtable[byte.MaxValue] = nickName;
			LoadBalancingClient.OpSetPropertiesOfActor(ID, hashtable);
		}
	}
}
