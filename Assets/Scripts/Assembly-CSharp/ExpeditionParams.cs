using System.Collections.Generic;
using System.IO;

public class ExpeditionParams : DataManager<DummyData>
{
	private static ExpeditionParams _instance;

	public static int StartingExpeditionSlots { get; private set; }

	public static float OutcomeRandomness { get; private set; }

	public static float UntypedChance { get; private set; }

	public static float FavoredFactionValue { get; private set; }

	public static int ExpeditionRefreshHours { get; private set; }

	public static int RepopulateCost { get; private set; }

	public static ExpeditionParams Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_ExpeditionParams.json");
				_instance = new ExpeditionParams(path);
			}
			return _instance;
		}
	}

	public ExpeditionParams(string path)
	{
		base.FilePath = path;
	}

	protected override void ParseRows(List<object> jlist)
	{
		if (jlist.Count == 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item;
			string key = TFUtils.LoadString(dictionary2, "Parameter", string.Empty);
			object value = dictionary2["Value"];
			dictionary.Add(key, value);
		}
		StartingExpeditionSlots = TFUtils.LoadInt(dictionary, "StartingExpeditionSlots", 2);
		OutcomeRandomness = TFUtils.LoadFloat(dictionary, "OutcomeRandomness", 0f);
		UntypedChance = TFUtils.LoadFloat(dictionary, "UntypedChance", 0f);
		FavoredFactionValue = TFUtils.LoadFloat(dictionary, "FavoredFactionValue", 0f);
		ExpeditionRefreshHours = TFUtils.LoadInt(dictionary, "ExpeditionRefreshHours", 0);
		RepopulateCost = TFUtils.LoadInt(dictionary, "RepopulateCost", 0);
	}
}
