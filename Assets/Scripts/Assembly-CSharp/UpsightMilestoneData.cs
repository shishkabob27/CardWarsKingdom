using System.Collections.Generic;

public class UpsightMilestoneData : ILoadableData
{
	private string _ID;

	public List<UpsightConditionData> conditionData = new List<UpsightConditionData>();

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public UpsightMilestoneData(string MSID = null)
	{
		_ID = MSID;
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}
}
