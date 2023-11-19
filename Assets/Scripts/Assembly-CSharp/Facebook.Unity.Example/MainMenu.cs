using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal sealed class MainMenu : MenuBase
	{
		protected override bool ShowBackButton()
		{
			return false;
		}

		protected override void GetGui()
		{
			bool flag = GUI.enabled;
			if (Button("FB.Init"))
			{
				FB.Init(OnInitComplete, OnHideUnity);
				base.Status = "FB.Init() called with " + FB.AppId;
			}
			GUILayout.BeginHorizontal();
			GUI.enabled = flag && FB.IsInitialized;
			if (Button("Login"))
			{
				CallFBLogin();
				base.Status = "Login called";
			}
			GUI.enabled = FB.IsLoggedIn;
			if (Button("Get publish_actions"))
			{
				CallFBLoginForPublish();
				base.Status = "Login (for publish_actions) called";
			}
			if (Button("Logout"))
			{
				CallFBLogout();
				base.Status = "Logout called";
			}
			GUILayout.Label(GUIContent.none, GUILayout.MinWidth(ConsoleBase.MarginFix));
			GUILayout.EndHorizontal();
			GUI.enabled = flag && FB.IsInitialized;
			if (Button("Share Dialog"))
			{
				SwitchMenu(typeof(DialogShare));
			}
			bool flag2 = GUI.enabled;
			GUI.enabled = flag && AccessToken.CurrentAccessToken != null && AccessToken.CurrentAccessToken.Permissions.Contains("publish_actions");
			if (Button("Game Groups"))
			{
				SwitchMenu(typeof(GameGroups));
			}
			GUI.enabled = flag2;
			if (Button("App Requests"))
			{
				SwitchMenu(typeof(AppRequests));
			}
			if (Button("Graph Request"))
			{
				SwitchMenu(typeof(GraphRequest));
			}
			if (Constants.IsWeb && Button("Pay"))
			{
				SwitchMenu(typeof(Pay));
			}
			if (Button("App Events"))
			{
				SwitchMenu(typeof(AppEvents));
			}
			if (Button("App Links"))
			{
				SwitchMenu(typeof(AppLinks));
			}
			if (Constants.IsMobile && Button("App Invites"))
			{
				SwitchMenu(typeof(AppInvites));
			}
			if (Constants.IsMobile && Button("Access Token"))
			{
				SwitchMenu(typeof(AccessTokenMenu));
			}
			GUI.enabled = flag;
		}

		private void CallFBLogin()
		{
			List<string> list = new List<string>();
			list.Add("public_profile");
			list.Add("email");
			list.Add("user_friends");
			FB.LogInWithReadPermissions(list, base.HandleResult);
		}

		private void CallFBLoginForPublish()
		{
			List<string> list = new List<string>();
			list.Add("publish_actions");
			FB.LogInWithPublishPermissions(list, base.HandleResult);
		}

		private void CallFBLogout()
		{
			FB.LogOut();
		}

		private void OnInitComplete()
		{
			base.Status = "Success - Check log for details";
			base.LastResponse = "Success Response: OnInitComplete Called\n";
			string log = string.Format("OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'", FB.IsLoggedIn, FB.IsInitialized);
			LogView.AddLog(log);
		}

		private void OnHideUnity(bool isGameShown)
		{
			base.Status = "Success - Check log for details";
			base.LastResponse = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
			LogView.AddLog("Is game shown: " + isGameShown);
		}
	}
}
