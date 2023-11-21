using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using MiniJSON;

[Serializable]
public class PlayerSaveData
{
	public class ActionResult
	{
		public bool success;
	}

	public delegate void ActionCallback(ActionResult result);

	public delegate void ProceedNextStep();

	public const int PLAYERINFO_VERSION = 1;

	public const int TIER_UNDEFINED = 0;

	public const int TIER_1 = 1;

	public const int TIER_2 = 2;

	public const int TIER_3 = 3;

	public const int TIER_4 = 4;

	public const int TIER_5 = 5;

	public const int TIER_6 = 6;

	public const int TIER_S = 7;

	public const int TIER_X = 8;

	public const int TIER_FREE = 9;

	private const string UUID_KEY1 = "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5";

	private const string UUID_KEY2 = "d98fF48f_B352#4914*85F1_5e34aB7C#cf47";

	private const string UUID_KEY3 = "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499";

	private const string UUID_KEY4 = "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE";

	public const int DEFAULT1 = 41592;

	public const int DEFAULT2 = 65358;

	public const int DEFAULT3 = 97932;

	public const int DEFAULT4 = 38462;

	private const string FRDPAS_KEY = "ffbdh41e9_54fabaeb7ea530016faxg4aimc";

	private const string FRDPAR_KEY = "5424493204pemhi3148ifmanseu4iksdf4_4";

	private const string FRDPCS_KEY = "ffbd#41e5_94fabbeb7ea530916fax4hainc";

	private const string FRDPCR_KEY = "5bill6535897assa860955058223172534_4";

	public int version;

	public string PlayerName;

	public bool HasAuthenticated;

	public int PlayersLastSavedLevel;

	public ObscuredString MultiplayerPlayerName = string.Empty;

	public int SelectedLoadout;

	public int MyHelperCreatureID = -1;

	public Dictionary<string, object> Unlocks = new Dictionary<string, object>();

	public List<LeaderItem> Leaders = new List<LeaderItem>();

	public List<Loadout> Loadouts = new List<Loadout>();

	public uint StaminaFullAtTime;

	public uint PvpStaminaFullAtTime;

	public ObscuredUInt ExtraQuestStamina;

	public ObscuredUInt ExtraPvpStamina;

	public ObscuredInt InventorySpace;

	public ObscuredInt AllyBoxSpace;

	public bool NotificationEnabled = true;

	public int TopCompletedQuestId;

	public int TopShownCompletedQuestId;

	public List<int> EarnedQuestStars = new List<int>();

	public Dictionary<string, int> EarnedDungeonStars = new Dictionary<string, int>();

	public int PvpBattles;

	public List<SortEntry> CreatureSorts = new List<SortEntry>();

	public List<SortEntry> ExCardSorts = new List<SortEntry>();

	public List<SortEntry> EvoMaterialSorts = new List<SortEntry>();

	public List<SortEntry> HelperSorts = new List<SortEntry>();

	public float QuestSelectScrollPos;

	public Dictionary<string, SpecialQuestStatus> DoneSpecialQuests = new Dictionary<string, SpecialQuestStatus>();

	public int LastAccessUTC;

	public string LastKnownLocation = string.Empty;

	public bool InviteCodeRedeemed;

	public Dictionary<string, object> IgnoredPlayers = new Dictionary<string, object>();

	public Dictionary<SpecialSaleData, SpecialSaleItem> Sales = new Dictionary<SpecialSaleData, SpecialSaleItem>();

	public SpecialSaleData SelectedSale;

	public uint SaleShowCooldown;

	public uint SaleEndTime;

	public uint SaleRestartCooldown;

	public bool UIDFixApplied = true;

	public ObscuredInt ExpeditionSlots;

	private ObscuredInt _MultiplayerLevel;

	public ObscuredInt BestMultiplayerLevel;

	public ObscuredInt PointsInMultiplayerLevel;

	public ObscuredInt MultiplayerWinStreak;

	public PvpSeasonData ActivePvpSeason;

	public ObscuredInt PvpRankRewardsGranted = -1;

	public bool PlayedFirstBattleInPvpSeason;

	public bool RankedPvpMatchStarted;

	public List<CardBackData> UnlockedCardBacks = new List<CardBackData>();

	public List<PlayerTitleData> UnlockedTitles = new List<PlayerTitleData>();

	public List<PlayerBadgeData> UnlockedBadges = new List<PlayerBadgeData>();

	public List<PlayerPortraitData> UnlockedPortraits = new List<PlayerPortraitData>();

	public Dictionary<GachaSlotData, uint> GachaSlotCooldowns = new Dictionary<GachaSlotData, uint>();

	public PlayerPortraitData SelectedPortrait;

	public CardBackData SelectedCardBack;

	public Dictionary<string, ObscuredInt> DungeonMaps = new Dictionary<string, ObscuredInt>();

	public bool TownTiltCam;

	public List<BattleHistory> BattleHistoryList = new List<BattleHistory>();

	public int PvpSpecialDomainNumber;

	public int ChatSpecialDomainNumber;

	public int InstalledDate;

	public ObscuredInt RandomDungeonLevel;

	public uint DateOfBirth;

	public int ConfirmedTOSVersion;

	public LanguageCode selectedLang;

	public bool RankUpInventoryGiven = true;

	public uint ExpeditionRefreshTime;

	public Dictionary<SpeedUpData, int> SpeedUps = new Dictionary<SpeedUpData, int>();

	public Dictionary<GachaSlotData, int> GachaKeys = new Dictionary<GachaSlotData, int>();

	private ObscuredInt _SoftCurrency;

	public ObscuredInt CustomizationCurrency;

	private ObscuredInt __PaidHardCurrency;

	private ObscuredInt __FreeHardCurrency;

	private int _Yuryo;

	private int _Harai;

	private int _Tada;

	private int _Muryo;

