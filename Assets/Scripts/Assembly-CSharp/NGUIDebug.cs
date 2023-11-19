using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	private static bool mRayDebug = false;

	private static List<string> mLines = new List<string>();

	private static NGUIDebug mInstance = null;

	public static bool debugRaycast
	{
		get
		{
			return mRayDebug;
		}
		set
		{
			if (Application.isPlaying)
			{
				mRayDebug = value;
				if (value)
				{
					CreateInstance();
				}
			}
		}
	}

	public static void CreateInstance()
	{
		if (mInstance == null)
		{
			GameObject gameObject = new GameObject("_NGUI Debug");
			mInstance = gameObject.AddComponent<NGUIDebug>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	private static void LogString(string text)
	{
		if (Application.isPlaying)
		{
			if (mLines.Count > 20)
			{
				mLines.RemoveAt(0);
			}
			mLines.Add(text);
			CreateInstance();
		}
	}

	public static void Log(params object[] objs)
	{
		string text = string.Empty;
		for (int i = 0; i < objs.Length; i++)
		{
			text = ((i != 0) ? (text + ", " + objs[i].ToString()) : (text + objs[i].ToString()));
		}
		LogString(text);
	}

	public static void DrawBounds(Bounds b)
	{
		Vector3 center = b.center;
		Vector3 vector = b.center - b.extents;
		Vector3 vector2 = b.center + b.extents;
	}

	private void OnGUI()
	{
		if (mLines.Count == 0)
		{
			if (mRayDebug && UICamera.hoveredObject != null && Application.isPlaying)
			{
				GUILayout.Label("Last Hit: " + NGUITools.GetHierarchy(UICamera.hoveredObject).Replace("\"", string.Empty));
			}
			return;
		}
		int i = 0;
		for (int count = mLines.Count; i < count; i++)
		{
			GUILayout.Label(mLines[i]);
		}
	}
}
