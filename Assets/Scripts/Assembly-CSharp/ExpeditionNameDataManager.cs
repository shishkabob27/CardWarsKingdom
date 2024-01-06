using System.IO;

public class ExpeditionNameDataManager : DataManager<ExpeditionNameData>
{
	private static ExpeditionNameDataManager _instance;

	public static ExpeditionNameDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_ExpeditionNames.json");
				_instance = new ExpeditionNameDataManager(path);
			}
			return _instance;
		}
	}

	public ExpeditionNameDataManager(string path)
	{
		base.FilePath = path;
	}
}
