using System.Collections.Generic;

public class AchievementData : ILoadableData
{
	private string _ID;

	private string _iOS_ID;

	private string _Android_ID;

	private int _Total_Steps;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string iOS_ID
	{
		get
		{
			return _iOS_ID;
		}
	}

	public string Android_ID
	{
		get
		{
			return _Android_ID;
		}
	}

	public int Total_Steps
	{
		get
		{
			return _Total_Steps;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "Achievement", string.Empty);
		_iOS_ID = TFUtils.LoadString(dict, "iOS ID", string.Empty);
		_Android_ID = TFUtils.LoadString(dict, "Android ID", string.Empty);
		_Total_Steps = TFUtils.LoadInt(dict, "Total Steps", 1);
	}
}
