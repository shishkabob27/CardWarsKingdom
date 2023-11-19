using System.IO;

public class UpsightRewardDataManager : DataManager<UpsightRewardData>
{
	private static UpsightRewardDataManager _instance;

	public static UpsightRewardDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_Achievements.json");
				_instance = new UpsightRewardDataManager(path);
			}
			return _instance;
		}
	}

	public UpsightRewardDataManager(string path)
	{
		base.FilePath = path;
	}
}
