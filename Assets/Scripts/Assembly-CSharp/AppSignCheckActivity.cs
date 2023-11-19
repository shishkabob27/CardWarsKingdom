using UnityEngine;

public class AppSignCheckActivity
{
	public static AndroidJavaClass activityClass = new AndroidJavaClass("com.kungfufactory.androidplugin.AppSignCheck");

	public static AndroidJavaClass unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

	public static AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
}