	private int _ReadWriteParam = 1981;

	private ObscuredInt _PvPCurrency;

	public CalendarTable ActiveCalendar;

	public DateTime LastOneTimeCalendarDateClaimed;

	public ObscuredInt OneTimeCalendarDaysClaimed = 0;

	public DateTime DailyMissionTimestamp;

	public int FriendInvites;

	public List<string> FBInviteRewards = new List<string>();

	public List<string> DeletedMail = new List<string>();

	private ObscuredInt _AllyBoxSpace;

	private List<InventorySlotItem> _InventorySlots = new List<InventorySlotItem>();

	private int mCreatureCount;

	private int mExCardCount;

	private int mEvoMaterialCount;

	private int mXPMaterialCount;

	private ObscuredInt _RankXP;

	private int mMailCount;

	private List<MailItem> _Mails = new List<MailItem>();

	public int MultiplayerLevel
	{
		get
		{
			return _MultiplayerLevel;
		}
		set
		{
			_MultiplayerLevel = value;
			if ((int)BestMultiplayerLevel == -1 || (int)_MultiplayerLevel < (int)BestMultiplayerLevel)
			{
				BestMultiplayerLevel = _MultiplayerLevel;
			}
		}
	}

	public int SoftCurrency
	{
		get
		{
			return _SoftCurrency;
		}
		set
		{
			int num = value - (int)_SoftCurrency;
			if (num < 0)
			{
				DetachedSingleton<MissionManager>.Instance.OnCoinsSpent(num);
			}
			_SoftCurrency = value;
		}
	}

