using System.Collections.Generic;

public class ChatCountryBlacklistData : ILoadableData
{
	public string ID { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Country", string.Empty);
	}
}
