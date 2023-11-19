using System;
using UnityEngine;

public class AndroidPermissionGranter : MonoBehaviour
{
	public enum AndroidPermission
	{
		WRITE_EXTERNAL_STORAGE
	}

	private const string WRITE_EXTERNAL_STORAGE = "WRITE_EXTERNAL_STORAGE";

	private const string PERMISSION_GRANTED = "PERMISSION_GRANTED";

	private const string PERMISSION_DENIED = "PERMISSION_DENIED";

	private const string ANDROID_PERMISSION_GRANTER = "AndroidPermissionGranter";

	public Action<bool> PermissionRequestCallback;

	public Action<bool> PermissionCheckCallback;

	public Action<bool> ShouldShowPermissionRationalCallback;

	public static AndroidPermissionGranter instance;

	private static bool initialized;

	private static AndroidJavaClass androidPermissionGranterClass;

	private static AndroidJavaObject activity;

	public static void Init()
	{
		if (!initialized)
		{
			initialize();
		}
	}

	public static void CheckPermission(AndroidPermission permission)
	{
		if (!initialized)
		{
			initialize();
		}
		androidPermissionGranterClass.CallStatic("checkPermission", activity, (int)permission);
	}

	public static void GrantPermission(AndroidPermission permission)
	{
		if (!initialized)
		{
			initialize();
		}
		androidPermissionGranterClass.CallStatic("grantPermission", activity, (int)permission);
	}

	public static void ShouldShowRequestPermission(AndroidPermission permission, string permissionRational)
	{
		if (!initialized)
		{
			initialize();
		}
		androidPermissionGranterClass.CallStatic("showPermissionRational", activity, permissionRational);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void Awake()
	{
		instance = this;
		if (base.name != "AndroidPermissionGranter")
		{
			base.name = "AndroidPermissionGranter";
		}
	}

	private static void initialize()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject();
			instance = gameObject.AddComponent<AndroidPermissionGranter>();
			gameObject.name = "AndroidPermissionGranter";
		}
		androidPermissionGranterClass = new AndroidJavaClass("com.turner.androidpermissionsgranter.AndroidPermissionGranter");
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		initialized = true;
	}

	private void permissionRequestCallbackInternal(string message)
	{
		bool obj = message == "PERMISSION_GRANTED";
		if (PermissionRequestCallback != null)
		{
			PermissionRequestCallback(obj);
		}
	}

	private void permissionCheckCallbackInternal(string message)
	{
		bool obj = message == "PERMISSION_GRANTED";
		if (PermissionCheckCallback != null)
		{
			PermissionCheckCallback(obj);
		}
	}

	private void shouldShowRequestPermissionCallbackInternal(string message)
	{
		bool obj = message == "true";
		if (ShouldShowPermissionRationalCallback != null)
		{
			ShouldShowPermissionRationalCallback(obj);
		}
	}
}
