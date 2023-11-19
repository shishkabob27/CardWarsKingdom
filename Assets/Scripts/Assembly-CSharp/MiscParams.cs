using System;
using System.Collections.Generic;
using System.IO;

public class MiscParams : DataManager<DummyData>
{
	private int _SecondsPerStamina;

	private int _StaminaRefillCost;

	private int _MaxPvpStamina;

	private int _PvpStaminaMatchCost;

	private int _SecondsPerPvpStamina;

	private int _StartingLoadoutCount;

	private float _TownGridSpacing;

	private float _TownBuildingCollisionHeight;

	private int _TownAutoPlacementPadding;

	private int _ReviveCost;

	private int _StartingInventorySpace;

	private int _MaxInventorySpace;

	private int _InventorySpacePurchaseCost;

	private int _InventorySpacePerPurchase;

	private int _StartingActionPoints;

	private int _StartingStaminaPoints;

	private int _MaxActionPoints;

	private int _ExtensionCardWeight;

	private int _ActionPointIncrement;

	private int _HelpRewardForExplorer;

	private int _HelpRewardForAlly;

	private int _StartingAllyBoxSpace;

	private int _MaxAllyBoxSpace;

	private int _AllyBoxPurchaseCost;

	private int _AllyBoxPerPurchase;

	private int _CardsToDraw;

	private int _CriticalPercent;

	private bool _DrawToHandSize;

	private int _HandSize;

	private int _ActionPointsPerDiscard;

	private int _ActionPointsPerHeldCard;

	private int _SpecialQuestPreviewDays;

	private int _LeagueBonusPreviewHours;

	private CreatureData _TutorialCreature;

	private CreatureData _TutorialEVOCreature;

	private LeaderData _TutorialHero;

	private List<CardData> _TutorialCards = new List<CardData>();

	private float _PvpEntryCostPercent;

	private int _PvPCompatibilityVersion;

	private int _ChatCompatibilityVersion;

	private int _ChatSegmentingMinutes;

	private bool _ChatSeparateRegeon;

	private bool _PvpEnable;

	private bool _ChatEnable;

	private int _StarThreshold3;

	private int _StarThreshold2;

	private int _StarThreshold1;

	private bool _ForceDisableDebug;

	private bool _UseGooglePlayKFFKey;

	private int _MultiplayerUnrankedSearchRange;

	private int _MultiplayerUnrankedSearchRange2nd;

	private int _MultiplayerUnrankedSearchRange3rd;

	private int _MultiplayerRankedSearchRange;

	private int _MultiplayerRankedSearchRange2nd;

	private int _MultiplayerRankedSearchRange3rd;

	private int _MultiplayerTimeLimitStart;

	private int _MultiplayerTimeLimitPerTurn;

	private int _MultiplayerTimeLimitShowAt;

	private int _MultiplayerNormalizedLevel;

	private List<int> _NotificationReminderHours;

	private int _MaxMatchesInHistory;

	private string _iTunesStoreURL;

	private string _marketStoreURL;

	private bool _ShowInviteButtons;

	private float _GachaXpLevelsPerPlayerRank;

	private int _MaxGachaXpLevel;

	private bool _DisableChat;

	private bool _DisablePvp;

	private bool _AndroidUsesDifferentFBAccount;

	private int _PvPHackThreshold;

	private float _PvPReconnectTimeoutHost;

	private float _PvPReconnectTimeoutClient;

	private float _PvPMessageAckTimeout;

	private int _PvPReconnectRetryCountHost;

	private int _PvPReconnectRetryCountClient;

	private float _PvPHeartBeat;

	private float _PvPResendDelay;

	private float _PvPLeaveDelay;

	private int _PvPPauseAllowMilliseconds;

	private int _PvPAllyMatchWaitLimitMilliseconds;

	private bool _HarderHashCheck;

	private string _CompassSupportURL;

	private bool _UseGameServerStats;

	private float _ThrottlePointsPerMessage;

	private float _ThrottleMultiplierPerMessage;

	private float _ThrottleDupeMultiplier;

	private float _ThrottlePointDecay;

	private float _ThrottleMultiplierDecay;

	private int _StandardGachaMultiPullLimit;

	private int _PremiumGachaMultiPullLimit;

