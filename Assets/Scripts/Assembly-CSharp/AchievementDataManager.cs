using System.IO;

public class AchievementDataManager : DataManager<AchievementData>
{
	private static AchievementDataManager _instance;

	public static AchievementDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_Achievements.json");
				_instance = new AchievementDataManager(path);
			}
			return _instance;
		}
	}

	public AchievementDataManager(string path)
	{
		base.FilePath = path;
	}
}
