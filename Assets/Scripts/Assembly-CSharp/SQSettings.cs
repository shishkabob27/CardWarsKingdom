using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MiniJSON;
using UnityEngine;

public class SQSettings
{
	private static string cdnUrl;

	private static string manifestUrl;

	private static string serverUrl;

	private static string photonChatAppID;

	private static string photonPUNAppID;

	private static int saveInterval;

	private static int patchingFileLimit;

	private static bool debugDisplayControllers;

	private static string bundleIdentifier;

	private static string bundleVersion;

	private static string bundleShortVersion;

	public static string CDN_URL
	{
		get
		{
			return serverUrl + "persist/static/";
		}
	}

	public static string MANIFEST_URL
	{
		get
		{
			return serverUrl + "persist/static/manifest.json";
		}
	}

	public static string SERVER_URL
	{
		get
		{
			return serverUrl;
		}
	}

	public static string PHOTON_CHAT_APP_ID
	{
		get
		{
			return photonChatAppID;
		}
	}

	public static string PHOTON_PUN_APP_ID
	{
		get
		{ 
			return photonPUNAppID;
		}
	}

	public static string SERVER_PREFIX
	{
		get
		{
			string[] array = serverUrl.Trim().Split(new string[2] { "://", "." }, StringSplitOptions.None);
			if (array.Length > 1)
			{
				return array[1];
			}
			return string.Empty;
		}
	}

	public static int SAVE_INTERVAL
	{
		get
		{
			return saveInterval;
		}
	}

	public static int PATCHING_FILE_LIMIT
	{
		get
		{
			return patchingFileLimit;
		}
	}

	public static string BundleIdentifier
	{
		get
		{
			return bundleIdentifier;
		}
	}

	public static string BundleVersion
	{
		get
		{
			return bundleVersion;
		}
	}

	public static string BundleShortVersion
	{
		get
		{
			return bundleShortVersion;
		}
	}

	public static bool DebugDisplayControllers
	{
		get
		{
			return debugDisplayControllers;
		}
	}

	private SQSettings()
	{
	}

	[DllImport("__Internal")]
	public static extern string getBundleIdentifier();

	[DllImport("__Internal")]
	public static extern string getCFBundleVersion();

	[DllImport("__Internal")]
	public static extern string getCFBundleShortVersion();

	public static string getJsonPath(string filePath)
	{
		WWW wWW = null;
		wWW = new WWW(filePath);
		while (!wWW.isDone)
		{
		}
		return wWW.text;
	}

	public static void Init()
	{
		TFUtils.DebugLog("Entering SQSettings Init()");
		bundleIdentifier = Application.identifier;
		string empty = string.Empty;
		string streamingAssetsFile = TFUtils.GetStreamingAssetsFile("server_settings.json");
		empty = ((!streamingAssetsFile.Contains("://")) ? File.ReadAllText(streamingAssetsFile) : getJsonPath(streamingAssetsFile));
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(empty);
		serverUrl = (string)dictionary["server_url"];
		photonChatAppID = (string)dictionary["photon_chat_app_id"];
		photonPUNAppID = (string)dictionary["photon_pun_app_id"];
		streamingAssetsFile = TFUtils.GetStreamingAssetsFile("global_settings.json");
		empty = ((!streamingAssetsFile.Contains("://")) ? File.ReadAllText(streamingAssetsFile) : getJsonPath(streamingAssetsFile));
		dictionary = (Dictionary<string, object>)Json.Deserialize(empty);
		saveInterval = TFUtils.LoadInt(dictionary, "save_interval");
		object value;
		if (dictionary.TryGetValue("debugDisplayControllers", out value))
		{
			debugDisplayControllers = (bool)value;
		}
		int? num = TFUtils.TryLoadInt(dictionary, "patching_file_limit");
		if (num.HasValue)
		{
			patchingFileLimit = num.Value;
		}
		else
		{
			patchingFileLimit = 10;
		}
	}
}