	private int _GachaChatAnnounceRarity;

	private CalendarTable _ActiveCalendar;

	private CalendarTable _RepeatingCalendar;

	private int _GlobalSaleCooldownMinutes;

	private float _GachaChatAnnounceCooldown;

	private int _CreaturesOnBoard;

	private int _CreaturesOnTeam;

	private bool _AllowCreatureFusion;

	private int _StandardGachaCost;

	private int _PremiumGachaCost;

	private int _fbAgeGate;

	private int _chatAgeGate;

	private string _defaultTutCreature_Plains;

	private string _defaultTutCreature_Corn;

	private string _defaultTutCreature_Nice;

	private int _BuySoftCurrencyCost;

	private int _BuySoftCurrencyAmount;

	private DateTime _AwakenEventStartDate;

	private DateTime _AwakenEventEndDate;

	private int _AwakenEventBonus;

	private static MiscParams _instance;

	public static int SecondsPerStamina
	{
		get
		{
			return Instance._SecondsPerStamina;
		}
	}

	public static int StaminaRefillCost
	{
		get
		{
			return Instance._StaminaRefillCost;
		}
	}

	public static int MaxPvpStamina
	{
		get
		{
			return Instance._MaxPvpStamina;
		}
	}

	public static int PvpStaminaMatchCost
	{
		get
		{
			return Instance._PvpStaminaMatchCost;
		}
	}

	public static int SecondsPerPvpStamina
	{
		get
		{
			return Instance._SecondsPerPvpStamina;
		}
	}

	public static int StartingLoadoutCount
	{
		get
		{
			return Instance._StartingLoadoutCount;
		}
	}

	public static float TownGridSpacing
	{
		get
		{
			return Instance._TownGridSpacing;
		}
	}

	public static float TownBuildingCollisionHeight
	{
		get
		{
			return Instance._TownBuildingCollisionHeight;
		}
	}

	public static int TownAutoPlacementPadding
	{
		get
		{
			return Instance._TownAutoPlacementPadding;
		}
	}

	public static int ReviveCost
	{
		get
		{
			return Instance._ReviveCost;
		}
	}

	public static int StartingInventorySpace
	{
		get
		{
			return Instance._StartingInventorySpace;
		}
	}

	public static int MaxInventorySpace
	{
		get
		{
			return Instance._MaxInventorySpace;
		}
	}

	public static int InventorySpacePurchaseCost
	{
		get
		{
			return Instance._InventorySpacePurchaseCost;
		}
	}

	public static int InventorySpacePerPurchase
	{
		get
		{
			return Instance._InventorySpacePerPurchase;
		}
	}

	public static int StartingActionPoints
	{
		get
		{
			return Instance._StartingActionPoints;
		}
	}

	public static int StartingStaminaPoints
	{
		get
		{
			return Instance._StartingStaminaPoints;
		}
	}

	public static int MaxActionPoints
	{
		get
		{
			return Instance._MaxActionPoints;
		}
	}

	public static int ActionPointIncrement
	{
		get
		{
			return Instance._ActionPointIncrement;
		}
	}

	public static int HelpPointForExplorer
	{
		get
		{
			return Instance._HelpRewardForExplorer;
		}
	}

	public static int HelpPointForAlly
	{
		get
		{
			return Instance._HelpRewardForAlly;
		}
	}

	public static int StartingAllyBoxSpace
	{
		get
		{
			return Instance._StartingAllyBoxSpace;
		}
	}

	public static int MaxAllyBoxSpace
	{
		get
		{
			return Instance._MaxAllyBoxSpace;
		}
	}

	public static int AllyBoxPurchaseCost
	{
		get
		{
			return Instance._AllyBoxPurchaseCost;
		}
	}

	public static int AllyBoxPerPurchase
	{
		get
		{
			return Instance._AllyBoxPerPurchase;
		}
	}

	public static int ExtensionCardWeight
	{
		get
		{
			return Instance._ExtensionCardWeight;
		}
	}

	public static int CardsToDraw
	{
		get
		{
			return Instance._CardsToDraw;
		}
	}

	public static int CriticalPercent
	{
		get
		{
			return Instance._CriticalPercent;
		}
	}

