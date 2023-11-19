using System.IO;

public class RandomDungeonFloorDataManager : DataManager<RandomDungeonFloorData>
{
	private static RandomDungeonFloorDataManager _instance;

	public static RandomDungeonFloorDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_RandomDungeonFloors.json");
				_instance = new RandomDungeonFloorDataManager(path);
			}
			return _instance;
		}
	}

	public RandomDungeonFloorDataManager(string path)
	{
		base.FilePath = path;
	}

	public RandomDungeonFloorData GetCurrentData()
	{
		return GetData(Singleton<PlayerInfoScript>.Instance.SaveData.RandomDungeonLevel.ToString());
	}
}
