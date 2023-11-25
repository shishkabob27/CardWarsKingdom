using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using JsonFx.Json;
using MiniJSON;
using UnityEngine;

public class PlayerInfoScript : Singleton<PlayerInfoScript>
{
	private enum ServerCall
	{
		IDLE,
		PROGRESS,
		SUCCESS,
		ERROR,
		RETRY
	}

	private enum ServerCallType
	{
		NONE,
		CURRENCY,
		SYNC
	}

	private enum CollectionBit
	{
		Seen,
		Collected
	}

	private const string GAME_FILE = "game.json";

	private const string LAST_USER_FILE = "lastUserName";

	private const string _CROOZ_PROJ_ID_ANDROID = "498";

	private const string _CROOZ_PROJ_ID_IOS = "451";

	private const string _CROOZ_TERMS_ANDROID = "16300";

	private const string _CROOZ_TERMS_IOS = "16304";

	private const string _CROOZ_PRIVACY_ANDROID = "16302";

	private const string _CROOZ_PRIVACY_IOS = "16306";

	private const string _CROOZ_SUPPORTID_ANDROID = "1";

	private const string _CROOZ_SUPPORTID_IOS = "3";

	private const string _CROOZ_P = "gsdragonwars";

	public const string CreatureTypeId = "CR";

	public const string CardTypeId = "CA";

	public const string EvoMaterialTypeId = "EV";

	public const string XPMaterialTypeId = "XP";

	private const int SecondsMultiplier = 3600;

	private readonly TimeSpan SAVE_TIMEOUT_TIME = new TimeSpan(0, 1, 0);

	public PlayerSaveData SaveData;

	public GameStateData StateData;

	public PvPGameStateData PvPData;

	public bool NewSession;

	private SessionManager.OnSaveDelegate saveCbQueued;

	private string remoteSavePendingJson;

	private bool remoteSaving;

	private ServerCall inServerCall;

	private ServerCallType whichType;

	private int paidSave;

	private int freeSave;

	private string priceSave = string.Empty;

	private string eventNameSave = string.Empty;

	private int handleSave = -1;

	private bool initialSyncSave;

	private bool syncDone;

	private XPTableData mRankXPTable;

	private XPLevelData _RankXpLevelData;

	private PlayerRankData _RankData;

	public string crooz_loginkey = string.Empty;

	public float MinPauseResetTimeSec = 900f;

	private static DateTime PauseBeginTime;

	private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

	private static string DaysString = null;

	private static string HoursString = null;

	private static string MinutesString = null;

	private static string SecondsString = null;

	public bool ServerSyncDone
	{
		get
		{
			return syncDone;
		}
	}

	public XPLevelData RankXpLevelData
	{
		get
		{
			if (_RankXpLevelData == null && mRankXPTable != null)
			{
				RefreshRankXpData();
			}
			return _RankXpLevelData;
		}
	}

	public bool HasAuthedBefore
	{
		get
		{
			return SaveData.HasAuthenticated;
		}
	}

	public PlayerRankData RankData
	{
		get
		{
			if (_RankData == null && mRankXPTable != null)
			{
				RefreshRankXpData();
			}
			return _RankData;
		}
	}

	public bool LoginAttempted { get; protected set; }

	public bool IsInitialized { get; protected set; }

	public string crooz_projID
	{
		get
		{
			return "498";
		}
	}

	public string crooz_supportID
	{
		get
		{
			return "1";
		}
	}

	public string crooz_p
	{
		get
		{
			return "gsdragonwars";
		}
	}

	public event Action OnLoginFail;

	public event Action OnFBLoginCancel;

	public XPLevelData RankXpLevelDataAt(int rankXp)
	{
		return mRankXPTable.GetLevelData(rankXp);
	}

	public int GetXpToReachRank(int rank)
	{
		return mRankXPTable.GetXpToReachLevel(rank);
	}

	public void RefreshRankXpData()
	{
		if (mRankXPTable != null)
		{
			_RankXpLevelData = mRankXPTable.GetLevelData(SaveData.RankXP);
			_RankData = PlayerRankDataManager.Instance.GetData(_RankXpLevelData.mCurrentLevel - 1);
		}
	}

	public bool IsFacebookLogin()
	{
		return false;
	}

	public string GetPlayerCode()
	{
		SessionManager instance = SessionManager.Instance;
		return instance.PlayerID;
	}

	public string GetCroozLoginKey()
	{
		return crooz_loginkey;
	}

	public string GetSupportLink()
	{
		return MiscParams.CompassSupportURL + "?project_id=" + crooz_projID + "&support_id=" + crooz_supportID + "&p=" + crooz_p + "&k=" + GetCroozLoginKey();
	}

	public string GetFormattedPlayerCode()
	{
		return GetPlayerCode().Replace('_', '-');
	}

	public string ConvertFormattedPlayerCodeToInternal(string formattedCode)
	{
		return formattedCode.Replace('-', '_');
	}

	public string GetPlayerName()
	{
		return SaveData.MultiplayerPlayerName;
	}

	public Texture2D GetPlayerPortrait()
	{
		if (IsFacebookLogin())
		{
			return Singleton<KFFSocialManager>.Instance.FBUser.Avatar;
		}
		return null;
	}

	private void InitSaveData()
	{
		SaveData = new PlayerSaveData();
		StateData = new GameStateData();
		PvPData = new PvPGameStateData();
		SaveData.version = 1;
		SaveData.PlayerName = LoadPlayerName();
		if (string.IsNullOrEmpty(SaveData.PlayerName))
		{
			SaveData.PlayerName = Guid.NewGuid().ToString();
			SavePlayerName();
		}
		LoginAttempted = false;
		IsInitialized = false;
	}

	private void OnUserLoginFail()
	{
		SessionManager instance = SessionManager.Instance;
		if (this.OnLoginFail != null)
		{
			LoginAttempted = false;
			instance.Logout();
			this.OnLoginFail();
		}
		else
		{
			instance.LoadPlayerFromFileSystem();
		}
	}

	private void OnEnable()
	{
		SessionManager instance = SessionManager.Instance;
		instance.OnUserLoginFail += OnUserLoginFail;
	}

	private void OnDisable()
	{
		SessionManager instance = SessionManager.Instance;
		if (instance != null)
		{
			instance.OnUserLoginFail -= OnUserLoginFail;
		}
	}

	private void Awake()
	{
		InitSaveData();
	}

