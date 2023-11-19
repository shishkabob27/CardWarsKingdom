using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>, IChatClientListener
{
	public class UserNameId
	{
		public string chatname;

		public string name;

		public string id;

		public bool onlineStatus;

		public UserNameId(string inputname, string inputid, bool status = false)
		{
			name = inputname;
			id = inputid;
			onlineStatus = status;
			chatname = name + "." + id;
		}
	}

	public enum chatEventCode
	{
		Connected,
		Disconnected,
		IncomingMessages,
		IncomingPrivateMessage,
		StatusUpdate
	}

	public class IncomingMessageObject
	{
		public string PlayerName;

		public string Message;

		public ChatMetaData MetaData;
	}

	public delegate void ChatEvent(chatEventCode e, object obj);

	public string DebugChatId;

	public string ReleaseChatId;

	public string[] ChannelsToJoinOnConnect;

	public int HistoryLengthToFetch;

	private string SpammerChannel = "Spammers";

	private string AdminChannel = "Admin";

	private bool UseSpammerChannel;

	private string myChatName = string.Empty;

	private string hval = "269E84524AF54D1EA625AA8C803D740F";

	private string myidhash = string.Empty;

	private string ChatAppId;

	public List<HelperItem> UserNameIdListOriginal;

	private string[] mMonitorUserarray;

	private List<UserNameId> mMonitorUserUserList = new List<UserNameId>();

	private bool mMonitorUser;

	private List<string> regionAsia = new List<string>
	{
		"AF", "AE", "YD", "YE", "IL", "IQ", "IR", "IN", "ID", "OM",
		"QA", "KH", "KW", "SA", "JK", "SY", "SG", "LK", "KR", "TH",
		"TR", "NP", "BH", "PK", "BD", "BU", "PH", "BT", "BN", "VN",
		"VD", "HK", "MO", "MY", "MM", "MV", "MN", "JO", "LA", "LB",
		"TW", "CN", "NT", "KP"
	};

	private List<string> regionEU = new List<string>
	{
		"AZ", "AM", "UA", "UZ", "KZ", "KG", "GE", "TJ", "TM", "BY",
		"MD", "RU", "IS", "IE", "AL", "AD", "IT", "EE", "AT", "NL",
		"CY", "GR", "GB", "HR", "SM", "GI", "GH", "SE", "SJ", "ES",
		"SK", "SI", "CS", "SU", "CS", "CZ", "DK", "DD", "DE", "NO",
		"VA", "HU", "FI", "FO", "FR", "FX", "BG", "BE", "PL", "BA",
		"PT", "MK", "MT", "MC", "YU", "LV", "LT", "LI", "RO", "LU",
		"HV", "DZ", "AO", "UG", "EG", "ET", "ER", "GH", "CV", "GA",
		"CM", "GM", "GW", "GN", "KE", "CI", "KM", "CG", "CD", "ZR",
		"ST", "ZM", "SL", "DJ", "ZW", "SD", "SZ", "SC", "SN", "SH",
		"SO", "DY", "TZ", "TD", "TN", "TG", "NG", "NA", "NE", "TF",
		"BF", "BI", "HM", "BJ", "BW", "YT", "MG", "MW", "ML", "RH",
		"MU", "MR", "MZ", "MA", "LR", "RW", "LS", "RE", "IO", "LY",
		"EH", "GQ", "CF", "ZA"
	};

	private List<string> regionUS = new List<string>
	{
		"US", "CA", "AW", "AI", "AG", "SV", "AN", "CU", "GT", "GP",
		"GL", "GD", "KY", "CR", "PM", "JM", "KN", "VC", "LC", "TC",
		"DO", "DM", "TT", "NI", "BM", "HT", "PZ", "PA", "BS", "BB",
		"PR", "VI", "BZ", "HN", "MQ", "MX", "MS", "VG", "AR", "UY",
		"EC", "GY", "CO", "SR", "CL", "PY", "PK", "BR", "GF", "VE",
		"PE", "BO", "GS"
	};

	private List<string> regionAU = new List<string>
	{
		"AU", "CT", "KI", "GU", "CK", "CX", "CC", "WS", "JT", "SB",
		"TV", "TK", "TO", "NR", "NU", "NC", "NZ", "NH", "NF", "VU",
		"PG", "PW", "PN", "FJ", "PF", "MH", "FM", "MI", "WF", "PC",
		"PU", "AS", "UM", "MP", "NQ", "BV", "AQ", "JP"
	};

	private List<string> regionJP = new List<string> { "JP" };

	private ChatChannel selectedChannel;

	private string selectedChannelName;

	private string spamChannelName;

	private string myChannel;

	private int selectedChannelIndex;

	private bool doingPrivateChat;

	private string roomChannel;

	public ChatClient chatClient;

	private static string WelcomeText = "Welcome to chat.\\help lists commands.";

	private static string HelpText = "\n\\subscribe <list of channelnames> subscribes channels.\n\\unsubscribe <list of channelnames> leaves channels.\n\\msg <username> <message> send private message to user.\n\\clear clears the current chat tab. private chats get closed.\n\\help gets this help message.";

	private bool pauseChat;

	private ChatEvent chatEventCallback;

	private bool OnLine;

	private bool chatClientStarted;

	public string UserName { get; set; }

	public string UserId { get; set; }

	public string TargetUserName { get; set; }

	public bool PausingChat
	{
		get
		{
			return pauseChat;
		}
		set
		{
			pauseChat = value;
		}
	}

	public void Start()
	{
		ChatAppId = ReleaseChatId;
		chatClient = new ChatClient(this);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Application.runInBackground = true;
	}

	public void OnApplicationQuit()
	{
		if (chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseChat && chatClient != null)
		{
			if (pauseStatus || !OnLine)
			{
				SetOnlineStatus(0);
			}
			else
			{
				SetOnlineStatus(2);
			}
		}
	}

	public void Update()
	{
		if (chatClient != null)
		{
			chatClient.Service();
		}
	}

	public void StartChat(string countrycodeIn, string username, string userid, ChatEvent callback)
	{
		string item = countrycodeIn.ToUpper();
		if (OnLine)
		{
			chatEventCallback = callback;
			chatEventCallback(chatEventCode.Connected, null);
			return;
		}
		UserName = username;
		UserId = userid;
		if (Singleton<PlayerInfoScript>.Instance.SaveData.ChatSpecialDomainNumber != 0 || Singleton<AnalyticsManager>.Instance.JailBroken)
		{
			UseSpammerChannel = true;
		}
		if (string.IsNullOrEmpty(UserName))
		{
			UserName = "GuestUser" + Environment.TickCount % 99;
		}
		if (chatClient == null)
		{
			chatClient = new ChatClient(this);
		}
		if (MiscParams.ChatSeparateRegeon)
		{
			if (regionAsia.Contains(item))
			{
				chatClient.ChatRegion = "asia";
			}
			else if (regionEU.Contains(item))
			{
				chatClient.ChatRegion = "eu";
			}
			else if (regionAU.Contains(item))
			{
				chatClient.ChatRegion = "au";
			}
			else
			{
				chatClient.ChatRegion = "us";
			}
		}
		int num = DateTime.UtcNow.Hour * 60 + DateTime.UtcNow.Minute;
		int chatSegmentingMinutes = MiscParams.ChatSegmentingMinutes;
		myChannel = "GL" + (num / chatSegmentingMinutes).ToString("0000");
		ChannelsToJoinOnConnect[0] = AdminChannel;
		ChannelsToJoinOnConnect[1] = myChannel;
		if (UseSpammerChannel)
		{
			ChannelsToJoinOnConnect[2] = SpammerChannel;
		}
		myChatName = CreateChatName(username, userid);
		chatEventCallback = callback;
		chatClient.Connect(ChatAppId, MiscParams.ChatCompatibilityVersion.ToString(), new AuthenticationValues(myChatName));
		chatClient.Service();
		HMACSHA1 hMACSHA = new HMACSHA1();
		hMACSHA.Key = Encoding.UTF8.GetBytes(hval);
		byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(Singleton<PlayerInfoScript>.Instance.GetPlayerCode()));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.AppendFormat("{0:X2}", array[i]);
		}
		myidhash = stringBuilder.ToString();
		hMACSHA.Clear();
		chatClientStarted = true;
	}

	public void ReStartChat()
	{
		if (chatClient != null)
		{
			if (Singleton<PlayerInfoScript>.Instance.SaveData.ChatSpecialDomainNumber != 0 || Singleton<AnalyticsManager>.Instance.JailBroken)
			{
				UseSpammerChannel = true;
			}
			ChannelsToJoinOnConnect[0] = AdminChannel;
			ChannelsToJoinOnConnect[1] = myChannel;
			if (UseSpammerChannel)
			{
				ChannelsToJoinOnConnect[2] = SpammerChannel;
			}
			chatClient.Connect(ChatAppId, MiscParams.ChatCompatibilityVersion.ToString(), new AuthenticationValues(myChatName));
			chatClient.Service();
		}
	}

	public void SwitchToPrivate(string targetUsername)
	{
		TargetUserName = targetUsername;
		SetToPrivate(true);
	}

	public void EndChat()
	{
		if (chatClient != null)
		{
			SetOnlineStatus(0);
			chatClient.Disconnect();
			OnLine = false;
		}
	}

	public void UpdateStatus()
	{
		if (OnLine)
		{
			SetOnlineStatus(2);
		}
	}

	public void SelectChannel()
	{
		doingPrivateChat = false;
		selectedChannelName = myChannel;
	}

	private void SetToPrivate(bool privateornot)
	{
		doingPrivateChat = privateornot;
	}

	public void RegisterMonitorUsers(List<HelperItem> users)
	{
		UserNameIdListOriginal = users;
		List<string> list = new List<string>();
		foreach (HelperItem user in users)
		{
			list.Add(CreateChatName(user.HelperName, user.HelperID));
			foreach (UserNameId mMonitorUserUser in mMonitorUserUserList)
			{
				if (mMonitorUserUser.name == user.HelperName && mMonitorUserUser.id == user.HelperID)
				{
					user.OnlineStatus = mMonitorUserUser.onlineStatus;
					break;
				}
			}
		}
		if (list.Count != 0)
		{
			string[] array = list.ToArray();
			if (!CompareMonitorFriends(array))
			{
				UnregisterMonitorUsers();
				chatClient.Service();
				chatClient.AddFriends(array);
				chatClient.Service();
				SaveMonitorFriends(array, UserNameIdListOriginal);
			}
			mMonitorUser = true;
		}
		else
		{
			mMonitorUser = false;
		}
	}

	public void UnMonitorFriendsList()
	{
		mMonitorUser = false;
	}

	public void UnregisterMonitorUsers()
	{
		if (mMonitorUserarray != null && mMonitorUserarray.Length != 0)
		{
			chatClient.RemoveFriends(mMonitorUserarray);
		}
	}

	public List<HelperItem> CheckOnlineStatus()
	{
		return UserNameIdListOriginal;
	}

	public bool IsUserOnline(HelperItem user)
	{
		if (UserNameIdListOriginal == null)
		{
			return false;
		}
		foreach (HelperItem item in UserNameIdListOriginal)
		{
			if (item.HelperName == user.HelperName && item.HelperID == user.HelperID)
			{
				return item.OnlineStatus;
			}
		}
		return false;
	}

	private void SaveMonitorFriends(string[] newlist, List<HelperItem> users)
	{
		mMonitorUserUserList.Clear();
		foreach (HelperItem user in users)
		{
			mMonitorUserUserList.Add(new UserNameId(user.HelperName, user.HelperID, user.OnlineStatus));
		}
		mMonitorUserarray = new string[newlist.Length];
		Array.Copy(newlist, mMonitorUserarray, newlist.Length);
	}

	private bool CompareMonitorFriends(string[] newlist)
	{
		if (newlist.Length == 0 || mMonitorUserarray == null || newlist.Length != mMonitorUserarray.Length)
		{
			return false;
		}
		foreach (string text in newlist)
		{
			bool flag = false;
			string[] array = mMonitorUserarray;
			foreach (string text2 in array)
			{
				if (text == text2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public bool SendLine(string inputLine)
	{
		string chatSwitch = SessionManager.Instance.theSession.ChatSwitch;
		if (chatSwitch == "0")
		{
			return false;
		}
		if (!MiscParams.ChatEnable || !Singleton<PlayerInfoScript>.Instance.IsPastAgeGate(MiscParams.chatAgeGate))
		{
			return false;
		}
		SendLine(inputLine, null, null);
		return true;
	}

	public bool SendGachaCreatureAnnouncement(string creatureID)
	{
		if (!Singleton<PlayerInfoScript>.Instance.IsPastAgeGate(MiscParams.chatAgeGate))
		{
			return false;
		}
		SendLine(string.Empty, null, null, creatureID);
		return true;
	}

	public bool SendDungeonAnnouncement(string questID)
	{
		if (!Singleton<PlayerInfoScript>.Instance.IsPastAgeGate(MiscParams.chatAgeGate))
		{
			return false;
		}
		SendLine(string.Empty, null, null, null, questID);
		return true;
	}

	public void SendLine(string inputLine, string targetName, string targetID, string gachaCreature = null, string dungeon = null, string inviteData = null)
	{
		if (!string.IsNullOrEmpty(inputLine) || !string.IsNullOrEmpty(gachaCreature) || !string.IsNullOrEmpty(dungeon) || !string.IsNullOrEmpty(inviteData))
		{
			if (targetName != null && targetID != null)
			{
				SwitchToPrivate(CreateChatName(targetName, targetID));
			}
			else
			{
				SetToPrivate(false);
			}
			PhotonMessage photonMessage = new PhotonMessage(inputLine, Singleton<PlayerInfoScript>.Instance, Singleton<CountryFlagManager>.Instance.GetMyCountryCode(), gachaCreature, dungeon, inviteData);
			Dictionary<string, object> message = photonMessage.Serialize();
			if (doingPrivateChat)
			{
				chatClient.SendPrivateMessage(TargetUserName, message);
			}
			else if (UseSpammerChannel)
			{
				chatClient.PublishMessage(SpammerChannel, message);
			}
			else
			{
				chatClient.PublishMessage(selectedChannelName, message);
			}
		}
	}

	private void PostHelpToCurrentChannel()
	{
		ChatChannel chatChannel = selectedChannel;
		if (chatChannel != null)
		{
			chatChannel.Add("info", HelpText);
		}
	}

	public void SubscribeAll()
	{
		chatClient.Subscribe(ChannelsToJoinOnConnect, HistoryLengthToFetch);
	}

	public void UnSubscribeAll()
	{
		chatClient.Unsubscribe(ChannelsToJoinOnConnect);
	}

	public void OnConnected()
	{
		if (ChannelsToJoinOnConnect != null && ChannelsToJoinOnConnect.Length > 0)
		{
			SubscribeAll();
		}
		SetOnlineStatus(2);
		OnLine = true;
		if (chatEventCallback != null)
		{
			chatEventCallback(chatEventCode.Connected, null);
		}
		if (mMonitorUserarray != null && mMonitorUserarray.Length != 0)
		{
			chatClient.AddFriends(mMonitorUserarray);
		}
	}

	public void OnDisconnected()
	{
		if (chatEventCallback != null)
		{
			chatEventCallback(chatEventCode.Disconnected, selectedChannelName);
		}
		SetOnlineStatus(0);
		OnLine = false;
	}

	public void OnChatStateChange(ChatState state)
	{
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
	}

	public void OnUnsubscribed(string[] channels)
	{
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (!channelName.Equals(selectedChannelName) && !channelName.Equals(SpammerChannel) && !channelName.Equals(AdminChannel))
		{
			return;
		}
		for (int i = 0; i < senders.Length; i++)
		{
			IncomingMessageObject incomingMessageObject = new IncomingMessageObject();
			if (channelName.Equals(AdminChannel))
			{
				incomingMessageObject.PlayerName = "DW Administrator";
			}
			else
			{
				incomingMessageObject.PlayerName = GetDisplayNamefromChatName(senders[i]);
			}
			PhotonMessage pm;
			if (PhotonMessage.TryDeserialize(messages[i], out pm))
			{
				incomingMessageObject.Message = pm.Body;
				incomingMessageObject.MetaData = pm.Data;
				if (pm.Data.ZapUserId != null && pm.Data.ZapUserId.Length != 0)
				{
					if (string.Compare(pm.Data.ZapUserId, myidhash, true) == 0)
					{
						Singleton<PlayerInfoScript>.Instance.SaveData.ChatSpecialDomainNumber = 1;
						Singleton<PlayerInfoScript>.Instance.Save();
						ReStartChat();
					}
					break;
				}
			}
			if (chatEventCallback != null)
			{
				chatEventCallback(chatEventCode.IncomingMessages, incomingMessageObject);
			}
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		IncomingMessageObject incomingMessageObject = new IncomingMessageObject();
		incomingMessageObject.PlayerName = GetDisplayNamefromChatName(sender);
		PhotonMessage pm;
		if (PhotonMessage.TryDeserialize(message, out pm))
		{
			incomingMessageObject.Message = pm.Body;
			incomingMessageObject.MetaData = pm.Data;
		}
		else
		{
			incomingMessageObject.Message = (string)message;
		}
		if (chatEventCallback != null)
		{
			chatEventCallback(chatEventCode.IncomingPrivateMessage, incomingMessageObject);
		}
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		ChatChannel chatChannel = selectedChannel;
		if (chatChannel != null)
		{
			chatChannel.Add("info", string.Format("{0} is {1}. Msg:{2}", user, status, message));
		}
		UpdateUserStatus(user, status);
		if (chatEventCallback != null)
		{
			chatEventCallback(chatEventCode.StatusUpdate, string.Format("{0} is {1}. Msg:{2}", user, status, message));
		}
	}

	private string CreateChatName(string name, string id)
	{
		return name + "." + id;
	}

	private string GetDisplayNamefromChatName(string chatname)
	{
		int num = chatname.LastIndexOf(".");
		if (num == -1)
		{
			return chatname;
		}
		return chatname.Substring(0, num);
	}

	private string GetIdfromChatName(string chatname)
	{
		int num = chatname.LastIndexOf(".");
		if (num == -1)
		{
			return string.Empty;
		}
		return chatname.Substring(num + 1);
	}

	private void UpdateUserStatus(string user, int status)
	{
		if (mMonitorUserUserList == null || mMonitorUserUserList.Count == 0)
		{
			return;
		}
		string displayNamefromChatName = GetDisplayNamefromChatName(user);
		string idfromChatName = GetIdfromChatName(user);
		for (int i = 0; i < mMonitorUserUserList.Count; i++)
		{
			if (mMonitorUserUserList[i].name == displayNamefromChatName && mMonitorUserUserList[i].id == idfromChatName)
			{
				mMonitorUserUserList[i].onlineStatus = status == 2;
			}
		}
		if (!mMonitorUser)
		{
			return;
		}
		for (int j = 0; j < UserNameIdListOriginal.Count; j++)
		{
			if (UserNameIdListOriginal[j].HelperName == displayNamefromChatName && UserNameIdListOriginal[j].HelperID == idfromChatName)
			{
				UserNameIdListOriginal[j].OnlineStatus = status == 2;
			}
		}
	}

	public void SetOnlineStatus(int status)
	{
		if (chatClient != null && chatClient.CanChat)
		{
			chatClient.SetOnlineStatus(status);
		}
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		switch (level)
		{
		case DebugLevel.ERROR:
			UnityEngine.Debug.LogError(message);
			break;
		case DebugLevel.WARNING:
			UnityEngine.Debug.LogWarning(message);
			break;
		default:
			UnityEngine.Debug.Log(message);
			break;
		}
	}
}
