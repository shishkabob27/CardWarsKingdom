namespace ExitGames.Client.Photon
{
	internal class CmdLogReceivedReliable : CmdLogItem
	{
		public int TimeSinceLastSend;

		public int TimeSinceLastSendAck;

		public CmdLogReceivedReliable(NCommand command, int timeInt, int rtt, int variance, int timeSinceLastSend, int timeSinceLastSendAck)
			: base(command, timeInt, rtt, variance)
		{
			TimeSinceLastSend = timeSinceLastSend;
			TimeSinceLastSendAck = timeSinceLastSendAck;
		}

		public override string ToString()
		{
			return $"Read reliable. {base.ToString()}  TimeSinceLastSend: {TimeSinceLastSend} TimeSinceLastSendAcks: {TimeSinceLastSendAck}";
		}
	}
}
