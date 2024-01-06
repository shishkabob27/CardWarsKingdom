using System.IO;

public class HelpDataManager : DataManager<HelpEntry>
{
	private static HelpDataManager _instance;

	public static HelpDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_Help.json");
				_instance = new HelpDataManager(path);
			}
			return _instance;
		}
	}

	public HelpDataManager(string path)
	{
		base.FilePath = path;
	}
}
