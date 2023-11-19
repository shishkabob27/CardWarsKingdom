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
