using System.IO;

public class QuickChatDataManager : DataManager<QuickChatData>
{
	private static QuickChatDataManager _instance;

	public static QuickChatDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_QuickChat.json");
				_instance = new QuickChatDataManager(path);
			}
			return _instance;
		}
	}

	public QuickChatDataManager(string path)
	{
		base.FilePath = path;
	}
}
