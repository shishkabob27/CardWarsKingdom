using System.IO;

public class ChatCountryBlacklistDataManager : DataManager<ChatCountryBlacklistData>
{
	private const string WorldwideCode = "WW";

	private static ChatCountryBlacklistDataManager _instance;

	public static ChatCountryBlacklistDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_ChatCountryBlacklist.json");
				_instance = new ChatCountryBlacklistDataManager(path);
			}
			return _instance;
		}
	}

	public ChatCountryBlacklistDataManager(string path)
	{
		base.FilePath = path;
	}

	public bool IsCountryBlacklisted(string countryCode)
	{
		return Database.ContainsKey("WW") || Database.ContainsKey(countryCode);
	}
}
