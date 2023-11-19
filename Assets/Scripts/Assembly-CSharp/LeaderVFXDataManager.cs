using System.Collections.Generic;
using System.IO;

public class LeaderVFXDataManager : DataManager<LeaderVFXData>
{
	private static LeaderVFXDataManager _instance;

	public static LeaderVFXDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_LeaderVFX.json");
				_instance = new LeaderVFXDataManager(path);
			}
			return _instance;
		}
	}

	public LeaderVFXDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		LeaderVFXData leaderVFXData = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "ID", string.Empty);
			if (leaderVFXData == null || text != "^")
			{
				leaderVFXData = new LeaderVFXData(text);
				if (!Database.ContainsKey(text))
				{
					Database.Add(text, leaderVFXData);
				}
				DatabaseArray.Add(leaderVFXData);
			}
			LeaderVFXEntry entry = new LeaderVFXEntry(dictionary);
			leaderVFXData.AddEntry(entry);
		}
	}
}
