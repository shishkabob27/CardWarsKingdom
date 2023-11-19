using System.Collections.Generic;

public class LeaderVFXData : ILoadableData
{
	private string _ID;

	private List<LeaderVFXEntry> _VFXEntries = new List<LeaderVFXEntry>();

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public IList<LeaderVFXEntry> Entries
	{
		get
		{
			return _VFXEntries.AsReadOnly();
		}
	}

	public LeaderVFXData(string loadoutID = null)
	{
		_ID = loadoutID;
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}

	public void AddEntry(LeaderVFXEntry entry)
	{
		_VFXEntries.Add(entry);
	}
}
