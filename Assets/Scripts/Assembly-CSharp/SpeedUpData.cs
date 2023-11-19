using System.Collections.Generic;

public class SpeedUpData : ILoadableData
{
	public string ID { get; private set; }

	public int Minutes { get; private set; }

	public int Price { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Minutes = TFUtils.LoadInt(dict, "SpeedupMinutes", 1);
		Price = TFUtils.LoadInt(dict, "Price", 1);
	}
}
