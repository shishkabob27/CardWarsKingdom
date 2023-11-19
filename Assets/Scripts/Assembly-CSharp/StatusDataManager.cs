using System.IO;

public class StatusDataManager : DataManager<StatusData>
{
	private static StatusDataManager _instance;

	public static StatusDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_StatusEffects.json");
				_instance = new StatusDataManager(path);
			}
			return _instance;
		}
	}

	public StatusDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(GameEventFXDataManager.Instance);
	}
}