	public static bool DrawToHandSize
	{
		get
		{
			return Instance._DrawToHandSize;
		}
	}

	public static int HandSize
	{
		get
		{
			return Instance._HandSize;
		}
	}

	public static int ActionPointsPerDiscard
	{
		get
		{
			return Instance._ActionPointsPerDiscard;
		}
	}

	public static int ActionPointsPerHeldCard
	{
		get
		{
			return Instance._ActionPointsPerHeldCard;
		}
	}

	public static int SpecialQuestPreviewDays
	{
		get
		{
			return Instance._SpecialQuestPreviewDays;
		}
	}

	public static int LeagueBonusPreviewHours
	{
		get
		{
			return Instance._LeagueBonusPreviewHours;
		}
	}

	public static CreatureData TutorialCreature
	{
		get
		{
			return Instance._TutorialCreature;
		}
	}

	public static CreatureData TutorialEVOCreature
	{
		get
		{
			return Instance._TutorialEVOCreature;
		}
	}

	public static LeaderData TutorialHero
	{
		get
		{
			return Instance._TutorialHero;
		}
	}

	public static List<CardData> TutorialCards
	{
		get
		{
			return Instance._TutorialCards;
		}
	}

	public static float PvpEntryCostPercent
	{
		get
		{
			return Instance._PvpEntryCostPercent;
		}
	}

	public static int PvPCompatibilityVersion
	{
		get
		{
			return Instance._PvPCompatibilityVersion;
		}
	}

	public static int ChatCompatibilityVersion
	{
		get
		{
			return Instance._ChatCompatibilityVersion;
		}
	}

	public static int ChatSegmentingMinutes
	{
		get
		{
			return Instance._ChatSegmentingMinutes;
		}
	}

	public static bool ChatSeparateRegeon
	{
		get
		{
			return Instance._ChatSeparateRegeon;
		}
	}

	public static bool PvpEnable
	{
		get
		{
			return Instance._PvpEnable;
		}
	}

	public static bool ChatEnable
	{
		get
		{
			return Instance._ChatEnable;
		}
	}

	public static float PvPReconnectTimeoutHost
	{
		get
		{
			return Instance._PvPReconnectTimeoutHost;
		}
	}

	public static float PvPReconnectTimeoutClient
	{
		get
		{
			return Instance._PvPReconnectTimeoutClient;
		}
	}

	public static float PvPMessageAckTimeout
	{
		get
		{
			return Instance._PvPMessageAckTimeout;
		}
	}

	public static int PvPReconnectRetryCountHost
	{
		get
		{
			return Instance._PvPReconnectRetryCountHost;
		}
	}

	public static int PvPReconnectRetryCountClient
	{
		get
		{
			return Instance._PvPReconnectRetryCountClient;
		}
	}

	public static float PvPHeartBeat
	{
		get
		{
			return Instance._PvPHeartBeat;
		}
	}

	public static float PvPResendDelay
	{
		get
		{
			return Instance._PvPResendDelay;
		}
	}

	public static float PvPLeaveDelay
	{
		get
		{
			return Instance._PvPLeaveDelay;
		}
	}

	public static int PvPPauseAllowMilliseconds
	{
		get
		{
			return Instance._PvPPauseAllowMilliseconds;
		}
	}

	public static int PvPAllyMatchWaitLimitMilliseconds
	{
		get
		{
			return Instance._PvPAllyMatchWaitLimitMilliseconds;
		}
	}

	public static bool HarderHashCheck
	{
		get
		{
			return Instance._HarderHashCheck;
		}
	}

	public static bool UseGameServerStats
	{
		get
		{
			return Instance._UseGameServerStats;
		}
	}

	public static int StarThreshold3
	{
		get
		{
			return Instance._StarThreshold3;
		}
	}

	public static int StarThreshold2
	{
		get
		{
			return Instance._StarThreshold2;
		}
	}

	public static int StarThreshold1
	{
		get
		{
			return Instance._StarThreshold1;
		}
	}

	public static bool ForceDisableDebug
	{
		get
		{
			return Instance._ForceDisableDebug;
		}
	}

	public static bool UseGooglePlayKFFKey
	{
		get
		{
			return Instance._UseGooglePlayKFFKey;
		}
	}

