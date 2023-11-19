namespace ExitGames.Client.Photon.LoadBalancing
{
	public enum JoinMode : byte
	{
		Default,
		CreateIfNotExists,
		JoinOrRejoin,
		RejoinOnly
	}
}
