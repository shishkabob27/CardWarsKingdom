using System;
using System.Collections.Generic;

public class MissionData : ILoadableData
{
	private string _ID;

	private string _Name;

	private string _Description;

	private string _ScriptName;

	private int _Val1;

	private int _Val2;

	private MissionType _Type;

	private bool _SingleBattle;

	private int _RankRequirement;

	private int _SoftCurrency;

	private int _HardCurrency;

	private int _SocialCurrency;

	private string mMissionPrerequisiteString;

	public string ID
	{
		get
		{
			return _ID;
		}
	}

	public string Name
	{
		get
		{
			return _Name;
		}
	}

	public string Description
	{
		get
		{
			return _Description;
		}
	}

	public string ScriptName
	{
		get
		{
			return _ScriptName;
		}
	}

	public int Val1
	{
		get
		{
			return _Val1;
		}
	}

	public int Val2
	{
		get
		{
			return _Val2;
		}
	}

	public MissionType Type
	{
		get
		{
			return _Type;
		}
	}

	public bool SingleBattle
	{
		get
		{
			return _SingleBattle;
		}
	}

	public int RankRequirement
	{
		get
		{
			return _RankRequirement;
		}
	}

	public int SoftCurrency
	{
		get
		{
			return _SoftCurrency;
		}
	}

	public int HardCurrency
	{
		get
		{
			return _HardCurrency;
		}
	}

	public int SocialCurrency
	{
		get
		{
			return _SocialCurrency;
		}
	}

	public MissionData MissionPrerequisite { get; private set; }

	public int Index { get; set; }

	public void Populate(Dictionary<string, object> dict)
	{
		_ID = TFUtils.LoadString(dict, "MissionID", string.Empty);
		_Val1 = TFUtils.LoadInt(dict, "Val1", 0);
		_Val2 = TFUtils.LoadInt(dict, "Val2", 0);
		_Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		_Description = TFUtils.LoadLocalizedString(dict, "Description", string.Empty).Replace("<val1>", _Val1.ToString()).Replace("<val2>", _Val2.ToString());
		_ScriptName = TFUtils.LoadString(dict, "Script", string.Empty);
		_Type = (MissionType)(int)Enum.Parse(typeof(MissionType), TFUtils.LoadString(dict, "Type", "None"), true);
		_SingleBattle = TFUtils.LoadBool(dict, "SingleBattle", false);
		_RankRequirement = TFUtils.LoadInt(dict, "RankRequirement", 0);
		_SoftCurrency = TFUtils.LoadInt(dict, "SoftCurrency", 0);
		_HardCurrency = TFUtils.LoadInt(dict, "HardCurrency", 0);
		_SocialCurrency = TFUtils.LoadInt(dict, "SocialCurrency", 0);
		mMissionPrerequisiteString = TFUtils.LoadString(dict, "MissionPrerequisite", null);
	}

	public void PostLoad()
	{
		if (mMissionPrerequisiteString != null)
		{
			MissionPrerequisite = MissionDataManager.Instance.GetData(mMissionPrerequisiteString);
			mMissionPrerequisiteString = null;
		}
	}

	public void GetRewardInfo(out int rewardAmount, out string rewardIcon)
	{
		if (HardCurrency > 0)
		{
			rewardAmount = HardCurrency;
			rewardIcon = "Icon_Currency_Hard";
		}
		else if (SocialCurrency > 0)
		{
			rewardAmount = SocialCurrency;
			rewardIcon = "Icon_Currency_PVPCurrency";
		}
		else
		{
			rewardAmount = SoftCurrency;
			rewardIcon = "Icon_Currency_Soft";
		}
	}
}