	private void Update()
	{
		if (inServerCall == ServerCall.IDLE)
		{
			return;
		}
		if (inServerCall == ServerCall.SUCCESS)
		{
			Singleton<BusyIconPanelController>.Instance.Hide();
			inServerCall = ServerCall.IDLE;
			if (whichType == ServerCallType.SYNC)
			{
				syncDone = true;
			}
			else
			{
				Singleton<StoreScreenController>.Instance.TriggerHardCurrencyGainTick();
			}
			SaveLocal();
		}
		else if (inServerCall == ServerCall.ERROR)
		{
			Singleton<BusyIconPanelController>.Instance.Hide();
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), OnCloseServerAccessErrorPopup);
		}
	}

	private void OnCloseServerAccessErrorPopup()
	{
		if (whichType == ServerCallType.CURRENCY)
		{
			AddHardCurrency2(paidSave, freeSave, eventNameSave, handleSave, priceSave);
		}
		else if (whichType == ServerCallType.SYNC)
		{
			User_Sync(initialSyncSave);
		}
	}

	public void OnApplicationPause(bool pause)
	{
	}

	public bool IsReady()
	{
		SessionManager instance = SessionManager.Instance;
		return instance.IsReady();
	}

	public void AddNewPlayerInfo()
	{
		IsInitialized = true;
	}

	public void ResetSaveData(SessionManager.OnSaveDelegate callback = null)
	{
		if (SaveData == null)
		{
			if (callback != null)
			{
				callback(true);
			}
			return;
		}
		string playerName = SaveData.PlayerName;
		SaveData = new PlayerSaveData();
		StateData = new GameStateData();
		PvPData = new PvPGameStateData();
		SaveData.version = 1;
		SaveData.PlayerName = playerName;
		InitNewSaveFile();
		Save(callback);
	}

	public static void SavePlayerName()
	{
		PlayerPrefs.SetString("lastUserName", Singleton<PlayerInfoScript>.Instance.SaveData.PlayerName);
		PlayerPrefs.Save();
	}

	public static string LoadPlayerName()
	{
		string text = Path.Combine(Application.persistentDataPath, "lastUserName");
		string text2 = GetOldPlayerName();
		if (text2.Length == 0)
		{
			text2 = PlayerPrefs.GetString("lastUserName");
		}
		if (text2.Length == 0)
		{
			string oldPlayerName = GetOldPlayerName();
			if (oldPlayerName.Length != 0)
			{
				return oldPlayerName;
			}
			string persistentDataPath = Application.persistentDataPath;
			if (persistentDataPath.Contains("/0/"))
			{
				string text3 = persistentDataPath.Replace("/0/", "/");
				string path = Path.Combine(text3, "lastUserName");
				if (File.Exists(path))
				{
					string text4 = File.ReadAllText(path);
					PlayerPrefs.SetString("lastUserName", text4);
					PlayerPrefs.Save();
					CopyContents(text3, persistentDataPath);
					return text4;
				}
				string text5 = persistentDataPath.Replace("/0/com.", "/com.");
				string path2 = Path.Combine(text5, "lastUserName");
				if (File.Exists(path2))
				{
					string text6 = File.ReadAllText(path2);
					PlayerPrefs.SetString("lastUserName", text6);
					PlayerPrefs.Save();
					CopyContents(text5, persistentDataPath);
					return text6;
				}
				return null;
			}
		}
		return text2;
	}

	private static string GetOldPlayerName()
	{
		string persistentDataPath = Application.persistentDataPath;
		string path = Path.Combine(persistentDataPath, "lastUserName");
		if (File.Exists(path))
		{
			string text = File.ReadAllText(path);
			PlayerPrefs.SetString("lastUserName", text);
			PlayerPrefs.Save();
			return text;
		}
		return string.Empty;
	}

	private static void CopyContents(string srcPath, string destPath)
	{
		string[] directories = Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories);
		foreach (string text in directories)
		{
			Directory.CreateDirectory(text.Replace(srcPath, destPath));
		}
		string[] files = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories);
		foreach (string text2 in files)
		{
			File.Copy(text2, text2.Replace(srcPath, destPath), true);
		}
	}

	public static void ResetPlayerName()
	{
		PlayerPrefs.DeleteKey("lastUserName");
		string path = Path.Combine(Application.persistentDataPath, "lastUserName");
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	private void DeleteSaveData()
	{
		SessionManager.Instance.DeleteLocal();
		ResetPlayerName();
	}

	public void Login()
	{
		DoLogin();
	}

	public void DoLogin()
	{
		if (!LoginAttempted)
		{
			LoginAttempted = true;
			SessionManager instance = SessionManager.Instance;
			instance.OnReadyCallback = crooz_LoginToCompassHelp;
			instance.Login(false, null, SaveData.PlayerName);
		}
	}

	public void Logout(bool clearUserdata)
	{
		LoginAttempted = false;
		SaveData.PlayerName = string.Empty;
		IsInitialized = false;
		if (clearUserdata)
		{
			DeleteSaveData();
		}
	}

	public void crooz_LoginToCompassHelp()
	{
		if (Language.CurrentLanguage() == LanguageCode.JA)
		{
			string playerCode = GetPlayerCode();
			string text = DateTimeOffset.UtcNow.ToString("yyyyMMdd");
			string sourcevalue = crooz_projID + "_" + crooz_supportID + "_" + playerCode + "_" + text;
			SessionManager.Instance.theSession.Server.CompassSupportLogin(crooz_projID, crooz_supportID, crooz_p, playerCode, Singleton<PlayerInfoScript>.Instance.SaveData.GetSHA256STRINGASCII(sourcevalue).ToLower(), crooz_logincallback);
		}
	}

	private void crooz_logincallback(string jsonResponse, HttpStatusCode status)
	{
		Hashtable hashtable = JSON.ParseJSON(jsonResponse);
		string text = string.Empty;
		if (hashtable.Contains("result"))
		{
			text = hashtable["result"].ToString();
		}
		if (text == "1" && hashtable.Contains("loginkey"))
		{
			crooz_loginkey = hashtable["loginkey"].ToString();
		}
	}

	public static bool SaveFileExists()
	{
		string playerDataPath = SessionManager.Instance.GetPlayerDataPath("game.json");
		return File.Exists(playerDataPath);
	}

	public static string MakeJS(string key, object val)
	{
		string text = "\"" + key + "\":";
		if (val is string || val is ObscuredString)
		{
			return string.Concat(text, "\"", val, "\"");
		}
		if (val is bool || val is ObscuredBool)
		{
			return text + ((!(bool)val) ? "0" : "1");
		}
		if (val != null)
		{
			return text + val.ToString();
		}
		return text;
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append(MakeJS("PlayerName", SaveData.PlayerName) + ",");
		stringBuilder.Append(MakeJS("MultiplayerPlayerName", SaveData.MultiplayerPlayerName) + ",");
		stringBuilder.Append(MakeJS("HasAuthenticated", SaveData.HasAuthenticated) + ",");
		stringBuilder.Append(MakeJS("AddedStones", SaveData.Muryo.ToString("X")) + ",");
		stringBuilder.Append(MakeJS("ReadWriteParam", SaveData.ReadWriteParam) + ",");
		stringBuilder.Append(MakeJS("SelectedLoadout", SaveData.SelectedLoadout) + ",");
		stringBuilder.Append(MakeJS("MyHelperCreatureID", SaveData.MyHelperCreatureID) + ",");
		stringBuilder.Append(MakeJS("SoftCurrency", SaveData.SoftCurrency) + ",");
		stringBuilder.Append(MakeJS("CustomizationCurrency", SaveData.CustomizationCurrency) + ",");
		stringBuilder.Append(MakeJS("PaidHardCurrency", SaveData.PaidHardCurrency) + ",");
		stringBuilder.Append(MakeJS("FreeHardCurrency", SaveData.FreeHardCurrency) + ",");
		stringBuilder.Append(MakeJS("PvpCurrency", SaveData.PvPCurrency) + ",");
		stringBuilder.Append(MakeJS("RankXP", SaveData.RankXP) + ",");
		stringBuilder.Append(MakeJS("StaminaTime", SaveData.StaminaFullAtTime) + ",");
		stringBuilder.Append(MakeJS("PvpStaminaTime", SaveData.PvpStaminaFullAtTime) + ",");
		stringBuilder.Append(MakeJS("ExtraQuestStamina", SaveData.ExtraQuestStamina) + ",");
		stringBuilder.Append(MakeJS("ExtraPvpStamina", SaveData.ExtraPvpStamina) + ",");
		stringBuilder.Append(MakeJS("InventorySpace", SaveData.InventorySpace) + ",");
		stringBuilder.Append(MakeJS("AllyBoxSpace", SaveData.AllyBoxSpace) + ",");
		stringBuilder.Append(MakeJS("TopCompletedQuestId", SaveData.TopCompletedQuestId) + ",");
		stringBuilder.Append(MakeJS("TopShownCompletedQuestId", SaveData.TopShownCompletedQuestId) + ",");
		stringBuilder.Append(MakeJS("PvpBattles", SaveData.PvpBattles) + ",");
		stringBuilder.Append(MakeJS("FriendInvites", SaveData.FriendInvites) + ",");
		stringBuilder.Append(MakeJS("QuestSelectScrollPos", SaveData.QuestSelectScrollPos) + ",");
		stringBuilder.Append(MakeJS("LastAccessUTC", SaveData.LastAccessUTC) + ",");
		stringBuilder.Append(MakeJS("InstalledDate", SaveData.InstalledDate) + ",");
		stringBuilder.Append(MakeJS("RandomDungeonLevel", SaveData.RandomDungeonLevel) + ",");
		stringBuilder.Append(MakeJS("SecurityKeyValue", SaveData.Harai.ToString("X")) + ",");
		if (SaveData.LastKnownLocation != null)
		{
			stringBuilder.Append(MakeJS("LastKnownLocation", SaveData.LastKnownLocation) + ",");
		}
		stringBuilder.Append(MakeJS("PlayersLastSavedLevel", SaveData.PlayersLastSavedLevel) + ",");
		stringBuilder.Append(MakeJS("MultiplayerLevel", SaveData.MultiplayerLevel) + ",");
		stringBuilder.Append(MakeJS("InviteGroup", SaveData.ChatSpecialDomainNumber) + ",");
		stringBuilder.Append(MakeJS("BestMultiplayerLevel", SaveData.BestMultiplayerLevel) + ",");
		stringBuilder.Append(MakeJS("PointsInMultiplayerLevel", SaveData.PointsInMultiplayerLevel) + ",");
		stringBuilder.Append(MakeJS("MultiplayerWinStreak", SaveData.MultiplayerWinStreak) + ",");
		stringBuilder.Append(MakeJS("PvpRankRewardsGranted", SaveData.PvpRankRewardsGranted) + ",");
		stringBuilder.Append(MakeJS("PvpPlayed", SaveData.PlayedFirstBattleInPvpSeason) + ",");
		stringBuilder.Append(MakeJS("PvpMatchStarted", SaveData.RankedPvpMatchStarted) + ",");
		stringBuilder.Append(MakeJS("Zxcvbnm", SaveData.PvpSpecialDomainNumber) + ",");
		stringBuilder.Append(MakeJS("TownTiltCam", SaveData.TownTiltCam) + ",");
		stringBuilder.Append(MakeJS("InviteCodeRedeemed", SaveData.InviteCodeRedeemed) + ",");
		stringBuilder.Append(MakeJS("CDExpeditionSlots", SaveData.ExpeditionSlots) + ",");
		stringBuilder.Append(MakeJS("DateOfBirth", SaveData.DateOfBirth) + ",");
		stringBuilder.Append(MakeJS("ConfirmedTOSVersion", SaveData.ConfirmedTOSVersion) + ",");
		if (SaveData.SelectedSale != null)
		{
			stringBuilder.Append(MakeJS("SelectedSale", SaveData.SelectedSale.ID) + ",");
		}
		stringBuilder.Append(MakeJS("SaleShowCooldown", SaveData.SaleShowCooldown) + ",");
		stringBuilder.Append(MakeJS("SaleEndTime", SaveData.SaleEndTime) + ",");
		stringBuilder.Append(MakeJS("SaleRestartCooldown", SaveData.SaleRestartCooldown) + ",");
		stringBuilder.Append(MakeJS("UIDFixApplied", SaveData.UIDFixApplied) + ",");
		stringBuilder.Append(MakeJS("selectedLang", SaveData.selectedLang.ToString()) + ",");
		stringBuilder.Append(MakeJS("RankUpInventoryGiven", SaveData.RankUpInventoryGiven.ToString()) + ",");
		stringBuilder.Append(MakeJS("ExpeditionRefreshTime", SaveData.ExpeditionRefreshTime) + ",");
		if (SaveData.SelectedPortrait != null)
		{
			stringBuilder.Append(MakeJS("SelectedPortrait", SaveData.SelectedPortrait.ID) + ",");
		}
		if (SaveData.SelectedCardBack != null)
		{
			stringBuilder.Append(MakeJS("SelectedCardBack", SaveData.SelectedCardBack.ID) + ",");
		}
		stringBuilder.Append(MakeJS("TitleDisplayTime", SaveData.Yuryo) + ",");
		if (SaveData.ActivePvpSeason != null)
		{
			stringBuilder.Append(MakeJS("ActivePvpSeason", SaveData.ActivePvpSeason.ID) + ",");
		}
		stringBuilder.Append("\"Unlocks\":" + SerializeUnlocks() + ",");
		stringBuilder.Append("\"Leaders\":" + SerializeLeaders() + ",");
		stringBuilder.Append("\"Inventory\":" + SerializeInventory() + ",");
		stringBuilder.Append("\"Mails\":" + SerializeAdminMessages() + ",");
		stringBuilder.Append("\"Collection\":" + SerializeCreatureCollection() + ",");
		stringBuilder.Append("\"CardCollection\":" + SerializeCardCollection() + ",");
		stringBuilder.Append("\"EvoMatCollection\":" + SerializeEvoMatCollection() + ",");
		stringBuilder.Append("\"XPMatCollection\":" + SerializeXPMatCollection() + ",");
		stringBuilder.Append("\"Loadouts\":" + SerializeLoadouts() + ",");
		stringBuilder.Append("\"TutorialsDone\":" + TutorialDataManager.Instance.Serialize() + ",");
		stringBuilder.Append("\"CreatureSort\":" + SerializeCreatureSorts() + ",");
		stringBuilder.Append("\"ExCardSort\":" + SerializeExCardSorts() + ",");
		stringBuilder.Append("\"HelperSort\":" + SerializeHelperSorts() + ",");
		stringBuilder.Append("\"FBInviteRewards\":" + SerializeFBRewards() + ",");
		stringBuilder.Append("\"DeletedMail\":" + SerializeDeletedMail() + ",");
		stringBuilder.Append("\"SpecialQuests\":" + SerializeDoneSpecialQuests() + ",");
		stringBuilder.Append("\"MissionProgress\":" + DetachedSingleton<MissionManager>.Instance.Serialize() + ",");
		stringBuilder.Append("\"QuestStars\":" + SerializeQuestStars() + ",");
		stringBuilder.Append("\"DungeonStars\":" + SerializeDungeonStars() + ",");
		stringBuilder.Append("\"CardBacks\":" + SerializeCardBacks() + ",");
		stringBuilder.Append("\"Portraits\":" + SerializePortraits() + ",");
		stringBuilder.Append("\"Titles\":" + SerializeTitles() + ",");
		stringBuilder.Append("\"Badges\":" + SerializeBadges() + ",");
		stringBuilder.Append("\"CDExpeditions\":" + DetachedSingleton<ExpeditionManager>.Instance.Serialize() + ",");
		stringBuilder.Append("\"SpeedUps\":" + SerializeSpeedUps() + ",");
		stringBuilder.Append("\"DungeonMaps\":" + SerializeDungeonMaps() + ",");
		stringBuilder.Append("\"BattleHistory\":" + SerializeBattleHistory() + ",");
		stringBuilder.Append("\"IgnoredPlayers\":" + SerializeIgnoredPlayers() + ",");
		stringBuilder.Append("\"Sales\":" + SerializeSales() + ",");
		stringBuilder.Append("\"GachaCooldowns\":" + SerializeGachaCooldowns() + ",");
		stringBuilder.Append("\"GachaKeys\":" + SerializeGachaKeys() + ",");
		stringBuilder.Append(MakeJS("MapClickInterval", SaveData.Tada) + ",");
		if (SaveData.ActiveCalendar != null)
		{
			stringBuilder.Append(MakeJS("ActiveCalendar", SaveData.ActiveCalendar.ID) + ",");
		}
		if ((int)SaveData.OneTimeCalendarDaysClaimed > 0)
		{
			stringBuilder.Append(MakeJS("OneTimeCalendarDaysClaimed", SaveData.OneTimeCalendarDaysClaimed) + ",");
			stringBuilder.Append(MakeJS("LastOneTimeCalendarDateClaimed", SaveData.LastOneTimeCalendarDateClaimed.ToString()) + ",");
		}
		stringBuilder.Append(MakeJS("DailyMissionTimestamp", SaveData.DailyMissionTimestamp.ToString()) + ",");
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public string PvPSerialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('{');
		stringBuilder.Append("\"Creatures\":" + SerializePvpCreatures() + ",");
		stringBuilder.Append("\"Loadout\":" + GetCurrentLoadout().Serialize() + ",");
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	public Loadout PvPDeserialize(string loadout)
	{
		Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(loadout);
		Loadout loadout2 = new Loadout();
		List<CreatureItem> list = new List<CreatureItem>();
		if (dictionary.ContainsKey("Creatures"))
		{
			object[] array = (object[])dictionary["Creatures"];
			int num = 0;
			object[] array2 = array;
			foreach (object obj in array2)
			{
				if (obj != null)
				{
					list.Add(new CreatureItem(obj as Dictionary<string, object>, true));
				}
			}
		}
		if (dictionary.ContainsKey("Loadout"))
		{
			loadout2.Deserialize((Dictionary<string, object>)dictionary["Loadout"], list);
		}
		return loadout2;
	}

	private string SerializePvpCreatures()
	{
		Loadout currentLoadout = GetCurrentLoadout();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < SaveData.InventorySlots.Count; i++)
		{
			InventorySlotItem inventorySlotItem = SaveData.InventorySlots[i];
			if (inventorySlotItem.SlotType == InventorySlotType.Creature && currentLoadout.CreatureSet.Contains(inventorySlotItem))
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(inventorySlotItem.Creature.Serialize());
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	public string SerializeHelperCreature()
	{
		StringBuilder stringBuilder = new StringBuilder();
		InventorySlotItem myHelperCreature = GetMyHelperCreature();
		stringBuilder.Append(myHelperCreature.Creature.Serialize());
		return stringBuilder.ToString();
	}

	public InventorySlotItem DeserializeHelperCreature(string helper)
	{
		Dictionary<string, object> dict = JsonReader.Deserialize<Dictionary<string, object>>(helper);
		CreatureItem creature = new CreatureItem(dict, true);
		return new InventorySlotItem(creature);
	}

	public void Deserialize(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			return;
		}
		NewSession = true;
		Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(json);
		SaveData.version = TFUtils.LoadInt(dictionary, "PlayerInfoVersion", 1);
		if (SaveData.version >= 1)
		{
			SaveData.HasAuthenticated = TFUtils.LoadBool(dictionary, "HasAuthenticated", false);
			SaveData.MultiplayerPlayerName = TFUtils.LoadString(dictionary, "MultiplayerPlayerName", string.Empty);
			SaveData.SelectedLoadout = TFUtils.LoadInt(dictionary, "SelectedLoadout", 0);
			string text = TFUtils.LoadString(dictionary, "SelectedInitFaction", "Red");
			SaveData.MyHelperCreatureID = TFUtils.LoadInt(dictionary, "MyHelperCreatureID", -1);
			SaveData.SoftCurrency = TFUtils.LoadInt(dictionary, "SoftCurrency", 0);
			SaveData.CustomizationCurrency = TFUtils.LoadInt(dictionary, "CustomizationCurrency", 0);
			SaveData.ManualSetHardCurrency(TFUtils.LoadInt(dictionary, "PaidHardCurrency", 0), TFUtils.LoadInt(dictionary, "FreeHardCurrency", 0));
			SaveData.ManualSetCurrencyHash(TFUtils.LoadInt(dictionary, "TitleDisplayTime", 41592), TFUtils.LoadInt(dictionary, "MapClickInterval", 65358));
			SaveData.PvPCurrency = TFUtils.LoadInt(dictionary, "PvpCurrency", 0);
			SaveData.RankXP = TFUtils.LoadInt(dictionary, "RankXP", 0);
			SaveData.StaminaFullAtTime = TFUtils.LoadUint(dictionary, "StaminaTime", 0u);
			SaveData.PvpStaminaFullAtTime = TFUtils.LoadUint(dictionary, "PvpStaminaTime", 0u);
			SaveData.ExtraPvpStamina = TFUtils.LoadUint(dictionary, "ExtraPvpStamina", 0u);
			SaveData.ExtraQuestStamina = TFUtils.LoadUint(dictionary, "ExtraQuestStamina", 0u);
			SaveData.InventorySpace = TFUtils.LoadInt(dictionary, "InventorySpace", MiscParams.StartingInventorySpace);
			SaveData.AllyBoxSpace = TFUtils.LoadInt(dictionary, "AllyBoxSpace", MiscParams.StartingAllyBoxSpace);
			if ((int)SaveData.AllyBoxSpace < MiscParams.StartingAllyBoxSpace)
			{
				SaveData.AllyBoxSpace = MiscParams.StartingAllyBoxSpace;
			}
			SaveData.TopCompletedQuestId = TFUtils.LoadInt(dictionary, "TopCompletedQuestId", 0);
			SaveData.TopShownCompletedQuestId = TFUtils.LoadInt(dictionary, "TopShownCompletedQuestId", 0);
			SaveData.PvpBattles = TFUtils.LoadInt(dictionary, "PvpBattles", 0);
			SaveData.QuestSelectScrollPos = TFUtils.LoadFloat(dictionary, "QuestSelectScrollPos", 0f);
			SaveData.LastAccessUTC = TFUtils.LoadInt(dictionary, "LastAccessUTC", 0);
			SaveData.LastKnownLocation = TFUtils.LoadString(dictionary, "LastKnownLocation", string.Empty);
			SaveData.FriendInvites = TFUtils.LoadInt(dictionary, "FriendInvites", 0);
			SaveData.PlayersLastSavedLevel = TFUtils.LoadInt(dictionary, "PlayersLastSavedLevel", _RankXpLevelData.mCurrentLevel);
			SaveData.BestMultiplayerLevel = TFUtils.LoadInt(dictionary, "BestMultiplayerLevel", -1);
			SaveData.MultiplayerLevel = TFUtils.LoadInt(dictionary, "MultiplayerLevel", -1);
			SaveData.PointsInMultiplayerLevel = TFUtils.LoadInt(dictionary, "PointsInMultiplayerLevel", 0);
			SaveData.MultiplayerWinStreak = TFUtils.LoadInt(dictionary, "MultiplayerWinStreak", 0);
			SaveData.PvpRankRewardsGranted = TFUtils.LoadInt(dictionary, "PvpRankRewardsGranted", -1);
			SaveData.PlayedFirstBattleInPvpSeason = TFUtils.LoadBool(dictionary, "PvpPlayed", false);
			SaveData.RankedPvpMatchStarted = TFUtils.LoadBool(dictionary, "PvpMatchStarted", false);
			SaveData.TownTiltCam = TFUtils.LoadBool(dictionary, "TownTiltCam", true);
			SaveData.PvpSpecialDomainNumber = TFUtils.LoadInt(dictionary, "Zxcvbnm", 0);
			SaveData.ChatSpecialDomainNumber = TFUtils.LoadInt(dictionary, "InviteGroup", 0);
			SaveData.InviteCodeRedeemed = TFUtils.LoadBool(dictionary, "InviteCodeRedeemed", false);
			SaveData.InstalledDate = TFUtils.LoadInt(dictionary, "InstalledDate", 0);
			SaveData.SaleShowCooldown = TFUtils.LoadUint(dictionary, "SaleShowCooldown", 0u);
			SaveData.SaleEndTime = TFUtils.LoadUint(dictionary, "SaleEndTime", 0u);
			SaveData.SaleRestartCooldown = TFUtils.LoadUint(dictionary, "SaleRestartCooldown", 0u);
			SaveData.ExpeditionSlots = TFUtils.LoadInt(dictionary, "CDExpeditionSlots", ExpeditionParams.StartingExpeditionSlots);
			SaveData.RandomDungeonLevel = TFUtils.LoadInt(dictionary, "RandomDungeonLevel", 1);
			SaveData.DateOfBirth = TFUtils.LoadUint(dictionary, "DateOfBirth", 0u);
			SaveData.ConfirmedTOSVersion = TFUtils.LoadInt(dictionary, "ConfirmedTOSVersion", 0);
			string text2 = TFUtils.LoadString(dictionary, "SelectedSale", null);
			if (text2 != null)
			{
				SaveData.SelectedSale = SpecialSaleDataManager.Instance.GetData(text2);
			}
			SaveData.UIDFixApplied = TFUtils.LoadBool(dictionary, "UIDFixApplied", false);
			SaveData.RankUpInventoryGiven = TFUtils.LoadBool(dictionary, "RankUpInventoryGiven", false);
			string text3 = TFUtils.LoadString(dictionary, "selectedLang", string.Empty);
			if (text3 == string.Empty)
			{
				SaveData.selectedLang = LanguageCode.N;
			}
			else
			{
				try
				{
					SaveData.selectedLang = Language.LanguageNameToCode((SystemLanguage)(int)Enum.Parse(typeof(SystemLanguage), text3, true));
				}
				catch
				{
					SaveData.selectedLang = LanguageCode.N;
				}
			}
			SaveData.SelectedPortrait = PlayerPortraitDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "SelectedPortrait", "Default"));
			SaveData.SelectedCardBack = CardBackDataManager.Instance.GetData(TFUtils.LoadString(dictionary, "SelectedCardBack", string.Empty));
			if (SaveData.SelectedCardBack == null)
			{
				SaveData.SelectedCardBack = CardBackDataManager.DefaultData;
			}
			string text4 = TFUtils.LoadString(dictionary, "ActivePvpSeason", null);
			if (text4 != null)
			{
				SaveData.ActivePvpSeason = PvpSeasonDataManager.Instance.GetData(text4);
			}
			else
			{
				SaveData.ActivePvpSeason = null;
			}
			int num = TFUtils.LoadInt(dictionary, "HardCurrency", 0);
			if (num > 0)
			{
				SaveData.ManualSetHardCurrency(SaveData.PaidHardCurrency + num, SaveData.FreeHardCurrency);
			}
			if (dictionary.ContainsKey("Unlocks"))
			{
				DeserializeUnlocks((object[])dictionary["Unlocks"]);
			}
			if (dictionary.ContainsKey("Leaders"))
			{
				DeserializeLeaders((object[])dictionary["Leaders"]);
			}
			if (dictionary.ContainsKey("Collection"))
			{
				DeserializeCreatureCollection((object[])dictionary["Collection"]);
			}
			if (dictionary.ContainsKey("CardCollection"))
			{
				DeserializeCardCollection((object[])dictionary["CardCollection"]);
			}
			if (dictionary.ContainsKey("EvoMatCollection"))
			{
				DeserializeEvoMatCollection((object[])dictionary["EvoMatCollection"]);
			}
			if (dictionary.ContainsKey("XPMatCollection"))
			{
				DeserializeXPMatCollection((object[])dictionary["XPMatCollection"]);
			}
			if (dictionary.ContainsKey("Inventory"))
			{
				DeserializeInventory((object[])dictionary["Inventory"]);
			}
			if (dictionary.ContainsKey("Loadouts"))
			{
				DeserializeLoadouts((object[])dictionary["Loadouts"]);
			}
			if (SaveData.MyHelperCreatureID == -1)
			{
				SetDefaultHelperCreature();
			}
			if (dictionary.ContainsKey("TutorialsDone"))
			{
				TutorialDataManager.Instance.Deserialize((object[])dictionary["TutorialsDone"]);
			}
			if (dictionary.ContainsKey("CreatureSort"))
			{
				DeserializeCreatureSorts((object[])dictionary["CreatureSort"]);
			}
			if (dictionary.ContainsKey("ExCardSort"))
			{
				DeserializeExCardSorts((object[])dictionary["ExCardSort"]);
			}
			if (dictionary.ContainsKey("HelperSort"))
			{
				DeserializeHelperSorts((object[])dictionary["HelperSort"]);
			}
			if (dictionary.ContainsKey("Mails"))
			{
				DeserializeAdminMessages((object[])dictionary["Mails"]);
			}
			if (dictionary.ContainsKey("FBInviteRewards"))
			{
				DeserializeFBRewards((object[])dictionary["FBInviteRewards"]);
			}
			if (dictionary.ContainsKey("DeletedMail"))
			{
				DeserializeDeletedMail((object[])dictionary["DeletedMail"]);
			}
			if (dictionary.ContainsKey("SpecialQuests"))
			{
				DeserializeDoneSpecialQuests(dictionary["SpecialQuests"]);
			}
			if (dictionary.ContainsKey("MissionProgress"))
			{
				DetachedSingleton<MissionManager>.Instance.Deserialize(dictionary["MissionProgress"] as Dictionary<string, object>);
			}
			else
			{
				DetachedSingleton<MissionManager>.Instance.Init();
			}
			SaveData.ExpeditionRefreshTime = TFUtils.LoadUint(dictionary, "ExpeditionRefreshTime", 0u);
			if (dictionary.ContainsKey("CDExpeditions") && dictionary["CDExpeditions"] is object[])
			{
				DetachedSingleton<ExpeditionManager>.Instance.Deserialize((object[])dictionary["CDExpeditions"]);
			}
			else
			{
				DetachedSingleton<ExpeditionManager>.Instance.AssignNewExpeditions();
			}
			if (dictionary.ContainsKey("DungeonMaps"))
			{
				DeserializeDungeonMaps(dictionary["DungeonMaps"] as Dictionary<string, object>);
			}
			if (dictionary.ContainsKey("QuestStars"))
			{
				DeserializeQuestStars(dictionary["QuestStars"] as int[]);
			}
			if (dictionary.ContainsKey("DungeonStars"))
			{
				DeserializeDungeonStars((Dictionary<string, object>)dictionary["DungeonStars"]);
			}
			if (dictionary.ContainsKey("CardBacks"))
			{
				DeserializeCardBacks((object[])dictionary["CardBacks"]);
			}
			else
			{
				SaveData.UnlockedCardBacks.Add(CardBackDataManager.Instance.GetData("Default"));
			}
			if (dictionary.ContainsKey("Portraits"))
			{
				DeserializePortraits((object[])dictionary["Portraits"]);
			}
			if (dictionary.ContainsKey("Badges"))
			{
				DeserializeBadges((object[])dictionary["Badges"]);
			}
			if (dictionary.ContainsKey("Titles"))
			{
				DeserializeTitles((object[])dictionary["Titles"]);
			}
			if (dictionary.ContainsKey("BattleHistory"))
			{
				DeserializeBattleHistory((object[])dictionary["BattleHistory"]);
			}
			if (dictionary.ContainsKey("IgnoredPlayers"))
			{
				DeserializeIgnoredPlayers((object[])dictionary["IgnoredPlayers"]);
			}
			if (dictionary.ContainsKey("Sales"))
			{
				DeserializeSales((object[])dictionary["Sales"]);
			}
			if (dictionary.ContainsKey("GachaCooldowns"))
			{
				DeserializeGachaCooldowns((object[])dictionary["GachaCooldowns"]);
			}
			if (dictionary.ContainsKey("SpeedUps"))
			{
				DeserializeSpeedUps((object[])dictionary["SpeedUps"]);
			}
			if (dictionary.ContainsKey("GachaKeys"))
			{
				DeserializeGachaKeys((object[])dictionary["GachaKeys"]);
			}
			string text5 = TFUtils.LoadString(dictionary, "ActiveCalendar", null);
			if (text5 != null)
			{
				SaveData.ActiveCalendar = CalendarGiftDataManager.Instance.GetData(text5);
			}
			else
			{
				SaveData.ActiveCalendar = MiscParams.ActiveCalendar;
			}
			SaveData.LastOneTimeCalendarDateClaimed = DateTime.MinValue;
			SaveData.OneTimeCalendarDaysClaimed = TFUtils.LoadInt(dictionary, "OneTimeCalendarDaysClaimed", 0);
			if ((int)SaveData.OneTimeCalendarDaysClaimed > 0)
			{
				string text6 = TFUtils.LoadString(dictionary, "LastOneTimeCalendarDateClaimed", null);
				if (text6 != null)
				{
					try
					{
						SaveData.LastOneTimeCalendarDateClaimed = DateTime.Parse(text6);
					}
					catch (Exception)
					{
					}
				}
			}
			if (dictionary.ContainsKey("DailyMissionTimestamp"))
			{
				string s = (string)dictionary["DailyMissionTimestamp"];
				SaveData.DailyMissionTimestamp = DateTime.Parse(s);
			}
			DateTime utcNow = DateTime.UtcNow;
			int unixTime = GetUnixTime(utcNow);
			if (dictionary.ContainsKey("LastAccessUTC"))
			{
				int num2 = (int)dictionary["LastAccessUTC"];
				int diff = unixTime - num2;
			}
			SaveData.LastAccessUTC = unixTime;
			if (!SaveData.UIDFixApplied)
			{
				SaveData.UIDFixApplied = true;
				SaveData.FixDuplicateUniqueIDs();
			}
			if (!SaveData.RankUpInventoryGiven)
			{
				SaveData.RankUpInventoryGiven = true;
				SaveData.GiveRetroactiveRankInventory();
			}
		}
		SaveData.version = 1;
	}

	private static int GetUnixTime(DateTime targetTime)
	{
		targetTime = targetTime.ToUniversalTime();
		return (int)(targetTime - UNIX_EPOCH).TotalSeconds;
	}

	public void InitNewSaveFile()
	{
		NewSession = true;
		ResetCreatureCollection();
		ResetCardCollection();
		ResetEvoMatCollection();
		ResetXPMatCollection();
		TutorialDataManager.Instance.ResetBlockCompletion();
		SaveData.CreatureSorts.Add(new SortEntry(SortTypeEnum.Newest));
		SaveData.ExCardSorts.Add(new SortEntry(SortTypeEnum.Newest));
		SaveData.HelperSorts.Add(new SortEntry(SortTypeEnum.Level));
		SaveData.ManualSetHardCurrency(0, GachaSlotDataManager.PremiumGachaCost);
		SaveData.PvPCurrency = 0;
		SaveData.InventorySpace = MiscParams.StartingInventorySpace;
		SaveData.AllyBoxSpace = MiscParams.StartingAllyBoxSpace;
		SaveData.Unlocks.Add("TBuilding_Quests", null);
		SaveData.Unlocks.Add("Lab_Fusion", null);
		List<LeaderData> database = LeaderDataManager.Instance.GetDatabase();
		foreach (LeaderData item2 in database)
		{
			if (item2.Playable && item2.BuyCost == -1)
			{
				LeaderItem item = new LeaderItem(item2.ID);
				SaveData.Leaders.Add(item);
			}
		}
		if (SaveData.Leaders.Count == 0)
		{
			return;
		}
		for (int i = 0; i < MiscParams.StartingLoadoutCount; i++)
		{
			Loadout loadout = new Loadout();
			loadout.Leader = SaveData.Leaders[0];
			for (int j = 0; j < MiscParams.CreaturesOnTeam; j++)
			{
				loadout.CreatureSet.Add(null);
			}
			SaveData.Loadouts.Add(loadout);
		}
		SaveData.SelectedLoadout = 0;
		SaveData.ClearInventory();
		QuestLoadoutData data = QuestLoadoutDataManager.Instance.GetData("StartDeck");
		for (int k = 0; k < data.Entries.Count; k++)
		{
			if (data.Entries[k] != null)
			{
				CreatureItem creature = data.Entries[k].BuildCreatureItem();
				InventorySlotItem value = SaveData.AddCreature(creature);
				if (k < MiscParams.CreaturesOnTeam)
				{
					SaveData.Loadouts[0].CreatureSet[k] = value;
				}
			}
		}
		data = QuestLoadoutDataManager.Instance.GetData("StartDeck_Plains");
		for (int l = 0; l < data.Entries.Count; l++)
		{
			if (data.Entries[l] != null)
			{
				CreatureItem creature2 = data.Entries[l].BuildCreatureItem();
				InventorySlotItem value2 = SaveData.AddCreature(creature2);
				if (l < MiscParams.CreaturesOnTeam)
				{
					SaveData.Loadouts[1].CreatureSet[l] = value2;
				}
			}
		}
		data = QuestLoadoutDataManager.Instance.GetData("StartDeck_Niceland");
		for (int m = 0; m < data.Entries.Count; m++)
		{
			if (data.Entries[m] != null)
			{
				CreatureItem creature3 = data.Entries[m].BuildCreatureItem();
				InventorySlotItem value3 = SaveData.AddCreature(creature3);
				if (m < MiscParams.CreaturesOnTeam)
				{
					SaveData.Loadouts[2].CreatureSet[m] = value3;
				}
			}
		}
		SetDefaultHelperCreature();
		SaveData.DailyMissionTimestamp = DateTime.MinValue;
		SaveData.PlayersLastSavedLevel = 1;
		SaveData.UnlockedCardBacks.Add(CardBackDataManager.Instance.GetData("Default"));
		SaveData.SelectedPortrait = PlayerPortraitDataManager.Instance.GetData("Default");
		SaveData.SelectedCardBack = CardBackDataManager.DefaultData;
		SaveData.MultiplayerLevel = -1;
		SaveData.BestMultiplayerLevel = -1;
		SaveData.PointsInMultiplayerLevel = 0;
		SaveData.ActivePvpSeason = null;
		SaveData.PvpRankRewardsGranted = -1;
		SaveData.TownTiltCam = true;
		PlayerPrefs.SetInt("EnableTiltCam", 1);
		SaveData.PvpSpecialDomainNumber = 0;
		SaveData.ChatSpecialDomainNumber = 0;
		SaveData.ActiveCalendar = MiscParams.ActiveCalendar;
		SaveData.ExpeditionSlots = ExpeditionParams.StartingExpeditionSlots;
		SaveData.RandomDungeonLevel = 1;
		DetachedSingleton<MissionManager>.Instance.Init();
	}

	private string SerializeUnlocks()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (KeyValuePair<string, object> unlock in SaveData.Unlocks)
		{
			stringBuilder.Append("\"" + unlock.Key + "\",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeUnlocks(object[] unlockArray)
	{
		SaveData.Unlocks.Clear();
		foreach (object obj in unlockArray)
		{
			SaveData.Unlocks.Add(Convert.ToString(obj), null);
		}
	}

	private string SerializeDoneSpecialQuests()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		foreach (KeyValuePair<string, SpecialQuestStatus> doneSpecialQuest in SaveData.DoneSpecialQuests)
		{
			stringBuilder.Append(MakeJS(doneSpecialQuest.Key, (int)doneSpecialQuest.Value) + ",");
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	private void DeserializeDoneSpecialQuests(object dataObj)
	{
		SaveData.DoneSpecialQuests.Clear();
		if (dataObj is object[])
		{
			object[] array = (object[])dataObj;
			foreach (object obj in array)
			{
				SaveData.DoneSpecialQuests.Add(Convert.ToString(obj), SpecialQuestStatus.Attempted);
			}
		}
		else
		{
			if (!(dataObj is Dictionary<string, object>))
			{
				return;
			}
			foreach (KeyValuePair<string, object> item in dataObj as Dictionary<string, object>)
			{
				SaveData.DoneSpecialQuests.Add(item.Key, (SpecialQuestStatus)Convert.ToInt32(item.Value));
			}
		}
	}

	private string SerializeDungeonMaps()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		foreach (KeyValuePair<string, ObscuredInt> dungeonMap in SaveData.DungeonMaps)
		{
			stringBuilder.Append(MakeJS(dungeonMap.Key, dungeonMap.Value) + ",");
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	private void DeserializeDungeonMaps(Dictionary<string, object> dict)
	{
		SaveData.DungeonMaps.Clear();
		foreach (KeyValuePair<string, object> item in dict)
		{
			SaveData.DungeonMaps.Add(item.Key, Convert.ToInt32(item.Value));
		}
	}

	private string SerializeQuestStars()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (int earnedQuestStar in SaveData.EarnedQuestStars)
		{
			stringBuilder.Append(earnedQuestStar + ",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeQuestStars(int[] starArray)
	{
		SaveData.EarnedQuestStars.Clear();
		if (starArray != null)
		{
			foreach (int item in starArray)
			{
				SaveData.EarnedQuestStars.Add(item);
			}
		}
	}

	private string SerializeDungeonStars()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		foreach (KeyValuePair<string, int> earnedDungeonStar in SaveData.EarnedDungeonStars)
		{
			stringBuilder.Append(MakeJS(earnedDungeonStar.Key, earnedDungeonStar.Value) + ",");
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	private void DeserializeDungeonStars(Dictionary<string, object> starArray)
	{
		SaveData.EarnedDungeonStars.Clear();
		foreach (KeyValuePair<string, object> item in starArray)
		{
			SaveData.EarnedDungeonStars.Add(item.Key, Convert.ToInt32(item.Value));
		}
	}

	private string SerializeLeaders()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < SaveData.Leaders.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(SaveData.Leaders[i].Serialize());
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeLeaders(object[] leaderArray)
	{
		SaveData.Leaders.Clear();
		foreach (object obj in leaderArray)
		{
			SaveData.Leaders.Add(new LeaderItem(obj as Dictionary<string, object>));
		}
	}

	private string SerializeCardBacks()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (CardBackData unlockedCardBack in SaveData.UnlockedCardBacks)
		{
			stringBuilder.Append("\"" + unlockedCardBack.ID + "\",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeCardBacks(object[] cardBackArray)
	{
		SaveData.UnlockedCardBacks.Clear();
		foreach (object obj in cardBackArray)
		{
			CardBackData data = CardBackDataManager.Instance.GetData(Convert.ToString(obj));
			if (data != null)
			{
				SaveData.UnlockedCardBacks.Add(data);
			}
		}
	}

	private string SerializePortraits()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (PlayerPortraitData unlockedPortrait in SaveData.UnlockedPortraits)
		{
			stringBuilder.Append("\"" + unlockedPortrait.ID + "\",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializePortraits(object[] portraitArray)
	{
		SaveData.UnlockedPortraits.Clear();
		foreach (object obj in portraitArray)
		{
			PlayerPortraitData data = PlayerPortraitDataManager.Instance.GetData(Convert.ToString(obj));
			if (data != null)
			{
				SaveData.UnlockedPortraits.Add(data);
			}
		}
	}

	private string SerializeBadges()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (PlayerBadgeData unlockedBadge in SaveData.UnlockedBadges)
		{
			stringBuilder.Append("\"" + unlockedBadge.ID + "\",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeBadges(object[] badgeArray)
	{
		SaveData.UnlockedBadges.Clear();
		foreach (object obj in badgeArray)
		{
			SaveData.UnlockedBadges.Add(PlayerBadgeDataManager.Instance.GetData(Convert.ToString(obj)));
		}
	}

	private string SerializeTitles()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (PlayerTitleData unlockedTitle in SaveData.UnlockedTitles)
		{
			stringBuilder.Append("\"" + unlockedTitle.ID + "\",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeTitles(object[] titleArray)
	{
		SaveData.UnlockedTitles.Clear();
		foreach (object obj in titleArray)
		{
			SaveData.UnlockedTitles.Add(PlayerTitleDataManager.Instance.GetData(Convert.ToString(obj)));
		}
	}

	private string SerializeBattleHistory()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (BattleHistory battleHistory in SaveData.BattleHistoryList)
		{
			stringBuilder.Append(Json.Serialize(battleHistory.ToJSONDictionary()) + ",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeBattleHistory(object[] historyArray)
	{
		SaveData.BattleHistoryList.Clear();
		foreach (object obj in historyArray)
		{
			SaveData.BattleHistoryList.Add(BattleHistory.FromJSONDictionary(obj as Dictionary<string, object>));
		}
	}

	private string SerializeIgnoredPlayers()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (KeyValuePair<string, object> ignoredPlayer in SaveData.IgnoredPlayers)
		{
			stringBuilder.Append("\"" + ignoredPlayer.Key + "\",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeIgnoredPlayers(object[] playerArray)
	{
		SaveData.IgnoredPlayers.Clear();
		foreach (object obj in playerArray)
		{
			SaveData.IgnoredPlayers.Add(Convert.ToString(obj), null);
		}
	}

	private string SerializeSales()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (SpecialSaleItem value in SaveData.Sales.Values)
		{
			if (!flag)
			{
				stringBuilder.Append(',');
			}
			flag = false;
			stringBuilder.Append(value.Serialize());
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeSales(object[] saleArray)
	{
		SaveData.Sales.Clear();
		foreach (object obj in saleArray)
		{
			SpecialSaleItem specialSaleItem = new SpecialSaleItem(obj as Dictionary<string, object>);
			SaveData.Sales.Add(specialSaleItem.Form, specialSaleItem);
		}
	}

	private string SerializeGachaCooldowns()
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (KeyValuePair<GachaSlotData, uint> gachaSlotCooldown in SaveData.GachaSlotCooldowns)
		{
			if (gachaSlotCooldown.Value > num)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				flag = false;
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", gachaSlotCooldown.Key.ID) + ",");
				stringBuilder.Append(MakeJS("Time", gachaSlotCooldown.Value) + ",");
				stringBuilder.Append("}");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeGachaCooldowns(object[] cooldownArray)
	{
		SaveData.GachaSlotCooldowns.Clear();
		uint num = TFUtils.ServerTime.UnixTimestamp();
		foreach (object obj in cooldownArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			uint num2 = TFUtils.LoadUint(dictionary, "Time", 0u);
			GachaSlotData data = GachaSlotDataManager.Instance.GetData(iD);
			if (data != null && num2 > num)
			{
				SaveData.GachaSlotCooldowns.Add(data, num2);
			}
		}
	}

	private string SerializeSpeedUps()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (KeyValuePair<SpeedUpData, int> speedUp in SaveData.SpeedUps)
		{
			if (speedUp.Value != 0)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				flag = false;
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", speedUp.Key.ID) + ",");
				stringBuilder.Append(MakeJS("Amount", speedUp.Value) + ",");
				stringBuilder.Append("}");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeSpeedUps(object[] speedupsArray)
	{
		SaveData.SpeedUps.Clear();
		foreach (object obj in speedupsArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			int value = TFUtils.LoadInt(dictionary, "Amount", 0);
			SpeedUpData data = SpeedUpDataManager.Instance.GetData(iD);
			if (data != null)
			{
				SaveData.SpeedUps.Add(data, value);
			}
		}
	}

	private string SerializeGachaKeys()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		bool flag = true;
		foreach (KeyValuePair<GachaSlotData, int> gachaKey in SaveData.GachaKeys)
		{
			if (gachaKey.Value != 0)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				flag = false;
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", gachaKey.Key.ID) + ",");
				stringBuilder.Append(MakeJS("Amount", gachaKey.Value) + ",");
				stringBuilder.Append("}");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeGachaKeys(object[] GachaKeysArray)
	{
		SaveData.GachaKeys.Clear();
		foreach (object obj in GachaKeysArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			int value = TFUtils.LoadInt(dictionary, "Amount", 0);
			GachaSlotData data = GachaSlotDataManager.Instance.GetData(iD);
			if (data != null)
			{
				SaveData.GachaKeys.Add(data, value);
			}
		}
	}

	private string SerializeInventory()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < SaveData.InventorySlots.Count; i++)
		{
			string value;
			if (SaveData.InventorySlots[i].SlotType == InventorySlotType.Creature)
			{
				value = SaveData.InventorySlots[i].Creature.Serialize();
			}
			else if (SaveData.InventorySlots[i].SlotType == InventorySlotType.Card)
			{
				value = SaveData.InventorySlots[i].Card.Serialize();
			}
			else if (SaveData.InventorySlots[i].SlotType == InventorySlotType.EvoMaterial)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("{");
				stringBuilder2.Append(MakeJS("_T", "EV") + ",");
				stringBuilder2.Append(MakeJS("ID", SaveData.InventorySlots[i].EvoMaterial.ID) + ",");
				stringBuilder2.Append("}");
				value = stringBuilder2.ToString();
			}
			else
			{
				if (SaveData.InventorySlots[i].SlotType != InventorySlotType.XPMaterial)
				{
					continue;
				}
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("{");
				stringBuilder3.Append(MakeJS("_T", "XP") + ",");
				stringBuilder3.Append(MakeJS("ID", SaveData.InventorySlots[i].XPMaterial.ID) + ",");
				stringBuilder3.Append("}");
				value = stringBuilder3.ToString();
			}
			if (i > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(value);
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeInventory(object[] itemArray)
	{
		SaveData.ClearInventory();
		CreatureItem.ResetMaxUniqueId();
		CardItem.ResetMaxUniqueId();
		foreach (object obj in itemArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			switch (TFUtils.LoadString(dictionary, "_T", string.Empty))
			{
			case "CR":
				SaveData.AddCreature(new CreatureItem(dictionary));
				break;
			case "EV":
			{
				string iD2 = TFUtils.LoadString(dictionary, "ID", string.Empty);
				SaveData.AddEvoMaterial(EvoMaterialDataManager.Instance.GetData(iD2));
				break;
			}
			case "XP":
			{
				string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
				SaveData.AddXPMaterial(XPMaterialDataManager.Instance.GetData(iD));
				break;
			}
			}
		}
		foreach (object obj2 in itemArray)
		{
			Dictionary<string, object> dictionary2 = obj2 as Dictionary<string, object>;
			string text = TFUtils.LoadString(dictionary2, "_T", string.Empty);
			if (!(text == "CA"))
			{
				continue;
			}
			InventorySlotItem slotItem = SaveData.AddExCard(new CardItem(dictionary2));
			if (slotItem.Card.CreatureUID == 0)
			{
				continue;
			}
			InventorySlotItem inventorySlotItem = Singleton<PlayerInfoScript>.Instance.SaveData.FindCreature((InventorySlotItem match) => match.Creature.UniqueId == slotItem.Card.CreatureUID);
			if (inventorySlotItem != null)
			{
				CreatureItem creature = inventorySlotItem.Creature;
				if (creature.ExCards[slotItem.Card.CreatureSlot] == null)
				{
					creature.ExCards[slotItem.Card.CreatureSlot] = slotItem;
				}
			}
			else
			{
				slotItem.Card.CreatureUID = 0;
				slotItem.Card.CreatureSlot = 0;
			}
		}
	}

	private string SerializeCreatureCollection()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (CreatureData item in CreatureDataManager.Instance.GetDatabase())
		{
			int num = 0;
			if (item.AlreadySeen)
			{
				num |= 1;
			}
			if (item.AlreadyCollected)
			{
				num |= 2;
			}
			if (num != 0)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", item.ID) + ",");
				stringBuilder.Append(MakeJS("Stat", num) + ",");
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeCreatureCollection(object[] collectionArray)
	{
		ResetCreatureCollection();
		foreach (object obj in collectionArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			int num = TFUtils.LoadInt(dictionary, "Stat", 0);
			CreatureData data = CreatureDataManager.Instance.GetData(iD);
			if (data != null)
			{
				data.AlreadySeen = (num & 1) != 0;
				data.AlreadyCollected = (num & 2) != 0;
			}
		}
	}

	private void ResetCreatureCollection()
	{
		foreach (CreatureData item in CreatureDataManager.Instance.GetDatabase())
		{
			item.AlreadySeen = false;
			item.AlreadyCollected = false;
		}
	}

	private string SerializeCardCollection()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (CardData item in CardDataManager.Instance.GetDatabase())
		{
			int num = 0;
			if (item.AlreadySeen)
			{
				num |= 1;
			}
			if (item.AlreadyCollected)
			{
				num |= 2;
			}
			if (num != 0)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", item.ID) + ",");
				stringBuilder.Append(MakeJS("Stat", num) + ",");
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeCardCollection(object[] collectionArray)
	{
		ResetCardCollection();
		foreach (object obj in collectionArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			int num = TFUtils.LoadInt(dictionary, "Stat", 0);
			CardData data = CardDataManager.Instance.GetData(iD);
			if (data != null)
			{
				data.AlreadySeen = (num & 1) != 0;
				data.AlreadyCollected = (num & 2) != 0;
			}
		}
	}

	private void ResetCardCollection()
	{
		foreach (CardData item in CardDataManager.Instance.GetDatabase())
		{
			item.AlreadySeen = false;
			item.AlreadyCollected = false;
		}
	}

	private string SerializeEvoMatCollection()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (EvoMaterialData item in EvoMaterialDataManager.Instance.GetDatabase())
		{
			int num = 0;
			if (item.AlreadySeen)
			{
				num |= 1;
			}
			if (item.AlreadyCollected)
			{
				num |= 2;
			}
			if (num != 0)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", item.ID) + ",");
				stringBuilder.Append(MakeJS("Stat", num) + ",");
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeEvoMatCollection(object[] collectionArray)
	{
		ResetEvoMatCollection();
		foreach (object obj in collectionArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			int num = TFUtils.LoadInt(dictionary, "Stat", 0);
			EvoMaterialData data = EvoMaterialDataManager.Instance.GetData(iD);
			if (data != null)
			{
				data.AlreadySeen = (num & 1) != 0;
				data.AlreadyCollected = (num & 2) != 0;
			}
		}
	}

	private void ResetEvoMatCollection()
	{
		foreach (EvoMaterialData item in EvoMaterialDataManager.Instance.GetDatabase())
		{
			item.AlreadySeen = false;
			item.AlreadyCollected = false;
		}
	}

	private string SerializeXPMatCollection()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (XPMaterialData item in XPMaterialDataManager.Instance.GetDatabase())
		{
			int num = 0;
			if (item.AlreadySeen)
			{
				num |= 1;
			}
			if (item.AlreadyCollected)
			{
				num |= 2;
			}
			if (num != 0)
			{
				stringBuilder.Append("{");
				stringBuilder.Append(MakeJS("ID", item.ID) + ",");
				stringBuilder.Append(MakeJS("Stat", num) + ",");
				stringBuilder.Append("},");
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeXPMatCollection(object[] collectionArray)
	{
		ResetXPMatCollection();
		foreach (object obj in collectionArray)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			string iD = TFUtils.LoadString(dictionary, "ID", string.Empty);
			int num = TFUtils.LoadInt(dictionary, "Stat", 0);
			XPMaterialData data = XPMaterialDataManager.Instance.GetData(iD);
			if (data != null)
			{
				data.AlreadySeen = (num & 1) != 0;
				data.AlreadyCollected = (num & 2) != 0;
			}
		}
	}

	private void ResetXPMatCollection()
	{
		foreach (XPMaterialData item in XPMaterialDataManager.Instance.GetDatabase())
		{
			item.AlreadySeen = false;
			item.AlreadyCollected = false;
		}
	}

	public string SerializeAdminMessages()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (MailItem mail in SaveData.Mails)
		{
			stringBuilder.Append(mail.Serialize());
			stringBuilder.Append(',');
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeAdminMessages(object[] messageArray)
	{
		SaveData.RemoveAllMails();
		MailItem.ResetMaxUniqueId();
		foreach (object obj in messageArray)
		{
			SaveData.AddMail(new MailItem(obj as Dictionary<string, object>));
		}
	}

	private string SerializeLoadouts()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < SaveData.Loadouts.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(SaveData.Loadouts[i].Serialize());
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeLoadouts(object[] loadoutArray)
	{
		SaveData.Loadouts.Clear();
		foreach (object obj in loadoutArray)
		{
			SaveData.Loadouts.Add(new Loadout(obj as Dictionary<string, object>));
		}
	}

	private string SerializeCreatureSorts()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (SortEntry creatureSort in SaveData.CreatureSorts)
		{
			stringBuilder.Append(creatureSort.Serialize() + ",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeCreatureSorts(object[] sortArray)
	{
		SaveData.CreatureSorts.Clear();
		foreach (object obj in sortArray)
		{
			SaveData.CreatureSorts.Add(new SortEntry(obj as Dictionary<string, object>));
		}
	}

	private string SerializeExCardSorts()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (SortEntry exCardSort in SaveData.ExCardSorts)
		{
			stringBuilder.Append(exCardSort.Serialize() + ",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeExCardSorts(object[] sortArray)
	{
		SaveData.ExCardSorts.Clear();
		foreach (object obj in sortArray)
		{
			SaveData.ExCardSorts.Add(new SortEntry(obj as Dictionary<string, object>));
		}
	}

	private string SerializeHelperSorts()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (SortEntry helperSort in SaveData.HelperSorts)
		{
			stringBuilder.Append(helperSort.Serialize() + ",");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeHelperSorts(object[] sortArray)
	{
		SaveData.HelperSorts.Clear();
		foreach (object obj in sortArray)
		{
			SaveData.HelperSorts.Add(new SortEntry(obj as Dictionary<string, object>));
		}
	}

	private string SerializeFBRewards()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < SaveData.FBInviteRewards.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append("\"");
			stringBuilder.Append(SaveData.FBInviteRewards[i]);
			stringBuilder.Append("\"");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeFBRewards(object[] FBRewardsArray)
	{
		SaveData.FBInviteRewards.Clear();
		foreach (object obj in FBRewardsArray)
		{
			string item = (string)obj;
			SaveData.FBInviteRewards.Add(item);
		}
	}

	private string SerializeDeletedMail()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < SaveData.DeletedMail.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append("\"");
			stringBuilder.Append(SaveData.DeletedMail[i]);
			stringBuilder.Append("\"");
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void DeserializeDeletedMail(object[] deletedMailArray)
	{
		SaveData.DeletedMail.Clear();
		foreach (object obj in deletedMailArray)
		{
			string item = (string)obj;
			SaveData.DeletedMail.Add(item);
		}
	}

	private void CheckSimpleHacking()
	{
		if (SaveData.PaidHardCurrency >= MiscParams.PvPHackThreshold || SaveData.FreeHardCurrency >= MiscParams.PvPHackThreshold)
		{
			SaveData.PvpSpecialDomainNumber = 1;
			SaveData.ChatSpecialDomainNumber = 1;
		}

		SaveData.ReadWriteParam = 1981;
	}

	public void SaveLocal()
	{
		CheckSimpleHacking();
		string gameStateJson = Serialize();
		SessionManager.Instance.SetGameStateJson(gameStateJson);
	}

	public void Save(SessionManager.OnSaveDelegate callback = null)
	{
		CheckSimpleHacking();
		string text = Serialize();
		SessionManager.Instance.SetGameStateJson(text);
		StartCoroutine(CoroutineRemoteSave(text, callback));
	}

	private IEnumerator CoroutineRemoteSave(string jsonQueued, SessionManager.OnSaveDelegate callback)
	{
		if (callback != null)
		{
			PlayerInfoScript playerInfoScript = this;
			playerInfoScript.saveCbQueued = (SessionManager.OnSaveDelegate)Delegate.Combine(playerInfoScript.saveCbQueued, callback);
		}
		remoteSavePendingJson = jsonQueued;
		if (remoteSaving)
		{
			yield break;
		}
		remoteSaving = true;
		yield return null;
		SessionManager smgr = SessionManager.Instance;
		do
		{
			string json = remoteSavePendingJson;
			remoteSavePendingJson = null;
			SessionManager.OnSaveDelegate saveCallbacks = saveCbQueued;
			saveCbQueued = null;
			bool isSaveComplete = false;
			bool saveSuccess = false;
			try
			{
				smgr.SaveToServer(json, delegate(bool success)
				{
					isSaveComplete = true;
					saveSuccess = success;
				});
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				isSaveComplete = true;
			}
			DateTime timeoutTime = TFUtils.ServerTime + SAVE_TIMEOUT_TIME;
			while (!isSaveComplete && !(TFUtils.ServerTime >= timeoutTime))
			{
				yield return null;
			}
			try
			{
				if (saveCallbacks != null)
				{
					saveCallbacks(saveSuccess);
				}
			}
			catch (Exception)
			{
			}
		}
		while (remoteSavePendingJson != null);
		remoteSaving = false;
	}

	public static void Load()
	{
		Singleton<PlayerInfoScript>.Instance.AddNewPlayerInfo();
		Singleton<PlayerInfoScript>.Instance.mRankXPTable = XPTableDataManager.Instance.GetData("PlayerRank");
		Singleton<TBPvPManager>.Instance.CheckIP();
		bool flag = true;
		string json = null;
		try
		{
			json = SessionManager.Instance.GetGameStateJson();
		}
		catch (FileNotFoundException)
		{
			flag = false;
		}
		catch (Exception)
		{
			flag = false;
		}
		if (flag)
		{
			Singleton<PlayerInfoScript>.Instance.Deserialize(json);
		}
		else
		{
			Singleton<PlayerInfoScript>.Instance.InitNewSaveFile();
		}
		DetachedSingleton<MissionManager>.Instance.AssignGlobalMissions();
	}

	public static string FormatTimeString(int totalSeconds, bool showSeconds = false, bool truncateZeroes = false)
	{
		DaysString = KFFLocalization.Get("!!DAY_CHAR");
		HoursString = KFFLocalization.Get("!!HOUR_CHAR");
		MinutesString = KFFLocalization.Get("!!MINUTE_CHAR");
		SecondsString = KFFLocalization.Get("!!SECOND_CHAR");
		if (!showSeconds)
		{
			totalSeconds += 60;
		}
		string text = string.Empty;
		int num = 0;
		int num2 = totalSeconds / 86400;
		if (num2 > 0)
		{
			text = text + num2 + DaysString;
			totalSeconds %= 86400;
			num++;
		}
		int num3 = totalSeconds / 3600;
		if (num3 > 0 || (num > 0 && !truncateZeroes))
		{
			text = text + num3 + HoursString;
			totalSeconds %= 3600;
			num++;
		}
		if (num >= 2)
		{
			return text;
		}
		int num4 = totalSeconds / 60;
		if (num4 > 0 || (num > 0 && !truncateZeroes))
		{
			text = text + num4 + MinutesString;
			totalSeconds %= 60;
			num++;
		}
		if (num >= 2)
		{
			return text;
		}
		if (showSeconds && (totalSeconds > 0 || !truncateZeroes))
		{
			text = text + totalSeconds + SecondsString;
		}
		return text;
	}

	public bool IsLeaderUnlocked(LeaderData leader)
	{
		return SaveData.Leaders.Find((LeaderItem item) => item.Form == leader) != null;
	}

	public LeaderItem GetLeaderItem(string leaderId)
	{
		return SaveData.Leaders.Find((LeaderItem item) => item.Form.ID == leaderId);
	}

	public LeaderItem GetLeaderItem(LeaderData leaderData)
	{
		return SaveData.Leaders.Find((LeaderItem item) => item.Form == leaderData);
	}

	public void GoToNextLoadout(bool skipEmpty = false)
	{
		do
		{
			SaveData.SelectedLoadout++;
			if (SaveData.SelectedLoadout >= SaveData.Loadouts.Count)
			{
				SaveData.SelectedLoadout = 0;
			}
		}
		while (skipEmpty && SaveData.Loadouts[SaveData.SelectedLoadout].CreatureCount() <= 0);
	}

	public void GoToPrevLoadout(bool skipEmpty = false)
	{
		do
		{
			SaveData.SelectedLoadout--;
			if (SaveData.SelectedLoadout < 0)
			{
				SaveData.SelectedLoadout = SaveData.Loadouts.Count - 1;
			}
		}
		while (skipEmpty && SaveData.Loadouts[SaveData.SelectedLoadout].CreatureCount() <= 0);
	}

	public Loadout GetCurrentLoadout()
	{
		return SaveData.Loadouts[SaveData.SelectedLoadout];
	}

	public void SetDefaultLoadout()
	{
		SaveData.SelectedLoadout = 0;
	}

	public bool IsCreatureInAnyLoadout(CreatureItem creature)
	{
		foreach (Loadout loadout in SaveData.Loadouts)
		{
			if (loadout.ContainsCreature(creature))
			{
				return true;
			}
		}
		return false;
	}

	public InventorySlotItem GetMyHelperCreature()
	{
		InventorySlotItem inventorySlotItem = SaveData.FindCreature((InventorySlotItem m) => m.Creature.UniqueId == SaveData.MyHelperCreatureID);
		if (inventorySlotItem == null)
		{
			SetDefaultHelperCreature();
			inventorySlotItem = SaveData.FindCreature((InventorySlotItem m) => m.Creature.UniqueId == SaveData.MyHelperCreatureID);
		}
		return inventorySlotItem;
	}

	public void SetDefaultHelperCreature()
	{
		SaveData.MyHelperCreatureID = SaveData.GetBestCreature().Creature.UniqueId;
	}

	public void SetMyHelperCreature(int id)
	{
		SaveData.MyHelperCreatureID = id;
	}

	public bool IsCreatureSetAsHelper(CreatureItem creature)
	{
		if (creature.FromOtherPlayer)
		{
			return false;
		}
		if (!CanUseHelper())
		{
			return false;
		}
		return SaveData.MyHelperCreatureID == creature.UniqueId;
	}

	public static string BuildTimerString(int seconds)
	{
		return seconds / 60 + ":" + (seconds % 60).ToString("D2");
	}

	public bool IsFeatureUnlocked(string id)
	{
		return SaveData.Unlocks.ContainsKey(id);
	}

	public void UnlockFeature(string id)
	{
		if (!IsFeatureUnlocked(id))
		{
			SaveData.Unlocks.Add(id, null);
		}
	}

	public bool CanEditDeck()
	{
		return IsFeatureUnlocked("TBuilding_EditDeck");
	}

	public bool CanEquipGems()
	{
		return IsFeatureUnlocked("Lab_GemCreate");
	}

	public bool CanEquipExCards()
	{
		return IsFeatureUnlocked("Lab_CardEquip");
	}

	public bool CanPvp()
	{
		return IsFeatureUnlocked("TBuilding_PVP");
	}

	public bool CanGacha()
	{
		return IsFeatureUnlocked("TBuilding_Gacha");
	}

	public bool CanLootCards()
	{
		return IsFeatureUnlocked("Lab_CardEquip");
	}

	public bool CanLootRunes()
	{
		return IsFeatureUnlocked("Lab_Evo") || IsFeatureUnlocked("Lab_Enhance");
	}

	public bool CanBuyLeaders()
	{
		return IsFeatureUnlocked("BuyHero");
	}

	public bool CanUseHelper()
	{
		return IsFeatureUnlocked("TBuilding_Social");
	}

	public bool IsQuestUnlocking(QuestData quest)
	{
		int intQuestId = quest.GetIntQuestId();
		if (intQuestId == -1)
		{
			return false;
		}
		return intQuestId <= SaveData.TopCompletedQuestId + 1 && intQuestId > SaveData.TopShownCompletedQuestId + 1;
	}

	public bool IsLeagueUnlocking(LeagueData league)
	{
		if (league.Quests.Count == 0)
		{
			return false;
		}
		return IsQuestUnlocking(league.Quests[0]);
	}

	public bool IsQuestUnlocked(QuestData quest)
	{
		if (quest.League.LinearDungeon)
		{
			int num = quest.League.Quests.IndexOf(quest);
			if (num < 1)
			{
				return true;
			}
			QuestData questData = quest.League.Quests[num - 1];
			return SaveData.EarnedDungeonStars.ContainsKey(questData.ID);
		}
		int intQuestId = quest.GetIntQuestId();
		if (intQuestId == -1)
		{
			return true;
		}
		return intQuestId <= SaveData.TopCompletedQuestId + 1;
	}

	public bool IsQuestComplete(QuestData quest)
	{
		int intQuestId = quest.GetIntQuestId();
		if (intQuestId == -1)
		{
			return false;
		}
		return intQuestId <= SaveData.TopCompletedQuestId;
	}

	public bool IsLeagueUnlocked(LeagueData league)
	{
		if (league.Quests.Count == 0)
		{
			return false;
		}
		return IsQuestUnlocked(league.Quests[0]);
	}

	public bool IsLeagueComplete(LeagueData league)
	{
		int intQuestId = league.Quests[league.Quests.Count - 1].GetIntQuestId();
		if (intQuestId == -1)
		{
			return false;
		}
		return intQuestId <= SaveData.TopCompletedQuestId;
	}

	public QuestData GetHighestUnlockedMainLineQuest()
	{
		QuestData data = QuestDataManager.Instance.GetData((SaveData.TopCompletedQuestId + 1).ToString());
		if (data == null)
		{
			return QuestData.HighestMainLineQuest;
		}
		return data;
	}

	private DateTime GetNow()
	{
		return TFUtils.ServerTime;
	}

	public DateTime GetNowTime()
	{
		return GetNow();
	}

	public List<CalendarGift> GetCalendarGifts()
	{
		return SaveData.ActiveCalendar.Entries;
	}

	public bool HasUnclaimedCalendarGift()
	{
		DateTime serverTime = TFUtils.ServerTime;
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day);
		DateTime dateTime2 = new DateTime(SaveData.LastOneTimeCalendarDateClaimed.Year, SaveData.LastOneTimeCalendarDateClaimed.Month, SaveData.LastOneTimeCalendarDateClaimed.Day);
		if (dateTime == dateTime2)
		{
			return false;
		}
		if ((int)SaveData.OneTimeCalendarDaysClaimed >= GetCalendarGifts().Count)
		{
			SaveData.ActiveCalendar = MiscParams.RepeatingCalendar;
			SaveData.OneTimeCalendarDaysClaimed = 0;
		}
		return true;
	}

	public void OnSpecialQuestAttempted(string questID)
	{
		SpecialQuestStatus value;
		if (SaveData.DoneSpecialQuests.TryGetValue(questID, out value))
		{
			if (value < SpecialQuestStatus.Attempted)
			{
				SaveData.DoneSpecialQuests[questID] = SpecialQuestStatus.Attempted;
			}
		}
		else
		{
			SaveData.DoneSpecialQuests.Add(questID, SpecialQuestStatus.Attempted);
		}
	}

	public void OnSpecialQuestCompleted(string questID)
	{
		SpecialQuestStatus value;
		if (SaveData.DoneSpecialQuests.TryGetValue(questID, out value))
		{
			if (value < SpecialQuestStatus.Completed)
			{
				SaveData.DoneSpecialQuests[questID] = SpecialQuestStatus.Completed;
			}
		}
		else
		{
			SaveData.DoneSpecialQuests.Add(questID, SpecialQuestStatus.Completed);
		}
	}

	public SpecialQuestStatus GetSpecialQuestStatus(string questID)
	{
		SpecialQuestStatus value;
		if (SaveData.DoneSpecialQuests.TryGetValue(questID, out value))
		{
			return value;
		}
		return SpecialQuestStatus.New;
	}

	public void UpdateBadgeCount(BadgeEnum badgeType)
	{
		if (!DetachedSingleton<SceneFlowManager>.Instance.InFrontEnd())
		{
			return;
		}
		int num = 0;
		switch (badgeType)
		{
		case BadgeEnum.Mail:
			num = Singleton<MailController>.Instance.GetUnreadMailCount();
			break;
		case BadgeEnum.Evo:
			if (!IsFeatureUnlocked("Lab_Evo"))
			{
				break;
			}
			foreach (InventorySlotItem inventorySlot in SaveData.InventorySlots)
			{
				if (inventorySlot.SlotType == InventorySlotType.Creature && inventorySlot.Creature.CanCurrentlyEvo())
				{
					num++;
				}
			}
			break;
		case BadgeEnum.Gacha:
		{
			int num2 = int.MaxValue;
			int num3 = int.MaxValue;
			int num4 = int.MaxValue;
			foreach (GachaSlotData item in GachaSlotDataManager.Instance.GetDatabase())
			{
				if (item.CurrencyType == DropTypeEnum.SoftCurrency)
				{
					if (item.Cost < num2 && item.ConditionsMet())
					{
						num2 = item.Cost;
					}
				}
				else if (item.CurrencyType == DropTypeEnum.SocialCurrency)
				{
					if (item.Cost < num3 && item.ConditionsMet())
					{
						num3 = item.Cost;
					}
				}
				else if (item.CurrencyType == DropTypeEnum.HardCurrency && item.Cost < num4 && item.ConditionsMet())
				{
					num4 = item.Cost;
				}
			}
			num += SaveData.SoftCurrency / num2;
			num += SaveData.PvPCurrency / num3;
			num += SaveData.HardCurrency / num4;
			break;
		}
		case BadgeEnum.Dungeon:
		{
			List<QuestSelectController.SpecialLeagueEntry> list = QuestSelectController.BuildSpecialLeagueList();
			foreach (QuestSelectController.SpecialLeagueEntry item2 in list)
			{
				if (!item2.Clickable)
				{
					continue;
				}
				foreach (QuestData quest in item2.League.Quests)
				{
					if (GetSpecialQuestStatus(quest.ID) == SpecialQuestStatus.New)
					{
						num++;
					}
				}
			}
			break;
		}
		}
		StateData.BadgeCounts[(int)badgeType] = num;
	}

	public void UpdateAllBadgeCounts()
	{
		for (int i = 0; i < StateData.BadgeCounts.Length; i++)
		{
			UpdateBadgeCount((BadgeEnum)i);
		}
	}

	public SceneFlowManager.ReturnLocation GetCurrentReturnLocation()
	{
		if (StateData.MultiplayerMode)
		{
			return SceneFlowManager.ReturnLocation.Pvp;
		}
		if (StateData.CurrentActiveQuest.League.QuestLine == QuestLineEnum.Special)
		{
			return SceneFlowManager.ReturnLocation.SpecialQuests;
		}
		return SceneFlowManager.ReturnLocation.Map;
	}

	public int GetQuestStars(int questId)
	{
		if (questId >= SaveData.EarnedQuestStars.Count)
		{
			return 0;
		}
		return SaveData.EarnedQuestStars[questId];
	}

	public void SetQuestStars(int questId, int stars)
	{
		while (SaveData.EarnedQuestStars.Count < questId + 1)
		{
			SaveData.EarnedQuestStars.Add(0);
		}
		if (SaveData.EarnedQuestStars[questId] < stars)
		{
			int num = stars - SaveData.EarnedQuestStars[questId];
			List<int> earnedQuestStars;
			List<int> list = (earnedQuestStars = SaveData.EarnedQuestStars);
			int index;
			int index2 = (index = questId);
			index = earnedQuestStars[index];
			list[index2] = index + num;
			DetachedSingleton<MissionManager>.Instance.AddStarsEarned(num);
		}
	}

	public int GetDungeonStars(string questId)
	{
		int value;
		if (SaveData.EarnedDungeonStars.TryGetValue(questId, out value))
		{
			return value;
		}
		return 0;
	}

	public void SetDungeonStars(string questId, int stars)
	{
		int value;
		if (!SaveData.EarnedDungeonStars.TryGetValue(questId, out value) || value < stars)
		{
			SaveData.EarnedDungeonStars[questId] = stars;
			DetachedSingleton<MissionManager>.Instance.AddStarsEarned(stars - value);
		}
	}

	public bool CheckPvpSeasonStatus()
	{
		bool flag = false;
		bool result = false;
		if (SaveData.RankedPvpMatchStarted)
		{
			SaveData.RankedPvpMatchStarted = false;
			RegisterPvpMatchResult(false, true);
			flag = true;
		}
		PvpSeasonData currentSeason = PvpSeasonDataManager.Instance.GetCurrentSeason();
		if (currentSeason != SaveData.ActivePvpSeason)
		{
			PvpSeasonData activePvpSeason = SaveData.ActivePvpSeason;
			SaveData.ActivePvpSeason = currentSeason;
			SaveData.PlayedFirstBattleInPvpSeason = false;
			SaveData.PvpRankRewardsGranted = -1;
			SaveData.MultiplayerWinStreak = 0;
			if (activePvpSeason == null || activePvpSeason != currentSeason.PreviousSeason)
			{
				SaveData.MultiplayerLevel = PvpRankDataManager.Instance.GetMaxRank();
				SaveData.PointsInMultiplayerLevel = 0;
			}
			else
			{
				SetMultiplayerRankFromPreviousRank();
			}
			int leaderboardRankInLastSeason = -1;
			if (!Singleton<TutorialController>.Instance.IsAnyTutorialActive())
			{
				Singleton<PVPNewSeasonPopupController>.Instance.Show(activePvpSeason, leaderboardRankInLastSeason);
				result = true;
			}
			flag = true;
		}
		if (flag)
		{
			Save();
		}
		return result;
	}

	private void SetMultiplayerRankFromPreviousRank()
	{
		if (SaveData.MultiplayerLevel == -1)
		{
			SaveData.MultiplayerLevel = PvpRankDataManager.Instance.GetMaxRank();
		}
		else
		{
			PvpRankData rank = PvpRankDataManager.Instance.GetRank(SaveData.MultiplayerLevel);
			if (rank.RankAfterSeasonReset == -1)
			{
				SaveData.MultiplayerLevel = PvpRankDataManager.Instance.GetMaxRank();
			}
			else
			{
				SaveData.MultiplayerLevel = rank.RankAfterSeasonReset;
			}
		}
		PvpRankData rank2 = PvpRankDataManager.Instance.GetRank(SaveData.MultiplayerLevel);
		if (rank2.NoPointLoss)
		{
			SaveData.PointsInMultiplayerLevel = 0;
		}
		else
		{
			SaveData.PointsInMultiplayerLevel = 1;
		}
	}

	public PvpMatchResultDetails RegisterPvpMatchResult(bool won, bool forceRankedLoss = false)
	{
		PvpMatchResultDetails pvpMatchResultDetails = new PvpMatchResultDetails();
		pvpMatchResultDetails.Won = won;
		if (PvPData.HistoryEntryForThisMatch != null)
		{
			BattleHistoryLocalSavesManager.Instance.FinalizeHistoryEntry(PvPData.HistoryEntryForThisMatch, won);
			PvPData.HistoryEntryForThisMatch = null;
		}
		if (!PvPData.RankedMode && !forceRankedLoss)
		{
			return pvpMatchResultDetails;
		}
		pvpMatchResultDetails.FirstMatchInSeason = !SaveData.PlayedFirstBattleInPvpSeason;
		pvpMatchResultDetails.StartingLevel = SaveData.MultiplayerLevel;
		pvpMatchResultDetails.StartingPointsInLevel = SaveData.PointsInMultiplayerLevel;
		SaveData.RankedPvpMatchStarted = false;
		SaveData.PlayedFirstBattleInPvpSeason = true;
		PvpRankData rank = PvpRankDataManager.Instance.GetRank(SaveData.MultiplayerLevel);
		if (won)
		{
			++SaveData.MultiplayerWinStreak;
			if ((int)SaveData.MultiplayerWinStreak >= 3)
			{
				if ((int)SaveData.PointsInMultiplayerLevel >= rank.PointsToAdvance - 1)
				{
					PlayerSaveData saveData = SaveData;
					saveData.PointsInMultiplayerLevel = (int)saveData.PointsInMultiplayerLevel + 1;
				}
				else
				{
					PlayerSaveData saveData2 = SaveData;
					saveData2.PointsInMultiplayerLevel = (int)saveData2.PointsInMultiplayerLevel + 2;
				}
			}
			else
			{
				PlayerSaveData saveData3 = SaveData;
				saveData3.PointsInMultiplayerLevel = (int)saveData3.PointsInMultiplayerLevel + 1;
			}
			if ((int)SaveData.PointsInMultiplayerLevel > rank.PointsToAdvance)
			{
				if (SaveData.MultiplayerLevel == 1)
				{
					SaveData.PointsInMultiplayerLevel = rank.PointsToAdvance;
				}
				else
				{
					SaveData.MultiplayerLevel--;
					SaveData.PointsInMultiplayerLevel = 1;
				}
			}
			pvpMatchResultDetails.WinRewards = GrantPvpWinRewards();
		}
		else
		{
			SaveData.MultiplayerWinStreak = 0;
			if (!rank.NoPointLoss)
			{
				--SaveData.PointsInMultiplayerLevel;
				if ((int)SaveData.PointsInMultiplayerLevel < 0)
				{
					if (SaveData.MultiplayerLevel >= PvpRankDataManager.Instance.GetMaxRank())
					{
						SaveData.PointsInMultiplayerLevel = 0;
					}
					else
					{
						SaveData.MultiplayerLevel++;
						rank = PvpRankDataManager.Instance.GetRank(SaveData.MultiplayerLevel);
						SaveData.PointsInMultiplayerLevel = rank.PointsAfterRankDown();
					}
				}
			}
		}
		pvpMatchResultDetails.EndingLevel = SaveData.MultiplayerLevel;
		pvpMatchResultDetails.EndingPointsInLevel = SaveData.PointsInMultiplayerLevel;
		GrantPvpRankRewards(out pvpMatchResultDetails.Rewards, out pvpMatchResultDetails.GrantedUnlocks);
		return pvpMatchResultDetails;
	}

	private List<GeneralReward> GrantPvpWinRewards()
	{
		int mCurrentLevel = RankXpLevelData.mCurrentLevel;
		PvpRankData rank = PvpRankDataManager.Instance.GetRank(SaveData.MultiplayerLevel);
		foreach (GeneralReward winReward in rank.WinRewards)
		{
			winReward.Grant("pvp win reward");
		}
		if (RankXpLevelData.mCurrentLevel > mCurrentLevel)
		{
			DetachedSingleton<StaminaManager>.Instance.RefillStamina();
			DetachedSingleton<MissionManager>.Instance.AssignGlobalMissions();
		}
		return new List<GeneralReward>(rank.WinRewards);
	}

	private void GrantPvpRankRewards(out List<GeneralReward> grantedRewards, out List<UnlockableData> grantedUnlocks)
	{
		grantedRewards = new List<GeneralReward>();
		grantedUnlocks = new List<UnlockableData>();
		int num = (((int)SaveData.PvpRankRewardsGranted != -1) ? ((int)SaveData.PvpRankRewardsGranted) : (PvpRankDataManager.Instance.GetMaxRank() + 1));
		for (int i = SaveData.MultiplayerLevel; i < num; i++)
		{
			List<GeneralReward> rankRewards = SaveData.ActivePvpSeason.GetRankRewards(i);
			if (rankRewards == null)
			{
				continue;
			}
			foreach (GeneralReward item in rankRewards)
			{
				item.Grant("pvp rank reward");
				if (item.RewardType == GeneralReward.TypeEnum.CardBack)
				{
					grantedUnlocks.Add(item.CardBack);
				}
				else
				{
					grantedRewards.Add(item);
				}
			}
		}
		if ((int)SaveData.PvpRankRewardsGranted == -1 || (int)SaveData.PvpRankRewardsGranted > SaveData.MultiplayerLevel)
		{
			SaveData.PvpRankRewardsGranted = SaveData.MultiplayerLevel;
		}
	}

	public void GrantEndOfSeasonPvpRewards(PvpSeasonData season, int placement, out List<GeneralReward> grantedRewards, out List<UnlockableData> grantedUnlocks)
	{
		grantedRewards = new List<GeneralReward>();
		grantedUnlocks = new List<UnlockableData>();
		List<GeneralReward> leaderboardRewardsForPlacement = season.GetLeaderboardRewardsForPlacement(placement);
		foreach (GeneralReward item in leaderboardRewardsForPlacement)
		{
			item.Grant("pvp season reward");
			if (item.RewardType == GeneralReward.TypeEnum.CardBack)
			{
				grantedUnlocks.Add(item.CardBack);
			}
			else
			{
				grantedRewards.Add(item);
			}
		}
	}

	public string GetTeamName(int index)
	{
		return KFFLocalization.Get("!!TEAM_" + (index + 1));
	}

	public bool AlreadySentMatchHistoryInvite(string playerID)
	{
		return StateData.MatchHistoryInvitesSent.ContainsKey(playerID);
	}

	public void OnSendMatchHistoryInvite(string playerID)
	{
		try
		{
			StateData.MatchHistoryInvitesSent.Add(playerID, null);
		}
		catch
		{
		}
	}

	public SpecialSaleData GetSaleToDisplay(bool ignoreShowDelay = false)
	{
		DateTime serverTime = TFUtils.ServerTime;
		uint num = serverTime.UnixTimestamp();
		if (!ignoreShowDelay && num < SaveData.SaleShowCooldown)
		{
			return null;
		}
		if (SaveData.SelectedSale != null && num < SaveData.SaleEndTime)
		{
			return SaveData.SelectedSale;
		}
		List<SpecialSaleData> list = new List<SpecialSaleData>();
		int num2 = int.MaxValue;
		foreach (SpecialSaleData item in SpecialSaleDataManager.Instance.GetDatabase())
		{
			if (!item.Repeatable)
			{
				SpecialSaleItem value = SaveData.Sales.GetValue(item, null);
				if (value != null && value.PurchasedCount > 0)
				{
					continue;
				}
			}
			if (item.PrerequisiteSale != null)
			{
				SpecialSaleItem value2 = SaveData.Sales.GetValue(item.PrerequisiteSale, null);
				if (value2 == null || value2.PurchasedCount == 0)
				{
					continue;
				}
			}
			if (!(serverTime < item.StartDate) && !(serverTime > item.EndDate) && num >= SaveData.SaleRestartCooldown)
			{
				if (item.Priority < num2)
				{
					list.Clear();
					num2 = item.Priority;
				}
				else if (item.Priority > num2)
				{
					continue;
				}
				list.Add(item);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		SpecialSaleData specialSaleData = list.RandomElement();
		if (SaveData.SelectedSale == specialSaleData && !ignoreShowDelay)
		{
			SaveData.SelectedSale = null;
		}
		return specialSaleData;
	}

	public void OnSaleShown(SpecialSaleData sale)
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		SaveData.SaleShowCooldown = num + (uint)(MiscParams.GlobalSaleCooldownMinutes * 60);
		if (sale != SaveData.SelectedSale)
		{
			SaveData.SelectedSale = sale;
			if (sale.EndDate != DateTime.MaxValue)
			{
				SaveData.SaleEndTime = sale.EndDate.UnixTimestamp();
				SaveData.SaleRestartCooldown = SaveData.SaleEndTime + 1;
			}
			else
			{
				SaveData.SaleEndTime = num + (uint)(sale.ShowDuration * 3600);
				int num2 = sale.ShowDuration + UnityEngine.Random.Range(sale.CooldownMin, sale.CooldownMax + 1);
				SaveData.SaleRestartCooldown = num + (uint)(num2 * 3600);
			}
		}
		Save();
	}

	public void OnSpecialSalePurchased(SpecialSaleData sale)
	{
		SpecialSaleItem specialSaleItem = SaveData.Sales.GetValue(sale, null);
		if (specialSaleItem == null)
		{
			specialSaleItem = new SpecialSaleItem(sale);
			SaveData.Sales.Add(sale, specialSaleItem);
		}
		specialSaleItem.OnPurchased();
		SaveData.SaleEndTime = 0u;
		SpecialSaleData specialSaleData = SpecialSaleDataManager.Instance.GetDatabase().Find((SpecialSaleData m) => m.PrerequisiteSale == sale);
		if (specialSaleData != null)
		{
			SaveData.SelectedSale = specialSaleData;
			uint num = TFUtils.ServerTime.UnixTimestamp();
			int num2 = UnityEngine.Random.Range(specialSaleData.CooldownMin, specialSaleData.CooldownMax + 1);
			SaveData.SaleRestartCooldown = num + (uint)(num2 * 3600);
		}
	}

	public void AddHardCurrency2(int paid, int free, string eventName, int handle, string price)
	{
		paidSave = paid;
		freeSave = free;
		eventNameSave = eventName;
		handleSave = handle;
		priceSave = price;
		inServerCall = ServerCall.PROGRESS;
		whichType = ServerCallType.CURRENCY;
		Singleton<BusyIconPanelController>.Instance.Show();
		SaveData.AddHardCurrency2(paid, free, eventName, handle, Singleton<TBPvPManager>.Instance.CountryCode, price, ServerAccessCallback);
	}

	private void ServerAccessCallback(PlayerSaveData.ActionResult result)
	{
		if (result.success)
		{
			inServerCall = ServerCall.SUCCESS;
		}
		else
		{
			inServerCall = ServerCall.ERROR;
		}
	}

	public void User_Sync(bool initialSync)
	{
		inServerCall = ServerCall.PROGRESS;
		whichType = ServerCallType.SYNC;
		syncDone = false;
		int misc = 0;
		if (initialSync)
		{
			misc = 6535;
		}
		initialSyncSave = initialSync;
		SaveData.User_Action(misc, ServerAccessCallback);
	}

	public int GetGachaSlotCooldownSeconds(GachaSlotData slotType)
	{
		uint value;
		if (!SaveData.GachaSlotCooldowns.TryGetValue(slotType, out value))
		{
			return 0;
		}
		uint num = TFUtils.ServerTime.UnixTimestamp();
		if (value <= num)
		{
			SaveData.GachaSlotCooldowns.Remove(slotType);
			return 0;
		}
		return (int)(value - num);
	}

	public bool IsPastAgeGate(int ageGateInt)
	{
		return true;
	}

	public void CalculateGoldPacksNeeded(int cost, out int packs, out int totalGold, out int totalGemCost)
	{
		int num = cost - SaveData.SoftCurrency;
		packs = 1 + (num - 1) / MiscParams.BuySoftCurrencyAmount;
		totalGemCost = packs * MiscParams.BuySoftCurrencyCost;
		totalGold = packs * MiscParams.BuySoftCurrencyAmount;
	}

	public void BuyGold(int packs, Action inCallback = null)
	{
		StartCoroutine(BuyGoldCo(packs, inCallback));
	}

	private IEnumerator BuyGoldCo(int packs, Action inCallback)
	{
		int result = -1;
		StartCoroutine(ConsumeHardCurrencyCo(MiscParams.BuySoftCurrencyCost * packs, "buy gold", delegate(bool success)
		{
			result = (success ? 1 : 0);
		}));
		while (true)
		{
			switch (result)
			{
			case -1:
				yield return null;
				continue;
			case 0:
				yield break;
			}
			SaveData.SoftCurrency += MiscParams.BuySoftCurrencyAmount * packs;
			Save();
			if (inCallback != null)
			{
				inCallback();
				inCallback = null;
			}
			Singleton<StoreScreenController>.Instance.isProcessingPurchase = false;
			yield break;
		}
	}

	public IEnumerator ConsumeHardCurrencyCo(int amount, string reason, Action<bool> result)
	{
		Singleton<BusyIconPanelController>.Instance.Show();
		int success = -1;
		SaveData.ConsumeHardCurrency2(amount, reason, delegate(PlayerSaveData.ActionResult actionResult)
		{
			if (actionResult.success)
			{
				success = 1;
			}
			else
			{
				success = 0;
			}
		});
		while (success == -1)
		{
			yield return null;
		}
		Singleton<BusyIconPanelController>.Instance.Hide();
		if (success == 0)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), null);
			result(false);
		}
		else
		{
			result(true);
		}
	}

	public int SpeedUpCount(SpeedUpData data)
	{
		int value;
		if (SaveData.SpeedUps.TryGetValue(data, out value))
		{
			return value;
		}
		return 0;
	}

	public void ConsumeSpeedUp(SpeedUpData data)
	{
		int value;
		if (SaveData.SpeedUps.TryGetValue(data, out value))
		{
			SaveData.SpeedUps[data] = value - 1;
		}
	}

	public void AddSpeedUp(SpeedUpData data, int amount)
	{
		int value;
		if (SaveData.SpeedUps.TryGetValue(data, out value))
		{
			SaveData.SpeedUps[data] = value + amount;
		}
		else
		{
			SaveData.SpeedUps.Add(data, amount);
		}
	}

	public int GachaKeyCount(GachaSlotData data)
	{
		int value;
		if (SaveData.GachaKeys.TryGetValue(data, out value))
		{
			return value;
		}
		return 0;
	}

	public void ConsumeGachaKey(GachaSlotData data, int amount)
	{
		int value;
		if (SaveData.GachaKeys.TryGetValue(data, out value))
		{
			SaveData.GachaKeys[data] = value - amount;
		}
	}

	public void AddGachaKey(GachaSlotData data, int amount)
	{
		int value;
		if (SaveData.GachaKeys.TryGetValue(data, out value))
		{
			SaveData.GachaKeys[data] = value + amount;
		}
		else
		{
			SaveData.GachaKeys.Add(data, amount);
		}
	}
}
