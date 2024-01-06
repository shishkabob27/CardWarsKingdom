using System.IO;

public class CreaturePassiveDataManager : DataManager<CreaturePassiveData>
{
	private static CreaturePassiveDataManager _instance;

	public static CreaturePassiveDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_CreaturePassives.json");
				_instance = new CreaturePassiveDataManager(path);
			}
			return _instance;
		}
	}

	public CreaturePassiveDataManager(string path)
	{
		base.FilePath = path;
	}
}
