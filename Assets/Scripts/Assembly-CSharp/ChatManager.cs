public class ChatManager : Singleton<ChatManager>
{
	public string DebugChatId;
	public string ReleaseChatId;
	public string[] ChannelsToJoinOnConnect;
	public int HistoryLengthToFetch;
}
