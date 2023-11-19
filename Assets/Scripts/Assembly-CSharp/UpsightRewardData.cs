using System.Collections.Generic;

public class UpsightRewardData : ILoadableData
{
	private string _ID;

	private string _WeightTable;

	private string _Description;

	private string _Quality;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string WeightTable
	{
		get
		{
			return _WeightTable;
		}
	}

	public string Description
	{
		get
		{
			return _Description;
		}
	}

	public string Quality
	{
		get
		{
			return _Quality;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_WeightTable = TFUtils.LoadString(dict, "WeightTable", string.Empty);
		_Description = TFUtils.LoadString(dict, "Description", string.Empty);
		_Quality = TFUtils.LoadString(dict, "Quality", string.Empty);
	}
}
