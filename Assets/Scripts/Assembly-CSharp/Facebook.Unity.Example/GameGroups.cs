using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class GameGroups : MenuBase
	{
		private string gamerGroupName = "Test group";

		private string gamerGroupDesc = "Test group for testing.";

		private string gamerGroupPrivacy = "closed";

		private string gamerGroupCurrentGroup = string.Empty;

		protected override void GetGui()
		{
			if (Button("Game Group Create - Closed"))
			{
				FB.GameGroupCreate("Test game group", "Test description", "CLOSED", base.HandleResult);
			}
			if (Button("Game Group Create - Open"))
			{
				FB.GameGroupCreate("Test game group", "Test description", "OPEN", base.HandleResult);
			}
			LabelAndTextField("Group Name", ref gamerGroupName);
			LabelAndTextField("Group Description", ref gamerGroupDesc);
			LabelAndTextField("Group Privacy", ref gamerGroupPrivacy);
			if (Button("Call Create Group Dialog"))
			{
				CallCreateGroupDialog();
			}
			LabelAndTextField("Group To Join", ref gamerGroupCurrentGroup);
			bool flag = GUI.enabled;
			GUI.enabled = flag && !string.IsNullOrEmpty(gamerGroupCurrentGroup);
			if (Button("Call Join Group Dialog"))
			{
				CallJoinGroupDialog();
			}
			GUI.enabled = flag && FB.IsLoggedIn;
			if (Button("Get All App Managed Groups"))
			{
				CallFbGetAllOwnedGroups();
			}
			if (Button("Get Gamer Groups Logged in User Belongs to"))
			{
				CallFbGetUserGroups();
			}
			if (Button("Make Group Post As User"))
			{
				CallFbPostToGamerGroup();
			}
			GUI.enabled = flag;
		}

		private void GroupCreateCB(IGroupCreateResult result)
		{
			HandleResult(result);
			if (result.GroupId != null)
			{
				gamerGroupCurrentGroup = result.GroupId;
			}
		}

		private void GetAllGroupsCB(IGraphResult result)
		{
			if (!string.IsNullOrEmpty(result.RawResult))
			{
				base.LastResponse = result.RawResult;
				IDictionary<string, object> resultDictionary = result.ResultDictionary;
				if (resultDictionary.ContainsKey("data"))
				{
					List<object> list = (List<object>)resultDictionary["data"];
					if (list.Count > 0)
					{
						Dictionary<string, object> dictionary = (Dictionary<string, object>)list[0];
						gamerGroupCurrentGroup = (string)dictionary["id"];
					}
				}
			}
			if (!string.IsNullOrEmpty(result.Error))
			{
				base.LastResponse = result.Error;
			}
		}

		private void CallFbGetAllOwnedGroups()
		{
			FB.API(FB.AppId + "/groups", HttpMethod.GET, GetAllGroupsCB);
		}

		private void CallFbGetUserGroups()
		{
			FB.API("/me/groups?parent=" + FB.AppId, HttpMethod.GET, base.HandleResult);
		}

		private void CallCreateGroupDialog()
		{
			FB.GameGroupCreate(gamerGroupName, gamerGroupDesc, gamerGroupPrivacy, GroupCreateCB);
		}

		private void CallJoinGroupDialog()
		{
			FB.GameGroupJoin(gamerGroupCurrentGroup, base.HandleResult);
		}

		private void CallFbPostToGamerGroup()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["message"] = "herp derp a post";
			FB.API(gamerGroupCurrentGroup + "/feed", HttpMethod.POST, base.HandleResult, dictionary);
		}
	}
}
