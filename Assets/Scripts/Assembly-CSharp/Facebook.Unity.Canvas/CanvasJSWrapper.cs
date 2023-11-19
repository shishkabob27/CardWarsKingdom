using UnityEngine;

namespace Facebook.Unity.Canvas
{
	internal class CanvasJSWrapper : ICanvasJSWrapper
	{
		private const string JSSDKBindingFileName = "JSSDKBindings";

		public string IntegrationMethodJs
		{
			get
			{
				TextAsset textAsset = Resources.Load("JSSDKBindings") as TextAsset;
				if ((bool)textAsset)
				{
					return textAsset.text;
				}
				return null;
			}
		}

		public string GetSDKVersion()
		{
			return "v2.5";
		}

		public void ExternalCall(string functionName, params object[] args)
		{
			Application.ExternalCall(functionName, args);
		}

		public void ExternalEval(string script)
		{
			Application.ExternalEval(script);
		}

		public void DisableFullScreen()
		{
			if (Screen.fullScreen)
			{
				Screen.fullScreen = false;
			}
		}
	}
}
