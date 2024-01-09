using System;
using System.Collections.Generic;
using System.Globalization;

public class MailData : IComparable<MailData>, ILoadableData
{
	private const int DAYSOFWEEK_NUM = 7;

	private const string DATE_FORMAT = "dd/MM/yyyy";

	private string _ID;

	private string _Title;

	private string _BodyText;

	private string _CurrencyType;

	private int _GiftQuantity;

	private string _Repeatable;

	private DateTime _ShowDate;

	private DateTime _EndDate;

	private string _PlayerID;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string RewardType
	{
		get
		{
			return _CurrencyType;
		}
	}

	public string Title
	{
		get
		{
			return _Title;
		}
	}

	public string BodyText
	{
		get
		{
			return _BodyText;
		}
	}

	public int GiftQuantity
	{
		get
		{
			return _GiftQuantity;
		}
	}

	public string Repeatable
	{
		get
		{
			return _Repeatable;
		}
	}

	public DateTime ShowDate
	{
		get
		{
			return _ShowDate;
		}
	}

	public DateTime EndDate
	{
		get
		{
			return _EndDate;
		}
	}

	public string playerId
	{
		get
		{
			return _PlayerID;
		}
	}

	public bool ShowInPopop { get; private set; }

	public string GiftID { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_Title = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		_BodyText = TFUtils.LoadLocalizedString(dict, "BodyText");
		_CurrencyType = TFUtils.LoadString(dict, "GiftType", string.Empty);
		_GiftQuantity = TFUtils.LoadInt(dict, "Quantity", 0);
		_Repeatable = TFUtils.LoadString(dict, "Repeats", string.Empty);
		ShowInPopop = TFUtils.LoadBool(dict, "ShowInPopup", false);
		GiftID = TFUtils.LoadString(dict, "GiftID", null);
		string text = TFUtils.TryLoadString(dict, "showDate");
		string text2 = TFUtils.TryLoadString(dict, "endDate");
		bool flag = string.IsNullOrEmpty(text);
		bool flag2 = string.IsNullOrEmpty(text2);
		if (!flag)
		{
			_ShowDate = DateTime.ParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
		}
		if (!flag2)
		{
			_EndDate = DateTime.ParseExact(text2, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
		}
		string tryid = TFUtils.TryLoadString(dict, "PlayerID");
		if (!string.IsNullOrEmpty(tryid))
		{
			UnityEngine.Debug.Log(tryid);
			_PlayerID = tryid;
		}
	}

	public int CompareTo(MailData mail)
	{
		if (ShowDate != DateTime.MinValue && mail.ShowDate != DateTime.MinValue)
		{
			return ShowDate.CompareTo(mail.ShowDate) * -1;
		}
		if (ShowDate != DateTime.MinValue)
		{
			return 1;
		}
		if (mail.ShowDate != DateTime.MinValue)
		{
			return -1;
		}
		return 0;
	}
}
