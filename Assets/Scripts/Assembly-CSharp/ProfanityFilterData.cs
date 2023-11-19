using System.Collections.Generic;

public class ProfanityFilterData : ILoadableData
{
	public string ID { get; protected set; }

	public string BadWord { get; protected set; }

	public string Language { get; protected set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Index", string.Empty);
		BadWord = TFUtils.LoadString(dict, "BadWord", string.Empty);
		Language = TFUtils.LoadString(dict, "Language", null);
	}
}
