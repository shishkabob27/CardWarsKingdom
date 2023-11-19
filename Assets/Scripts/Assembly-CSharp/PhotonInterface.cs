using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon;

public class PhotonInterface : LoadBalancingClient
{
	public PhotonInterface() : base(default(ConnectionProtocol))
	{
	}

	public TBPvPManager photonManager;
	public string LobbyName;
	public string RoomName;
	public int PlayerNr;
	public string myFilterName;
	public int TurnNumber;
	public int PlayerIdToMakeThisTurn;
	public byte MyPoints;
	public byte OthersPoints;
}
