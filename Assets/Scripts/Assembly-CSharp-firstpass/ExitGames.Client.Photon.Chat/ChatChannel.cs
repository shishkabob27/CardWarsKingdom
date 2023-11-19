using System.Collections.Generic;
using System.Text;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatChannel
	{
		public readonly string Name;

		public readonly List<string> Senders = new List<string>();

		public readonly List<object> Messages = new List<object>();

		public int MessageLimit;

		public bool IsPrivate { get; protected internal set; }

		public int MessageCount
		{
			get
			{
				return Messages.Count;
			}
		}

		public ChatChannel(string name)
		{
			Name = name;
		}

		public void Add(string sender, object message)
		{
			Senders.Add(sender);
			Messages.Add(message);
			TruncateMessages();
		}

		public void Add(string[] senders, object[] messages)
		{
			Senders.AddRange(senders);
			Messages.AddRange(messages);
			TruncateMessages();
		}

		public void TruncateMessages()
		{
			if (MessageLimit > 0 && Messages.Count > MessageLimit)
			{
				int count = Messages.Count - MessageLimit;
				Senders.RemoveRange(0, count);
				Messages.RemoveRange(0, count);
			}
		}

		public void ClearMessages()
		{
			Senders.Clear();
			Messages.Clear();
		}

		public string ToStringMessages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Messages.Count; i++)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", Senders[i], Messages[i]));
			}
			return stringBuilder.ToString();
		}
	}
}
