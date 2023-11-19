using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatClient : IPhotonPeerListener
	{
		private const int FriendRequestListMax = 1024;

		private const string ChatApppName = "chat";

		private string chatRegion = "EU";

		public int MessageLimit;

		public readonly Dictionary<string, ChatChannel> PublicChannels;

		public readonly Dictionary<string, ChatChannel> PrivateChannels;

		private readonly IChatClientListener listener;

		internal ChatPeer chatPeer;

		private bool didAuthenticate;

		private int msDeltaForServiceCalls = 50;

		private int msTimestampOfLastServiceCall;

		public string NameServerAddress { get; private set; }

		public string FrontendAddress { get; private set; }

		public string ChatRegion
		{
			get
			{
				return chatRegion;
			}
			set
			{
				chatRegion = value;
			}
		}

		public ChatState State { get; private set; }

		public ChatDisconnectCause DisconnectedCause { get; private set; }

		public bool CanChat
		{
			get
			{
				return State == ChatState.ConnectedToFrontEnd && HasPeer;
			}
		}

		private bool HasPeer
		{
			get
			{
				return chatPeer != null;
			}
		}

		public string AppVersion { get; private set; }

		public string AppId { get; private set; }

		public AuthenticationValues AuthValues { get; set; }

		public string UserId
		{
			get
			{
				return (AuthValues == null) ? null : AuthValues.UserId;
			}
			private set
			{
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.UserId = value;
			}
		}

		public DebugLevel DebugOut
		{
			get
			{
				return chatPeer.DebugOut;
			}
			set
			{
				chatPeer.DebugOut = value;
			}
		}

		public ChatClient(IChatClientListener listener, ConnectionProtocol protocol = ConnectionProtocol.Udp)
		{
			this.listener = listener;
			State = ChatState.Uninitialized;
			chatPeer = new ChatPeer(this, protocol);
			PublicChannels = new Dictionary<string, ChatChannel>();
			PrivateChannels = new Dictionary<string, ChatChannel>();
		}

		void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
		{
			listener.DebugReturn(level, message);
		}

		void IPhotonPeerListener.OnEvent(EventData eventData)
		{
			switch (eventData.Code)
			{
			case 0:
				HandleChatMessagesEvent(eventData);
				break;
			case 2:
				HandlePrivateMessageEvent(eventData);
				break;
			case 4:
				HandleStatusUpdate(eventData);
				break;
			case 5:
				HandleSubscribeEvent(eventData);
				break;
			case 6:
				HandleUnsubscribeEvent(eventData);
				break;
			case 1:
			case 3:
				break;
			}
		}

		void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
		{
			switch (operationResponse.OperationCode)
			{
			case 230:
				HandleAuthResponse(operationResponse);
				return;
			}
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == -2)
				{
					listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} unknown on server. Check your AppId and make sure it's for a Chat application.", operationResponse.OperationCode));
				}
				else
				{
					listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} failed (Code: {1}). Debug Message: {2}", operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage));
				}
			}
		}

		void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
		{
			switch (statusCode)
			{
			case StatusCode.Connect:
				if (!chatPeer.IsProtocolSecure)
				{
					UnityEngine.Debug.Log("Establishing Encryption");
					chatPeer.EstablishEncryption();
				}
				else
				{
					UnityEngine.Debug.Log("Skipping Encryption");
					if (!didAuthenticate)
					{
						didAuthenticate = chatPeer.AuthenticateOnNameServer(AppId, AppVersion, chatRegion, AuthValues);
						if (!didAuthenticate)
						{
							((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + State);
						}
					}
				}
				if (State == ChatState.ConnectingToNameServer)
				{
					State = ChatState.ConnectedToNameServer;
					listener.OnChatStateChange(State);
				}
				else if (State == ChatState.ConnectingToFrontEnd)
				{
					AuthenticateOnFrontEnd();
				}
				break;
			case StatusCode.EncryptionEstablished:
				if (!didAuthenticate)
				{
					didAuthenticate = chatPeer.AuthenticateOnNameServer(AppId, AppVersion, chatRegion, AuthValues);
					if (!didAuthenticate)
					{
						((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + State);
					}
				}
				break;
			case StatusCode.EncryptionFailedToEstablish:
				State = ChatState.Disconnecting;
				chatPeer.Disconnect();
				break;
			case StatusCode.Disconnect:
				if (State == ChatState.Authenticated)
				{
					ConnectToFrontEnd();
					break;
				}
				State = ChatState.Disconnected;
				listener.OnChatStateChange(ChatState.Disconnected);
				listener.OnDisconnected();
				break;
			}
		}

		public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
		{
			chatPeer.TimePingInterval = 3000;
			DisconnectedCause = ChatDisconnectCause.None;
			if (authValues != null)
			{
				AuthValues = authValues;
				if (AuthValues.UserId == null || AuthValues.UserId == string.Empty)
				{
					listener.DebugReturn(DebugLevel.ERROR, "Connect failed: no UserId specified in authentication values");
					return false;
				}
				AppId = appId;
				AppVersion = appVersion;
				didAuthenticate = false;
				msDeltaForServiceCalls = 100;
				chatPeer.QuickResendAttempts = 2;
				chatPeer.SentCountAllowance = 7;
				PublicChannels.Clear();
				PrivateChannels.Clear();
				NameServerAddress = chatPeer.NameServerAddress;
				bool flag = chatPeer.Connect();
				if (flag)
				{
					State = ChatState.ConnectingToNameServer;
				}
				return flag;
			}
			listener.DebugReturn(DebugLevel.ERROR, "Connect failed: no authentication values specified");
			return false;
		}

		public void Service()
		{
			if (HasPeer && (Environment.TickCount - msTimestampOfLastServiceCall > msDeltaForServiceCalls || msTimestampOfLastServiceCall == 0))
			{
				msTimestampOfLastServiceCall = Environment.TickCount;
				chatPeer.Service();
			}
		}

		public void Disconnect()
		{
			if (HasPeer && chatPeer.PeerState != 0)
			{
				chatPeer.Disconnect();
			}
		}

		public void StopThread()
		{
			if (HasPeer)
			{
				chatPeer.StopThread();
			}
		}

		public bool Subscribe(string[] channels)
		{
			return Subscribe(channels, 0);
		}

		public bool Subscribe(string[] channels, int messagesFromHistory)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "Subscribe called while not connected to front end server.");
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
				return false;
			}
			return SendChannelOperation(channels, 0, messagesFromHistory);
		}

		public bool Unsubscribe(string[] channels)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "Unsubscribe called while not connected to front end server.");
				return false;
			}
			if (channels == null || channels.Length == 0)
			{
				listener.DebugReturn(DebugLevel.WARNING, "Unsubscribe can't be called for empty or null channels-list.");
				return false;
			}
			return SendChannelOperation(channels, 1, 0);
		}

		public bool PublishMessage(string channelName, object message)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "PublishMessage called while not connected to front end server.");
				return false;
			}
			if (string.IsNullOrEmpty(channelName) || message == null)
			{
				listener.DebugReturn(DebugLevel.WARNING, "PublishMessage parameters must be non-null and not empty.");
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(1, channelName);
			dictionary.Add(3, message);
			Dictionary<byte, object> customOpParameters = dictionary;
			return chatPeer.OpCustom(2, customOpParameters, true);
		}

		public bool SendPrivateMessage(string target, object message)
		{
			return SendPrivateMessage(target, message, false);
		}

		public bool SendPrivateMessage(string target, object message, bool encrypt)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "SendPrivateMessage called while not connected to front end server.");
				return false;
			}
			if (string.IsNullOrEmpty(target) || message == null)
			{
				listener.DebugReturn(DebugLevel.WARNING, "SendPrivateMessage parameters must be non-null and not empty.");
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(225, target);
			dictionary.Add(3, message);
			Dictionary<byte, object> customOpParameters = dictionary;
			return chatPeer.OpCustom(3, customOpParameters, true, 0, encrypt);
		}

		private bool SetOnlineStatus(int status, object message, bool skipMessage)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "SetOnlineStatus called while not connected to front end server.");
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(10, status);
			Dictionary<byte, object> dictionary2 = dictionary;
			if (skipMessage)
			{
				dictionary2[12] = true;
			}
			else
			{
				dictionary2[3] = message;
			}
			return chatPeer.OpCustom(5, dictionary2, true);
		}

		public bool SetOnlineStatus(int status)
		{
			return SetOnlineStatus(status, null, true);
		}

		public bool SetOnlineStatus(int status, object message)
		{
			return SetOnlineStatus(status, message, false);
		}

		public bool AddFriends(string[] friends)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "AddFriends called while not connected to front end server.");
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				listener.DebugReturn(DebugLevel.WARNING, "AddFriends can't be called for empty or null list.");
				return false;
			}
			if (friends.Length > 1024)
			{
				listener.DebugReturn(DebugLevel.WARNING, "AddFriends max list size exceeded: " + friends.Length + " > " + 1024);
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(11, friends);
			Dictionary<byte, object> customOpParameters = dictionary;
			return chatPeer.OpCustom(6, customOpParameters, true);
		}

		public bool RemoveFriends(string[] friends)
		{
			if (!CanChat)
			{
				listener.DebugReturn(DebugLevel.ERROR, "RemoveFriends called while not connected to front end server.");
				return false;
			}
			if (friends == null || friends.Length == 0)
			{
				listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends can't be called for empty or null list.");
				return false;
			}
			if (friends.Length > 1024)
			{
				listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends max list size exceeded: " + friends.Length + " > " + 1024);
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(11, friends);
			Dictionary<byte, object> customOpParameters = dictionary;
			return chatPeer.OpCustom(7, customOpParameters, true);
		}

		public string GetPrivateChannelNameByUser(string userName)
		{
			return string.Format("{0}:{1}", UserId, userName);
		}

		public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
		{
			if (!isPrivate)
			{
				return PublicChannels.TryGetValue(channelName, out channel);
			}
			return PrivateChannels.TryGetValue(channelName, out channel);
		}

		public bool TryGetChannel(string channelName, out ChatChannel channel)
		{
			bool flag = false;
			if (PublicChannels.TryGetValue(channelName, out channel))
			{
				return true;
			}
			return PrivateChannels.TryGetValue(channelName, out channel);
		}

		public void SendAcksOnly()
		{
			if (chatPeer != null)
			{
				chatPeer.SendAcksOnly();
			}
		}

		private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(0, channels);
			Dictionary<byte, object> dictionary2 = dictionary;
			if (historyLength != 0)
			{
				dictionary2.Add(14, historyLength);
			}
			return chatPeer.OpCustom(operation, dictionary2, true);
		}

		private void HandlePrivateMessageEvent(EventData eventData)
		{
			object message = eventData.Parameters[3];
			string text = (string)eventData.Parameters[5];
			string privateChannelNameByUser;
			if (UserId != null && UserId.Equals(text))
			{
				string userName = (string)eventData.Parameters[225];
				privateChannelNameByUser = GetPrivateChannelNameByUser(userName);
			}
			else
			{
				privateChannelNameByUser = GetPrivateChannelNameByUser(text);
			}
			ChatChannel value;
			if (!PrivateChannels.TryGetValue(privateChannelNameByUser, out value))
			{
				value = new ChatChannel(privateChannelNameByUser);
				value.IsPrivate = true;
				value.MessageLimit = MessageLimit;
				PrivateChannels.Add(value.Name, value);
			}
			value.Add(text, message);
			listener.OnPrivateMessage(text, message, privateChannelNameByUser);
		}

		private void HandleChatMessagesEvent(EventData eventData)
		{
			object[] messages = (object[])eventData.Parameters[2];
			string[] senders = (string[])eventData.Parameters[4];
			string text = (string)eventData.Parameters[1];
			ChatChannel value;
			if (!PublicChannels.TryGetValue(text, out value))
			{
				listener.DebugReturn(DebugLevel.WARNING, "Channel " + text + " for incoming message event not found.");
				return;
			}
			value.Add(senders, messages);
			listener.OnGetMessages(text, senders, messages);
		}

		private void HandleSubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData.Parameters[0];
			bool[] array2 = (bool[])eventData.Parameters[15];
			for (int i = 0; i < array.Length; i++)
			{
				if (array2[i])
				{
					string text = array[i];
					if (!PublicChannels.ContainsKey(text))
					{
						ChatChannel chatChannel = new ChatChannel(text);
						chatChannel.MessageLimit = MessageLimit;
						PublicChannels.Add(chatChannel.Name, chatChannel);
					}
				}
			}
			listener.OnSubscribed(array, array2);
		}

		private void HandleUnsubscribeEvent(EventData eventData)
		{
			string[] array = (string[])eventData[0];
			foreach (string key in array)
			{
				PublicChannels.Remove(key);
			}
			listener.OnUnsubscribed(array);
		}

		private void HandleAuthResponse(OperationResponse operationResponse)
		{
			listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " on: " + chatPeer.NameServerAddress);
			if (operationResponse.ReturnCode == 0)
			{
				if (State == ChatState.ConnectedToNameServer)
				{
					State = ChatState.Authenticated;
					listener.OnChatStateChange(State);
					if (operationResponse.Parameters.ContainsKey(221))
					{
						if (AuthValues == null)
						{
							AuthValues = new AuthenticationValues();
						}
						AuthValues.Token = operationResponse[221] as string;
						FrontendAddress = (string)operationResponse[230];
						chatPeer.Disconnect();
					}
					else
					{
						listener.DebugReturn(DebugLevel.ERROR, "No secret in authentication response.");
					}
				}
				else if (State == ChatState.ConnectingToFrontEnd)
				{
					msDeltaForServiceCalls *= 4;
					State = ChatState.ConnectedToFrontEnd;
					listener.OnChatStateChange(State);
					listener.OnConnected();
				}
				return;
			}
			switch (operationResponse.ReturnCode)
			{
			case short.MaxValue:
				DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
				break;
			case 32755:
				DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
				break;
			case 32756:
				DisconnectedCause = ChatDisconnectCause.InvalidRegion;
				break;
			case 32757:
				DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
				break;
			case -3:
				DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
				break;
			}
			listener.DebugReturn(DebugLevel.ERROR, "Authentication request error: " + operationResponse.ReturnCode + ". Disconnecting.");
			State = ChatState.Disconnecting;
			chatPeer.Disconnect();
		}

		private void HandleStatusUpdate(EventData eventData)
		{
			string user = (string)eventData.Parameters[5];
			int status = (int)eventData.Parameters[10];
			object message = null;
			bool flag = eventData.Parameters.ContainsKey(3);
			if (flag)
			{
				message = eventData.Parameters[3];
			}
			listener.OnStatusUpdate(user, status, flag, message);
		}

		private void ConnectToFrontEnd()
		{
			State = ChatState.ConnectingToFrontEnd;
			listener.DebugReturn(DebugLevel.INFO, "Connecting to frontend " + FrontendAddress);
			chatPeer.Connect(FrontendAddress, "chat");
		}

		private bool AuthenticateOnFrontEnd()
		{
			if (AuthValues != null)
			{
				if (AuthValues.Token == null || AuthValues.Token == string.Empty)
				{
					listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Secret is not set");
					return false;
				}
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(221, AuthValues.Token);
				Dictionary<byte, object> customOpParameters = dictionary;
				return chatPeer.OpCustom(230, customOpParameters, true);
			}
			listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Authentication Values are not set");
			return false;
		}
	}
}
