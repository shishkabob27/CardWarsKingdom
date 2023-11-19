using System;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public enum EventCaching : byte
	{
		DoNotCache = 0,
		[Obsolete]
		MergeCache = 1,
		[Obsolete]
		ReplaceCache = 2,
		[Obsolete]
		RemoveCache = 3,
		AddToRoomCache = 4,
		AddToRoomCacheGlobal = 5,
		RemoveFromRoomCache = 6,
		RemoveFromRoomCacheForActorsLeft = 7,
		SliceIncreaseIndex = 10,
		SliceSetIndex = 11,
		SlicePurgeIndex = 12,
		SlicePurgeUpToIndex = 13
	}
}
