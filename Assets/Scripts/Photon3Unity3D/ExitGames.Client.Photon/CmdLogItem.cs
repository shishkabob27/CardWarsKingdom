namespace ExitGames.Client.Photon
{
	internal class CmdLogItem
	{
		public int TimeInt;

		public int Channel;

		public int SequenceNumber;

		public int Rtt;

		public int Variance;

		public CmdLogItem()
		{
		}

		public CmdLogItem(NCommand command, int timeInt, int rtt, int variance)
		{
			Channel = command.commandChannelID;
			SequenceNumber = command.reliableSequenceNumber;
			TimeInt = timeInt;
			Rtt = rtt;
			Variance = variance;
		}

		public override string ToString()
		{
			return string.Format("NOW: {0,5}  CH: {1,3} SQ: {2,4} RTT: {3,4} VAR: {4,3}", TimeInt, Channel, SequenceNumber, Rtt, Variance);
		}
	}
}
