using System.Collections.Generic;
using System.IO;

public class LeagueDataManager : DataManager<LeagueData>
{
	private static LeagueDataManager _instance;

	public static LeagueDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_Leagues.json");
				_instance = new LeagueDataManager(path);
			}
			return _instance;
		}
	}

	public LeagueDataManager(string path)
	{
		base.FilePath = path;
	}

	public List<LeagueData> GetLeagues(QuestLineEnum questLine)
	{
		return DatabaseArray.FindAll((LeagueData m) => m.QuestLine == questLine && (m.HideUntilPassed == null || Singleton<PlayerInfoScript>.Instance.IsLeagueComplete(m.HideUntilPassed)));
	}

	public List<LeagueData> GetNotificationOnLeagues()
	{
		return DatabaseArray.FindAll((LeagueData m) => m.NotificationOn);
	}

	protected override void PostLoad()
	{
		foreach (LeagueData item in DatabaseArray)
		{
			item.PostLoad();
		}
	}
}
