namespace Facebook.Unity.Mobile.IOS
{
	internal class IOSFacebookLoader : FB.CompiledFacebookLoader
	{
		protected override FacebookGameObject FBGameObject
		{
			get
			{
				IOSFacebookGameObject component = ComponentFactory.GetComponent<IOSFacebookGameObject>();
				if (component.Facebook == null)
				{
					component.Facebook = new IOSFacebook();
				}
				return component;
			}
		}
	}
}
