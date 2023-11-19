using System.Collections;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class GraphRequest : MenuBase
	{
		private string apiQuery = string.Empty;

		private Texture2D profilePic;

		protected override void GetGui()
		{
			bool flag = GUI.enabled;
			GUI.enabled = flag && FB.IsLoggedIn;
			if (Button("Basic Request - Me"))
			{
				FB.API("/me", HttpMethod.GET, base.HandleResult);
			}
			if (Button("Retrieve Profile Photo"))
			{
				FB.API("/me/picture", HttpMethod.GET, ProfilePhotoCallback);
			}
			if (Button("Take and Upload screenshot"))
			{
				StartCoroutine(TakeScreenshot());
			}
			LabelAndTextField("Request", ref apiQuery);
			if (Button("Custom Request"))
			{
				FB.API(apiQuery, HttpMethod.GET, base.HandleResult);
			}
			if (profilePic != null)
			{
				GUILayout.Box(profilePic);
			}
			GUI.enabled = flag;
		}

		private void ProfilePhotoCallback(IGraphResult result)
		{
			if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
			{
				profilePic = result.Texture;
			}
			HandleResult(result);
		}

		private IEnumerator TakeScreenshot()
		{
			yield return new WaitForEndOfFrame();
			int width = Screen.width;
			int height = Screen.height;
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
			tex.Apply();
			byte[] screenshot = tex.EncodeToPNG();
			WWWForm wwwForm = new WWWForm();
			wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
			wwwForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
			FB.API("me/photos", HttpMethod.POST, base.HandleResult, wwwForm);
		}
	}
}
