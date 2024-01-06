using System.Collections.Generic;
using System.IO;

public class CustomAIDataManager : DataManager<CustomAIData>
{
	private static CustomAIDataManager _instance;

	public static CustomAIDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_CustomAI.json");
				_instance = new CustomAIDataManager(path);
			}
			return _instance;
		}
	}

	public CustomAIDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		CustomAIData customAIData = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "ID", string.Empty);
			if (customAIData == null || text != "^")
			{
				customAIData = new CustomAIData();
				customAIData.Populate(dictionary);
				Database.Add(customAIData.ID, customAIData);
				DatabaseArray.Add(customAIData);
			}
			else
			{
				customAIData.AddRow(dictionary);
			}
		}
	}
}
