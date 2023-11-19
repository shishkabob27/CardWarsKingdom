using System.Collections.Generic;

public class PlayerBadgeData : ILoadableData, UnlockableData
{
	private string _ID;

	private string _Name;

	private string _UITexture;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string Name
	{
		get
		{
			return _Name;
		}
	}

	public string UITexture
	{
		get
		{
			return _UITexture;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_Name = TFUtils.LoadString(dict, "Name", string.Empty);
		_UITexture = TFUtils.LoadString(dict, "UITexture", string.Empty);
	}
}
