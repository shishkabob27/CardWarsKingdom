using System.Collections.Generic;
using System.IO;

public class TutorialCardOverridesDataManager : DataManager<TutorialCardOverridesEntry>
{
	private static TutorialCardOverridesDataManager _instance;

	public static TutorialCardOverridesDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_TutorialCardOverrides.json");
				_instance = new TutorialCardOverridesDataManager(path);
			}
			return _instance;
		}
	}

	public TutorialCardOverridesDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(TutorialDataManager.Instance);
	}

	protected override void ParseRows(List<object> jlist)
	{
		TutorialDataManager.TutorialBlock tutorialBlock = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> data = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(data, "Block", string.Empty);
			if (tutorialBlock == null || text != "^")
			{
				tutorialBlock = TutorialDataManager.Instance.GetBlock(text);
			}
			for (int i = 0; i < 6; i++)
			{
				string text2 = TFUtils.LoadString(data, ((CreatureFaction)i).ClassName(), null);
				if (text2 != null)
				{
					if (tutorialBlock.CardOverrides[i] == null)
					{
						tutorialBlock.CardOverrides[i] = new List<string>();
					}
					tutorialBlock.CardOverrides[i].Add(text2);
				}
				string text3 = TFUtils.LoadString(data, "Enemy" + ((CreatureFaction)i).ClassName(), null);
				if (text3 != null)
				{
					if (tutorialBlock.EnemyCardOverrides[i] == null)
					{
						tutorialBlock.EnemyCardOverrides[i] = new List<string>();
					}
					tutorialBlock.EnemyCardOverrides[i].Add(text3);
				}
			}
		}
	}
}
