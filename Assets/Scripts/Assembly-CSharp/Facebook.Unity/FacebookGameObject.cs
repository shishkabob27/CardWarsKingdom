using UnityEngine;

namespace Facebook.Unity
{
	internal abstract class FacebookGameObject : MonoBehaviour, IFacebookCallbackHandler
	{
		public IFacebookImplementation Facebook { get; set; }

		public void Awake()
		{
			Object.DontDestroyOnLoad(this);
			AccessToken.CurrentAccessToken = null;
			OnAwake();
		}

		public void OnInitComplete(string message)
		{
			Facebook.OnInitComplete(message);
		}

		public void OnLoginComplete(string message)
		{
			Facebook.OnLoginComplete(message);
		}

		public void OnLogoutComplete(string message)
		{
			Facebook.OnLogoutComplete(message);
		}

		public void OnGetAppLinkComplete(string message)
		{
			Facebook.OnGetAppLinkComplete(message);
		}

		public void OnGroupCreateComplete(string message)
		{
			Facebook.OnGroupCreateComplete(message);
		}

		public void OnGroupJoinComplete(string message)
		{
			Facebook.OnGroupJoinComplete(message);
		}

		public void OnAppRequestsComplete(string message)
		{
			Facebook.OnAppRequestsComplete(message);
		}

		public void OnShareLinkComplete(string message)
		{
			Facebook.OnShareLinkComplete(message);
		}

		protected virtual void OnAwake()
		{
		}
	}
}