	public static int MultiplayerUnrankedSearchRange
	{
		get
		{
			return Instance._MultiplayerUnrankedSearchRange;
		}
	}

	public static int MultiplayerUnrankedSearchRange2nd
	{
		get
		{
			return Instance._MultiplayerUnrankedSearchRange2nd;
		}
	}

	public static int MultiplayerUnrankedSearchRange3rd
	{
		get
		{
			return Instance._MultiplayerUnrankedSearchRange3rd;
		}
	}

	public static int MultiplayerRankedSearchRange
	{
		get
		{
			return Instance._MultiplayerRankedSearchRange;
		}
	}

	public static int MultiplayerRankedSearchRange2nd
	{
		get
		{
			return Instance._MultiplayerRankedSearchRange2nd;
		}
	}

	public static int MultiplayerRankedSearchRange3rd
	{
		get
		{
			return Instance._MultiplayerRankedSearchRange3rd;
		}
	}

	public static int MultiplayerTimeLimitStart
	{
		get
		{
			return Instance._MultiplayerTimeLimitStart;
		}
	}

	public static int MultiplayerTimeLimitPerTurn
	{
		get
		{
			return Instance._MultiplayerTimeLimitPerTurn;
		}
	}

	public static int MultiplayerTimeLimitShowAt
	{
		get
		{
			return Instance._MultiplayerTimeLimitShowAt;
		}
	}

	public static int MultiplayerNormalizedLevel
	{
		get
		{
			return Instance._MultiplayerNormalizedLevel;
		}
	}

	public static List<int> NotificationReminderHours
	{
		get
		{
			return Instance._NotificationReminderHours;
		}
	}

	public static int MaxMatchesInHistory
	{
		get
		{
			return Instance._MaxMatchesInHistory;
		}
	}

	public static string iTunesStoreURL
	{
		get
		{
			return Instance._iTunesStoreURL;
		}
	}

	public static string marketStoreURL
	{
		get
		{
			return Instance._marketStoreURL;
		}
	}

	public static string CompassSupportURL
	{
		get
		{
			return Instance._CompassSupportURL;
		}
	}

	public static bool ShowInviteButtons
	{
		get
		{
			return Instance._ShowInviteButtons;
		}
	}

	public static float GachaXpLevelsPerPlayerRank
	{
		get
		{
			return Instance._GachaXpLevelsPerPlayerRank;
		}
	}

	public static int MaxGachaXpLevel
	{
		get
		{
			return Instance._MaxGachaXpLevel;
		}
	}

	public static bool DisableChat
	{
		get
		{
			return Instance._DisableChat;
		}
	}

	public static bool DisablePvp
	{
		get
		{
			return Instance._DisablePvp;
		}
	}

	public static int PvPHackThreshold
	{
		get
		{
			return Instance._PvPHackThreshold;
		}
	}

	public static bool AndroidUsesDifferentFBAccount
	{
		get
		{
			return Instance._AndroidUsesDifferentFBAccount;
		}
	}

	public static float ThrottlePointsPerMessage
	{
		get
		{
			return Instance._ThrottlePointsPerMessage;
		}
	}

	public static float ThrottleMultiplierPerMessage
	{
		get
		{
			return Instance._ThrottleMultiplierPerMessage;
		}
	}

	public static float ThrottleDupeMultiplier
	{
		get
		{
			return Instance._ThrottleDupeMultiplier;
		}
	}

	public static float ThrottlePointDecay
	{
		get
		{
			return Instance._ThrottlePointDecay;
		}
	}

	public static float ThrottleMultiplierDecay
	{
		get
		{
			return Instance._ThrottleMultiplierDecay;
		}
	}

	public static int StandardGachaMultiPullLimit
	{
		get
		{
			return Instance._StandardGachaMultiPullLimit;
		}
	}

	public static int PremiumGachaMultiPullLimit
	{
		get
		{
			return Instance._PremiumGachaMultiPullLimit;
		}
	}

	public static int GachaChatAnnounceRarity
	{
		get
		{
			return Instance._GachaChatAnnounceRarity;
		}
	}

	public static CalendarTable ActiveCalendar
	{
		get
		{
			return Instance._ActiveCalendar;
		}
	}

	public static CalendarTable RepeatingCalendar
	{
		get
		{
			return Instance._RepeatingCalendar;
		}
	}

