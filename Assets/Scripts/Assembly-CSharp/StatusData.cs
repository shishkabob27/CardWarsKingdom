using System;
using System.Collections.Generic;

public class StatusData : ILoadableData
{
	public const int MAX_CANCELS = 5;

	private string _ID;

	private StackType _StackRule;

	private DisplayStackType _DisplayType;

	private GameEvent _EnableMessage;

	private GameEvent _DisableMessage;

	private string _ParamName;

	private string _ParamUnit;

	private StatusType _StatusType;

	private string _InflictedStatus;

	private CreatureFaction _Vulnerability;

	private string _StatusIconSprite;

	private GameEventFXData _FXData;

	public string[] StatusCancels = new string[5];

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public float FixedValue1 { get; private set; }

	public float FixedValue2 { get; private set; }

	public StackType StackRule
	{
		get
		{
			return _StackRule;
		}
	}

	public DisplayStackType DisplayType
	{
		get
		{
			return _DisplayType;
		}
	}

	public GameEvent EnableMessage
	{
		get
		{
			return _EnableMessage;
		}
	}

	public GameEvent DisableMessage
	{
		get
		{
			return _DisableMessage;
		}
	}

	public string ParamName
	{
		get
		{
			return _ParamName;
		}
	}

	public string ParamUnit
	{
		get
		{
			return _ParamUnit;
		}
	}

	public StatusType StatusType
	{
		get
		{
			return _StatusType;
		}
	}

	public string InflictedStatus
	{
		get
		{
			return _InflictedStatus;
		}
	}

	public CreatureFaction Vulnerability
	{
		get
		{
			return _Vulnerability;
		}
	}

	public string StatusIconSprite
	{
		get
		{
			return _StatusIconSprite;
		}
	}

	public GameEventFXData FXData
	{
		get
		{
			return _FXData;
		}
	}

	public bool Instant { get; private set; }

	public bool IsLandscape { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "StatusName", string.Empty);
		FixedValue1 = TFUtils.LoadFloat(dict, "FixedValue1", 0f);
		FixedValue2 = TFUtils.LoadFloat(dict, "FixedValue2", 0f);
		_StackRule = (StackType)(int)Enum.Parse(typeof(StackType), TFUtils.LoadString(dict, "StackType", "None"), true);
		_DisplayType = (DisplayStackType)(int)Enum.Parse(typeof(DisplayStackType), TFUtils.LoadString(dict, "DisplayType", "None"), true);
		_EnableMessage = (GameEvent)(int)Enum.Parse(typeof(GameEvent), TFUtils.LoadString(dict, "EnableMessage", "None"), true);
		_DisableMessage = (GameEvent)(int)Enum.Parse(typeof(GameEvent), TFUtils.LoadString(dict, "DisableMessage", "None"), true);
		_StatusType = (StatusType)(int)Enum.Parse(typeof(StatusType), TFUtils.LoadString(dict, "StatusType", "None"), true);
		_InflictedStatus = TFUtils.LoadString(dict, "InflictStatus", string.Empty);
		_ParamName = TFUtils.LoadString(dict, "ParamName", string.Empty);
		_ParamUnit = TFUtils.LoadString(dict, "ParamUnit", string.Empty);
		_Vulnerability = (CreatureFaction)(int)Enum.Parse(typeof(CreatureFaction), TFUtils.LoadString(dict, "Vulnerability", "Colorless"), true);
		_StatusIconSprite = TFUtils.LoadString(dict, "StatusIconSprite", string.Empty);
		Instant = TFUtils.LoadBool(dict, "Instant", false);
		for (int i = 0; i < 5; i++)
		{
			string text = TFUtils.LoadString(dict, "Cancel" + (i + 1), string.Empty);
			StatusCancels[i] = text;
		}
		string enableString = EnableMessage.ToString().ToLower();
		_FXData = GameEventFXDataManager.Instance.Find((GameEventFXData m) => m.ID.ToLower() == enableString);
		IsLandscape = ID == "LandCorn" || ID == "LandPlains" || ID == "LandSand" || ID == "LandNice" || ID == "LandSwamp";
	}

	public string GetValueString(CreatureState creature, bool verbose)
	{
		StatusState statusState = creature.StatusEffects.Find((StatusState m) => m.Data == this);
		if (statusState == null)
		{
			return string.Empty;
		}
		float value;
		switch (DisplayType)
		{
		case DisplayStackType.Count:
			value = statusState.Count;
			break;
		case DisplayStackType.Turns:
			value = ((StackRule != StackType.Count) ? ((float)statusState.Duration) : ((float)statusState.Count));
			break;
		case DisplayStackType.Percent:
		case DisplayStackType.Amount:
		case DisplayStackType.IntAmount:
			value = statusState.Intensity;
			break;
		default:
			value = 0f;
			break;
		}
		if (verbose)
		{
			string text = DisplayType.Format(value, true);
			if (text != string.Empty)
			{
				text = "(" + text + ")";
			}
			return text;
		}
		return DisplayType.Format(value);
	}

	public float GetValueAmount(GameMessage message)
	{
		switch (DisplayType)
		{
		case DisplayStackType.Turns:
		case DisplayStackType.Count:
			return message.Amount;
		case DisplayStackType.Percent:
		case DisplayStackType.Amount:
		case DisplayStackType.IntAmount:
			return message.RawAmount;
		default:
			return 0f;
		}
	}
}
