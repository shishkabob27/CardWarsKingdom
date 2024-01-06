using System.IO;

public class CardBackDataManager : DataManager<CardBackData>
{
	public static CardBackData DefaultData;

	private static CardBackDataManager _instance;

	public static CardBackDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_CardBacks.json");
				_instance = new CardBackDataManager(path);
			}
			return _instance;
		}
	}

	public CardBackDataManager(string path)
	{
		base.FilePath = path;
	}
}
