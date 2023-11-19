using System.Collections.Generic;

public class DummyData : ILoadableData
{
	private string _ID;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
	}
}
