using System.IO;

public class TownBuildingDataManager : DataManager<TownBuildingData>
{
	private static TownBuildingDataManager _instance;

	public static TownBuildingDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_TownBuildings.json");
				_instance = new TownBuildingDataManager(path);
			}
			return _instance;
		}
	}

	public TownBuildingDataManager(string path)
	{
		base.FilePath = path;
	}
}
