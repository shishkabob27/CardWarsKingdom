using System;
using UnityEngine;

public class FacebookPoster
{
	private const string WALL_POST_URL = "http://www.facebook.com/dialog/feed";

	private const string APP_ID = "517544165097077";

	private const string PAGE_LINK = "http://www.facebook.com";

	public static void PrefillAndShow(string title, string caption, string body)
	{
		string url = "http://www.facebook.com/dialog/feed?app_id=517544165097077&link=http://www.facebook.com&name=" + Uri.EscapeDataString(title) + "&caption=" + Uri.EscapeDataString(caption) + "&description=" + Uri.EscapeDataString(body) + "&redirect_uri=http://www.facebook.com";
		Application.OpenURL(url);
	}
}
