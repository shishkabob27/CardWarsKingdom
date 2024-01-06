using System.IO;

public class LeaderDataManager : DataManager<LeaderData>
{
	private static LeaderDataManager _instance;

	public static LeaderDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Leaders.json");
				_instance = new LeaderDataManager(path);
			}
			return _instance;
		}
	}

	public LeaderDataManager(string path)
	{
		base.FilePath = path;
	}
}
