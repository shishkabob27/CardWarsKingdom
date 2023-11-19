using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class AppRequests : MenuBase
	{
		private string requestMessage = string.Empty;

		private string requestTo = string.Empty;

		private string requestFilter = string.Empty;

		private string requestExcludes = string.Empty;

		private string requestMax = string.Empty;

		private string requestData = string.Empty;

		private string requestTitle = string.Empty;

		private string requestObjectID = string.Empty;

		private int selectedAction;

		private string[] actionTypeStrings = new string[4]
		{
			"NONE",
			OGActionType.SEND.ToString(),
			OGActionType.ASKFOR.ToString(),
			OGActionType.TURN.ToString()
		};

		protected override void GetGui()
		{
			if (Button("Select - Filter None"))
			{
				FacebookDelegate<IAppRequestResult> callback = base.HandleResult;
				FB.AppRequest("Test Message", null, null, null, null, string.Empty, string.Empty, callback);
			}
			if (Button("Select - Filter app_users"))
			{
				List<object> list = new List<object>();
				list.Add("app_users");
				List<object> filters = list;
				FB.AppRequest("Test Message", null, filters, null, 0, string.Empty, string.Empty, base.HandleResult);
			}
			if (Button("Select - Filter app_non_users"))
			{
				List<object> list = new List<object>();
				list.Add("app_non_users");
				List<object> filters2 = list;
				FB.AppRequest("Test Message", null, filters2, null, 0, string.Empty, string.Empty, base.HandleResult);
			}
			LabelAndTextField("Message: ", ref requestMessage);
			LabelAndTextField("To (optional): ", ref requestTo);
			LabelAndTextField("Filter (optional): ", ref requestFilter);
			LabelAndTextField("Exclude Ids (optional): ", ref requestExcludes);
			LabelAndTextField("Filters: ", ref requestExcludes);
			LabelAndTextField("Max Recipients (optional): ", ref requestMax);
			LabelAndTextField("Data (optional): ", ref requestData);
			LabelAndTextField("Title (optional): ", ref requestTitle);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Request Action (optional): ", base.LabelStyle, GUILayout.MaxWidth(200f * base.ScaleFactor));
			selectedAction = GUILayout.Toolbar(selectedAction, actionTypeStrings, base.ButtonStyle, GUILayout.MinHeight((float)ConsoleBase.ButtonHeight * base.ScaleFactor), GUILayout.MaxWidth(ConsoleBase.MainWindowWidth - 150));
			GUILayout.EndHorizontal();
			LabelAndTextField("Request Object ID (optional): ", ref requestObjectID);
			if (Button("Custom App Request"))
			{
				OGActionType? selectedOGActionType = GetSelectedOGActionType();
				if (selectedOGActionType.HasValue)
				{
					FB.AppRequest(requestMessage, selectedOGActionType.Value, requestObjectID, (requestTo == null) ? null : requestTo.Split(','), requestData, requestTitle, base.HandleResult);
					return;
				}
				FB.AppRequest(requestMessage, (!string.IsNullOrEmpty(requestTo)) ? requestTo.Split(',') : null, (!string.IsNullOrEmpty(requestFilter)) ? requestFilter.Split(',').OfType<object>().ToList() : null, (!string.IsNullOrEmpty(requestExcludes)) ? requestExcludes.Split(',') : null, (!string.IsNullOrEmpty(requestMax)) ? int.Parse(requestMax) : 0, requestData, requestTitle, base.HandleResult);
			}
		}

		private OGActionType? GetSelectedOGActionType()
		{
			string text = actionTypeStrings[selectedAction];
			if (text == OGActionType.SEND.ToString())
			{
				return OGActionType.SEND;
			}
			if (text == OGActionType.ASKFOR.ToString())
			{
				return OGActionType.ASKFOR;
			}
			if (text == OGActionType.TURN.ToString())
			{
				return OGActionType.TURN;
			}
			return null;
		}
	}
}
