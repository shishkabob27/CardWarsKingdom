namespace ExitGames.Client.Photon
{
	internal class CmdLogReceivedAck : CmdLogItem
	{
		public int ReceivedSentTime;

		public CmdLogReceivedAck(NCommand command, int timeInt, int rtt, int variance)
		{
			TimeInt = timeInt;
			Channel = command.commandChannelID;
			SequenceNumber = command.ackReceivedReliableSequenceNumber;
			Rtt = rtt;
			Variance = variance;
			ReceivedSentTime = command.ackReceivedSentTime;
		}

		public override string ToString()
		{
			return string.Format("ACK  NOW: {0,5}  CH: {1,3} SQ: {2,4} RTT: {3,4} VAR: {4,3}  Sent: {5,5} Diff: {6,4}", TimeInt, Channel, SequenceNumber, Rtt, Variance, ReceivedSentTime, TimeInt - ReceivedSentTime);
		}
	}
}