	public static int GlobalSaleCooldownMinutes
	{
		get
		{
			return Instance._GlobalSaleCooldownMinutes;
		}
	}

	public static float GachaChatAnnounceCooldown
	{
		get
		{
			return Instance._GachaChatAnnounceCooldown;
		}
	}

	public static int CreaturesOnBoard
	{
		get
		{
			return Instance._CreaturesOnBoard;
		}
	}

	public static int CreaturesOnTeam
	{
		get
		{
			return Instance._CreaturesOnTeam;
		}
	}

	public static bool AllowCreatureFusion
	{
		get
		{
			return Instance._AllowCreatureFusion;
		}
	}

	public static int fbAgeGate
	{
		get
		{
			return Instance._fbAgeGate;
		}
	}

	public static int chatAgeGate
	{
		get
		{
			return Instance._chatAgeGate;
		}
	}

	public static string defaultTutCreature_Plains
	{
		get
		{
			return Instance._defaultTutCreature_Plains;
		}
	}

	public static string defaultTutCreature_Corn
	{
		get
		{
			return Instance._defaultTutCreature_Corn;
		}
	}

	public static string defaultTutCreature_Nice
	{
		get
		{
			return Instance._defaultTutCreature_Nice;
		}
	}

	public static int BuySoftCurrencyCost
	{
		get
		{
			return Instance._BuySoftCurrencyCost;
		}
	}

	public static int BuySoftCurrencyAmount
	{
		get
		{
			return Instance._BuySoftCurrencyAmount;
		}
	}

	public static DateTime AwakenEventStartDate
	{
		get
		{
			return Instance._AwakenEventStartDate;
		}
	}

	public static DateTime AwakenEventEndDate
	{
		get
		{
			return Instance._AwakenEventEndDate;
		}
	}

	public static int AwakenEventBonus
	{
		get
		{
			return Instance._AwakenEventBonus;
		}
	}

