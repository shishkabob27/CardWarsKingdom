using System.Collections.Generic;

public class CreatureStarRatingData : ILoadableData
{
	public string ID { get; private set; }

	public int MaxLevel { get; private set; }

	public int StartingLevel { get; private set; }

	public XPTableData XPTable { get; private set; }

	public int CostToEnhance { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Stars", string.Empty);
		MaxLevel = TFUtils.LoadInt(dict, "MaxLevel", 1);
		StartingLevel = TFUtils.LoadInt(dict, "StartingLevel", 1);
		XPTable = XPTableDataManager.Instance.GetData(TFUtils.LoadString(dict, "XPTable", string.Empty));
		CostToEnhance = TFUtils.LoadInt(dict, "CurrencyCost", 1);
	}
}
