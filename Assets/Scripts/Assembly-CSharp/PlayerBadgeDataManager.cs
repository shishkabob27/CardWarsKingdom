using System.IO;

public class PlayerBadgeDataManager : DataManager<PlayerBadgeData>
{
	private static PlayerBadgeDataManager _instance;

	public static PlayerBadgeDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_PlayerBadges.json");
				_instance = new PlayerBadgeDataManager(path);
			}
			return _instance;
		}
	}

	public PlayerBadgeDataManager(string path)
	{
		base.FilePath = path;
	}
}
