using System.Collections.Generic;
using System.IO;

public class QuestLoadoutDataManager : DataManager<QuestLoadoutData>
{
	private static QuestLoadoutDataManager _instance;

	public static QuestLoadoutDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_QuestLoadouts.json");
				_instance = new QuestLoadoutDataManager(path);
			}
			return _instance;
		}
	}

	public QuestLoadoutDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(CreatureDataManager.Instance);
		AddDependency(CardDataManager.Instance);
	}

	protected override void ParseRows(List<object> jlist)
	{
		QuestLoadoutData questLoadoutData = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "LoadoutID", string.Empty);
			if (questLoadoutData == null || text != "^")
			{
				questLoadoutData = new QuestLoadoutData(text);
				if (!Database.ContainsKey(text))
				{
					Database.Add(text, questLoadoutData);
				}
				DatabaseArray.Add(questLoadoutData);
			}
			string text2 = TFUtils.LoadString(dictionary, "CreatureID", string.Empty);
			if (text2 == string.Empty)
			{
				questLoadoutData.AddEntry(null);
				continue;
			}
			QuestLoadoutEntry entry = new QuestLoadoutEntry(dictionary);
			questLoadoutData.AddEntry(entry);
		}
	}
}
