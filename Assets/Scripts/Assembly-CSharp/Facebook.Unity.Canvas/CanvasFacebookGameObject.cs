using UnityEngine;

namespace Facebook.Unity.Canvas
{
	internal class CanvasFacebookGameObject : FacebookGameObject, ICanvasFacebookCallbackHandler, IFacebookCallbackHandler
	{
		protected ICanvasFacebookImplementation CanvasFacebookImpl
		{
			get
			{
				return (ICanvasFacebookImplementation)base.Facebook;
			}
		}

		public void OnPayComplete(string result)
		{
			CanvasFacebookImpl.OnPayComplete(result);
		}

		public void OnFacebookAuthResponseChange(string message)
		{
			CanvasFacebookImpl.OnFacebookAuthResponseChange(message);
		}

		public void OnUrlResponse(string message)
		{
			CanvasFacebookImpl.OnUrlResponse(message);
		}

		public void OnHideUnity(bool hide)
		{
			CanvasFacebookImpl.OnHideUnity(hide);
		}

		protected override void OnAwake()
		{
			GameObject gameObject = new GameObject("FacebookJsBridge");
			gameObject.AddComponent<JsBridge>();
			gameObject.transform.parent = base.gameObject.transform;
		}
	}
}
