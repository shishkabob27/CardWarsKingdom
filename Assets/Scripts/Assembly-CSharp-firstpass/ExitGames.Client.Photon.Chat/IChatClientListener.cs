namespace ExitGames.Client.Photon.Chat
{
	public interface IChatClientListener
	{
		void DebugReturn(DebugLevel level, string message);

		void OnDisconnected();

		void OnConnected();

		void OnChatStateChange(ChatState state);

		void OnGetMessages(string channelName, string[] senders, object[] messages);

		void OnPrivateMessage(string sender, object message, string channelName);

		void OnSubscribed(string[] channels, bool[] results);

		void OnUnsubscribed(string[] channels);

		void OnStatusUpdate(string user, int status, bool gotMessage, object message);
	}
}
