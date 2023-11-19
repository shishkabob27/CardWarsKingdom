using System.Collections.Generic;

public class QuickChatData : ILoadableData
{
	public string ID { get; private set; }

	public string Text { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Text = TFUtils.LoadLocalizedString(dict, "Text", string.Empty);
	}
}
