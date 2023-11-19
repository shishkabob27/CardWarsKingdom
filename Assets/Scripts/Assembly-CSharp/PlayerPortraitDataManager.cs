using System.IO;

public class PlayerPortraitDataManager : DataManager<PlayerPortraitData>
{
	public const string DEFAULT = "Default";

	public const string FACEBOOK = "Facebook";

	private static PlayerPortraitDataManager _instance;

	public static PlayerPortraitDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_PlayerPortraits.json");
				_instance = new PlayerPortraitDataManager(path);
			}
			return _instance;
		}
	}

	public PlayerPortraitDataManager(string path)
	{
		base.FilePath = path;
	}
}
