using System.IO;

public class PvpRankDataManager : DataManager<PvpRankData>
{
	private static PvpRankDataManager _instance;

	public static PvpRankDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_PVPRanks.json");
				_instance = new PvpRankDataManager(path);
			}
			return _instance;
		}
	}

	public PvpRankDataManager(string path)
	{
		base.FilePath = path;
	}

	public PvpRankData GetRank(int rank)
	{
		if (rank < 1 || rank > DatabaseArray.Count)
		{
			return null;
		}
		return DatabaseArray[rank - 1];
	}

	public int GetMaxRank()
	{
		return DatabaseArray.Count;
	}
}
