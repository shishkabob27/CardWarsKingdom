using System;
using System.Collections;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class Kochava_Demo_JS : MonoBehaviour
{
	public Rect WindowControllerRect;

	public Vector2 scrollPosition;

	public Kochava_Demo_JS()
	{
		WindowControllerRect = new Rect(0f, 0f, 200f, 0f);
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void OnGUI()
	{
		WindowControllerRect = new Rect(Screen.width - 260, 20f, 250f, Screen.height - 30);
		WindowControllerRect = GUI.Window(10, WindowControllerRect, WindowController, "Kochava SDK Demo (JS)");
	}

	public void WindowController(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		GUILayout.Label("Event Queue Length: " + Kochava.eventQueueLength);
		GUILayout.Label("Last Event Post Time: " + Kochava.eventPostingTime);
		GUILayout.FlexibleSpace();
		Kochava.DebugMode = GUILayout.Toggle(Kochava.DebugMode, "Debug Mode");
		Kochava.RequestAttribution = GUILayout.Toggle(Kochava.RequestAttribution, "Request Attribution");
		GUILayout.FlexibleSpace();
		if (GUILayout.Toggle(Kochava.SessionTracking == Kochava.KochSessionTracking.full, "Full Session Tracking") != (Kochava.SessionTracking == Kochava.KochSessionTracking.full))
		{
			Kochava.SessionTracking = Kochava.KochSessionTracking.full;
		}
		if (GUILayout.Toggle(Kochava.SessionTracking == Kochava.KochSessionTracking.basic, "Basic Session Tracking") != (Kochava.SessionTracking == Kochava.KochSessionTracking.basic))
		{
			Kochava.SessionTracking = Kochava.KochSessionTracking.basic;
		}
		if (GUILayout.Toggle(Kochava.SessionTracking == Kochava.KochSessionTracking.minimal, "Minimal Session Tracking") != (Kochava.SessionTracking == Kochava.KochSessionTracking.minimal))
		{
			Kochava.SessionTracking = Kochava.KochSessionTracking.minimal;
		}
		if (GUILayout.Toggle(Kochava.SessionTracking == Kochava.KochSessionTracking.none, "No Session Tracking") != (Kochava.SessionTracking == Kochava.KochSessionTracking.none))
		{
			Kochava.SessionTracking = Kochava.KochSessionTracking.none;
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Fire JS Event"))
		{
			Kochava.FireEvent("_Test JS Event", "Test JS Event Data");
		}
		if (GUILayout.Button("Fire Numerical Event"))
		{
			Kochava.FireEvent("Numerical Event", "13");
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("JSON String Event"))
		{
			Kochava.FireEvent("Test Json String", "{\"realCost\":1.990000,\"localCurrency\":\"USD\",\"virtualCurrency\":\"Bedrock\",\"virtualCurrencyAmount\":100}");
		}
		if (GUILayout.Button("JSON Object Event"))
		{
			Kochava.FireEvent(new Hashtable(new Hash
			{
				{ "event_name", "Test JSON Object" },
				{ "the_answer", 42 },
				{ "x", 120 },
				{ "y", 310 },
				{ "z", 13 }
			}));
		}
		GUILayout.FlexibleSpace();
		if (Kochava.EventLog.Count > 0 && GUILayout.Button("Clear Event Log"))
		{
			Kochava.ClearLog();
		}
		if (GUILayout.Button("Resend Initial"))
		{
			Kochava.InitInitial();
		}
		if (Kochava.eventQueueLength > 0 && GUILayout.Button("Clear Event Queue"))
		{
			Kochava.ClearQueue();
		}
		GUILayout.FlexibleSpace();
		if (Kochava.RequestAttribution)
		{
			if (Kochava.AttributionDataStr != null)
			{
				GUILayout.Label("Device Attribution Data: " + Kochava.AttributionDataStr);
			}
			else
			{
				GUILayout.Label("Device Attribution Data: unavailable");
			}
		}
		else
		{
			GUILayout.Label("Device Attribution Data: disabled");
		}
		GUILayout.EndScrollView();
	}

	public void Main()
	{
	}
}
