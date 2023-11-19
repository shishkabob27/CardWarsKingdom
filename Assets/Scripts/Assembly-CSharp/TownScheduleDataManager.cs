using System.IO;

public class TownScheduleDataManager : DataManager<TownScheduleData>
{
	private static TownScheduleDataManager _instance;

	public static TownScheduleDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_TownSchedule.json");
				_instance = new TownScheduleDataManager(path);
			}
			return _instance;
		}
	}

	public TownScheduleDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void PostLoad()
	{
		base.PostLoad();
		DatabaseArray.Sort((TownScheduleData lhs, TownScheduleData rhs) => lhs.StartDate.CompareTo(rhs.StartDate));
	}

	public TownScheduleData GetCurrentScheduledTownData()
	{
		int dayOfYear = TFUtils.ServerTime.DayOfYear;
		for (int i = 0; i < DatabaseArray.Count; i++)
		{
			if (DatabaseArray[i].StartDate > dayOfYear)
			{
				if (i == 0)
				{
					return DatabaseArray[DatabaseArray.Count - 1];
				}
				return DatabaseArray[i - 1];
			}
		}
		if (DatabaseArray == null || DatabaseArray.Count == 0)
		{
			return null;
		}
		return DatabaseArray[DatabaseArray.Count - 1];
	}
}
