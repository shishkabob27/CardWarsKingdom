using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

public class FBUtil : ScriptableObject
{
	public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
	{
		string text = string.Format("/{0}/picture", facebookID);
		string text2 = ((!width.HasValue) ? string.Empty : ("&width=" + width));
		text2 += ((!height.HasValue) ? string.Empty : ("&height=" + height));
		text2 += ((type == null) ? string.Empty : ("&type=" + type));
		text2 += "&redirect=false";
		if (text2 != string.Empty)
		{
			text = text + "?g" + text2;
		}
		return text;
	}

	public static Dictionary<string, string> RandomFriend(List<object> friends)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)friends[Random.Range(0, friends.Count)];
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2["id"] = (string)dictionary["id"];
		dictionary2["first_name"] = (string)dictionary["first_name"];
		Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary["picture"];
		Dictionary<string, object> dictionary4 = (Dictionary<string, object>)dictionary3["data"];
		dictionary2["image_url"] = (string)dictionary4["url"];
		return dictionary2;
	}

	public static Dictionary<string, string> DeserializeJSONProfile(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		object value;
		if (dictionary.TryGetValue("first_name", out value))
		{
			dictionary2["first_name"] = (string)value;
		}
		object value2;
		if (dictionary.TryGetValue("id", out value2))
		{
			dictionary2["id"] = (string)value2;
		}
		return dictionary2;
	}

	public static List<object> DeserializeScores(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		List<object> result = new List<object>();
		object value;
		if (dictionary.TryGetValue("data", out value))
		{
			result = (List<object>)value;
		}
		return result;
	}

	public static List<object> DeserializeJSONInvitableFriends(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		List<object> result = new List<object>();
		object value;
		if (dictionary.TryGetValue("invitable_friends", out value))
		{
			result = (List<object>)((Dictionary<string, object>)value)["data"];
		}
		return result;
	}

	public static List<object> DeserializeJSONInvitedFriends(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		List<object> result = new List<object>();
		object value;
		if (dictionary.TryGetValue("friends", out value))
		{
			result = (List<object>)((Dictionary<string, object>)value)["data"];
		}
		return result;
	}

	public static List<object> DeserializeJSONFriends(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		List<object> list = new List<object>();
		object value;
		if (dictionary.TryGetValue("invitable_friends", out value))
		{
			list.AddRange((List<object>)((Dictionary<string, object>)value)["data"]);
		}
		if (dictionary.TryGetValue("friends", out value))
		{
			list.AddRange((List<object>)((Dictionary<string, object>)value)["data"]);
		}
		return list;
	}

	public static string DeserializePictureURLString(string response)
	{
		return DeserializePictureURLObject(Json.Deserialize(response));
	}

	public static string DeserializePictureURLObject(object pictureObj)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)((Dictionary<string, object>)pictureObj)["data"];
		object value = null;
		if (dictionary.TryGetValue("url", out value))
		{
			return (string)value;
		}
		return null;
	}

	public static void DrawActualSizeTexture(Vector2 pos, Texture texture, float scale = 1f)
	{
		Rect position = new Rect(pos.x, pos.y, (float)texture.width * scale, (float)texture.height * scale);
		GUI.DrawTexture(position, texture);
	}

	public static void DrawSimpleText(Vector2 pos, GUIStyle style, string text)
	{
		Rect position = new Rect(pos.x, pos.y, Screen.width, Screen.height);
		GUI.Label(position, text, style);
	}

	private static void JavascriptLog(string msg)
	{
		Application.ExternalCall("console.log", msg);
	}

	public static void Log(string message)
	{

	}

	public static void LogError(string message)
	{
		
	}
}
