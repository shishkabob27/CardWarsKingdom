using System.IO;

public class QuestDataManager : DataManager<QuestData>
{
	private static QuestDataManager _instance;

	public static QuestDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_Quest.json");
				_instance = new QuestDataManager(path);
			}
			return _instance;
		}
	}

	public QuestDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(LeaderDataManager.Instance);
		AddDependency(QuestLoadoutDataManager.Instance);
		AddDependency(LeagueDataManager.Instance);
		AddDependency(CustomAIDataManager.Instance);
	}
}
