using System.Collections.Generic;

public class HelpEntry : ILoadableData
{
	private int _ID;

	private string _Title;

	private string _Body;

	private static int mLastLoadedID;

	public string ID
	{
		get
		{
			return _ID.ToString();
		}
	}

	public string Title
	{
		get
		{
			return _Title;
		}
	}

	public string Body
	{
		get
		{
			return _Body;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = mLastLoadedID;
		mLastLoadedID++;
		_Title = TFUtils.LoadLocalizedString(dict, "Title", string.Empty);
		_Body = TFUtils.LoadLocalizedString(dict, "Body", string.Empty);
	}
}
