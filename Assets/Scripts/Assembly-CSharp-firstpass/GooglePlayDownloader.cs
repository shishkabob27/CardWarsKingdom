using System;
using System.IO;
using UnityEngine;

public class GooglePlayDownloader
{
	private const string Environment_MEDIA_MOUNTED = "mounted";

	private static AndroidJavaClass detectAndroidJNI;

	private static AndroidJavaClass Environment;

	private static string obb_package;

	private static int obb_version;

	static GooglePlayDownloader()
	{
		if (!RunningOnAndroid())
		{
			return;
		}
		Environment = new AndroidJavaClass("android.os.Environment");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderService"))
		{
			androidJavaClass.SetStatic("BASE64_PUBLIC_KEY", "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwTF5/nPBvCC65k8fPQxkw+WDfhy/f07CEi2bZIsW33LmZzqXEse/BCkgEU0g8jkV5dMaG4N2a6Q2XnodgdYmPqJyhAXDRmE1ChbuW4bTGbHCQ2RxAsyB77YQb98gzsLe3EN8mATrE2IBARAPEc/89iTviO/lFzrpL/8znGnEIpf1Rk9g8JxwqMFosQTESF/MRoPWEpR9tg+IEtx/1VCUSbMRLVe5x35qA63DOv47l3P4cYChoquJlWomMc3LKrBOOm14PuGoj2Ky8y4P2hc0zOkJkL1WQ8Vmc+g/D4UPt5mQ46CFtVdCi0dW/OpKbEYIcF68w46aCvM4qr3TdMgh3QIDAQAB");
			androidJavaClass.SetStatic("SALT", new byte[20]
			{
				1, 43, 244, 255, 54, 98, 156, 244, 43, 2,
				248, 252, 9, 5, 150, 148, 223, 45, 255, 84
			});
		}
	}

	public static bool RunningOnAndroid()
	{
		if (detectAndroidJNI == null)
		{
			detectAndroidJNI = new AndroidJavaClass("android.os.Build");
		}
		return detectAndroidJNI.GetRawClass() != IntPtr.Zero;
	}

	public static string GetExpansionFilePath()
	{
		//Discarded unreachable code: IL_006e
		populateOBBData();
		if (Environment.CallStatic<string>("getExternalStorageState", new object[0]) != "mounted")
		{
			return null;
		}
		using (AndroidJavaObject androidJavaObject = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]))
		{
			string arg = androidJavaObject.Call<string>("getPath", new object[0]);
			return string.Format("{0}/{1}/{2}", arg, "Android/obb", obb_package);
		}
	}

	public static string GetMainOBBPath(string expansionFilePath)
	{
		populateOBBData();
		if (expansionFilePath == null)
		{
			return null;
		}
		string text = string.Format("{0}/main.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static string GetPatchOBBPath(string expansionFilePath)
	{
		populateOBBData();
		if (expansionFilePath == null)
		{
			return null;
		}
		string text = string.Format("{0}/patch.{1}.{2}.obb", expansionFilePath, obb_version, obb_package);
		if (!File.Exists(text))
		{
			return null;
		}
		return text;
	}

	public static void FetchOBB()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent", @static, new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity"));
			int num = 65536;
			androidJavaObject.Call<AndroidJavaObject>("addFlags", new object[1] { num });
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				"unityplayer.Activity",
				@static.Call<AndroidJavaObject>("getClass", new object[0]).Call<string>("getName", new object[0])
			});
			@static.Call("startActivity", androidJavaObject);
			if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
			{
				AndroidJNI.ExceptionDescribe();
				AndroidJNI.ExceptionClear();
			}
		}
	}

	private static void populateOBBData()
	{
		if (obb_version != 0)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			obb_package = @static.Call<string>("getPackageName", new object[0]);
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("getPackageInfo", new object[2] { obb_package, 0 });
			obb_version = androidJavaObject.Get<int>("versionCode");
		}
	}
}
