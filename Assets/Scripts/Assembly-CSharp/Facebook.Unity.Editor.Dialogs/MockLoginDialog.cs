using System;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook.Unity.Editor.Dialogs
{
	internal class MockLoginDialog : EditorFacebookMockDialog
	{
		private string accessToken = string.Empty;

		protected override string DialogTitle
		{
			get
			{
				return "Mock Login Dialog";
			}
		}

		protected override void DoGui()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("User Access Token:");
			accessToken = GUILayout.TextField(accessToken, GUI.skin.textArea, GUILayout.MinWidth(400f));
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (GUILayout.Button("Find Access Token"))
			{
				Application.OpenURL(string.Format("https://developers.facebook.com/tools/accesstoken/?app_id={0}", FB.AppId));
			}
			GUILayout.Space(20f);
		}

		protected override void SendSuccessResult()
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				SendErrorResult("Empty Access token string");
				return;
			}
			FB.API("/me?fields=id&access_token=" + accessToken, HttpMethod.GET, delegate(IGraphResult graphResult)
			{
				if (!string.IsNullOrEmpty(graphResult.Error))
				{
					SendErrorResult("Graph API error: " + graphResult.Error);
				}
				else
				{
					string facebookID = graphResult.ResultDictionary["id"] as string;
					FB.API("/me/permissions?access_token=" + accessToken, HttpMethod.GET, delegate(IGraphResult permResult)
					{
						if (!string.IsNullOrEmpty(permResult.Error))
						{
							SendErrorResult("Graph API error: " + permResult.Error);
						}
						else
						{
							List<string> list = new List<string>();
							List<string> list2 = new List<string>();
							List<object> list3 = permResult.ResultDictionary["data"] as List<object>;
							foreach (Dictionary<string, object> item in list3)
							{
								if (item["status"] as string== "granted")
								{
									list.Add(item["permission"] as string);
								}
								else
								{
									list2.Add(item["permission"] as string);
								}
							}
							AccessToken accessToken = new AccessToken(this.accessToken, facebookID, DateTime.Now.AddDays(60.0), list, DateTime.Now);
							IDictionary<string, object> dictionary2 = (IDictionary<string, object>)Json.Deserialize(accessToken.ToJson());
							dictionary2.Add("granted_permissions", list);
							dictionary2.Add("declined_permissions", list2);
							if (!string.IsNullOrEmpty(base.CallbackID))
							{
								dictionary2["callback_id"] = base.CallbackID;
							}
							if (base.Callback != null)
							{
								base.Callback(Json.Serialize(dictionary2));
							}
						}
					});
				}
			});
		}
	}
}
