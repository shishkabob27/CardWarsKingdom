using System.Collections.Generic;
using System.IO;

public class MissionDataManager : DataManager<MissionData>
{
	private static MissionDataManager _instance;

	public static MissionDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_Missions.json");
				_instance = new MissionDataManager(path);
			}
			return _instance;
		}
	}

	public MissionDataManager(string path)
	{
		base.FilePath = path;
	}

	public List<MissionData> GetMissions(MissionType type)
	{
		return DatabaseArray.FindAll((MissionData m) => m.Type == type);
	}

	protected override void PostLoad()
	{
		int num = 0;
		foreach (MissionData item in DatabaseArray)
		{
			item.Index = num;
			item.PostLoad();
			num++;
		}
	}
}
