using System;
using System.Collections.Generic;

public class TipEntry : ILoadableData
{
	public enum TipContext
	{
		Any,
		Victory,
		Failure
	}

	private int _ID;

	private string _Text;

	private TipContext _Context;

	private static int mLastLoadedID;

	public string ID
	{
		get
		{
			return _ID.ToString();
		}
	}

	public string Text
	{
		get
		{
			return _Text;
		}
	}

	public TipContext Context
	{
		get
		{
			return _Context;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = mLastLoadedID;
		mLastLoadedID++;
		_Text = TFUtils.LoadLocalizedString(dict, "Text", string.Empty);
		_Context = (TipContext)(int)Enum.Parse(typeof(TipContext), TFUtils.LoadLocalizedString(dict, "Context", string.Empty));
	}
}
