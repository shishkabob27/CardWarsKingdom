using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HockeyAppIOS : MonoBehaviour
{
	public enum AuthenticatorType
	{
		Anonymous,
		Device,
		HockeyAppUser,
		HockeyAppEmail,
		WebAuth
	}

	protected const string HOCKEYAPP_BASEURL = "https://rink.hockeyapp.net/";

	protected const string HOCKEYAPP_CRASHESPATH = "api/2/apps/[APPID]/crashes/upload";

	protected const string LOG_FILE_DIR = "/logs/";

	protected const int MAX_CHARS = 199800;

	public string appID = "your-hockey-app-id";

	public AuthenticatorType authenticatorType;

	public string secret = "your-hockey-app-secret";

	public string serverURL = "your-custom-server-url";

	public bool autoUpload;

	public bool exceptionLogging = true;

	public bool updateManager;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void GameViewLoaded(string message)
	{
	}

	protected virtual List<string> GetLogHeaders()
	{
		return new List<string>();
	}

	protected virtual WWWForm CreateForm(string log)
	{
		WWWForm result = new WWWForm();
		byte[] array = null;
		return result;
	}

	protected virtual List<string> GetLogFiles()
	{
		return new List<string>();
	}

	protected virtual IEnumerator SendLogs(List<string> logs)
	{
		string crashPath = "api/2/apps/[APPID]/crashes/upload";
		string url = GetBaseURL() + crashPath.Replace("[APPID]", appID);
		foreach (string log in logs)
		{
			WWWForm postForm = CreateForm(log);
			string lContent2 = postForm.headers["Content-Type"].ToString();
			lContent2 = lContent2.Replace("\"", string.Empty);
			WWW www = new WWW(headers: new Dictionary<string, string> { { "Content-Type", lContent2 } }, url: url, postData: postForm.data);
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				continue;
			}
			try
			{
				File.Delete(log);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				if (!Debug.isDebugBuild)
				{
				}
			}
		}
	}

	protected virtual void WriteLogToDisk(string logString, string stackTrace)
	{
	}

	protected virtual string GetBaseURL()
	{
		return string.Empty;
	}

	protected virtual string GetAuthenticatorTypeString()
	{
		return string.Empty;
	}

	protected virtual bool IsConnected()
	{
		return false;
	}

	protected virtual void HandleException(string logString, string stackTrace)
	{
	}

	public void OnHandleLogCallback(string logString, string stackTrace, LogType type)
	{
	}

	public void OnHandleUnresolvedException(object sender, UnhandledExceptionEventArgs args)
	{
	}
}
