namespace ExitGames.Client.Photon
{
	internal class CmdLogSentReliable : CmdLogItem
	{
		public int Resend;

		public int RoundtripTimeout;

		public int Timeout;

		public bool TriggeredTimeout;

		public CmdLogSentReliable(NCommand command, int timeInt, int rtt, int variance, bool triggeredTimeout = false)
		{
			TimeInt = timeInt;
			Channel = command.commandChannelID;
			SequenceNumber = command.reliableSequenceNumber;
			Rtt = rtt;
			Variance = variance;
			Resend = command.commandSentCount;
			RoundtripTimeout = command.roundTripTimeout;
			Timeout = command.timeoutTime;
			TriggeredTimeout = triggeredTimeout;
		}

		public override string ToString()
		{
			return string.Format("SND  NOW: {0,5}  CH: {1,3} SQ: {2,4} RTT: {3,4} VAR: {4,3}  Resend#: {5,2} ResendIn: {7} Timeout: {6,5} {8}", TimeInt, Channel, SequenceNumber, Rtt, Variance, Resend, Timeout, RoundtripTimeout, TriggeredTimeout ? "< TIMEOUT" : "");
		}
	}
}