	private ObscuredInt _PaidHardCurrency
	{
		get
		{
			return __PaidHardCurrency;
		}
		set
		{
			__PaidHardCurrency = value;
			Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Gacha);
		}
	}

	private ObscuredInt _FreeHardCurrency
	{
		get
		{
			return __FreeHardCurrency;
		}
		set
		{
			__FreeHardCurrency = value;
			Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Gacha);
		}
	}

	public int HardCurrency
	{
		get
		{
			return (int)_PaidHardCurrency + (int)_FreeHardCurrency;
		}
	}

	public int PaidHardCurrency
	{
		get
		{
			return _PaidHardCurrency;
		}
	}

	public int FreeHardCurrency
	{
		get
		{
			return _FreeHardCurrency;
		}
	}

	public int Yuryo
	{
		get
		{
			return _Yuryo;
		}
	}

	public int Harai
	{
		get
		{
			return _Harai;
		}
	}

	public int Tada
	{
		get
		{
			return _Tada;
		}
	}

	public int Muryo
	{
		get
		{
			return _Muryo;
		}
	}

	public int ReadWriteParam
	{
		get
		{
			return _ReadWriteParam;
		}
		set
		{
			_ReadWriteParam = value;
		}
	}

	public int PvPCurrency
	{
		get
		{
			return _PvPCurrency;
		}
		set
		{
			int num = value - (int)_PvPCurrency;
			if (num < 0)
			{
				DetachedSingleton<MissionManager>.Instance.OnTeethSpent(num);
			}
			_PvPCurrency = value;
			Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Gacha);
		}
	}

	public ReadOnlyCollection<InventorySlotItem> InventorySlots
	{
		get
		{
			return _InventorySlots.AsReadOnly();
		}
	}

	public int CreatureCount
	{
		get
		{
			return mCreatureCount;
		}
	}

	public int ExCardCount
	{
		get
		{
			return mExCardCount;
		}
	}

	public int EvoMaterialCount
	{
		get
		{
			return mEvoMaterialCount;
		}
	}

	public int XPMaterialCount
	{
		get
		{
			return mXPMaterialCount;
		}
	}

	public int FilledInventoryCount
	{
		get
		{
			return mCreatureCount + mExCardCount + mEvoMaterialCount + mXPMaterialCount;
		}
	}

	public int RankXP
	{
		get
		{
			return _RankXP;
		}
		set
		{
			_RankXP = value;
			Singleton<PlayerInfoScript>.Instance.RefreshRankXpData();
		}
	}

	public int MailCount
	{
		get
		{
			return mMailCount;
		}
	}

	public ReadOnlyCollection<MailItem> Mails
	{
		get
		{
			return _Mails.AsReadOnly();
		}
	}

	public void ModifyPaidHardCurrency(int value, bool absolute = false)
	{
		int hashSHORT = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
		if ((!absolute && hashSHORT != _Yuryo) || (int)_PaidHardCurrency > MiscParams.PvPHackThreshold)
		{
			if (MiscParams.HarderHashCheck)
			{
				PvpSpecialDomainNumber = 1;
				_PaidHardCurrency = 0;
				_FreeHardCurrency = GachaSlotDataManager.PremiumGachaCost;
			}
			_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
			_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
		}
		else if (absolute && value > MiscParams.PvPHackThreshold)
		{
			if (MiscParams.HarderHashCheck)
			{
				PvpSpecialDomainNumber = 1;
				_PaidHardCurrency = 0;
				_FreeHardCurrency = GachaSlotDataManager.PremiumGachaCost;
			}
			_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
			_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
		}
		else if (absolute)
		{
			_PaidHardCurrency = value;
		}
		else
		{
			_PaidHardCurrency = (int)_PaidHardCurrency + value;
		}
		_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
		_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
	}

	public void ModifyFreeHardCurrency(int value, bool absolute = false)
	{
		int hashSHORT = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
		if ((!absolute && hashSHORT != _Tada) || (int)_FreeHardCurrency > MiscParams.PvPHackThreshold)
		{
			if (MiscParams.HarderHashCheck)
			{
				PvpSpecialDomainNumber = 1;
				_PaidHardCurrency = 0;
				_FreeHardCurrency = GachaSlotDataManager.PremiumGachaCost;
			}
			_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
			_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
		}
		else if ((int)_FreeHardCurrency > MiscParams.PvPHackThreshold)
		{
			if (MiscParams.HarderHashCheck)
			{
				PvpSpecialDomainNumber = 1;
				_PaidHardCurrency = 0;
				_FreeHardCurrency = GachaSlotDataManager.PremiumGachaCost;
			}
			_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
			_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
		}
		else if (absolute)
		{
			_FreeHardCurrency = value;
		}
		else
		{
			_FreeHardCurrency = (int)_FreeHardCurrency + value;
		}
		_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
		_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
	}

	public void AddHardCurrency(int paid, int free, string eventName, string countrycode = null, string price = null)
	{
		Session theSession = SessionManager.Instance.theSession;
		ModifyPaidHardCurrency(paid);
		ModifyFreeHardCurrency(free);
		if (paid > 0)
		{
			LogUserTransactionServerHistory(tier: eventName.Contains("package1") ? 1 : (eventName.Contains("package2") ? 2 : (eventName.Contains("package3") ? 3 : (eventName.Contains("package4") ? 4 : (eventName.Contains("package5") ? 5 : (eventName.Contains("package6") ? 6 : ((!eventName.Contains("Sale")) ? 8 : 7)))))), session: theSession, country: Singleton<TBPvPManager>.Instance.CountryCode, transaction: paid);
		}
		if (free > 0)
		{
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, free, 9);
		}
	}

	public void ConsumeHardCurrency(int amount, string eventName)
	{
		Session theSession = SessionManager.Instance.theSession;
		int num = 0;
		int num2 = 0;
		if (amount <= (int)_PaidHardCurrency)
		{
			num = amount;
			ModifyPaidHardCurrency(-amount);
		}
		else
		{
			num = _PaidHardCurrency;
			amount -= (int)_PaidHardCurrency;
			ModifyPaidHardCurrency(0, true);
			num2 = amount;
			ModifyFreeHardCurrency(-amount);
		}
		if (num > 0)
		{
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, -num, 8);
		}
		if (num2 > 0)
		{
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, -num2, 9);
		}
	}

	public void ManualSetHardCurrency(int paid, int free)
	{
		ModifyPaidHardCurrency(paid, true);
		ModifyFreeHardCurrency(free, true);
	}

	public void ManualSetCurrencyHash(int paid, int free)
	{
		if (paid == 41592 && free == 65358)
		{
			_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
			_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
			return;
		}
		if ((int)_PaidHardCurrency != 0)
		{
			int hashSHORT = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			if (hashSHORT != paid)
			{
				if (MiscParams.HarderHashCheck)
				{
					PvpSpecialDomainNumber = 1;
					_PaidHardCurrency = 0;
					_FreeHardCurrency = GachaSlotDataManager.PremiumGachaCost;
				}
				_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
				_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
				_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
				_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
			}
			else
			{
				_Yuryo = paid;
				_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
			}
		}
		else
		{
			_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
			_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
		}
		if ((int)_FreeHardCurrency != GachaSlotDataManager.PremiumGachaCost)
		{
			int hashSHORT2 = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			if (hashSHORT2 != free)
			{
				if (MiscParams.HarderHashCheck)
				{
					PvpSpecialDomainNumber = 1;
					_PaidHardCurrency = 0;
					_FreeHardCurrency = GachaSlotDataManager.PremiumGachaCost;
				}
				_Yuryo = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "DbbDC9d5#5087*4B32_83Ed?bD772a71766C5");
				_Harai = GetHashSHORT(Convert.ToString(_PaidHardCurrency), "B9fdD61a*71C5_4Ae7#83e8#B63bE6a9F0499");
				_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
				_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
			}
			else
			{
				_Tada = free;
				_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
			}
		}
		else
		{
			_Tada = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "d98fF48f_B352#4914*85F1_5e34aB7C#cf47");
			_Muryo = GetHashSHORT(Convert.ToString(_FreeHardCurrency), "2dE60E43#3767a436f*89DD_A9Cd359#aE3eE");
		}
	}

	public void AddEmptyAllySlots(int count)
	{
		if (count > 0)
		{
			AllyBoxSpace = (int)AllyBoxSpace + count;
			RewardManager.UpdatePvPInfo();
		}
	}

	public void AddEmptyInventorySlots(int count)
	{
		if (count > 0)
		{
			InventorySpace = (int)InventorySpace + count;
			CheckEmptyInventorySlots();
		}
	}

	private void CheckEmptyInventorySlots()
	{
		if (_InventorySlots.Count > 0)
		{
			_InventorySlots.RemoveAt(_InventorySlots.Count - 1);
		}
		while (_InventorySlots.Count < (int)InventorySpace)
		{
			_InventorySlots.Add(new InventorySlotItem(InventorySlotType.Empty));
		}
		_InventorySlots.Add(new InventorySlotItem(InventorySlotType.Purchase));
	}

	public bool IsInventorySpaceFull()
	{
		return FilledInventoryCount >= (int)InventorySpace;
	}

	public void RemoveInventoryItems(List<InventorySlotItem> itemsToRemove)
	{
		foreach (InventorySlotItem item in itemsToRemove)
		{
			if (item.SlotType == InventorySlotType.Creature)
			{
				item.Creature.RemoveExCards();
			}
		}
		int num = _InventorySlots.RemoveAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Creature && itemsToRemove.Contains(m));
		int num2 = _InventorySlots.RemoveAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Card && itemsToRemove.Contains(m));
		int num3 = _InventorySlots.RemoveAll(delegate(InventorySlotItem m)
		{
			if (m.SlotType != InventorySlotType.EvoMaterial)
			{
				return false;
			}
			if (itemsToRemove.Contains(m))
			{
				itemsToRemove.Remove(m);
				return true;
			}
			return false;
		});
		int num4 = _InventorySlots.RemoveAll(delegate(InventorySlotItem m)
		{
			if (m.SlotType != InventorySlotType.XPMaterial)
			{
				return false;
			}
			if (itemsToRemove.Contains(m))
			{
				itemsToRemove.Remove(m);
				return true;
			}
			return false;
		});
		int num5 = num + num2 + num3 + num4;
		if (num5 < itemsToRemove.Count)
		{
		}
		mCreatureCount -= num;
		mExCardCount -= num2;
		mEvoMaterialCount -= num3;
		mXPMaterialCount -= num4;
		CheckEmptyInventorySlots();
	}

	public void ClearInventory()
	{
		_InventorySlots.Clear();
		mCreatureCount = 0;
		mExCardCount = 0;
		mEvoMaterialCount = 0;
		mXPMaterialCount = 0;
		CheckEmptyInventorySlots();
	}

	public InventorySlotItem AddCreature(CreatureItem creature)
	{
		if (creature.Form == null)
		{
			return null;
		}
		if (creature.UniqueId == 0)
		{
			creature.AssignUniqueId();
		}
		InventorySlotItem inventorySlotItem = _InventorySlots[FilledInventoryCount];
		bool flag = inventorySlotItem.SlotType == InventorySlotType.Purchase;
		inventorySlotItem.Fill(creature);
		mCreatureCount++;
		inventorySlotItem.FirstTimeCollected = !creature.Form.AlreadyCollected;
		creature.Form.AlreadySeen = true;
		creature.Form.AlreadyCollected = true;
		if (flag)
		{
			_InventorySlots.Add(new InventorySlotItem(InventorySlotType.Purchase));
		}
		return inventorySlotItem;
	}

	public void RemoveCreature(InventorySlotItem creature)
	{
		List<InventorySlotItem> list = new List<InventorySlotItem>();
		list.Add(creature);
		RemoveCreatures(list);
	}

	public void RemoveCreatures(List<InventorySlotItem> creaturesToRemove)
	{
		bool flag = creaturesToRemove.Find((InventorySlotItem m) => m.Creature.UniqueId == MyHelperCreatureID) != null;
		foreach (InventorySlotItem item in creaturesToRemove)
		{
			item.Creature.RemoveExCards();
		}
		int num = _InventorySlots.RemoveAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Creature && creaturesToRemove.Contains(m));
		if (num < creaturesToRemove.Count)
		{
		}
		mCreatureCount -= num;
		CheckEmptyInventorySlots();
		if (flag)
		{
			Singleton<PlayerInfoScript>.Instance.SetDefaultHelperCreature();
		}
	}

	public void RemoveAllCreatures()
	{
		_InventorySlots.RemoveAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Creature);
		mCreatureCount = 0;
		CheckEmptyInventorySlots();
	}

	public InventorySlotItem FindCreature(Predicate<InventorySlotItem> match)
	{
		return _InventorySlots.Find((InventorySlotItem m) => m.SlotType == InventorySlotType.Creature && match(m));
	}

	public List<InventorySlotItem> FindAllCreatures(Predicate<InventorySlotItem> match)
	{
		return _InventorySlots.FindAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Creature && match(m));
	}

	public InventorySlotItem GetCreatureItem(int uniqueId)
	{
		return _InventorySlots.Find((InventorySlotItem m) => m.SlotType == InventorySlotType.Creature && m.Creature.UniqueId == uniqueId);
	}

	public InventorySlotItem GetBestCreature(bool notMaxStars = false)
	{
		InventorySlotItem inventorySlotItem = null;
		foreach (InventorySlotItem inventorySlot in _InventorySlots)
		{
			if (inventorySlot.SlotType == InventorySlotType.Creature && (!notMaxStars || (inventorySlot.Creature.StarRating < CreatureStarRatingDataManager.Instance.MaxStarRating() && !DetachedSingleton<ExpeditionManager>.Instance.IsCreatureOnExpedition(inventorySlot.Creature))))
			{
				if (inventorySlotItem == null)
				{
					inventorySlotItem = inventorySlot;
				}
				else if (inventorySlot.Creature.StarRating > inventorySlotItem.Creature.StarRating)
				{
					inventorySlotItem = inventorySlot;
				}
				else if (inventorySlot.Creature.StarRating == inventorySlotItem.Creature.StarRating && inventorySlot.Creature.Level > inventorySlotItem.Creature.Level)
				{
					inventorySlotItem = inventorySlot;
				}
			}
		}
		return inventorySlotItem;
	}

	public InventorySlotItem GetNewestCreature()
	{
		InventorySlotItem inventorySlotItem = null;
		foreach (InventorySlotItem inventorySlot in _InventorySlots)
		{
			if (inventorySlot.SlotType == InventorySlotType.Creature && (inventorySlotItem == null || inventorySlot.Creature.UniqueId > inventorySlotItem.Creature.UniqueId))
			{
				inventorySlotItem = inventorySlot;
			}
		}
		return inventorySlotItem;
	}

	public void FixDuplicateUniqueIDs()
	{
		List<SortEntry> list = new List<SortEntry>();
		list.Add(new SortEntry(SortTypeEnum.Newest, true));
		InventoryComparer comparer = new InventoryComparer(InventorySlotType.Creature, list, ExCardSorts, false);
		_InventorySlots.Sort(comparer);
		CreatureItem.ResetMaxUniqueId();
		for (int i = 0; i < _InventorySlots.Count; i++)
		{
			InventorySlotItem inventorySlotItem = _InventorySlots[i];
			if (inventorySlotItem.SlotType != InventorySlotType.Creature)
			{
				continue;
			}
			int oldUniqueID = inventorySlotItem.Creature.UniqueId;
			inventorySlotItem.Creature.AssignUniqueId();
			int uniqueId = inventorySlotItem.Creature.UniqueId;
			List<InventorySlotItem> list2 = _InventorySlots.FindAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Card && m.Card.CreatureUID == oldUniqueID);
			foreach (InventorySlotItem item in list2)
			{
				item.Card.CreatureUID = uniqueId;
			}
			foreach (ExpeditionItem currentExpedition in DetachedSingleton<ExpeditionManager>.Instance.CurrentExpeditions)
			{
				for (int j = 0; j < currentExpedition.UsedCreatureUIDs.Count; j++)
				{
					if (currentExpedition.UsedCreatureUIDs[j] == oldUniqueID)
					{
						currentExpedition.UsedCreatureUIDs[j] = uniqueId;
					}
				}
			}
			if (MyHelperCreatureID == oldUniqueID)
			{
				MyHelperCreatureID = uniqueId;
			}
		}
	}

	public void GiveRetroactiveRankInventory()
	{
		int num = 0;
		for (int i = 1; i <= PlayersLastSavedLevel; i++)
		{
			PlayerRankData data = PlayerRankDataManager.Instance.GetData(i - 1);
			num += data.InventorySpace;
		}
		if (num > 0)
		{
			AddEmptyInventorySlots(num);
		}
	}

	public InventorySlotItem AddExCard(CardItem card)
	{
		if (card.UniqueId == 0)
		{
			card.AssignUniqueId();
		}
		InventorySlotItem inventorySlotItem = _InventorySlots[FilledInventoryCount];
		bool flag = inventorySlotItem.SlotType == InventorySlotType.Purchase;
		inventorySlotItem.Fill(card);
		mExCardCount++;
		if (flag)
		{
			_InventorySlots.Add(new InventorySlotItem(InventorySlotType.Purchase));
		}
		inventorySlotItem.FirstTimeCollected = !card.Form.AlreadyCollected;
		card.Form.AlreadySeen = true;
		card.Form.AlreadyCollected = true;
		return inventorySlotItem;
	}

	public InventorySlotItem FindExCard(Predicate<CardItem> match)
	{
		return _InventorySlots.Find((InventorySlotItem m) => m.SlotType == InventorySlotType.Card && match(m.Card));
	}

	public List<InventorySlotItem> FindAllExCards(Predicate<CardItem> match)
	{
		return _InventorySlots.FindAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Card && match(m.Card));
	}

	public void RemoveExCards(List<InventorySlotItem> cardsToRemove)
	{
		int num = _InventorySlots.RemoveAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Card && cardsToRemove.Contains(m));
		if (num < cardsToRemove.Count)
		{
		}
		mExCardCount -= num;
		CheckEmptyInventorySlots();
	}

	public void RemoveAllExCards()
	{
		_InventorySlots.RemoveAll((InventorySlotItem m) => m.SlotType == InventorySlotType.Card);
		mExCardCount = 0;
		CheckEmptyInventorySlots();
	}

	public InventorySlotItem AddEvoMaterial(EvoMaterialData evoMaterial)
	{
		InventorySlotItem inventorySlotItem = _InventorySlots[FilledInventoryCount];
		bool flag = inventorySlotItem.SlotType == InventorySlotType.Purchase;
		inventorySlotItem.Fill(evoMaterial);
		mEvoMaterialCount++;
		if (flag)
		{
			_InventorySlots.Add(new InventorySlotItem(InventorySlotType.Purchase));
		}
		inventorySlotItem.FirstTimeCollected = !evoMaterial.AlreadyCollected;
		evoMaterial.AlreadySeen = true;
		evoMaterial.AlreadyCollected = true;
		return inventorySlotItem;
	}

	public InventorySlotItem FindEvoMaterial(Predicate<InventorySlotItem> match)
	{
		return _InventorySlots.Find((InventorySlotItem m) => m.SlotType == InventorySlotType.EvoMaterial && match(m));
	}

	public List<InventorySlotItem> FindAllEvoMaterials(Predicate<InventorySlotItem> match)
	{
		return _InventorySlots.FindAll((InventorySlotItem m) => m.SlotType == InventorySlotType.EvoMaterial && match(m));
	}

	public void RemoveEvoMaterials(List<EvoMaterialData> evoMatsToRemove)
	{
		int num = _InventorySlots.RemoveAll(delegate(InventorySlotItem slot)
		{
			if (slot.SlotType != InventorySlotType.EvoMaterial)
			{
				return false;
			}
			if (evoMatsToRemove.Contains(slot.EvoMaterial))
			{
				evoMatsToRemove.Remove(slot.EvoMaterial);
				return true;
			}
			return false;
		});
		if (num < evoMatsToRemove.Count)
		{
		}
		mEvoMaterialCount -= num;
		CheckEmptyInventorySlots();
	}

	public InventorySlotItem AddXPMaterial(XPMaterialData xpMaterial)
	{
		InventorySlotItem inventorySlotItem = _InventorySlots[FilledInventoryCount];
		bool flag = inventorySlotItem.SlotType == InventorySlotType.Purchase;
		inventorySlotItem.Fill(xpMaterial);
		mXPMaterialCount++;
		if (flag)
		{
			_InventorySlots.Add(new InventorySlotItem(InventorySlotType.Purchase));
		}
		inventorySlotItem.FirstTimeCollected = !xpMaterial.AlreadyCollected;
		xpMaterial.AlreadySeen = true;
		xpMaterial.AlreadyCollected = true;
		return inventorySlotItem;
	}

	public void RemoveXPMaterials(List<InventorySlotItem> xpMatsToRemove)
	{
		int num = _InventorySlots.RemoveAll(delegate(InventorySlotItem slot)
		{
			if (slot.SlotType != InventorySlotType.XPMaterial)
			{
				return false;
			}
			if (xpMatsToRemove.Contains(slot))
			{
				xpMatsToRemove.Remove(slot);
				return true;
			}
			return false;
		});
		if (num < xpMatsToRemove.Count)
		{
		}
		mXPMaterialCount -= num;
		CheckEmptyInventorySlots();
	}

	public List<InventorySlotItem> GetAllXPMaterials()
	{
		return _InventorySlots.FindAll((InventorySlotItem m) => m.SlotType == InventorySlotType.XPMaterial);
	}

	public void SortInventory(InventorySlotType primaryType, bool evoableCreaturesFirst = false)
	{
		InventoryComparer comparer = new InventoryComparer(primaryType, CreatureSorts, ExCardSorts, evoableCreaturesFirst);
		_InventorySlots.Sort(comparer);
	}

	public void DeleteMail(string id)
	{
		if (!DeletedMail.Contains(id))
		{
			DeletedMail.Add(id);
		}
	}

	public bool HasDeletedMail(string id)
	{
		return DeletedMail.Contains(id);
	}

	public void GiveFBInviteReward(string rewardID)
	{
		if (!FBInviteRewards.Contains(rewardID))
		{
			FBInviteRewards.Add(rewardID);
		}
	}

	public bool IsFBRewardGiven(string rewardID)
	{
		return FBInviteRewards.Contains(rewardID);
	}

	public void ResetFBInviteReward()
	{
		FBInviteRewards.Clear();
	}

	public void AddMail(MailItem mail)
	{
		if (mail.UniqueId == 0)
		{
			mail.AssignUniqueId();
		}
		_Mails.Add(mail);
		mMailCount++;
	}

	public void RemoveMail(MailItem mail)
	{
		List<MailItem> list = new List<MailItem>();
		list.Add(mail);
		RemoveMails(list);
	}

	public void RemoveMail(int n)
	{
		_Mails.RemoveAt(n);
	}

	public void RemoveMails(List<MailItem> mails)
	{
		int num = 0;
		foreach (MailItem mail in mails)
		{
			if (_Mails.Remove(mail))
			{
				num++;
			}
		}
		mMailCount -= num;
	}

	public void RemoveMails(Predicate<MailItem> match)
	{
		_Mails.RemoveAll(match);
	}

	public void RemoveAllMails()
	{
		mMailCount = 0;
		_Mails.Clear();
	}

	public MailItem FindMail(Predicate<MailItem> match)
	{
		return _Mails.Find(match);
	}

	public List<MailItem> FindAllMails(Predicate<MailItem> match)
	{
		return _Mails.FindAll((MailItem finalMatch) => match(finalMatch));
	}

	public void AddDungeonMap(string questID, int quantity)
	{
		ObscuredInt value;
		if (DungeonMaps.TryGetValue(questID, out value))
		{
			DungeonMaps[questID] = (int)value + quantity;
		}
		else
		{
			DungeonMaps[questID] = quantity;
		}
	}

	public void ConsumeDungeonMap(string questID)
	{
		ObscuredInt value;
		if (DungeonMaps.TryGetValue(questID, out value))
		{
			if ((int)value == 1)
			{
				DungeonMaps.Remove(questID);
			}
			else
			{
				DungeonMaps[questID] = (int)value - 1;
			}
		}
	}

	public int GetHashSHORT(string sourcevalue, string key)
	{
		//Discarded unreachable code: IL_0071
		using (HMACSHA1 hMACSHA = new HMACSHA1())
		{
			hMACSHA.Key = Encoding.UTF8.GetBytes(key);
			byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(sourcevalue));
			StringBuilder stringBuilder = new StringBuilder();
			int num = 3141;
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.AppendFormat("{0:X2}", array[i]);
				num += array[i];
			}
			return num;
		}
	}

	public string GetHashSTRING(string sourcevalue, string key)
	{
		//Discarded unreachable code: IL_0064
		using (HMACSHA256 hMACSHA = new HMACSHA256())
		{
			hMACSHA.Key = Encoding.UTF8.GetBytes(key);
			byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(sourcevalue));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.AppendFormat("{0:X2}", array[i]);
			}
			return stringBuilder.ToString();
		}
	}

	public string GetSHA256STRINGASCII(string sourcevalue)
	{
		SHA256 sHA = SHA256.Create();
		byte[] array = sHA.ComputeHash(Encoding.ASCII.GetBytes(sourcevalue));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.AppendFormat("{0:X2}", array[i]);
		}
		return stringBuilder.ToString();
	}

	public int GetHashLONG(string sourcevalue, string key)
	{
		//Discarded unreachable code: IL_0071
		using (HMACSHA256 hMACSHA = new HMACSHA256())
		{
			hMACSHA.Key = Encoding.UTF8.GetBytes(key);
			byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(sourcevalue));
			StringBuilder stringBuilder = new StringBuilder();
			int num = 3141;
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.AppendFormat("{0:X2}", array[i]);
				num += array[i];
			}
			return num;
		}
	}

	public void SetInstalledDate(uint utctick)
	{
		InstalledDate = (int)utctick;
	}

	public void LogUserTransactionServerHistory(Session session, string country, int transaction, int tier)
	{
		if (!MiscParams.UseGameServerStats)
		{
			return;
		}
		TFServer.JsonResponseHandler callback = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && !((string)data["success"] == "True"))
			{
			}
		};
		string country2 = country;
		if ((int)_PaidHardCurrency > MiscParams.PvPHackThreshold || (int)_FreeHardCurrency > MiscParams.PvPHackThreshold)
		{
			country2 = "ZE";
		}
		session.Server.User_currency_history(country2, transaction, tier, _PaidHardCurrency, _FreeHardCurrency, callback);
	}

	public void DoNotCall(Session session)
	{
		if (!MiscParams.UseGameServerStats)
		{
			return;
		}
		TFServer.JsonResponseHandler callback = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && !(bool)data["success"])
			{
			}
		};
		session.Server.User_currency_history2(10, callback);
	}

	public void AddHardCurrency2(int paid, int free, string eventName, int handle, string countrycode, string price, ActionCallback callback)
	{
		string hashSTRING = GetHashSTRING(SessionManager.Instance.PlayerID, "ffbdh41e9_54fabaeb7ea530016faxg4aimc" + SessionManager.Instance.PlayerID + _PaidHardCurrency.ToString() + _FreeHardCurrency.ToString());
		Session theSession = SessionManager.Instance.theSession;
		string empty = string.Empty;
		if (paid > 0)
		{
			int num = 0;
			if (eventName.Contains("package1"))
			{
				num = 1;
				empty = "TIER_1";
			}
			else if (eventName.Contains("package2"))
			{
				num = 2;
				empty = "TIER_2";
			}
			else if (eventName.Contains("package3"))
			{
				num = 3;
				empty = "TIER_3";
			}
			else if (eventName.Contains("package4"))
			{
				num = 4;
				empty = "TIER_4";
			}
			else if (eventName.Contains("package5"))
			{
				num = 5;
				empty = "TIER_5";
			}
			else if (eventName.Contains("package6"))
			{
				num = 6;
				empty = "TIER_6";
			}
			else if (eventName.Contains("Sale"))
			{
				num = 7;
				empty = "TIER_S";
			}
			else
			{
				num = 8;
				empty = "TIER_X";
			}
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, paid, num);
		}
		if (free > 0)
		{
			empty = eventName;
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, free, 9);
		}
		TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			ActionResult actionResult = new ActionResult();
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
			{
				Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Gacha);
				if (data.ContainsKey("data"))
				{
					string text = (string)data["data"];
					string json = text.Replace("[", string.Empty).Replace("]", string.Empty);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
					if (dictionary.Count > 0 && dictionary.ContainsKey("fields"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["fields"];
						string key = "5424493204pemhi3148ifmanseu4iksdf4_4" + SessionManager.Instance.PlayerID + "0";
						string hashSTRING2 = GetHashSTRING(SessionManager.Instance.PlayerID, key);
						string text2 = Convert.ToString(dictionary2["handle"]);
						if (!Singleton<PurchaseManager>.Instance.IsAmazon && hashSTRING2.ToLower() != text2.ToLower())
						{
							actionResult.success = false;
						}
						else
						{
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level1"))
							{
								int num2 = Convert.ToInt32(dictionary2["level1"]);
								_PaidHardCurrency = num2;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level2"))
							{
								int num3 = Convert.ToInt32(dictionary2["level2"]);
								_FreeHardCurrency = num3;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level3"))
							{
								int num4 = Convert.ToInt32(dictionary2["level3"]);
								CustomizationCurrency = num4;
							}
						}
					}
				}
				actionResult.success = true;
				callback(actionResult);
			}
			else
			{
				actionResult.success = false;
				callback(actionResult);
			}
		};
		int misc = 0;
		if (Singleton<PurchaseManager>.Instance.IsAmazon)
		{
			misc = 31415;
		}
		theSession.Server.User_action(_PaidHardCurrency, _FreeHardCurrency, CustomizationCurrency, paid, free, 0, hashSTRING, handle, misc, eventName, Singleton<TBPvPManager>.Instance.CountryCode, callback2);
	}

	public void ConsumeHardCurrency2(int amount, string eventName, ActionCallback callback)
	{
		string hashSTRING = GetHashSTRING(SessionManager.Instance.PlayerID, "ffbdh41e9_54fabaeb7ea530016faxg4aimc" + SessionManager.Instance.PlayerID + _PaidHardCurrency.ToString() + _FreeHardCurrency.ToString());
		TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			ActionResult actionResult = new ActionResult();
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
			{
				Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Gacha);
				if (data.ContainsKey("data"))
				{
					string text = (string)data["data"];
					string json = text.Replace("[", string.Empty).Replace("]", string.Empty);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
					if (dictionary.Count > 0 && dictionary.ContainsKey("fields"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["fields"];
						string key = "5424493204pemhi3148ifmanseu4iksdf4_4" + SessionManager.Instance.PlayerID + "0";
						string hashSTRING2 = GetHashSTRING(SessionManager.Instance.PlayerID, key);
						string text2 = Convert.ToString(dictionary2["handle"]);
						if (hashSTRING2.ToLower() != text2.ToLower())
						{
							actionResult.success = false;
						}
						else
						{
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level1"))
							{
								int num3 = Convert.ToInt32(dictionary2["level1"]);
								_PaidHardCurrency = num3;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level2"))
							{
								int num4 = Convert.ToInt32(dictionary2["level2"]);
								_FreeHardCurrency = num4;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level3"))
							{
								int num5 = Convert.ToInt32(dictionary2["level3"]);
								CustomizationCurrency = num5;
							}
						}
					}
				}
				actionResult.success = true;
				callback(actionResult);
			}
			else
			{
				actionResult.success = false;
				callback(actionResult);
			}
		};
		Session theSession = SessionManager.Instance.theSession;
		int num = 0;
		int num2 = 0;
		if (amount <= (int)_PaidHardCurrency)
		{
			num = amount;
		}
		else
		{
			num = _PaidHardCurrency;
			amount -= (int)_PaidHardCurrency;
			num2 = amount;
		}
		if (num > 0)
		{
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, -num, 8);
		}
		if (num2 > 0)
		{
			LogUserTransactionServerHistory(theSession, Singleton<TBPvPManager>.Instance.CountryCode, -num2, 9);
		}
		theSession.Server.User_action(_PaidHardCurrency, _FreeHardCurrency, CustomizationCurrency, -num, -num2, 0, hashSTRING, -1, 0, eventName, Singleton<TBPvPManager>.Instance.CountryCode, callback2);
	}

	public void ConsumeHardCurrencyCustom(int amount, string eventName, ActionCallback callback)
	{
		string hashSTRING = GetHashSTRING(SessionManager.Instance.PlayerID, "ffbdh41e9_54fabaeb7ea530016faxg4aimc" + SessionManager.Instance.PlayerID + _PaidHardCurrency.ToString() + _FreeHardCurrency.ToString());
		TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			ActionResult actionResult = new ActionResult();
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
			{
				Singleton<PlayerInfoScript>.Instance.UpdateBadgeCount(BadgeEnum.Gacha);
				if (data.ContainsKey("data"))
				{
					string text = (string)data["data"];
					string json = text.Replace("[", string.Empty).Replace("]", string.Empty);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
					if (dictionary.Count > 0 && dictionary.ContainsKey("fields"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["fields"];
						string key = "5424493204pemhi3148ifmanseu4iksdf4_4" + SessionManager.Instance.PlayerID + "0";
						string hashSTRING2 = GetHashSTRING(SessionManager.Instance.PlayerID, key);
						string text2 = Convert.ToString(dictionary2["handle"]);
						if (hashSTRING2.ToLower() != text2.ToLower())
						{
							actionResult.success = false;
						}
						else
						{
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level1"))
							{
								int num = Convert.ToInt32(dictionary2["level1"]);
								_PaidHardCurrency = num;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level2"))
							{
								int num2 = Convert.ToInt32(dictionary2["level2"]);
								_FreeHardCurrency = num2;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level3"))
							{
								int num3 = Convert.ToInt32(dictionary2["level3"]);
								CustomizationCurrency = num3;
							}
						}
					}
				}
				actionResult.success = true;
				callback(actionResult);
			}
			else
			{
				actionResult.success = false;
				callback(actionResult);
			}
		};
		Session theSession = SessionManager.Instance.theSession;
		theSession.Server.User_action(_PaidHardCurrency, _FreeHardCurrency, CustomizationCurrency, 0, 0, -amount, hashSTRING, -1, 0, eventName, Singleton<TBPvPManager>.Instance.CountryCode, callback2);
	}

	public void User_Action(int misc, ActionCallback callback)
	{
		string hashSTRING = GetHashSTRING(SessionManager.Instance.PlayerID, "ffbdh41e9_54fabaeb7ea530016faxg4aimc" + SessionManager.Instance.PlayerID + _PaidHardCurrency.ToString() + _FreeHardCurrency.ToString());
		Session theSession = SessionManager.Instance.theSession;
		TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			ActionResult actionResult = new ActionResult();
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
			{
				if (data.ContainsKey("data"))
				{
					string text = (string)data["data"];
					string json = text.Replace("[", string.Empty).Replace("]", string.Empty);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
					if (dictionary.Count > 0 && dictionary.ContainsKey("fields"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["fields"];
						string key = "5424493204pemhi3148ifmanseu4iksdf4_4" + SessionManager.Instance.PlayerID + Convert.ToString(misc);
						string hashSTRING2 = GetHashSTRING(SessionManager.Instance.PlayerID, key);
						string text2 = Convert.ToString(dictionary2["handle"]);
						if (hashSTRING2.ToLower() != text2.ToLower())
						{
							actionResult.success = false;
						}
						else
						{
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level1"))
							{
								int num = Convert.ToInt32(dictionary2["level1"]);
								_PaidHardCurrency = num;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level2"))
							{
								int num2 = Convert.ToInt32(dictionary2["level2"]);
								_FreeHardCurrency = num2;
							}
							if (dictionary2.Count > 0 && dictionary2.ContainsKey("level3"))
							{
								int num3 = Convert.ToInt32(dictionary2["level3"]);
								CustomizationCurrency = num3;
							}
						}
					}
				}
				actionResult.success = true;
				callback(actionResult);
			}
			else
			{
				actionResult.success = false;
				callback(actionResult);
			}
		};
		theSession.Server.User_action(_PaidHardCurrency, _FreeHardCurrency, CustomizationCurrency, 0, 0, 0, hashSTRING, -1, misc, string.Empty, Singleton<TBPvPManager>.Instance.CountryCode, callback2);
	}

	public void User_Action2(int paid, int free, int flag, ActionCallback callback)
	{
		string hashSTRING = GetHashSTRING(SessionManager.Instance.PlayerID, "ffbdh41e9_54fabaeb7ea530016faxg4aimc" + SessionManager.Instance.PlayerID + _PaidHardCurrency.ToString() + _FreeHardCurrency.ToString());
		Session theSession = SessionManager.Instance.theSession;
		TFServer.JsonResponseHandler callback2 = delegate(Dictionary<string, object> data, HttpStatusCode status)
		{
			ActionResult actionResult = new ActionResult();
			if (status == HttpStatusCode.OK && data != null && data.ContainsKey("success") && (bool)data["success"])
			{
				if (data.ContainsKey("data"))
				{
					string text = (string)data["data"];
					string json = text.Replace("[", string.Empty).Replace("]", string.Empty);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
					if (dictionary.Count > 0 && dictionary.ContainsKey("fields"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["fields"];
						if (dictionary2.Count > 0 && dictionary2.ContainsKey("level1"))
						{
							int num = Convert.ToInt32(dictionary2["level1"]);
							_PaidHardCurrency = num;
						}
						if (dictionary2.Count > 0 && dictionary2.ContainsKey("level2"))
						{
							int num2 = Convert.ToInt32(dictionary2["level2"]);
							_FreeHardCurrency = num2;
						}
					}
				}
				actionResult.success = true;
				if (callback != null)
				{
					callback(actionResult);
				}
			}
			else
			{
				actionResult.success = false;
				if (callback != null)
				{
					callback(actionResult);
				}
			}
		};
		theSession.Server.User_action(0, 0, 0, paid, free, 100, hashSTRING, -1, flag, "debugmenu", Singleton<TBPvPManager>.Instance.CountryCode, callback2);
	}
}
