using System.IO;

public class CardDataManager : DataManager<CardData>
{
	private static CardDataManager _instance;

	public static CardDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_ActionCards.json");
				_instance = new CardDataManager(path);
			}
			return _instance;
		}
	}

	public CardDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(StatusDataManager.Instance);
	}
}
