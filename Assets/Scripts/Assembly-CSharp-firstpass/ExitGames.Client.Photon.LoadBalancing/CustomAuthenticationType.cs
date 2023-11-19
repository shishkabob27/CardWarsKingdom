namespace ExitGames.Client.Photon.LoadBalancing
{
	public enum CustomAuthenticationType : byte
	{
		Custom = 0,
		Steam = 1,
		Facebook = 2,
		None = byte.MaxValue
	}
}
