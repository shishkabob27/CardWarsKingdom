using System;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class ParameterCode
	{
		public const byte SuppressRoomEvents = 237;

		public const byte EmptyRoomTTL = 236;

		public const byte PlayerTTL = 235;

		public const byte EventForward = 234;

		[Obsolete("Use: IsInactive")]
		public const byte IsComingBack = 233;

		public const byte IsInactive = 233;

		public const byte CheckUserOnJoin = 232;

		public const byte ExpectedValues = 231;

		public const byte Address = 230;

		public const byte PeerCount = 229;

		public const byte GameCount = 228;

		public const byte MasterPeerCount = 227;

		public const byte UserId = 225;

		public const byte ApplicationId = 224;

		public const byte Position = 223;

		public const byte MatchMakingType = 223;

		public const byte GameList = 222;

		public const byte Secret = 221;

		public const byte AppVersion = 220;

		[Obsolete("TCP routing was removed after becoming obsolete.")]
		public const byte AzureNodeInfo = 210;

		[Obsolete("TCP routing was removed after becoming obsolete.")]
		public const byte AzureLocalNodeId = 209;

		[Obsolete("TCP routing was removed after becoming obsolete.")]
		public const byte AzureMasterNodeId = 208;

		public const byte RoomName = byte.MaxValue;

		public const byte Broadcast = 250;

		public const byte ActorList = 252;

		public const byte ActorNr = 254;

		public const byte PlayerProperties = 249;

		public const byte CustomEventContent = 245;

		public const byte Data = 245;

		public const byte Code = 244;

		public const byte GameProperties = 248;

		public const byte Properties = 251;

		public const byte TargetActorNr = 253;

		public const byte ReceiverGroup = 246;

		public const byte Cache = 247;

		public const byte CleanupCacheOnLeave = 241;

		public const byte Group = 240;

		public const byte Remove = 239;

		public const byte PublishUserId = 239;

		public const byte Add = 238;

		public const byte Info = 218;

		public const byte ClientAuthenticationType = 217;

		public const byte ClientAuthenticationParams = 216;

		public const byte JoinMode = 215;

		public const byte ClientAuthenticationData = 214;

		public const byte MasterClientId = 203;

		public const byte FindFriendsRequestList = 1;

		public const byte FindFriendsResponseOnlineList = 1;

		public const byte FindFriendsResponseRoomIdList = 2;

		public const byte LobbyName = 213;

		public const byte LobbyType = 212;

		public const byte LobbyStats = 211;

		public const byte Region = 210;

		public const byte UriPath = 209;

		public const byte WebRpcParameters = 208;

		public const byte WebRpcReturnCode = 207;

		public const byte WebRpcReturnMessage = 206;

		public const byte CacheSliceIndex = 205;

		public const byte Plugins = 204;

		public const byte NickName = 202;

		public const byte PluginName = 201;

		public const byte PluginVersion = 200;
	}
}
