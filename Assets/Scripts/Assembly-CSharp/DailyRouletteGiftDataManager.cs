using System.IO;
using UnityEngine;

public class DailyRouletteGiftDataManager : DataManager<DailyRouletteGiftData>
{
	private static DailyRouletteGiftDataManager _instance;

	public static DailyRouletteGiftDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_DailyGiftRouletteWeights.json");
				_instance = new DailyRouletteGiftDataManager(path);
			}
			return _instance;
		}
	}

	public DailyRouletteGiftDataManager(string path)
	{
		base.FilePath = path;
	}

	public DailyRouletteGiftData Spin()
	{
		int num = 0;
		foreach (DailyRouletteGiftData item in DatabaseArray)
		{
			num += item.Weight;
		}
		int num2 = Random.Range(0, num);
		foreach (DailyRouletteGiftData item2 in DatabaseArray)
		{
			num2 -= item2.Weight;
			if (num2 < 0)
			{
				return item2;
			}
		}
		return null;
	}
}
