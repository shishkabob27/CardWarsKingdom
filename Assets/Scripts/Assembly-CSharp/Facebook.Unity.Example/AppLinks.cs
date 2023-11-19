namespace Facebook.Unity.Example
{
	internal class AppLinks : MenuBase
	{
		protected override void GetGui()
		{
			if (Button("Get App Link"))
			{
				FB.GetAppLink(base.HandleResult);
			}
			if (Constants.IsMobile && Button("Fetch Deferred App Link"))
			{
				FB.Mobile.FetchDeferredAppLinkData(base.HandleResult);
			}
		}
	}
}