	public static MiscParams Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_MiscParams.json");
				_instance = new MiscParams(path);
			}
			return _instance;
		}
	}

	public MiscParams(string path)
	{
		base.FilePath = path;
		AddDependency(CreatureDataManager.Instance);
		AddDependency(LeaderDataManager.Instance);
		AddDependency(CalendarGiftDataManager.Instance);
	}

	protected override void ParseRows(List<object> jlist)
	{
		if (jlist.Count == 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item;
			if (dictionary2.ContainsKey("Value"))
			{
				string key = TFUtils.LoadString(dictionary2, "Parameter", string.Empty);
				object value = dictionary2["Value"];
				dictionary.Add(key, value);
			}
		}
		_SecondsPerStamina = TFUtils.LoadInt(dictionary, "SecondsPerStamina", 1);
		_StaminaRefillCost = TFUtils.LoadInt(dictionary, "StaminaRefillCost", 1);
		_MaxPvpStamina = TFUtils.LoadInt(dictionary, "MaxPvpStamina", 1);
		_PvpStaminaMatchCost = TFUtils.LoadInt(dictionary, "PvpStaminaMatchCost", 1);
		_SecondsPerPvpStamina = TFUtils.LoadInt(dictionary, "SecondsPerPvpStamina", 1);
		_StartingLoadoutCount = TFUtils.LoadInt(dictionary, "StartingLoadoutCount", 1);
		_TownGridSpacing = TFUtils.LoadFloat(dictionary, "TownGridSpacing", 1f);
		_TownBuildingCollisionHeight = TFUtils.LoadFloat(dictionary, "TownBuildingCollisionHeight", 1f);
		_TownAutoPlacementPadding = TFUtils.LoadInt(dictionary, "TownAutoPlacementPadding", 1);
		_ReviveCost = TFUtils.LoadInt(dictionary, "ReviveCost", 1);
		_StartingInventorySpace = TFUtils.LoadInt(dictionary, "StartingInventorySpace", 1);
		_MaxInventorySpace = TFUtils.LoadInt(dictionary, "MaxInventorySpace", 1);
		_InventorySpacePurchaseCost = TFUtils.LoadInt(dictionary, "InventorySpacePurchaseCost", 1);
		_InventorySpacePerPurchase = TFUtils.LoadInt(dictionary, "InventorySpacePerPurchase", 1);
		_StartingActionPoints = TFUtils.LoadInt(dictionary, "StartingActionPoints", 50);
		_StartingStaminaPoints = TFUtils.LoadInt(dictionary, "StartingStaminaPoints", 2);
		_MaxActionPoints = TFUtils.LoadInt(dictionary, "MaxActionPoints", 100);
		_ActionPointIncrement = TFUtils.LoadInt(dictionary, "ActionPointIncrement", 5);
		_HelpRewardForExplorer = TFUtils.LoadInt(dictionary, "HelpPointForExplorer", 5);
		_HelpRewardForAlly = TFUtils.LoadInt(dictionary, "HelpPointForAlly", 10);
		_StartingAllyBoxSpace = TFUtils.LoadInt(dictionary, "StartingAllyBoxSpace", 1);
		_MaxAllyBoxSpace = TFUtils.LoadInt(dictionary, "MaxAllyBoxSpace", 1);
		_AllyBoxPurchaseCost = TFUtils.LoadInt(dictionary, "AllyBoxPurchaseCost", 1);
		_AllyBoxPerPurchase = TFUtils.LoadInt(dictionary, "AllyBoxPerPurchase", 1);
		_CardsToDraw = TFUtils.LoadInt(dictionary, "CardsToDraw", 1);
		_CriticalPercent = TFUtils.LoadInt(dictionary, "CriticalPercent", 25);
		_ExtensionCardWeight = TFUtils.LoadInt(dictionary, "ExtensionCardWeight", 10);
		_DrawToHandSize = TFUtils.LoadBool(dictionary, "DrawToHandSize", false);
		_HandSize = TFUtils.LoadInt(dictionary, "HandSize", 5);
		_ActionPointsPerDiscard = TFUtils.LoadInt(dictionary, "ActionPointsPerDiscard", 1);
		_ActionPointsPerHeldCard = TFUtils.LoadInt(dictionary, "ActionPointsPerHeldCard", 10);
		_SpecialQuestPreviewDays = TFUtils.LoadInt(dictionary, "SpecialQuestPreviewDays", 0);
		_LeagueBonusPreviewHours = TFUtils.LoadInt(dictionary, "LeagueBonusPreviewHours", 0);
		_TutorialCreature = CreatureDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "TutorialCreature", string.Empty));
		_TutorialEVOCreature = CreatureDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "TutorialEVOCreature", string.Empty));
		_TutorialHero = LeaderDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "TutorialBuyHero", string.Empty));
		string text = TFUtils.LoadString(dictionary, "TutorialCards", string.Empty);
		string[] array = text.Split(',');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			CardData data = CardDataManager.Instance.GetData(text2.Trim());
			if (data != null)
			{
				_TutorialCards.Add(data);
			}
		}
		_PvpEntryCostPercent = TFUtils.LoadFloat(dictionary, "PvpEntryCostPercent", 0f);
		_PvPCompatibilityVersion = TFUtils.LoadInt(dictionary, "PvPCompatibilityVersion", 110);
		_ChatCompatibilityVersion = TFUtils.LoadInt(dictionary, "ChatCompatibilityVersion", 100);
		_ChatSegmentingMinutes = TFUtils.LoadInt(dictionary, "ChatSegmentingMinutes", 60);
		_ChatSeparateRegeon = TFUtils.LoadBool(dictionary, "ChatSeparateRegeon", false);
		_PvpEnable = TFUtils.LoadBool(dictionary, "PvpEnable", true);
		_ChatEnable = TFUtils.LoadBool(dictionary, "ChatEnable", true);
		_StarThreshold3 = TFUtils.LoadInt(dictionary, "3StarThreshold", 1);
		_StarThreshold2 = TFUtils.LoadInt(dictionary, "2StarThreshold", 2);
		_StarThreshold1 = TFUtils.LoadInt(dictionary, "1StarThreshold", 3);
		_ForceDisableDebug = TFUtils.LoadBool(dictionary, "ForceDisableDebug", false);
		_UseGooglePlayKFFKey = TFUtils.LoadBool(dictionary, "UseGooglePlayKFFKey", false);
		_MultiplayerUnrankedSearchRange = TFUtils.LoadInt(dictionary, "MultiplayerUnrankedSearchRange", 1);
		_MultiplayerUnrankedSearchRange2nd = TFUtils.LoadInt(dictionary, "MultiplayerUnrankedSearchRange2ndPath", 2);
		_MultiplayerUnrankedSearchRange3rd = TFUtils.LoadInt(dictionary, "MultiplayerUnrankedSearchRange3rdPath", 3);
		_MultiplayerRankedSearchRange = TFUtils.LoadInt(dictionary, "MultiplayerRankedSearchRange", 1);
		_MultiplayerRankedSearchRange2nd = TFUtils.LoadInt(dictionary, "MultiplayerRankedSearchRange2ndPath", 2);
		_MultiplayerRankedSearchRange3rd = TFUtils.LoadInt(dictionary, "MultiplayerRankedSearchRange3rdPath", 3);
		_MultiplayerTimeLimitStart = TFUtils.LoadInt(dictionary, "MultiplayerTimeLimitStart", 600);
		_MultiplayerTimeLimitPerTurn = TFUtils.LoadInt(dictionary, "MultiplayerTimeLimitPerTurn", 5);
		_MultiplayerTimeLimitShowAt = TFUtils.LoadInt(dictionary, "MultiplayerTimeLimitShowAt", 15);
		_MultiplayerNormalizedLevel = TFUtils.LoadInt(dictionary, "MultiplayerNormalizedLevel", 25);
		_MaxMatchesInHistory = TFUtils.LoadInt(dictionary, "MaxMatchesInHistory", 50);
		_iTunesStoreURL = TFUtils.LoadString(dictionary, "iTunesStoreURL", "itunes.apple.com/us/app/card-wars-kingdom-adventure/id1084805156?ls=1&mt=8&uo=4&at=11lmxx");
		_marketStoreURL = TFUtils.LoadString(dictionary, "marketStoreURL", "com.turner.cardwars2");
		_CompassSupportURL = TFUtils.LoadString(dictionary, "CompassSupportURL", "http://dev4.compasssupport.crooz.jp/");
		_ShowInviteButtons = TFUtils.LoadBool(dictionary, "ShowInviteButtons", false);
		_GachaXpLevelsPerPlayerRank = TFUtils.LoadFloat(dictionary, "GachaXpLevelsPerPlayerRank", 0.1f);
		_MaxGachaXpLevel = TFUtils.LoadInt(dictionary, "MaxGachaXpLevel", 10);
		_DisableChat = TFUtils.LoadBool(dictionary, "DisableChat", false);
		_DisablePvp = TFUtils.LoadBool(dictionary, "DisablePvp", false);
		_PvPHackThreshold = TFUtils.LoadInt(dictionary, "PvPHackThreshold", 9989);
		_PvPReconnectTimeoutHost = TFUtils.LoadFloat(dictionary, "PvPReconnectTimeoutHost", 11f);
		_PvPReconnectTimeoutClient = TFUtils.LoadFloat(dictionary, "PvPReconnectTimeoutClient", 4f);
		_PvPMessageAckTimeout = TFUtils.LoadFloat(dictionary, "PvPMessageAckTimeout", 4f);
		_PvPReconnectRetryCountHost = TFUtils.LoadInt(dictionary, "PvPReconnectRetryCountHost", 3);
		_PvPReconnectRetryCountClient = TFUtils.LoadInt(dictionary, "PvPReconnectRetryCountClient", 7);
		_PvPHeartBeat = TFUtils.LoadFloat(dictionary, "PvPHeartBeat", 5f);
		_PvPResendDelay = TFUtils.LoadFloat(dictionary, "PvPResendDelay", 1f);
		_PvPLeaveDelay = TFUtils.LoadFloat(dictionary, "PvPLeaveDelay", 0.5f);
		_PvPPauseAllowMilliseconds = TFUtils.LoadInt(dictionary, "PvPPauseAllowMilliseconds", 3500);
		_PvPAllyMatchWaitLimitMilliseconds = TFUtils.LoadInt(dictionary, "PvPAllyMatchWaitLimitMilliseconds", 60000);
		_HarderHashCheck = TFUtils.LoadBool(dictionary, "HarderHashCheck", false);
		_UseGameServerStats = TFUtils.LoadBool(dictionary, "UseGameServerStats", false);
		_AndroidUsesDifferentFBAccount = TFUtils.LoadBool(dictionary, "AndroidFBAccount", false);
		_ThrottlePointsPerMessage = TFUtils.LoadFloat(dictionary, "ThrottlePPM", 0f);
		_ThrottleMultiplierPerMessage = TFUtils.LoadFloat(dictionary, "ThrottleMPM", 0f);
		_ThrottlePointDecay = TFUtils.LoadFloat(dictionary, "ThrottlePD", 0f);
		_ThrottleMultiplierDecay = TFUtils.LoadFloat(dictionary, "ThrottleMD", 0f);
		_ThrottleDupeMultiplier = TFUtils.LoadFloat(dictionary, "ThrottleDX", 0f);
		_StandardGachaMultiPullLimit = TFUtils.LoadInt(dictionary, "StandardGachaMultiPullLimit", 30);
		_PremiumGachaMultiPullLimit = TFUtils.LoadInt(dictionary, "PremiumGachaMultiPullLimit", 30);
		_GachaChatAnnounceRarity = TFUtils.LoadInt(dictionary, "GachaChatAnnounceRarity", 5);
		_GachaChatAnnounceCooldown = TFUtils.LoadFloat(dictionary, "GachaChatAnnounceCooldown", 120f);
		_ActiveCalendar = CalendarGiftDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "ActiveCalendar", string.Empty));
		_RepeatingCalendar = CalendarGiftDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "RepeatingCalendar", string.Empty));
		_GlobalSaleCooldownMinutes = TFUtils.LoadInt(dictionary, "GlobalSaleCooldownMinutes", 0);
		_CreaturesOnBoard = TFUtils.LoadInt(dictionary, "CreaturesOnBoard", 4);
		_CreaturesOnTeam = TFUtils.LoadInt(dictionary, "CreaturesOnTeam", 5);
		_AllowCreatureFusion = TFUtils.LoadBool(dictionary, "AllowCreatureFusion", false);
		_StandardGachaCost = TFUtils.LoadInt(dictionary, "StandardGachaCost", 100);
		_PremiumGachaCost = TFUtils.LoadInt(dictionary, "PremiumGachaCost", 500);
		_fbAgeGate = TFUtils.LoadInt(dictionary, "fbAgeGate", 13);
		_chatAgeGate = TFUtils.LoadInt(dictionary, "chatAgeGate", 18);
		_defaultTutCreature_Plains = TFUtils.LoadString(dictionary, "defaultTutCreature_Plains", "AncientScholar_Base");
		_defaultTutCreature_Corn = TFUtils.LoadString(dictionary, "defaultTutCreature_Corn", "HuskerKnight_Base");
		_defaultTutCreature_Nice = TFUtils.LoadString(dictionary, "defaultTutCreature_Nice", "Shepard_Base");
		_BuySoftCurrencyAmount = TFUtils.LoadInt(dictionary, "BuySoftCurrencyAmount", 1000);
		_BuySoftCurrencyCost = TFUtils.LoadInt(dictionary, "BuySoftCurrencyCost", 250);
		_NotificationReminderHours = new List<int>();
		string text3 = TFUtils.LoadString(dictionary, "NotificationReminderHours", null);
		if (text3 != null)
		{
			string[] array3 = text3.Split(',');
			string[] array4 = array3;
			foreach (string text4 in array4)
			{
				_NotificationReminderHours.Add(Convert.ToInt32(text4.Trim()));
			}
		}
		string text5 = TFUtils.LoadString(dictionary, "AwakenEventStartDate", null);
		string text6 = TFUtils.LoadString(dictionary, "AwakenEventEndDate", null);
		if (text5 != null && text6 != null)
		{
			_AwakenEventStartDate = DateTime.Parse(text5);
			_AwakenEventEndDate = DateTime.Parse(text6);
			_AwakenEventBonus = TFUtils.LoadInt(dictionary, "AwakenEventBonus", 0);
		}
		else
		{
			_AwakenEventStartDate = DateTime.MinValue;
			_AwakenEventEndDate = DateTime.MinValue;
		}
	}

	protected override void PostLoad()
	{
	}
}
