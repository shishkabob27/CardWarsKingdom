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
