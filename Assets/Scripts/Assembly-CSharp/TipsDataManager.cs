using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TipsDataManager : DataManager<TipEntry>
{
	private static TipsDataManager _instance;

	public static TipsDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Tips.json");
				_instance = new TipsDataManager(path);
			}
			return _instance;
		}
	}

	public TipsDataManager(string path)
	{
		base.FilePath = path;
	}

	public TipEntry GetRandomTip(TipEntry.TipContext context)
	{
		List<TipEntry> list = DatabaseArray.FindAll((TipEntry m) => m.Context == context || m.Context == TipEntry.TipContext.Any);
		int index = Random.Range(0, list.Count);
		return list[index];
	}
}
