using System;
using System.Collections.Generic;
using System.IO;

public class GachaWeightDataManager : DataManager<GachaWeightTable>
{
	private static GachaWeightDataManager _instance;

	public static GachaWeightDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_GachaWeights.json");
				_instance = new GachaWeightDataManager(path);
			}
			return _instance;
		}
	}

	public GachaWeightDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		if (jlist.Count == 0)
		{
			return;
		}
		Dictionary<string, GachaWeightTable> dictionary = new Dictionary<string, GachaWeightTable>();
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item;
			string dropId = TFUtils.LoadString(dictionary2, "DropID", string.Empty);
			int starOverride = TFUtils.LoadInt(dictionary2, "StarOverride", -1);
			foreach (KeyValuePair<string, object> item2 in dictionary2)
			{
				if (item2.Key == "DropID" || item2.Key == "StarOverride" || !((string)item2.Value != string.Empty))
				{
					continue;
				}
				int num = Convert.ToInt32(item2.Value);
				if (num > 0)
				{
					GachaWeightTable value;
					if (!dictionary.TryGetValue(item2.Key, out value))
					{
						value = new GachaWeightTable(item2.Key);
						dictionary.Add(item2.Key, value);
					}
					value.AddEntry(dropId, starOverride, num);
				}
			}
		}
		foreach (GachaWeightTable value2 in dictionary.Values)
		{
			Database.Add(value2.ID, value2);
			DatabaseArray.Add(value2);
		}
	}
}
