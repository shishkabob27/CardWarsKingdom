using System;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
	public int version;
	public string PlayerName;
	public bool HasAuthenticated;
	public int PlayersLastSavedLevel;
	public ObscuredString MultiplayerPlayerName;
	public int SelectedLoadout;
	public int MyHelperCreatureID;
	public uint StaminaFullAtTime;
	public uint PvpStaminaFullAtTime;
	public ObscuredUInt ExtraQuestStamina;
	public ObscuredUInt ExtraPvpStamina;
	public ObscuredInt InventorySpace;
	public ObscuredInt AllyBoxSpace;
	public bool NotificationEnabled;
	public int TopCompletedQuestId;
	public int TopShownCompletedQuestId;
	public List<int> EarnedQuestStars;
	public int PvpBattles;
	public float QuestSelectScrollPos;
	public int LastAccessUTC;
	public string LastKnownLocation;
	public bool InviteCodeRedeemed;
	public uint SaleShowCooldown;
	public uint SaleEndTime;
	public uint SaleRestartCooldown;
	public bool UIDFixApplied;
	public ObscuredInt ExpeditionSlots;
	public ObscuredInt BestMultiplayerLevel;
	public ObscuredInt PointsInMultiplayerLevel;
	public ObscuredInt MultiplayerWinStreak;
	public ObscuredInt PvpRankRewardsGranted;
	public bool PlayedFirstBattleInPvpSeason;
	public bool RankedPvpMatchStarted;
	public bool TownTiltCam;
	public int PvpSpecialDomainNumber;
	public int ChatSpecialDomainNumber;
	public int InstalledDate;
	public ObscuredInt RandomDungeonLevel;
	public uint DateOfBirth;
	public int ConfirmedTOSVersion;
	public LanguageCode selectedLang;
	public bool RankUpInventoryGiven;
	public uint ExpeditionRefreshTime;
	public ObscuredInt CustomizationCurrency;
	public ObscuredInt OneTimeCalendarDaysClaimed;
	public int FriendInvites;
	public List<string> FBInviteRewards;
	public List<string> DeletedMail;
}
