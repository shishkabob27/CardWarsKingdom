using System.Collections.Generic;

namespace Messages
{
	public class Message
	{
		private static MessagesCallback getMessagescallback;

		private static SuccessCallback successCallback;

		public static void GetMessages(Session session, MessagesCallback callback)
		{
			getMessagescallback = callback;
			session.GetMessagesList();
		}

		public static void GotallMessagesCallback(List<string> messages, TFWebFileResponse response)
		{
			if (getMessagescallback != null)
			{
				if (SessionManager.Instance.HasNewMessagesReady)
				{
					getMessagescallback(messages, ResponseFlag.Success);
				}
				else
				{
					getMessagescallback(null, ResponseFlag.None);
				}
				getMessagescallback = null;
			}
		}

		public static void ClearMessage()
		{
			SessionManager.Instance.ClearMessages();
		}
	}
}
