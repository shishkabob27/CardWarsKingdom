using System.Collections.Generic;

namespace Facebook.Unity.Example
{
	internal class AppEvents : MenuBase
	{
		protected override void GetGui()
		{
			if (Button("Log FB App Event"))
			{
				base.Status = "Logged FB.AppEvent";
				FB.LogAppEvent("fb_mobile_achievement_unlocked", null, new Dictionary<string, object> { { "fb_description", "Clicked 'Log AppEvent' button" } });
				LogView.AddLog("You may see results showing up at https://www.facebook.com/analytics/" + FB.AppId);
			}
		}
	}
}
