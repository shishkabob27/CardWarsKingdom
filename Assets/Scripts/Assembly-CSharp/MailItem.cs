using System;
using System.Collections.Generic;
using System.Text;

public class MailItem
{
	private static int mMaxUniqueId;

	private int _UniqueId;

	private string _ID;

	private string _MailTitle = string.Empty;

	private string _MailBody = string.Empty;

	private MailType _MailType;

	private int _HardQuantity;

	private int _SoftQuantity;

	private int _PVPQuantity;

	private string _Repeatable;

	private DateTime _ShowDate;

	private DateTime _EndDate;

	private HelperItem _Inviter;

	private bool _Opened;

	private bool _Rewarded;

	public DateTime TimeStamp;

	public int UniqueId
	{
		get
		{
			return _UniqueId;
		}
	}

	public string ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
		}
	}

	public string MailTitle
	{
		get
		{
			return _MailTitle;
		}
		set
		{
			_MailTitle = value;
		}
	}

	public string MailBody
	{
		get
		{
			return _MailBody;
		}
		set
		{
			_MailBody = value;
		}
	}

	public MailType MailType
	{
		get
		{
			return _MailType;
		}
		set
		{
			_MailType = value;
		}
	}

	public int HardQuantity
	{
		get
		{
			return _HardQuantity;
		}
		set
		{
			_HardQuantity = value;
		}
	}

	public int SoftQuantity
	{
		get
		{
			return _SoftQuantity;
		}
		set
		{
			_SoftQuantity = value;
		}
	}

	public int PVPQuantity
	{
		get
		{
			return _PVPQuantity;
		}
		set
		{
			_PVPQuantity = value;
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

	public string XPMaterialID { get; private set; }

	public int XPMaterialQuantity { get; private set; }

	public string CreatureID { get; private set; }

	public HelperItem Inviter
	{
		get
		{
			return _Inviter;
		}
		set
		{
			_Inviter = value;
		}
	}

	public bool Opened
	{
		get
		{
			return _Opened;
		}
		set
		{
			_Opened = value;
		}
	}

	public bool Rewarded
	{
		get
		{
			return _Rewarded;
		}
		set
		{
			_Rewarded = value;
		}
	}

	public MailItem(MailData Data)
	{
		_ID = Data.ID;
		_MailType = MailType.Scheduled;
		_MailTitle = Data.Title;
		_MailBody = Data.BodyText;
		_Repeatable = Data.Repeatable;
		_ShowDate = Data.ShowDate;
		_EndDate = Data.EndDate;
		if (Data.RewardType == "SoftCurrency")
		{
			_SoftQuantity = Data.GiftQuantity;
		}
		else if (Data.RewardType == "HardCurrency")
		{
			_HardQuantity = Data.GiftQuantity;
		}
		else if (Data.RewardType == "PvPCurrency")
		{
			_PVPQuantity = Data.GiftQuantity;
		}
		else if (Data.RewardType == "XPMaterials")
		{
			XPMaterialQuantity = Data.GiftQuantity;
			XPMaterialID = Data.GiftID;
		}
		else if (Data.RewardType == "Creature")
		{
			CreatureID = Data.GiftID;
		}
	}

	public MailItem(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public MailItem(MailType mailType)
	{
		MailType = mailType;
	}

	public static void ResetMaxUniqueId()
	{
		mMaxUniqueId = 0;
	}

	public void AssignUniqueId()
	{
		mMaxUniqueId++;
		_UniqueId = mMaxUniqueId;
	}

	public void SetTime()
	{
		TimeStamp = Singleton<PlayerInfoScript>.Instance.GetNowTime();
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		if (_ID != null)
		{
			stringBuilder.Append(PlayerInfoScript.MakeJS("ID", _ID) + ",");
		}
		stringBuilder.Append(PlayerInfoScript.MakeJS("UniqueID", _UniqueId) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("MailTitle", _MailTitle) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("MailBody", _MailBody) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("HardQuantity", _HardQuantity) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("SoftQuantity", _SoftQuantity) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("PVPQuantity", _PVPQuantity) + ",");
		if (_Repeatable != null)
		{
			stringBuilder.Append(PlayerInfoScript.MakeJS("Repeatable", _Repeatable) + ",");
		}
		stringBuilder.Append(PlayerInfoScript.MakeJS("ShowDate", _ShowDate.ToString()) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("EndDate", _EndDate.ToString()) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("MailType", _MailType.ToString()) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Opened", _Opened) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Rewarded", _Rewarded) + ",");
		if (XPMaterialID != null)
		{
			stringBuilder.Append(PlayerInfoScript.MakeJS("XPMaterialID", XPMaterialID) + ",");
			stringBuilder.Append(PlayerInfoScript.MakeJS("XPMaterialQuantity", XPMaterialQuantity) + ",");
		}
		if (CreatureID != null)
		{
			stringBuilder.Append(PlayerInfoScript.MakeJS("CreatureID", CreatureID) + ",");
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private void Deserialize(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "ID", null);
		_UniqueId = TFUtils.LoadInt(dict, "UniqueID", 0);
		_MailTitle = TFUtils.LoadString(dict, "MailTitle", string.Empty);
		_MailBody = TFUtils.LoadString(dict, "MailBody", string.Empty);
		_HardQuantity = TFUtils.LoadInt(dict, "HardQuantity", 0);
		_SoftQuantity = TFUtils.LoadInt(dict, "SoftQuantity", 0);
		_PVPQuantity = TFUtils.LoadInt(dict, "PVPQuantity", 0);
		_Repeatable = TFUtils.LoadString(dict, "Repeatable", null);
		XPMaterialID = TFUtils.LoadString(dict, "XPMaterialID", null);
		CreatureID = TFUtils.LoadString(dict, "CreatureID", null);
		XPMaterialQuantity = TFUtils.LoadInt(dict, "XPMaterialQuantity", 0);
		try
		{
			string s = TFUtils.LoadString(dict, "ShowDate", string.Empty);
			_ShowDate = DateTime.Parse(s);
			string s2 = TFUtils.LoadString(dict, "EndDate", string.Empty);
			_EndDate = DateTime.Parse(s2);
		}
		catch
		{
			_ShowDate = DateTime.MinValue;
			_EndDate = DateTime.MinValue;
		}
		_Opened = TFUtils.LoadBool(dict, "Opened", false);
		_Rewarded = TFUtils.LoadBool(dict, "Rewarded", false);
		string text = TFUtils.LoadString(dict, "MailType", string.Empty);
		_MailType = MailType.None;
		switch (text)
		{
		case "Scheduled":
			_MailType = MailType.Scheduled;
			break;
		case "AdminMessage":
			_MailType = MailType.AdminMessage;
			break;
		case "AllyInvite":
			_MailType = MailType.AllyInvite;
			break;
		}
	}

	public bool IsMailApplicable()
	{
		DateTime serverTime = TFUtils.ServerTime;
		if (!string.IsNullOrEmpty(Repeatable))
		{
			if (Repeatable == "Monday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Monday;
			}
			if (Repeatable == "Tuesday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Tuesday;
			}
			if (Repeatable == "Wednesday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Wednesday;
			}
			if (Repeatable == "Thursday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Thursday;
			}
			if (Repeatable == "Friday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Friday;
			}
			if (Repeatable == "Saturday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Saturday;
			}
			if (Repeatable == "Sunday")
			{
				return serverTime.DayOfWeek == DayOfWeek.Sunday;
			}
		}
		if (ShowDate != DateTime.MinValue && EndDate != DateTime.MinValue)
		{
			return serverTime >= ShowDate && serverTime <= EndDate;
		}
		if (ShowDate != DateTime.MinValue)
		{
			return serverTime >= ShowDate;
		}
		return true;
	}
}
