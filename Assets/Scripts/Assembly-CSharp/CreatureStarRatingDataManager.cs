using System.IO;

public class CreatureStarRatingDataManager : DataManager<CreatureStarRatingData>
{
	private static CreatureStarRatingDataManager _instance;

	public static CreatureStarRatingDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_CreatureStarRatings.json");
				_instance = new CreatureStarRatingDataManager(path);
			}
			return _instance;
		}
	}

	public CreatureStarRatingDataManager(string path)
	{
		base.FilePath = path;
	}

	public CreatureStarRatingData GetDataByRating(int rating)
	{
		return DatabaseArray[rating - 1];
	}

	public int MaxStarRating()
	{
		return DatabaseArray.Count;
	}
}
