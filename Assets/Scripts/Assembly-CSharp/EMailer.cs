using System;
using System.Collections.Generic;
using UnityEngine;

public class EMailer
{
	public static void PrefillAndShow(string subject, string body)
	{
		PrefillAndShow(null, subject, body);
	}

	public static void PrefillAndShow(List<string> recipients, string subject, string body)
	{
		string text = CleanEmails(recipients);
		string text2 = Uri.EscapeDataString(subject);
		string text3 = Uri.EscapeDataString(body);
		string url = "mailto:" + text + "?subject=" + text2 + "&body=" + text3;
		Application.OpenURL(url);
	}

	public static string CleanEmails(List<string> addresses)
	{
		string text = string.Empty;
		if (addresses == null)
		{
			return text;
		}
		for (int i = 0; i < addresses.Count; i++)
		{
			text += Uri.EscapeDataString(addresses[i]);
			if (i != addresses.Count - 1)
			{
				text += ",";
			}
		}
		return text;
	}
}
