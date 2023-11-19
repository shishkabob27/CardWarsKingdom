using System.Collections.Generic;

public class PlayerTitleData : ILoadableData, UnlockableData
{
	private string _ID;

	private string _LocID;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string LocID
	{
		get
		{
			return _LocID;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_LocID = TFUtils.LoadString(dict, "LocID", string.Empty);
	}
}
