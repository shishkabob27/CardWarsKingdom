using System;
using System.Collections.Generic;

public class TownBuildingData : ILoadableData
{
	private string _ID;

	private string _TweenController;

	private string _ControllerScript;

	private string _Name;

	private string _Param;

	private string _UITexture;

	private BadgeEnum _Badge;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string TweenController
	{
		get
		{
			return _TweenController;
		}
	}

	public string ControllerScript
	{
		get
		{
			return _ControllerScript;
		}
	}

	public string Name
	{
		get
		{
			return _Name;
		}
	}

	public string Param
	{
		get
		{
			return _Param;
		}
	}

	public string UITexture
	{
		get
		{
			return _UITexture;
		}
	}

	public BadgeEnum Badge
	{
		get
		{
			return _Badge;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_TweenController = TFUtils.LoadString(dict, "TweenController", string.Empty);
		_ControllerScript = TFUtils.LoadString(dict, "ControllerScript", string.Empty);
		_Name = TFUtils.LoadString(dict, "Name", string.Empty);
		_Param = TFUtils.LoadString(dict, "Param", null);
		_UITexture = "UI/Icons_Buildings/" + TFUtils.LoadString(dict, "UITexture", string.Empty);
		string text = TFUtils.LoadString(dict, "BadgeEnum", string.Empty);
		if (text != string.Empty)
		{
			_Badge = (BadgeEnum)(int)Enum.Parse(typeof(BadgeEnum), text);
		}
		else
		{
			_Badge = BadgeEnum.None;
		}
	}
}
