using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
	private const bool KontagentTestMode = false;

	private const int NUM_BUCKETS = 255;

	private const string PLAYER_ID = "PlayerId";

	private const string DEVICE_ID = "DeviceId";

	private const string DEVICE_INFO = "DeviceInfo";

	private const string OFFLINE = "Offline";

	private const string SUBTYPE_1 = "subtype1";

	private const string SUBTYPE_2 = "subtype2";

	private const string SUBTYPE_3 = "subtype3";

	private const string LEVEL = "level";

	private const string VALUE = "value";

	private const string CATEGORY_GAMEFLOW = "GAMEFLOW";

	private const string CATEGORY_PLAYER = "PLAYER";

	private const string CATEGORY_CREATURE = "CREATURE";

	private const string CATEGORY_HERO = "HERO";

	private const string CATEGORY_TUTORIAL = "TUTORIAL";

	private const string CATEGORY_LAB = "LAB";

	private const string CATEGORY_PRIVATE = "PRIVATE";

	private const string CATEGORY_ACCOUNT = "ACCOUNT";

	private const string SUBCATEGORY_CONNECT = "Login";

	private const string SUBCATEGORY_MAINMENU = "MainMenu";

	private const string SUBCATEGORY_MAP = "Map";

	private const string SUBCATEGORY_END_RESULTS = "EndResults";

	private const string SUBCATEGORY_QUEST = "Quest";

	private const string SUBCATEGORY_RANK = "Rank";

	private const string SUBCATEGORY_CREATURE = "Creature";

	private const string SUBCATEGORY_GEM = "Gem";

	private const string SUBCATEGORY_CARD = "Card";

	private const string SUBCATEGORY_NORMAL_CHEST = "Normal Chest";

	private const string SUBCATEGORY_PREMIUM_CHEST = "Premium Chest";

	private const string SUBCATEGORY_SPECIAL_CHEST = "Special Chest";

	private const string SUBCATEGORY_IAP = "IAP";

	private const string SUBCATEGORY_BATTLE = "Battle";

	private const string SUBCATEGORY_PVE_BATTLE = "PvE";

	private const string SUBCATEGORY_PVP_BATTLE = "PvP";

	private const string SUBCATEGORY_PVP_RANK_ATTAINED = "PvPRankAttained";

	private const string SUBCATEGORY_TUTORIAL_STATE = "TutorialStateStart";

	private const string SUBCATEGORY_PRIVATE_INSTALLED = "Installed";

	private const string SUBCATEGORY_PRIVATE_COUNTRYCD = "CountryCode";

	private const string SUBCATEGORY_PRIVATE_STAMINA1 = "Stamina1";

	private const string SUBCATEGORY_PRIVATE_STAMINA2 = "Stamina2";

	private const string SUBCATEGORY_PRIVATE_XP = "XP";

	private const string SUBCATEGORY_PRIVATE_CURRENCY1 = "Currency1";

	private const string SUBCATEGORY_PRIVATE_CURRENCY2 = "Currency2";

	private const string SUBCATEGORY_PRIVATE_CURRENCY3 = "Currency3";

	private const string SUBCATEGORY_PRIVATE_INTERVAL = "Interval";

	private const string SUBCATEGORY_MULTIPLAYER = "Multiplayer";

	private const string SUBCATEGORY_ACCOUNT_GAIN_PAID = "StAddPaid";

	private const string SUBCATEGORY_ACCOUNT_GAIN_FREE = "StAddFree";

	private const string SUBCATEGORY_ACCOUNT_USED_PAID = "StUsePaid";

	private const string SUBCATEGORY_ACCOUNT_USED_FREE = "StUseFree";

	private const string SUBCATEGORY_INVITE_SENT = "InviteSent";

	private const string SUBCATEGORY_INVITE_USED = "InviteUsed";

	private const string EVENT_FIRSTTIME_PLAY = "FirstTimePlay";

	private const string EVENT_LOGIN_FB = "LoginFB";

	private const string EVENT_LOGIN_GUEST = "LoginGuest";

	private const string EVENT_QUEST_START = "QuestStart";

	private const string EVENT_QUEST_WON = "QuestWon";

	private const string EVENT_QUEST_LOST = "QuestLost";

	private const string EVENT_QUEST_QUIT = "QuestQuit";

	private const string EVENT_QUEST_REVIVE = "QuestRevive";

	private const string EVENT_PVP_SEARCH_START = "PvPSearch";

	private const string EVENT_PVP_SEARCH_FOUND = "PvPFound";

	private const string EVENT_PVP_BATTLE_START = "PvPStart";

	private const string EVENT_PVP_BATTLE_END = "PvPEnd";

	private const string EVENT_HIGHEST_QUEST = "HighestQuest";

	private const string EVENT_HIGHEST_RANK = "HighestRank";

	private const string EVENT_INVENTORY_INCREASE = "InventoryIncrease";

	private const string EVENT_OPEN_NORMAL_CHEST = "OpenNormalChest";

	private const string EVENT_OPEN_PREMIUM_CHEST = "OpenPremiumChest";

	private const string EVENT_OPEN_SPECIAL_CHEST = "OpenSpecialChest";

	private const string EVENT_IAP_PURCHASE = "IAP_Purchase";

	private const string EVENT_CREATURE_USED = "CreatureUses";

	private const string EVENT_HERO_BOUGHT = "HeroBought";

	private const string EVENT_HERO_USED = "HeroUsed";

	private const string EVENT_HERO_SKIN_BOUGHT = "HeroSkinBought";

	private const string EVENT_HERO_SKIN_USED = "HeroSkinUsed";

	private const string EVENT_SPEEDUP_USED = "SpeedupUsed";

	private const string EVENT_INCREASE_INVENTORY = "IncreaseInventory";

	private const string EVENT_POWER_UP = "PowerUp";

	private const string EVENT_EVO = "Evo";

	private const string EVENT_CRAFT = "Craft";

	private string KontagentApiKey;

	private bool TurnOff;

	private string deviceId;

	private string deviceInfo;

	private string playerId;

	private int BATTLE_COUNT_BUCKET_SIZE = 10;

	private static int m_LastUpdateDay;

	private static bool m_JailBroken;

	private static bool isNewReported;

	public bool JailBroken
	{
		get
		{
			return m_JailBroken;
		}
	}

	private void Awake()
	{
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StartSession();
		}
		else
		{
			StopSession();
		}
	}

	private int CompressData(int input, int bucketSize)
	{
		int num = bucketSize * 255;
		float num2 = bucketSize;
		float num3 = (float)input / num2;
		if (input >= num)
		{
			num3 = 255f;
		}
		return (int)num3;
	}

	private void AddCommon(Dictionary<string, string> KontagentEventData, string[] Categories, string eventName, int dataLevel)
	{
		KontagentEventData["DeviceId"] = deviceId;
		KontagentEventData["DeviceInfo"] = deviceInfo;
		KontagentEventData["n"] = eventName;
		KontagentEventData["l"] = dataLevel.ToString();
		if (Categories[0] != null)
		{
			KontagentEventData["st1"] = Categories[0];
		}
		if (Categories[1] != null)
		{
			KontagentEventData["st2"] = Categories[1];
		}
		if (Categories[2] != null)
		{
			KontagentEventData["st3"] = Categories[2];
		}
	}

	public void SendDeviceInfo()
	{
		if (!TurnOff)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["v_maj"] = "1.0";
			dictionary["d"] = SystemInfo.deviceModel;
			KontagentBinding.sendDeviceInformation(dictionary);
		}
	}

	public void StartSession()
	{
		if (!TurnOff)
		{
			KontagentBinding.startSession(KontagentApiKey, false);
		}
	}

	public void StopSession()
	{
		if (!TurnOff)
		{
			KontagentBinding.stopSession();
		}
	}

	private void LogEvent(string[] Categories, string eventName, string dataValue, int dataLevel)
	{
		if (!TurnOff)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			AddCommon(dictionary, Categories, eventName, dataLevel);
			dictionary["v"] = dataValue;
			KontagentBinding.customEvent(eventName, dictionary);
		}
	}

	private void LogEvent(string[] Categories, string eventName, string dataValue = "")
	{
		if (!TurnOff)
		{
			LogEvent(Categories, eventName, dataValue, Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel);
		}
	}

	private void LogEventValue(string[] Categories, string eventName, string dataValue, int intValue)
	{
		if (!TurnOff)
		{
			LogEvent(Categories, eventName, dataValue, intValue);
		}
	}

	private void Start()
	{
		deviceId = Guid.NewGuid().ToString().Replace("-", string.Empty);
		deviceInfo = string.Format("{0} {1} {2}", SystemInfo.deviceModel, SystemInfo.processorType, SystemInfo.operatingSystem);
		KontagentApiKey = "d8852b525ee04ae4bd0c8bfdc209a7dd";
		StartSession();
		SendDeviceInfo();
		m_JailBroken = TFUtils.IsAndroidDeviceRooted();
		if (!m_JailBroken)
		{
			m_JailBroken = !Application.genuine;
		}
		LogFirstTimePlay();
	}

	public void LogFirstTimePlay()
	{
		if (!TurnOff && PlayerPrefs.GetInt("FirstTimePlay", 0) == 0)
		{
			string[] categories = new string[3] { "GAMEFLOW", null, null };
			LogEvent(categories, "FirstTimePlay", string.Empty, 0);
			PlayerPrefs.SetInt("FirstTimePlay", 1);
		}
	}

	public void LogMainMenuClick(string ControllerName)
	{
		string[] categories = new string[3] { "GAMEFLOW", "MainMenu", null };
		LogEvent(categories, ControllerName, string.Empty);
		uint num = DateTime.UtcNow.UnixTimestamp();
		int day = DateTime.UtcNow.Day;
		if (m_LastUpdateDay != day)
		{
			string empty = string.Empty;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			empty = Singleton<PlayerInfoScript>.Instance.GetPlayerCode();
			num2 = Singleton<PlayerInfoScript>.Instance.SaveData.PaidHardCurrency;
			num3 = Singleton<PlayerInfoScript>.Instance.SaveData.FreeHardCurrency;
			num4 = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency;
			num5 = Singleton<PlayerInfoScript>.Instance.SaveData.PvPCurrency;
			string manifestVersionName = KFFAndroidPlugin.GetManifestVersionName();
			string dataValue = KFFAndroidPlugin.GetManifestVersionCode().ToString();
			string[] categories2 = new string[3] { "PRIVATE", "Installed", null };
			int num6 = Convert.ToInt32(m_JailBroken);
			LogEventValue(categories2, "Installed", Convert.ToString(num6), KFFCSUtils.GetInstalledDate());
			string[] categories3 = new string[3] { "PRIVATE", "Installed", null };
			LogEventValue(categories3, "Version", dataValue, KFFCSUtils.GetInstalledDate());
			if (SessionManager.Instance.theSession.ThePlayer.isNew && !isNewReported)
			{
				isNewReported = true;
				string[] categories4 = new string[3] { "PRIVATE", "Installed", null };
				LogEventValue(categories4, "NewInstall", dataValue, (int)SessionManager.Instance.theSession.ThePlayer.InstalledDate);
				string[] array = empty.Split('_');
				int intValue = Convert.ToInt32(array[0]);
				int intValue2 = Convert.ToInt32(array[1]);
				string[] categories5 = new string[3] { "PRIVATE", "Installed", null };
				LogEventValue(categories5, "user_id1", dataValue, intValue);
				string[] categories6 = new string[3] { "PRIVATE", "Installed", null };
				LogEventValue(categories6, "user_id2", dataValue, intValue2);
				Singleton<PlayerInfoScript>.Instance.SaveData.LogUserTransactionServerHistory(SessionManager.Instance.theSession, Singleton<TBPvPManager>.Instance.CountryCode, GachaSlotDataManager.PremiumGachaCost, 9);
			}
			try
			{
				string[] categories7 = new string[3] { "PRIVATE", "CountryCode", null };
				LogEventValue(categories7, "CountryCode", Singleton<TBPvPManager>.Instance.CountryCode, BitConverter.ToInt32(IPAddress.Parse(Singleton<TBPvPManager>.Instance.IPAddress).GetAddressBytes(), 0));
			}
			catch
			{
			}
			string[] categories8 = new string[3] { "PRIVATE", "Currency1", null };
			LogEventValue(categories8, "HardCurrency", Convert.ToString(num3), num2);
			string[] categories9 = new string[3] { "PRIVATE", "Currency2", null };
			LogEventValue(categories9, "SoftCurrency", Convert.ToString(num4), num4);
			string[] categories10 = new string[3] { "PRIVATE", "Currency3", null };
			LogEventValue(categories10, "PvPCurrency", Convert.ToString(num5), num5);
			int currentStamina;
			int maxStamina;
			int secondsUntilNextStamina;
			DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Quests, out currentStamina, out maxStamina, out secondsUntilNextStamina);
			string[] categories11 = new string[3] { "PRIVATE", "Stamina1", null };
			LogEventValue(categories11, "Stamina", Convert.ToString(Singleton<PlayerInfoScript>.Instance.SaveData.OneTimeCalendarDaysClaimed), currentStamina);
			int rankXP = Singleton<PlayerInfoScript>.Instance.SaveData.RankXP;
			string[] categories12 = new string[3] { "PRIVATE", "XP", null };
			LogEventValue(categories12, "RankXP", Convert.ToString(Singleton<PlayerInfoScript>.Instance.SaveData.FriendInvites), rankXP);
			m_LastUpdateDay = day;
		}
	}

	public void LogLastAccessInterval(int diff)
	{
		int intValue = ((diff > 15552000) ? 180 : ((diff > 7776000) ? 90 : ((diff > 5184000) ? 60 : ((diff > 2592000) ? 30 : ((diff > 1209600) ? 14 : ((diff > 604800) ? 7 : ((diff > 518400) ? 6 : ((diff > 432000) ? 5 : ((diff > 345600) ? 4 : ((diff > 259200) ? 3 : ((diff <= 172800) ? 1 : 2)))))))))));
		string playerCode = Singleton<PlayerInfoScript>.Instance.GetPlayerCode();
		string[] categories = new string[3]
		{
			"PRIVATE",
			"Interval",
			Singleton<TBPvPManager>.Instance.CountryCode
		};
		LogEventValue(categories, "continues", Singleton<PlayerInfoScript>.Instance.GetPlayerName(), intValue);
	}

	public void LogInviteCodeSent(int date)
	{
		string[] categories = new string[3] { "PLAYER", "InviteSent", null };
		LogEvent(categories, "InviteSent", date.ToString());
	}

	public void LogInviteCodeUsed(int date)
	{
		string[] categories = new string[3] { "PLAYER", "InviteUsed", null };
		LogEvent(categories, "InviteUsed", date.ToString());
	}

	public void LogPvPSearch(string country)
	{
		string[] categories = new string[3] { "GAMEFLOW", "PvP", country };
		LogEvent(categories, "PvPSearch", Convert.ToString(DateTime.Now.Ticks / 10000000));
	}

	public void LogPvPFound(string side)
	{
		string[] categories = new string[3] { "GAMEFLOW", "PvP", side };
		LogEvent(categories, "PvPFound", Convert.ToString(DateTime.Now.Ticks / 10000000));
	}

	public void LogPvPStart(string side)
	{
		string[] categories = new string[3] { "GAMEFLOW", "PvP", side };
		LogEvent(categories, "PvPStart", Convert.ToString(DateTime.Now.Ticks / 10000000));
	}

	public void LogPvPLeagueDifference(int difference)
	{
		string[] categories = new string[3] { "GAMEFLOW", "PvP", "LeagueDifference" };
		LogEvent(categories, difference.ToString(), string.Empty);
	}

	public void LogPvPEnd(string reason)
	{
		string[] categories = new string[3] { "GAMEFLOW", "PvP", reason };
		LogEvent(categories, "PvPEnd", Convert.ToString(DateTime.Now.Ticks / 10000000));
	}

	public void logPvpRankAttained(int rank)
	{
		string[] categories = new string[3] { "PLAYER", "PvPRankAttained", null };
		LogEvent(categories, rank.ToString(), string.Empty);
	}

	public void LogQuestStart(string QuestID)
	{
		string[] categories = new string[3] { "GAMEFLOW", "Map", null };
		LogEvent(categories, "QuestStart", QuestID);
	}

	public void LogQuestWon(string QuestID)
	{
		string[] categories = new string[3] { "GAMEFLOW", "Map", null };
		LogEvent(categories, "QuestWon", QuestID);
	}

	public void LogQuestLost(string QuestID)
	{
		string[] categories = new string[3] { "GAMEFLOW", "Map", null };
		LogEvent(categories, "QuestLost", QuestID);
	}

	public void LogQuestQuit(string QuestID)
	{
		string[] categories = new string[3] { "GAMEFLOW", "Map", null };
		LogEvent(categories, "QuestQuit", QuestID);
	}

	public void LogQuestRevive(string QuestID)
	{
		string[] categories = new string[3] { "GAMEFLOW", "Map", null };
		LogEvent(categories, "QuestRevive", QuestID);
	}

	public void LogMultiplayerWon()
	{
		string[] categories = new string[3] { "GAMEFLOW", "Multiplayer", null };
		LogEvent(categories, "QuestWon", string.Empty);
	}

	public void LogMultiplayerLost()
	{
		string[] categories = new string[3] { "GAMEFLOW", "Multiplayer", null };
		LogEvent(categories, "QuestLost", string.Empty);
	}

	public void LogMultiplayerQuit()
	{
		string[] categories = new string[3] { "GAMEFLOW", "Multiplayer", null };
		LogEvent(categories, "QuestQuit", string.Empty);
	}

	public void LogFBLogin()
	{
		string[] categories = new string[3] { "GAMEFLOW", "Login", null };
		LogEvent(categories, "LoginFB", string.Empty);
	}

	public void LogGuestLogin()
	{
		string[] categories = new string[3] { "GAMEFLOW", "Login", null };
		LogEvent(categories, "LoginGuest", string.Empty);
	}

	public void logHighestQuest(int QuestID)
	{
		string[] categories = new string[3] { "PLAYER", "Quest", null };
		LogEvent(categories, "HighestQuest", QuestID.ToString());
	}

	public void logHighestRank(int Rank)
	{
		string[] categories = new string[3] { "PLAYER", "Rank", null };
		LogEvent(categories, "Rank", Rank.ToString());
	}

	public void logIAP(string ProductID)
	{
		string[] categories = new string[3] { "PLAYER", "IAP", null };
		LogEvent(categories, ProductID, string.Empty);
	}

	public void logNormalGatcha()
	{
		string[] categories = new string[3] { "PLAYER", "Normal Chest", null };
		LogEvent(categories, "OpenNormalChest", string.Empty);
	}

	public void logPremiumGatcha()
	{
		string[] categories = new string[3] { "PLAYER", "Premium Chest", null };
		LogEvent(categories, "OpenPremiumChest", string.Empty);
	}

	public void logSpecialGatcha()
	{
		string[] categories = new string[3] { "PLAYER", "Special Chest", null };
		LogEvent(categories, "OpenSpecialChest", string.Empty);
	}

	public void logHeroBought(string heroID)
	{
		string[] categories = new string[3] { "HERO", null, null };
		LogEvent(categories, "HeroBought", heroID);
	}

	public void logHeroSkinBought(string heroID)
	{
		string[] categories = new string[3] { "HERO", null, null };
		LogEvent(categories, "HeroSkinBought", heroID);
	}

	public void logHeroSkinEquip(string heroID)
	{
		string[] categories = new string[3] { "HERO", null, null };
		LogEvent(categories, "HeroSkinUsed", heroID);
	}

	public void logHeroUsed(bool pvp)
	{
		string[] categories = new string[3]
		{
			"HERO",
			(!pvp) ? "PvE" : "PvP",
			"Used"
		};
		LogEvent(categories, Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader.Form.ID, string.Empty);
	}

	public void logHeroWon(bool pvp)
	{
		string[] categories = new string[3]
		{
			"HERO",
			(!pvp) ? "PvE" : "PvP",
			"Won"
		};
		LogEvent(categories, Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader.Form.ID, string.Empty);
	}

	public void logCreatureUsed(string CreatureID, bool pvp)
	{
		string[] categories = new string[3]
		{
			"CREATURE",
			(!pvp) ? "PvE" : "PvP",
			"Used"
		};
		LogEvent(categories, CreatureID, string.Empty);
	}

	public void logCreatureWon(string CreatureID, bool pvp)
	{
		string[] categories = new string[3]
		{
			"CREATURE",
			(!pvp) ? "PvE" : "PvP",
			"Won"
		};
		LogEvent(categories, CreatureID, string.Empty);
	}

	public void logIncreaseCreatureInventory()
	{
		string[] categories = new string[3] { "PLAYER", "Creature", null };
		LogEvent(categories, "IncreaseInventory", string.Empty);
	}

	public void logIncreaseGemInventory()
	{
		string[] categories = new string[3] { "PLAYER", "Gem", null };
		LogEvent(categories, "IncreaseInventory", string.Empty);
	}

	public void logIncreaseExCardsInventory()
	{
		string[] categories = new string[3] { "PLAYER", "Card", null };
		LogEvent(categories, "IncreaseInventory", string.Empty);
	}

	public void logTutorialStateStarted(string stateID)
	{
		string[] categories = new string[3] { "TUTORIAL", "TutorialStateStart", null };
		LogEvent(categories, stateID, string.Empty);
	}

	public void logCreatureFuse(string creatureID)
	{
		string[] categories = new string[3] { "LAB", "Creature", null };
		LogEvent(categories, "PowerUp", creatureID);
	}

	public void logCreatureEvo(string creatureID)
	{
		string[] categories = new string[3] { "LAB", "Creature", null };
		LogEvent(categories, "Evo", creatureID);
	}

	public void logGemCraft(string gemID)
	{
		string[] categories = new string[3] { "LAB", "Gem", null };
		LogEvent(categories, "Craft", gemID);
	}

	public void logCardCraft(string cardID)
	{
		string[] categories = new string[3] { "LAB", "Card", null };
		LogEvent(categories, "Craft", cardID);
	}

	public void logGemFuse(string gemID)
	{
		string[] categories = new string[3] { "LAB", "Gem", null };
		LogEvent(categories, "PowerUp", gemID);
	}

	public void logAccountDepositePaid(int count, int totalcountPaid, string productIdentifier, string countrycode, string price)
	{
		if (!m_JailBroken)
		{
			string[] categories = new string[3]
			{
				"ACCOUNT_" + countrycode,
				"StAddPaid",
				price
			};
			LogEventValue(categories, productIdentifier, totalcountPaid.ToString(), count);
		}
	}

	public void logAccountConsumePaid(int count, int totalcountPaid, string reason)
	{
		if (!m_JailBroken)
		{
			string[] categories = new string[3] { "ACCOUNT", "StUsePaid", null };
			LogEventValue(categories, reason, totalcountPaid.ToString(), count);
		}
	}

	public void logAccountDepositeFree(int count, int totalcountFree, string productIdentifier)
	{
		if (!m_JailBroken)
		{
			string[] categories = new string[3] { "ACCOUNT", "StAddFree", null };
			LogEventValue(categories, productIdentifier, totalcountFree.ToString(), count);
		}
	}

	public void logAccountConsumeFree(int count, int totalcountFree, string reason)
	{
		if (!m_JailBroken)
		{
			string[] categories = new string[3] { "ACCOUNT", "StUseFree", null };
			LogEventValue(categories, reason, totalcountFree.ToString(), count);
		}
	}
}
