using System.IO;
using UnityEngine;

public class KFFLocalization
{
	private static bool initialized;

	private static string ANDROID_STRING_KEY_SUFFIX_GOOGLE = "__GOOGLE";

	private static string ANDROID_STRING_KEY_SUFFIX_AMAZON = "__AMAZON";

	public static LanguageCode lang;

	public static string Get(string key)
	{
		if (!initialized)
		{
			Language.Init(Path.Combine(Application.persistentDataPath, "Contents"));
			initialized = true;
		}
		if (key == null)
		{
			return string.Empty;
		}
		return Language.Get(key);
	}

	public static LanguageCode ReturnLang()
	{
		if (!initialized)
		{
			Language.Init(Path.Combine(Application.persistentDataPath, "Contents"));
			initialized = true;
		}
		return Language.CurrentLanguage();
	}
}
