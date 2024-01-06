using System.IO;

public class EventTemplateDataManager : DataManager<EventTemplateData>
{
	private static EventTemplateDataManager _instance;

	public static EventTemplateDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_EventTemplate.json");
				_instance = new EventTemplateDataManager(path);
			}
			return _instance;
		}
	}

	public EventTemplateDataManager(string path)
	{
		base.FilePath = path;
	}

	public bool CheckEventCondition(EventTemplateData tdata)
	{
		if (!tdata.TemplateCustomData.ContainsKey("delegate"))
		{
			return true;
		}
		string text = tdata.TemplateCustomData["delegate"];
		bool result = true;
		switch (text)
		{
		case "OpenGacha":
			result = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Gacha");
			break;
		case "OpenDungeons":
			result = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Dungeon");
			break;
		case "OpenStoreHeroes":
			if (tdata.TemplateCustomData.ContainsKey("hero"))
			{
				string text2 = tdata.TemplateCustomData["hero"];
				LeaderData leaderData = LeaderDataManager.Instance.GetDatabase().Find((LeaderData m) => m.ID == tdata.TemplateCustomData["hero"]);
				result = ((leaderData == null) ? Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("BuyHero") : (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("BuyHero") && !Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(leaderData)));
			}
			else
			{
				result = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("BuyHero");
			}
			break;
		case "OpenCustomization":
			result = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("BuyHero");
			break;
		case "OpenExpeditions":
			result = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Expedition");
			break;
		case "OpenLabAwakenings":
			result = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Lab_CardEquip");
			break;
		case "OpenPVP":
			result = Singleton<PlayerInfoScript>.Instance.CanPvp();
			break;
		}
		return result;
	}
}
