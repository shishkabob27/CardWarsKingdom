using System.IO;
using UnityEngine;

public class KFFLODManager
{
	public const string LOW_PREFIX = "low_";

	private static bool isFirstCall = true;

	private static bool isLowEndDevice;

	public static string hiResFolderName = "_hirez";

	public static string lowResFolderName = "_lowrez";

	public static bool IsLowEndDevice()
	{
		if (!isFirstCall)
		{
			return isLowEndDevice;
		}
		isFirstCall = false;
		if (SystemInfo.deviceModel == "samsung SCH-I535" || SystemInfo.deviceModel == "samsung Nexus 10" || SystemInfo.deviceModel == "asus Nexus 7" || SystemInfo.deviceModel == "Nexus 7" || SystemInfo.deviceModel == "Nexus 6P" || SystemInfo.deviceModel == "Lenovo A806" || SystemInfo.deviceModel == "Sony Xperia L" || SystemInfo.deviceModel == "Sony Xperia Z" || SystemInfo.deviceModel == "HTC HTC Desire 626s" || SystemInfo.deviceModel == "HTC Desire 626s" || SystemInfo.deviceModel == "samsung SM-T230NU" || SystemInfo.deviceModel == "samsung SAMSUNG-SGH-I337" || SystemInfo.deviceModel == "samsung SM-J500F" || SystemInfo.deviceModel == "samsung GT-I9505" || SystemInfo.deviceModel == "samsung GT-I9505G" || SystemInfo.deviceModel == "samsung GT-I9506" || SystemInfo.deviceModel == "samsung SM-T230" || SystemInfo.deviceModel == "samsung SM-T230NU" || SystemInfo.deviceModel == "Samsung SM-T230NU" || SystemInfo.deviceModel == "samsung SM-T535" || SystemInfo.deviceModel == "Samsung SM-T535" || SystemInfo.deviceModel == "samsung SM-T535NU" || SystemInfo.deviceModel == "samsung SM-T530" || SystemInfo.deviceModel == "Samsung SM-T530" || SystemInfo.deviceModel == "samsung SM-G7105" || SystemInfo.deviceModel == "samsung SM-T530NU" || SystemInfo.deviceModel == "Samsung SM-T530NU" || SystemInfo.deviceModel == "samsung SGH-I747M" || SystemInfo.deviceModel == "samsung SAMSUNG-SGH-I747" || SystemInfo.deviceModel == "Samsung SAMSUNG-SGH-I747" || SystemInfo.deviceModel == "Samsung SGH-I747M" || SystemInfo.deviceModel == "samsung SGH-I337M" || SystemInfo.deviceModel == "samsung GT-I9500" || SystemInfo.deviceModel == "samsung SGH-M919V" || SystemInfo.deviceModel == "samsung SPH-L720" || SystemInfo.deviceModel == "samsung SCH-I545" || SystemInfo.deviceModel == "samsung SCH-R530" || SystemInfo.deviceModel == "samsung SCH-S960L" || SystemInfo.deviceModel == "samsung SCH-S968C" || SystemInfo.deviceModel == "samsung SM-G900F" || SystemInfo.deviceModel == "samsung SM-G900FD" || SystemInfo.deviceModel == "samsung SM-G900H" || SystemInfo.deviceModel == "samsung SM-G901F" || SystemInfo.deviceModel == "samsung SM-G903F" || SystemInfo.deviceModel == "samsung SM-G900P" || SystemInfo.deviceModel == "samsung SM-G900V" || SystemInfo.deviceModel == "samsung SM-G900A" || SystemInfo.deviceModel == "samsung SM-G870A")
		{
			isLowEndDevice = true;
			return true;
		}
		int systemMemorySize = SystemInfo.systemMemorySize;
		int graphicsMemorySize = SystemInfo.graphicsMemorySize;
		int width = Screen.width;
		int height = Screen.height;
		if (systemMemorySize <= 1280 || graphicsMemorySize <= 512 || width <= 480 || height <= 480)
		{
			isLowEndDevice = true;
			return isLowEndDevice;
		}
		bool flag = false;
		string text = SystemInfo.deviceModel.ToLower();
		int num = text.IndexOf("amazon");
		if (num >= 0)
		{
			flag = true;
		}
		if (flag)
		{
			return true;
		}
		return false;
	}

	public static string GetHiLowResFolderName()
	{
		if (IsLowEndDevice())
		{
			return lowResFolderName;
		}
		return hiResFolderName;
	}

	public static string FixPathForLowRes(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		string fileName = Path.GetFileName(path);
		return directoryName + "/" + GetPrefabName(fileName);
	}

	public static string GetPrefabName(string name)
	{
		if (IsLowEndDevice() && !name.StartsWith("low_"))
		{
			name = "low_" + name;
		}
		return name;
	}
}
