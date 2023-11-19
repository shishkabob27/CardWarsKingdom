using System;

namespace ExitGames.Client.Photon.LoadBalancing
{
	[Flags]
	public enum PropertyTypeFlag : byte
	{
		None = 0,
		Game = 1,
		Actor = 2,
		GameAndActor = 3
	}
}
