using UnityEngine;

public class Kochava_Demo_C : MonoBehaviour
{
	private Rect WindowControllerRect = new Rect(0f, 0f, 200f, 0f);

	private Vector2 scrollPosition;

	private void Start()
	{
		Kochava.AttributionCallback attributionCallback = AttributionCallback;
		Kochava.SetAttributionCallback(attributionCallback);
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		WindowControllerRect = new Rect(10f, 20f, 250f, Screen.height - 30);
		WindowControllerRect = GUI.Window(0, WindowControllerRect, WindowController, "Kochava SDK Demo (C#)");
	}

	private void WindowController(int windowID)
	{
		GUILayout.FlexibleSpace();
		GUILayout.Label("Event Queue Length: " + Kochava.eventQueueLength);
		GUILayout.Label("Last Event Post Time: " + Kochava.eventPostingTime);
		GUILayout.FlexibleSpace();
		GUILayout.Space(10f);
		if (GUILayout.Button("Fire C# Event"))
		{
			Kochava.FireEvent("Test C# Event", "Test C# Event Data - " + Random.Range(1, 15));
		}
		GUILayout.FlexibleSpace();
		if (Kochava.EventLog.Count > 0)
		{
			GUILayout.Space(10f);
			GUILayout.Label("Event Log:");
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			foreach (Kochava.LogEvent item in Kochava.EventLog)
			{
				GUILayout.Space(10f);
				if (item.level == Kochava.KochLogLevel.error)
				{
					GUILayout.Label("*** " + item.text + " ***");
				}
				else
				{
					GUILayout.Label(item.text);
				}
			}
			GUILayout.EndScrollView();
		}
		GUILayout.FlexibleSpace();
	}

	private static void AttributionCallback(string data)
	{
	}

	private static void iBeaconCallback(string data)
	{
	}
}
