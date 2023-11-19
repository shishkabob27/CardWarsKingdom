using UnityEngine;

public class AppSignCheck : MonoBehaviour
{
	private static AndroidJavaObject appSignCheck = AppSignCheckActivity.activityObj.Get<AndroidJavaObject>("appSignCheck");

	public static string AndroidStr()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return appSignCheck.Call<string>("GetAndroidString", new object[0]);
		}
		return "Err:Cannot get android string";
	}

	public static int GetResult()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return appSignCheck.Call<int>("GetAppCheckResult", new object[0]);
		}
		return 0;
	}
}
