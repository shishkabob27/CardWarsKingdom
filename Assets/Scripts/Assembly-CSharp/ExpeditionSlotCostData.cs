using System.Collections.Generic;

public class ExpeditionSlotCostData : ILoadableData
{
	public string ID { get; private set; }

	public int Cost { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Slot", string.Empty);
		Cost = TFUtils.LoadInt(dict, "Cost", 1);
	}
}
