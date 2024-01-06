using System.IO;

public class PlayerTitleDataManager : DataManager<PlayerTitleData>
{
	private static PlayerTitleDataManager _instance;

	public static PlayerTitleDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_PlayerTitles.json");
				_instance = new PlayerTitleDataManager(path);
			}
			return _instance;
		}
	}

	public PlayerTitleDataManager(string path)
	{
		base.FilePath = path;
	}
}
