using UnityEngine;

namespace Facebook.Unity.Mobile.Android
{
	internal class AndroidFacebookGameObject : MobileFacebookGameObject
	{
		protected override void OnAwake()
		{
			AndroidJNIHelper.debug = UnityEngine.Debug.isDebugBuild;
		}
	}
}
