namespace Facebook.Unity.Mobile
{
	internal abstract class MobileFacebookGameObject : FacebookGameObject, IFacebookCallbackHandler, IMobileFacebookCallbackHandler
	{
		private IMobileFacebookImplementation MobileFacebook
		{
			get
			{
				return (IMobileFacebookImplementation)base.Facebook;
			}
		}

		public void OnAppInviteComplete(string message)
		{
			MobileFacebook.OnAppInviteComplete(message);
		}

		public void OnFetchDeferredAppLinkComplete(string message)
		{
			MobileFacebook.OnFetchDeferredAppLinkComplete(message);
		}

		public void OnRefreshCurrentAccessTokenComplete(string message)
		{
			MobileFacebook.OnRefreshCurrentAccessTokenComplete(message);
		}
	}
}
