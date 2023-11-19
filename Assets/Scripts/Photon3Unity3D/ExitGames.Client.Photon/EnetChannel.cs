using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	internal class EnetChannel
	{
		internal byte ChannelNumber;

		internal Dictionary<int, NCommand> incomingReliableCommandsList;

		internal Dictionary<int, NCommand> incomingUnreliableCommandsList;

		internal Queue<NCommand> outgoingReliableCommandsList;

		internal Queue<NCommand> outgoingUnreliableCommandsList;

		internal int incomingReliableSequenceNumber;

		internal int incomingUnreliableSequenceNumber;

		internal int outgoingReliableSequenceNumber;

		internal int outgoingUnreliableSequenceNumber;

		public EnetChannel(byte channelNumber, int commandBufferSize)
		{
			ChannelNumber = channelNumber;
			incomingReliableCommandsList = new Dictionary<int, NCommand>(commandBufferSize);
			incomingUnreliableCommandsList = new Dictionary<int, NCommand>(commandBufferSize);
			outgoingReliableCommandsList = new Queue<NCommand>(commandBufferSize);
			outgoingUnreliableCommandsList = new Queue<NCommand>(commandBufferSize);
		}

		public bool ContainsUnreliableSequenceNumber(int unreliableSequenceNumber)
		{
			return incomingUnreliableCommandsList.ContainsKey(unreliableSequenceNumber);
		}

		public NCommand FetchUnreliableSequenceNumber(int unreliableSequenceNumber)
		{
			return incomingUnreliableCommandsList[unreliableSequenceNumber];
		}

		public bool ContainsReliableSequenceNumber(int reliableSequenceNumber)
		{
			return incomingReliableCommandsList.ContainsKey(reliableSequenceNumber);
		}

		public NCommand FetchReliableSequenceNumber(int reliableSequenceNumber)
		{
			return incomingReliableCommandsList[reliableSequenceNumber];
		}

		public void clearAll()
		{
			lock (this)
			{
				incomingReliableCommandsList.Clear();
				incomingUnreliableCommandsList.Clear();
				outgoingReliableCommandsList.Clear();
				outgoingUnreliableCommandsList.Clear();
			}
		}
	}
}
