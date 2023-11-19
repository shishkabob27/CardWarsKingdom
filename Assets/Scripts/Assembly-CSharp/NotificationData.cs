using System;
using System.Collections.Generic;

public class NotificationData : ILoadableData
{
	private string _ID;

	private NotificationType _nType;

	private StaminaType _TargetStaminaType;

	private NotificationEventType _nEventType;

	private int _HoursAfterLastLogin;

	private DateTime _nDateTime;

	private string _TextID;

	private bool _Repeat;

	private int _RepeatFreq;

	private int _DaysBefore;

	private int _HoursBefore;

	private int _MinutesBefore;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public NotificationType nType
	{
		get
		{
			return _nType;
		}
	}

	public StaminaType TargetStaminaType
	{
		get
		{
			return _TargetStaminaType;
		}
	}

	public NotificationEventType nEventType
	{
		get
		{
			return _nEventType;
		}
	}

	public int HoursAfterLastLogin
	{
		get
		{
			return _HoursAfterLastLogin;
		}
	}

	public DateTime nDateTime
	{
		get
		{
			return _nDateTime;
		}
	}

	public string TextID
	{
		get
		{
			return _TextID;
		}
	}

	public bool Repeat
	{
		get
		{
			return _Repeat;
		}
	}

	public int RepeatFreq
	{
		get
		{
			return _RepeatFreq;
		}
	}

	public int DaysBefore
	{
		get
		{
			return _DaysBefore;
		}
	}

	public int HoursBefore
	{
		get
		{
			return _HoursBefore;
		}
	}

	public int MinutesBefore
	{
		get
		{
			return _MinutesBefore;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		char[] separator = new char[2] { ':', ',' };
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		string text = TFUtils.LoadString(dict, "Type", null);
		if (text != null)
		{
			string[] array = text.Trim().Split(separator);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			if (array.Length > 0)
			{
				try
				{
					_nType = (NotificationType)(int)Enum.Parse(typeof(NotificationType), array[0], true);
				}
				catch
				{
					_nType = NotificationType.None;
				}
				if (_nType == NotificationType.Stamina && array.Length > 1)
				{
					try
					{
						_TargetStaminaType = (StaminaType)(int)Enum.Parse(typeof(StaminaType), array[1], true);
					}
					catch
					{
						_nType = NotificationType.None;
					}
				}
				else if ((_nType == NotificationType.EventStart || _nType == NotificationType.EventEnd) && array.Length > 1)
				{
					try
					{
						_nEventType = (NotificationEventType)(int)Enum.Parse(typeof(NotificationEventType), array[1], true);
					}
					catch
					{
						_nType = NotificationType.None;
					}
				}
			}
		}
		_HoursAfterLastLogin = TFUtils.LoadInt(dict, "HoursAfterLastLogin", 0);
		_DaysBefore = TFUtils.LoadInt(dict, "DaysBefore", 0);
		_HoursBefore = TFUtils.LoadInt(dict, "HoursBefore", 0);
		_MinutesBefore = TFUtils.LoadInt(dict, "MinutesBefore", 0);
		string text2 = TFUtils.LoadString(dict, "DateTime", string.Empty);
		if (text2 != string.Empty)
		{
			string format = "MM/dd/yyyy HH:mm:ss";
			_nDateTime = DateTime.ParseExact(text2, format, null);
		}
		_TextID = TFUtils.LoadString(dict, "TextID", string.Empty);
		string text3 = TFUtils.LoadString(dict, "Repeat", string.Empty);
		if (text3 != string.Empty)
		{
			if (string.Compare(text3.ToLower(), "always") == 0)
			{
				_Repeat = true;
				return;
			}
			_Repeat = false;
			int.TryParse(text3, out _RepeatFreq);
		}
	}
}
