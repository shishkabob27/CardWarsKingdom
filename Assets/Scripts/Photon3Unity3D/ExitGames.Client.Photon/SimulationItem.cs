using System.Diagnostics;

namespace ExitGames.Client.Photon
{
	internal class SimulationItem
	{
		internal readonly Stopwatch stopw;

		public int TimeToExecute;

		public PeerBase.MyAction ActionToExecute;

		public int Delay { get; internal set; }

		public SimulationItem()
		{
			stopw = new Stopwatch();
			stopw.Start();
		}
	}
}
