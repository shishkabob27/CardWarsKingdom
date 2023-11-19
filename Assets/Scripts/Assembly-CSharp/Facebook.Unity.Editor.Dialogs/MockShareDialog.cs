using System.Collections.Generic;
using System.Text;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook.Unity.Editor.Dialogs
{
	internal class MockShareDialog : EditorFacebookMockDialog
	{
		public string SubTitle { private get; set; }

		protected override string DialogTitle
		{
			get
			{
				return "Mock " + SubTitle + " Dialog";
			}
		}

		protected override void DoGui()
		{
		}

		protected override void SendSuccessResult()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (FB.IsLoggedIn)
			{
				dictionary["postId"] = GenerateFakePostID();
			}
			else
			{
				dictionary["did_complete"] = true;
			}
			if (!string.IsNullOrEmpty(base.CallbackID))
			{
				dictionary["callback_id"] = base.CallbackID;
			}
			if (base.Callback != null)
			{
				base.Callback(Json.Serialize(dictionary));
			}
		}

		protected override void SendCancelResult()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["cancelled"] = "true";
			if (!string.IsNullOrEmpty(base.CallbackID))
			{
				dictionary["callback_id"] = base.CallbackID;
			}
			base.Callback(Json.Serialize(dictionary));
		}

		private string GenerateFakePostID()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(AccessToken.CurrentAccessToken.UserId);
			stringBuilder.Append('_');
			for (int i = 0; i < 17; i++)
			{
				stringBuilder.Append(Random.Range(0, 10));
			}
			return stringBuilder.ToString();
		}
	}
}
