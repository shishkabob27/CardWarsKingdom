namespace ExitGames.Client.Photon.Chat
{
	public class ChatOperationCode
	{
		public const byte Authenticate = 230;

		public const byte Subscribe = 0;

		public const byte Unsubscribe = 1;

		public const byte Publish = 2;

		public const byte SendPrivate = 3;

		public const byte ChannelHistory = 4;

		public const byte UpdateStatus = 5;

		public const byte AddFriends = 6;

		public const byte RemoveFriends = 7;
	}
}
