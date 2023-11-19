namespace Facebook.Unity
{
	internal interface IFacebookCallbackHandler
	{
		void OnInitComplete(string message);

		void OnLoginComplete(string message);

		void OnLogoutComplete(string message);

		void OnGetAppLinkComplete(string message);

		void OnGroupCreateComplete(string message);

		void OnGroupJoinComplete(string message);

		void OnAppRequestsComplete(string message);

		void OnShareLinkComplete(string message);
	}
}
