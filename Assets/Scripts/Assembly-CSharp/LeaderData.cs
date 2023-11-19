using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class LeaderData : ILoadableData
{
	public const int ACTION_CARDS_PER_LEADER = 5;

	public const int MAX_COMBO_LENGTH = 7;

	private string _ID;

	private string _Name;

	private string _Desc;

	private string _Prefab;

	private bool _UseChair;

	private float _ChairOffsetX;

	private float _ChairOffsetY;

	private float _CharacterOffsetX;

	private float _CharacterOffsetY;

	private string _Hand;

	private string _PortraitTexture;

	private string _QuestPortraitTexture;

	private string _CameraWinAnim;

	private string _PIPTransformP1;

	private string _PIPTransformP2;

	private CardData[] _ActionCards = new CardData[5];

	private CreatureFaction[] _AttackCombo = new CreatureFaction[7];

	private float _CardRemoveAnimTime;

	private float _CardPlayAnimTime;

	private float _BigCardPlayAnimTime;

	private string _FlvAge;

	private string _FlvHeight;

	private string _FlvWeight;

	private string _FlvSpecies;

	private string _FlvQuote;

	private float _HeightAdjust;

	private bool _Playable;

	private ObscuredInt _BuyCost;

	private ObscuredInt _APThreshold;

	private LeaderVFXData _LeaderVFX;

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

	public string Desc
	{
		get
		{
			return _Desc;
		}
	}

	public string Prefab
	{
		get
		{
			return _Prefab;
		}
	}

	public bool UseChair
	{
		get
		{
			return _UseChair;
		}
	}

	public float ChairOffsetX
	{
		get
		{
			return _ChairOffsetX;
		}
	}

	public float ChairOffsetY
	{
		get
		{
			return _ChairOffsetY;
		}
	}

	public float CharacterOffsetX
	{
		get
		{
			return _CharacterOffsetX;
		}
	}

	public float CharacterOffsetY
	{
		get
		{
			return _CharacterOffsetY;
		}
	}

	public string Hand
	{
		get
		{
			return _Hand;
		}
	}

	public string PortraitTexture
	{
		get
		{
			return _PortraitTexture;
		}
	}

	public string QuestPortraitTexture
	{
		get
		{
			return _QuestPortraitTexture;
		}
	}

	public string CameraWinAnim
	{
		get
		{
			return _CameraWinAnim;
		}
	}

	public string PIPTransformP1
	{
		get
		{
			return _PIPTransformP1;
		}
	}

	public string PIPTransformP2
	{
		get
		{
			return _PIPTransformP2;
		}
	}

	public float CardRemoveAnimTime
	{
		get
		{
			return _CardRemoveAnimTime;
		}
	}

	public float CardPlayAnimTime
	{
		get
		{
			return _CardPlayAnimTime;
		}
	}

	public float BigCardPlayAnimTime
	{
		get
		{
			return _BigCardPlayAnimTime;
		}
	}

	public IList<CardData> ActionCards
	{
		get
		{
			return Array.AsReadOnly(_ActionCards);
		}
	}

	public IList<CreatureFaction> AttackCombo
	{
		get
		{
			return Array.AsReadOnly(_AttackCombo);
		}
	}

	public string FlvAge
	{
		get
		{
			return _FlvAge;
		}
	}

	public string FlvHeight
	{
		get
		{
			return _FlvHeight;
		}
	}

	public string FlvWeight
	{
		get
		{
			return _FlvWeight;
		}
	}

	public string FlvSpecies
	{
		get
		{
			return _FlvSpecies;
		}
	}

	public string FlvQuote
	{
		get
		{
			return _FlvQuote;
		}
	}

	public float HeightAdjust
	{
		get
		{
			return _HeightAdjust;
		}
	}

	public bool Playable
	{
		get
		{
			return _Playable;
		}
	}

	public int BuyCost
	{
		get
		{
			return _BuyCost;
		}
	}

	public int APThreshold
	{
		get
		{
			return _APThreshold;
		}
	}

	public LeaderVFXData LeaderVFX
	{
		get
		{
			return _LeaderVFX;
		}
	}

	public List<LeaderData> AlternateSkins { get; private set; }

	public LeaderData SkinParentLeader { get; private set; }

	public bool BuyableSkin { get; private set; }

	public int SkinBuyCost { get; private set; }

	public string VO { get; private set; }

	public string VSScreenBackground { get; private set; }

	public bool AlwaysAvailable { get; private set; }

	public List<StartEndDate> AvailableDates { get; private set; }

	public float ThoughtBubbleHeightAdjust { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		AlternateSkins = new List<LeaderData>();
		_ID = TFUtils.LoadString(dict, "ID", string.Empty);
		_Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		_Desc = TFUtils.LoadLocalizedString(dict, "Desc", string.Empty);
		_Prefab = TFUtils.LoadString(dict, "Prefab", string.Empty);
		_CharacterOffsetX = TFUtils.LoadFloat(dict, "CharacterOffsetX", 0f);
		_CharacterOffsetY = TFUtils.LoadFloat(dict, "CharacterOffsetY", 0f);
		_UseChair = TFUtils.LoadBool(dict, "UseChair", true);
		_ChairOffsetX = TFUtils.LoadFloat(dict, "ChairOffsetX", 0f);
		_ChairOffsetY = TFUtils.LoadFloat(dict, "ChairOffsetY", 0f);
		_Hand = TFUtils.LoadString(dict, "Hand", string.Empty);
		_PortraitTexture = "UI/FTUEBundle/Icons_Leaders/" + TFUtils.LoadString(dict, "PortraitTexture", string.Empty);
		_QuestPortraitTexture = "UI/Icons_Leagues/" + TFUtils.LoadString(dict, "QuestPortraitTexture", string.Empty);
		_CameraWinAnim = TFUtils.LoadString(dict, "CameraWinAnim", string.Empty);
		_PIPTransformP1 = TFUtils.LoadString(dict, "PIPTransformP1", string.Empty);
		_PIPTransformP2 = TFUtils.LoadString(dict, "PIPTransformP2", string.Empty);
		_CardRemoveAnimTime = (float)TFUtils.LoadInt(dict, "CardRemoveAnimTime", 1) / 30f;
		_CardPlayAnimTime = (float)TFUtils.LoadInt(dict, "CardPlayAnimTime", 1) / 30f;
		_BigCardPlayAnimTime = (float)TFUtils.LoadInt(dict, "BigCardPlayAnimTime", 1) / 30f;
		_FlvAge = TFUtils.LoadLocalizedString(dict, "Age", string.Empty);
		_FlvHeight = TFUtils.LoadLocalizedString(dict, "Height", string.Empty);
		_FlvWeight = TFUtils.LoadLocalizedString(dict, "Weight", string.Empty);
		_FlvSpecies = TFUtils.LoadLocalizedString(dict, "Species", string.Empty);
		_FlvQuote = TFUtils.LoadLocalizedString(dict, "Quote", string.Empty);
		_HeightAdjust = TFUtils.LoadFloat(dict, "HeightAdjust", 0f);
		_Playable = TFUtils.LoadBool(dict, "Playable", false);
		_BuyCost = TFUtils.LoadInt(dict, "BuyCost", 0);
		_APThreshold = TFUtils.LoadInt(dict, "APThreshold", 100);
		BuyableSkin = TFUtils.LoadBool(dict, "BuyableSkin", false);
		SkinBuyCost = TFUtils.LoadInt(dict, "SkinBuyCost", 0);
		VO = TFUtils.LoadString(dict, "VO", null);
		VSScreenBackground = "UI/Textures/PlayerBackdrops/" + TFUtils.LoadString(dict, "VSScreenBackground", string.Empty);
		ThoughtBubbleHeightAdjust = TFUtils.LoadFloat(dict, "ThoughtBubbleHeightAdjust", 0f);
		string text = TFUtils.LoadString(dict, "LeaderVFX", null);
		if (text != null)
		{
			_LeaderVFX = LeaderVFXDataManager.Instance.GetData(text);
		}
		if (_CardPlayAnimTime < _CardRemoveAnimTime)
		{
			_CardPlayAnimTime = _CardRemoveAnimTime;
		}
		if (_BigCardPlayAnimTime < _CardPlayAnimTime)
		{
			_BigCardPlayAnimTime = _CardPlayAnimTime;
		}
		for (int i = 1; i <= 5; i++)
		{
			string iD = TFUtils.LoadString(dict, "ActionCard" + i, string.Empty);
			_ActionCards[i - 1] = CardDataManager.Instance.GetData(iD);
			if (_ActionCards[i - 1] == null)
			{
			}
		}
		for (int j = 1; j <= 7; j++)
		{
			string value = TFUtils.LoadString(dict, "Combo" + j, "Colorless");
			_AttackCombo[j - 1] = (CreatureFaction)(int)Enum.Parse(typeof(CreatureFaction), value, true);
		}
		string text2 = TFUtils.LoadString(dict, "SkinOf", null);
		if (text2 != null)
		{
			LeaderData data = LeaderDataManager.Instance.GetData(text2);
			if (data != null)
			{
				SkinParentLeader = data;
				data.AlternateSkins.Add(this);
			}
		}
		int num = 1;
		while (true)
		{
			string text3 = TFUtils.LoadString(dict, "SaleStartDate" + num, null);
			string text4 = TFUtils.LoadString(dict, "SaleEndDate" + num, null);
			if (text3 != null && text3.ToLower() == "always")
			{
				AlwaysAvailable = true;
				AvailableDates = null;
				break;
			}
			if (AvailableDates == null)
			{
				AvailableDates = new List<StartEndDate>();
			}
			if (text3 == null || text4 == null)
			{
				break;
			}
			StartEndDate startEndDate = new StartEndDate();
			startEndDate.StartDate = DateTime.Parse(text3);
			startEndDate.EndDate = DateTime.Parse(text4);
			AvailableDates.Add(startEndDate);
			num++;
		}
	}

	public void ParseKeywords()
	{
		foreach (CardData actionCard in ActionCards)
		{
			actionCard.ParseKeywords();
		}
	}

	public LeaderData GetNextPlayableLeader(bool forwards)
	{
		List<LeaderData> database = LeaderDataManager.Instance.GetDatabase();
		int num = database.IndexOf(this);
		int num2 = num;
		do
		{
			num2 += (forwards ? 1 : (-1));
			if (num2 >= database.Count)
			{
				num2 = 0;
			}
			else if (num2 < 0)
			{
				num2 = database.Count - 1;
			}
		}
		while (num2 != num && !database[num2].ShowInSaleList() && !Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(database[num2]));
		return database[num2];
	}

	public bool ShowInSaleList()
	{
		if (!Playable)
		{
			return false;
		}
		if (AlwaysAvailable)
		{
			return true;
		}
		return GetActiveAvailablePeriod() != null;
	}

	public StartEndDate GetActiveAvailablePeriod()
	{
		foreach (StartEndDate availableDate in AvailableDates)
		{
			if (availableDate.IsWithinDates())
			{
				return availableDate;
			}
		}
		return null;
	}
}
