using System;
using UnityEngine;

public class Twitterer
{
	private const int MAX_CHARACTERS = 140;

	private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";

	public static void Tweet(string message)
	{
		string defaultLangCode = Language.settings.defaultLangCode;
		if (message.Length > 140)
		{
			message = message.Substring(0, 140);
		}
		Application.OpenURL("http://twitter.com/intent/tweet?text=" + Uri.EscapeDataString(message) + "&amp;lang=" + defaultLangCode);
	}
}
