using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook.Unity.Editor
{
	internal abstract class EditorFacebookMockDialog : MonoBehaviour
	{
		public delegate void OnComplete(string result);

		private Rect modalRect;

		private GUIStyle modalStyle;

		public OnComplete Callback { protected get; set; }

		public string CallbackID { protected get; set; }

		protected abstract string DialogTitle { get; }

		public EditorFacebookMockDialog()
		{
			modalRect = new Rect(10f, 10f, Screen.width - 20, Screen.height - 20);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1f));
			texture2D.Apply();
			modalStyle = new GUIStyle(GUI.skin.window);
			modalStyle.normal.background = texture2D;
		}

		public void OnGUI()
		{
			GUI.ModalWindow(GetHashCode(), modalRect, OnGUIDialog, DialogTitle, modalStyle);
		}

		protected abstract void DoGui();

		protected abstract void SendSuccessResult();

		protected virtual void SendCancelResult()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["cancelled"] = true;
			if (!string.IsNullOrEmpty(CallbackID))
			{
				dictionary["callback_id"] = CallbackID;
			}
			Callback(Json.Serialize(dictionary));
		}

		protected virtual void SendErrorResult(string errorMessage)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["error"] = errorMessage;
			if (!string.IsNullOrEmpty(CallbackID))
			{
				dictionary["callback_id"] = CallbackID;
			}
			Callback(Json.Serialize(dictionary));
		}

		private void OnGUIDialog(int windowId)
		{
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			GUILayout.Label("Warning! Mock dialog responses will NOT match production dialogs");
			GUILayout.Label("Test your app on one of the supported platforms");
			DoGui();
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUIContent content = new GUIContent("Send Success");
			Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.button);
			if (GUI.Button(rect, content))
			{
				SendSuccessResult();
				Object.Destroy(this);
			}
			GUIContent content2 = new GUIContent("Send Cancel");
			Rect rect2 = GUILayoutUtility.GetRect(content2, GUI.skin.button);
			if (GUI.Button(rect2, content2, GUI.skin.button))
			{
				SendCancelResult();
				Object.Destroy(this);
			}
			GUIContent content3 = new GUIContent("Send Error");
			Rect rect3 = GUILayoutUtility.GetRect(content2, GUI.skin.button);
			if (GUI.Button(rect3, content3, GUI.skin.button))
			{
				SendErrorResult("Error: Error button pressed");
				Object.Destroy(this);
			}
			GUILayout.EndHorizontal();
		}
	}
}
