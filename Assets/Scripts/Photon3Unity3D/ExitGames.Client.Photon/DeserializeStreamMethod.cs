using System.IO;

namespace ExitGames.Client.Photon
{
	public delegate object DeserializeStreamMethod(MemoryStream inStream, short length);
}
