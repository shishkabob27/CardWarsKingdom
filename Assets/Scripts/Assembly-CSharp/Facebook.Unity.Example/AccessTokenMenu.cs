namespace Facebook.Unity.Example
{
	internal class AccessTokenMenu : MenuBase
	{
		protected override void GetGui()
		{
			if (Button("Refresh Access Token"))
			{
				FB.Mobile.RefreshCurrentAccessToken(base.HandleResult);
			}
		}
	}
}
