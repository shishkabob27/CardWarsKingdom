using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class Language
{
	public static string settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";

	public static LocalizationSettings settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));

	public static string Backup_settingsAssetPath = "Assets/Localization/Resources/Languages/BackupLocalizationSettings.asset";

	public static LocalizationSettings backup_settings = (LocalizationSettings)Resources.Load("Languages/" + Path.GetFileNameWithoutExtension(Backup_settingsAssetPath), typeof(LocalizationSettings));

	private static List<string> availableLanguages;

	private static LanguageCode currentLanguage = LanguageCode.N;

	private static Dictionary<string, Hashtable> currentEntrySheets;

	private static string _persistentDataPath = string.Empty;

	public static void Init(string persistentPath)
	{
		_persistentDataPath = persistentPath;
		LoadAvailableLanguages();
		if (settings == null)
		{
			return;
		}
		bool useSystemLanguagePerDefault = settings.useSystemLanguagePerDefault;
		LanguageCode code = LocalizationSettings.GetLanguageEnum(settings.defaultLangCode);
		if (useSystemLanguagePerDefault)
		{
			LanguageCode languageCode = LanguageNameToCode(Application.systemLanguage);
			if (languageCode == LanguageCode.N)
			{
				string twoLetterISOLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
				if (twoLetterISOLanguageName != "iv")
				{
					languageCode = LocalizationSettings.GetLanguageEnum(twoLetterISOLanguageName);
				}
			}
			if (availableLanguages.Contains(string.Concat(languageCode, string.Empty)))
			{
				code = languageCode;
			}
		}
		string @string = PlayerPrefs.GetString("M2H_lastLanguage", string.Empty);
		if (@string != string.Empty && availableLanguages.Contains(@string))
		{
			SwitchLanguage(@string);
		}
		else
		{
			SwitchLanguage(code);
		}
	}

	private static void LoadAvailableLanguages()
	{
		availableLanguages = new List<string>();
		if (settings == null || settings.sheetTitles == null || settings.sheetTitles.Length <= 0)
		{
			settings = backup_settings;
			if (backup_settings == null || backup_settings.sheetTitles == null || backup_settings.sheetTitles.Length <= 0)
			{
				return;
			}
		}
		foreach (int value in Enum.GetValues(typeof(LanguageCode)))
		{
			if (HasLanguageFile(string.Concat((LanguageCode)value, string.Empty), settings.sheetTitles[0]))
			{
				availableLanguages.Add(string.Concat((LanguageCode)value, string.Empty));
			}
		}
		availableLanguages.Remove("ZH");
		Resources.UnloadUnusedAssets();
	}

	public static string[] GetLanguages()
	{
		return availableLanguages.ToArray();
	}

	public static bool SwitchLanguage(string langCode)
	{
		return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
	}

	public static bool SwitchLanguage(LanguageCode code)
	{
		if (availableLanguages.Contains(string.Concat(code, string.Empty)))
		{
			DoSwitch(code);
			return true;
		}
		if (currentLanguage == LanguageCode.N && availableLanguages.Count > 0)
		{
			DoSwitch(LocalizationSettings.GetLanguageEnum("EN"));
		}
		return false;
	}

	public static void ReloadLanguage()
	{
		DoSwitch(currentLanguage);
	}

	private static void DoSwitch(LanguageCode newLang)
	{
		if (false || settings == null)
		{
			return;
		}
		PlayerPrefs.GetString("M2H_lastLanguage", string.Concat(newLang, string.Empty));
		currentLanguage = newLang;
		currentEntrySheets = new Dictionary<string, Hashtable>();
		XMLParser xMLParser = new XMLParser();
		string[] sheetTitles = settings.sheetTitles;
		foreach (string text in sheetTitles)
		{
			currentEntrySheets[text] = new Hashtable();
			string languageFileContents = GetLanguageFileContents(text);
			if (languageFileContents == null)
			{
				continue;
			}
			Hashtable hashtable = (Hashtable)xMLParser.Parse(languageFileContents);
			ArrayList arrayList = (ArrayList)(((ArrayList)hashtable["entries"])[0] as Hashtable)["entry"];
			foreach (Hashtable item in arrayList)
			{
				string key = (string)item["@name"];
				string s = string.Concat(item["_text"], string.Empty);
				s = s.UnescapeXML();
				currentEntrySheets[text][key] = s;
			}
		}
		LocalizedAsset[] array = (LocalizedAsset[])UnityEngine.Object.FindObjectsOfType(typeof(LocalizedAsset));
		LocalizedAsset[] array2 = array;
		foreach (LocalizedAsset localizedAsset in array2)
		{
			localizedAsset.LocalizeAsset();
		}
		SendMonoMessage("ChangedLanguage", currentLanguage);
	}

	public static UnityEngine.Object GetAsset(string name)
	{
		return Resources.Load(string.Concat("Languages/Assets/", CurrentLanguage(), "/", name));
	}

	private static bool HasLanguageFile(string lang, string sheetTitle)
	{
		return (TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null;
	}

	private static string GetLanguageFileContents(string sheetTitle)
	{
		string text = string.Concat("Languages/", currentLanguage, "_", sheetTitle);
		string path = _persistentDataPath + "/" + text + ".xml";
		if (File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		TextAsset textAsset = (TextAsset)Resources.Load(text, typeof(TextAsset));
		if (textAsset != null)
		{
			return textAsset.text;
		}
		return null;
	}

	public static LanguageCode CurrentLanguage()
	{
		return currentLanguage;
	}

	public static string Get(string key)
	{
		if (key.StartsWith("!!") && !object.ReferenceEquals(settings, null))
		{
			return Get(key, settings.sheetTitles[0]);
		}
		return key;
	}

	public static string Get(string key, string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			return string.Empty;
		}
		if (currentEntrySheets[sheetTitle].ContainsKey(key))
		{
			return (string)currentEntrySheets[sheetTitle][key];
		}
		return "MISSING LANG:" + key;
	}

	public static bool ContainsKey(string key)
	{
		if (key.StartsWith("!!"))
		{
			return ContainsKey(key, settings.sheetTitles[0]);
		}
		return false;
	}

	public static bool ContainsKey(string key, string sheetTitle)
	{
		if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
		{
			return false;
		}
		return currentEntrySheets[sheetTitle].ContainsKey(key);
	}

	private static void SendMonoMessage(string methodString, params object[] parameters)
	{
		if (parameters == null || parameters.Length > 1)
		{
		}
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject && gameObject.transform.parent == null)
			{
				if (parameters != null && parameters.Length == 1)
				{
					gameObject.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					gameObject.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public static LanguageCode LanguageNameToCode(SystemLanguage name)
	{
		switch (name)
		{
		case SystemLanguage.Afrikaans:
			return LanguageCode.AF;
		case SystemLanguage.Arabic:
			return LanguageCode.AR;
		case SystemLanguage.Basque:
			return LanguageCode.BA;
		case SystemLanguage.Belarusian:
			return LanguageCode.BE;
		case SystemLanguage.Bulgarian:
			return LanguageCode.BG;
		case SystemLanguage.Catalan:
			return LanguageCode.CA;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
		case SystemLanguage.ChineseTraditional:
			return LanguageCode.ZH;
		case SystemLanguage.Czech:
			return LanguageCode.CS;
		case SystemLanguage.Danish:
			return LanguageCode.DA;
		case SystemLanguage.Dutch:
			return LanguageCode.NL;
		case SystemLanguage.English:
			return LanguageCode.EN;
		case SystemLanguage.Estonian:
			return LanguageCode.ET;
		case SystemLanguage.Faroese:
			return LanguageCode.FA;
		case SystemLanguage.Finnish:
			return LanguageCode.FI;
		case SystemLanguage.French:
			return LanguageCode.FR;
		case SystemLanguage.German:
			return LanguageCode.DE;
		case SystemLanguage.Greek:
			return LanguageCode.EL;
		case SystemLanguage.Hebrew:
			return LanguageCode.HE;
		case SystemLanguage.Hungarian:
			return LanguageCode.HU;
		case SystemLanguage.Icelandic:
			return LanguageCode.IS;
		case SystemLanguage.Indonesian:
			return LanguageCode.ID;
		case SystemLanguage.Italian:
			return LanguageCode.IT;
		case SystemLanguage.Japanese:
			return LanguageCode.JA;
		case SystemLanguage.Korean:
			return LanguageCode.KO;
		case SystemLanguage.Latvian:
			return LanguageCode.LA;
		case SystemLanguage.Lithuanian:
			return LanguageCode.LT;
		case SystemLanguage.Norwegian:
			return LanguageCode.NO;
		case SystemLanguage.Polish:
			return LanguageCode.PL;
		case SystemLanguage.Portuguese:
			return LanguageCode.PT;
		case SystemLanguage.Romanian:
			return LanguageCode.RO;
		case SystemLanguage.Russian:
			return LanguageCode.RU;
		case SystemLanguage.SerboCroatian:
			return LanguageCode.SH;
		case SystemLanguage.Slovak:
			return LanguageCode.SK;
		case SystemLanguage.Slovenian:
			return LanguageCode.SL;
		case SystemLanguage.Spanish:
			return LanguageCode.ES;
		case SystemLanguage.Swedish:
			return LanguageCode.SW;
		case SystemLanguage.Thai:
			return LanguageCode.TH;
		case SystemLanguage.Turkish:
			return LanguageCode.TR;
		case SystemLanguage.Ukrainian:
			return LanguageCode.UK;
		case SystemLanguage.Vietnamese:
			return LanguageCode.VI;
		default:
			switch (name)
			{
			case SystemLanguage.Hungarian:
				return LanguageCode.HU;
			case SystemLanguage.Unknown:
				return LanguageCode.N;
			default:
				return LanguageCode.N;
			}
		}
	}

	public static SystemLanguage LanguageCodeToName(LanguageCode code)
	{
		switch (code)
		{
		case LanguageCode.AF:
			return SystemLanguage.Afrikaans;
		case LanguageCode.AR:
			return SystemLanguage.Arabic;
		case LanguageCode.BA:
			return SystemLanguage.Basque;
		case LanguageCode.BE:
			return SystemLanguage.Belarusian;
		case LanguageCode.BG:
			return SystemLanguage.Bulgarian;
		case LanguageCode.CA:
			return SystemLanguage.Catalan;
		case LanguageCode.ZH:
			return SystemLanguage.Chinese;
		case LanguageCode.CS:
			return SystemLanguage.Czech;
		case LanguageCode.DA:
			return SystemLanguage.Danish;
		case LanguageCode.NL:
			return SystemLanguage.Dutch;
		case LanguageCode.EN:
			return SystemLanguage.English;
		case LanguageCode.ET:
			return SystemLanguage.Estonian;
		case LanguageCode.FA:
			return SystemLanguage.Faroese;
		case LanguageCode.FI:
			return SystemLanguage.Finnish;
		case LanguageCode.FR:
			return SystemLanguage.French;
		case LanguageCode.DE:
			return SystemLanguage.German;
		case LanguageCode.EL:
			return SystemLanguage.Greek;
		case LanguageCode.HE:
			return SystemLanguage.Hebrew;
		case LanguageCode.HU:
			return SystemLanguage.Hungarian;
		case LanguageCode.IS:
			return SystemLanguage.Icelandic;
		case LanguageCode.ID:
			return SystemLanguage.Indonesian;
		case LanguageCode.IT:
			return SystemLanguage.Italian;
		case LanguageCode.JA:
			return SystemLanguage.Japanese;
		case LanguageCode.KO:
			return SystemLanguage.Korean;
		case LanguageCode.LA:
			return SystemLanguage.Latvian;
		case LanguageCode.LT:
			return SystemLanguage.Lithuanian;
		case LanguageCode.NO:
			return SystemLanguage.Norwegian;
		case LanguageCode.PL:
			return SystemLanguage.Polish;
		case LanguageCode.PT:
			return SystemLanguage.Portuguese;
		case LanguageCode.RO:
			return SystemLanguage.Romanian;
		case LanguageCode.RU:
			return SystemLanguage.Russian;
		case LanguageCode.SH:
			return SystemLanguage.SerboCroatian;
		case LanguageCode.SK:
			return SystemLanguage.Slovak;
		case LanguageCode.SL:
			return SystemLanguage.Slovenian;
		case LanguageCode.ES:
			return SystemLanguage.Spanish;
		case LanguageCode.SW:
			return SystemLanguage.Swedish;
		case LanguageCode.TH:
			return SystemLanguage.Thai;
		case LanguageCode.TR:
			return SystemLanguage.Turkish;
		case LanguageCode.UK:
			return SystemLanguage.Ukrainian;
		case LanguageCode.VI:
			return SystemLanguage.Vietnamese;
		default:
			switch (code)
			{
			case LanguageCode.HU:
				return SystemLanguage.Hungarian;
			case LanguageCode.N:
				return SystemLanguage.Unknown;
			default:
				return SystemLanguage.English;
			}
		}
	}
}
