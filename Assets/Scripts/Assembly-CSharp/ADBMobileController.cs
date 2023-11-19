using System.Collections.Generic;
using com.adobe.mobile;
using UnityEngine;

public class ADBMobileController : MonoBehaviour
{
	private const string ADOBE_SDK_VERSION = "4.5.0";

	public string appName;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		ADBMobile.SetContext();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("appname", appName);
		dictionary.Add("sdkversion", "4.5.0:" + Application.version);
		ADBMobile.CollectLifecycleData(dictionary);
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (isPaused)
		{
			ADBMobile.PauseCollectingLifecycleData();
		}
		else
		{
			ADBMobile.CollectLifecycleData();
		}
	}
}
