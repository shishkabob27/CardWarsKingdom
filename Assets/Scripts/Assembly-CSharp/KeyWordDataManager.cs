using System;
using System.Collections.Generic;
using System.IO;

public class KeyWordDataManager : DataManager<KeyWordData>
{
	private static KeyWordDataManager _instance;

	public static KeyWordDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_KeyWords.json");
				_instance = new KeyWordDataManager(path);
			}
			return _instance;
		}
	}

	public KeyWordDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(GameEventFXDataManager.Instance);
	}

	protected override void PostLoad()
	{
		foreach (KeyWordData item in DatabaseArray)
		{
			foreach (KeyWordData item2 in DatabaseArray)
			{
				if (item != item2 && item.DisplayName.Contains(item2.DisplayName))
				{
					item.RedundantKeywords.Add(item2);
				}
			}
		}
	}

	public static List<CreatureStat> ParseStatsList(string statsString)
	{
		List<CreatureStat> list = new List<CreatureStat>();
		if (statsString != string.Empty)
		{
			char[] separator = new char[1] { ',' };
			string[] array = statsString.Split(separator);
			string[] array2 = array;
			foreach (string text in array2)
			{
				list.Add((CreatureStat)(int)Enum.Parse(typeof(CreatureStat), text.Trim().ToUpper()));
			}
		}
		return list;
	}
}
