namespace Facebook.Unity.Mobile.Android
{
	internal class AndroidFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override FacebookGameObject FBGameObject
		{
			get
			{
				AndroidFacebookGameObject component = ComponentFactory.GetComponent<AndroidFacebookGameObject>();
				if (component.Facebook == null)
				{
					component.Facebook = new AndroidFacebook();
				}
				return component;
			}
		}
	}
}
