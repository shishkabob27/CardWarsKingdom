using System.IO;

public class SpeedUpDataManager : DataManager<SpeedUpData>
{
	private static SpeedUpDataManager _instance;

	public static SpeedUpDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_SpeedUps.json");
				_instance = new SpeedUpDataManager(path);
			}
			return _instance;
		}
	}

	public SpeedUpDataManager(string path)
	{
		base.FilePath = path;
	}

	public SpeedUpData GetDataByTime(int minutes)
	{
		return GetDatabase().Find((SpeedUpData m) => m.Minutes == minutes);
	}
}
