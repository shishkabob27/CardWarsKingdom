namespace ExitGames.Client.Photon
{
	public class TrafficStats
	{
		public int PackageHeaderSize { get; internal set; }

		public int ReliableCommandCount { get; internal set; }

		public int UnreliableCommandCount { get; internal set; }

		public int FragmentCommandCount { get; internal set; }

		public int ControlCommandCount { get; internal set; }

		public int TotalPacketCount { get; internal set; }

		public int TotalCommandsInPackets { get; internal set; }

		public int ReliableCommandBytes { get; internal set; }

		public int UnreliableCommandBytes { get; internal set; }

		public int FragmentCommandBytes { get; internal set; }

		public int ControlCommandBytes { get; internal set; }

		public int TotalCommandCount => ReliableCommandCount + UnreliableCommandCount + FragmentCommandCount + ControlCommandCount;

		public int TotalCommandBytes => ReliableCommandBytes + UnreliableCommandBytes + FragmentCommandBytes + ControlCommandBytes;

		public int TotalPacketBytes => TotalCommandBytes + TotalPacketCount * PackageHeaderSize;

		public int TimestampOfLastAck { get; set; }

		public int TimestampOfLastReliableCommand { get; set; }

		internal TrafficStats(int packageHeaderSize)
		{
			PackageHeaderSize = packageHeaderSize;
		}

		internal void CountControlCommand(int size)
		{
			ControlCommandBytes += size;
			ControlCommandCount++;
		}

		internal void CountReliableOpCommand(int size)
		{
			ReliableCommandBytes += size;
			ReliableCommandCount++;
		}

		internal void CountUnreliableOpCommand(int size)
		{
			UnreliableCommandBytes += size;
			UnreliableCommandCount++;
		}

		internal void CountFragmentOpCommand(int size)
		{
			FragmentCommandBytes += size;
			FragmentCommandCount++;
		}

		public override string ToString()
		{
			return $"TotalPacketBytes: {TotalPacketBytes} TotalCommandBytes: {TotalCommandBytes} TotalPacketCount: {TotalPacketCount} TotalCommandsInPackets: {TotalCommandsInPackets}";
		}
	}
}
