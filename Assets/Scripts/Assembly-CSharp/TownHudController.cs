using System;
using System.Collections.Generic;
using UnityEngine;

public class TownHudController : Singleton<TownHudController>
{
	public UITweenController MissionCompleteTween;

	public UITweenController BuildingLockedTween;

	public UILabel PlayerName;

	public UILabel PlayerLevel;

	public UITexture PlayerPortrait;

	public UILabel PlayerXpPercent;

	public UISprite PlayerXpBar;

	public UILabel PlayerStamina;

	public UILabel PlayerStaminaTimer;

	public UILabel PlayerPvpStamina;

	public UILabel PlayerPvpStaminaTimer;

	public UILabel PlayerHardCurrency;

	public UILabel PlayerSoftCurrency;

	public UILabel PlayerPvPCurrency;

	public GameObject PvpStaminaObject;

	public GameObject SoftCurrencyObject;

	public GameObject HardCurrencyObject;

	public GameObject PvpCurrencyObject;

	public UIGrid CurrenciesGrid;

	public GameObject MissionStackObject;

	public UILabel MissionStackCount;

	public UILabel MissionTimerLabel;

	public GameObject GachaButtonsShowHide;

	public GameObject DungeonButtonsShowHide;

	public GameObject MissionsButton;

	public GameObject ExpeditionsButton;

	public UILabel ExpeditionStackValue;

	public UILabel ExpeditionTimerLabel;

	public UILabel DailyChestTimerLabel;

	public UILabel DailyDungeonNameLabel;

	public UILabel DailyDungeonTimerLabel;

	public UITexture DailyDungeonIcon;

	public GameObject BuildingUnlockPanel;

	public UITweenController ShowHUD;

	public UITweenController HideHUD;

	public UITweenController ShowBuildingUnlockBanner;

	public UITweenController ScribbleStartTween;

	public UILabel BuildingUnlockLabel;

	public UILabel BuildingLockedLabel;

	public UISprite[] DeepLinkButtonBgs = new UISprite[0];

	public UITexture LeaderTex;

	public GameObject promoBannerObject;

	private bool mPopulated;

	private int mDisplayedCompleteMissionCount;

	private int mDisplayedExpeditionCount = -1;

	private int mDisplayedCompletedExpeditionCount;

	private UIPanel mPanel;

	private bool mShowingLoadingScreen = true;

	private GachaSlotData _DailyGachaSlotData;

	private QuestSelectController.SpecialLeagueEntry _DailyDungeonLeague;

	private int _GlintIndex;

	public GameObject UIBar;

	private List<FrontEndBuildingUIBar> UIBarObjects = new List<FrontEndBuildingUIBar>();

	private void Awake()
	{
		mPanel = GetComponent<UIPanel>();
	}

	private void Update()
	{
		PopulateIfReady();
		if (mPopulated)
		{
			UpdateDynamicInfo();
			UpdateTimers();
			UpdateCurrencies();
			UpdateMissions();
			UpdateExpeditions();
			UpdateDailyDungeonTimer();
			if (mShowingLoadingScreen)
			{
				mPanel.Refresh();
			}
			if (!LoadingScreenController.ShowingLoadingScreen())
			{
				mShowingLoadingScreen = false;
			}
			else
			{
				mShowingLoadingScreen = true;
			}
			if (!GachaButtonsShowHide.activeSelf)
			{
				GachaButtonsShowHide.SetActive(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Gacha"));
			}
			if (!DungeonButtonsShowHide.activeSelf)
			{
				DungeonButtonsShowHide.SetActive(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Dungeon"));
			}
		}
	}

	private void PopulateIfReady()
	{
		if (!mPopulated && SessionManager.Instance.IsLoadDataDone() && Singleton<PlayerInfoScript>.Instance.IsInitialized)
		{
			mPopulated = true;
			DetachedSingleton<MissionManager>.Instance.ShouldCheckCompletion = true;
			PopulatePlayerInfo();
			_DailyDungeonLeague = GetDailyDungeon();
			UpdateTimers();
			UpdateCurrencies();
			UpdateLeaderIcon();
			SetScheduledUIElements();
			GachaButtonsShowHide.SetActive(false);
			DungeonButtonsShowHide.SetActive(false);
			ScribbleStartTween.Play();
			EnablePromoBanner();
		}
	}

	public void UpdateLeaderIcon()
	{
		Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait.ApplyTexture(LeaderTex);
	}

	public void PopulatePlayerInfo()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		XPLevelData rankXpLevelData = Singleton<PlayerInfoScript>.Instance.RankXpLevelData;
		rankXpLevelData.PopulateUI(false, PlayerLevel, null, PlayerXpPercent, PlayerXpBar);
		PlayerLevel.text = KFFLocalization.Get("!!RANK") + " " + PlayerLevel.text;
		PlayerName.text = Singleton<PlayerInfoScript>.Instance.GetPlayerName();
		Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait.ApplyTexture(PlayerPortrait);
		bool active = Singleton<PlayerInfoScript>.Instance.CanGacha();
		PvpCurrencyObject.SetActive(active);
		HardCurrencyObject.SetActive(active);
		PvpStaminaObject.SetActive(false);
		SoftCurrencyObject.SetActive(Singleton<PlayerInfoScript>.Instance.CanEditDeck());
		CurrenciesGrid.Reposition();
	}

	private void UpdateDynamicInfo()
	{
		PlayerName.text = Singleton<PlayerInfoScript>.Instance.GetPlayerName();
	}

	private void UpdateTimers()
	{
		int currentStamina;
		int maxStamina;
		int secondsUntilNextStamina;
		DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Quests, out currentStamina, out maxStamina, out secondsUntilNextStamina);
		PlayerStamina.text = currentStamina + " / " + maxStamina;
		PlayerStaminaTimer.text = ((secondsUntilNextStamina <= 0) ? string.Empty : PlayerInfoScript.BuildTimerString(secondsUntilNextStamina));
		DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Pvp, out currentStamina, out maxStamina, out secondsUntilNextStamina);
		PlayerPvpStamina.text = currentStamina + " / " + maxStamina;
		PlayerPvpStaminaTimer.text = ((secondsUntilNextStamina <= 0) ? string.Empty : PlayerInfoScript.BuildTimerString(secondsUntilNextStamina));
		MissionTimerLabel.text = KFFLocalization.Get("!!TIME_VAL_LEFT").Replace("<val1>", PlayerInfoScript.FormatTimeString((int)DetachedSingleton<MissionManager>.Instance.GetMinutesUntilQuestRefresh() * 60));
		if (GachaSlotDataManager.Instance != null)
		{
			if (_DailyGachaSlotData == null)
			{
				_DailyGachaSlotData = GachaSlotDataManager.Instance.GetData("Daily");
			}
			int gachaSlotCooldownSeconds = Singleton<PlayerInfoScript>.Instance.GetGachaSlotCooldownSeconds(_DailyGachaSlotData);
			if (gachaSlotCooldownSeconds > 0)
			{
				DailyChestTimerLabel.text = PlayerInfoScript.FormatTimeString(gachaSlotCooldownSeconds, true);
			}
			else
			{
				DailyChestTimerLabel.text = KFFLocalization.Get("!!FREE_CHEST");
			}
		}
		else
		{
			DailyChestTimerLabel.text = string.Empty;
		}
		DailyDungeonTimerLabel.text = "00:00";
	}

	private void UpdateCurrencies()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		PlayerHardCurrency.text = saveData.HardCurrency.ToString();
		PlayerSoftCurrency.text = saveData.SoftCurrency.ToString();
		PlayerPvPCurrency.text = saveData.PvPCurrency.ToString();
	}

	private QuestSelectController.SpecialLeagueEntry GetDailyDungeon()
	{
		List<QuestSelectController.SpecialLeagueEntry> list = QuestSelectController.BuildSpecialLeagueList().FindAll((QuestSelectController.SpecialLeagueEntry m) => m.Clickable);
		for (int i = 0; i < list.Count; i++)
		{
			QuestSelectController.SpecialLeagueEntry specialLeagueEntry = list[i];
			if (specialLeagueEntry.League.AvailabilityType == QuestAvailabilityType.Weekday || specialLeagueEntry.League.AvailabilityType == QuestAvailabilityType.Weekend)
			{
				DailyDungeonNameLabel.text = specialLeagueEntry.League.Name;
				DailyDungeonIcon.ReplaceTexture(specialLeagueEntry.League.LeagueIconTexture);
				return specialLeagueEntry;
			}
		}
		DailyDungeonNameLabel.text = string.Empty;
		return null;
	}

	public void UpdateDailyDungeonTimer()
	{
		if (_DailyDungeonLeague != null)
		{
			bool isVisible;
			bool isClickable;
			bool expired;
			string timeText;
			string fullTimeText;
			DateTime startTime;
			DateTime endTime;
			_DailyDungeonLeague.League.GetTimeStatus(out isVisible, out isClickable, out expired, out timeText, out fullTimeText, out startTime, out endTime);
			DailyDungeonTimerLabel.text = timeText;
		}
		else
		{
			_DailyDungeonLeague = GetDailyDungeon();
		}
	}

	public void OnClickMissions()
	{
		Singleton<MissionListController>.Instance.Show();
	}

	public void OnClickExpeditions()
	{
		Singleton<ExpeditionStartController>.Instance.Show();
	}

	private void UpdateMissions(bool forceUpdate = false)
	{
		bool flag = forceUpdate;
		if (DetachedSingleton<MissionManager>.Instance.IsTimeForDailyMissions())
		{
			DetachedSingleton<MissionManager>.Instance.AssignDailyMissions();
			DetachedSingleton<MissionManager>.Instance.StampDailyMissions();
			flag = true;
		}
		if (flag || DetachedSingleton<MissionManager>.Instance.ShouldCheckCompletion)
		{
			DetachedSingleton<MissionManager>.Instance.CheckCompletion();
			DetachedSingleton<MissionManager>.Instance.ShouldCheckCompletion = false;
			int completedMissionCount = DetachedSingleton<MissionManager>.Instance.GetCompletedMissionCount();
			if (completedMissionCount > 0)
			{
				MissionCompleteTween.Play();
				MissionStackObject.SetActive(true);
				MissionStackCount.text = completedMissionCount.ToString();
				if (completedMissionCount > mDisplayedCompleteMissionCount)
				{
					Singleton<CornerNotificationPopupController>.Instance.Show(CornerNotificationPopupController.PopupTypeEnum.MissionsComplete, completedMissionCount);
				}
			}
			else
			{
				MissionCompleteTween.StopAndReset();
				MissionStackObject.SetActive(false);
			}
			mDisplayedCompleteMissionCount = completedMissionCount;
		}
		if (flag)
		{
			Singleton<MissionListController>.Instance.Repopulate();
		}
	}

	public void ClaimCompletedMission(Mission mission)
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.SoftCurrency += mission.Data.SoftCurrency;
		saveData.PvPCurrency += mission.Data.SocialCurrency;
		Singleton<PlayerInfoScript>.Instance.AddHardCurrency2(0, mission.Data.HardCurrency, "mission reward", -1, string.Empty);
		if (mission.Data.Type == MissionType.Global)
		{
			mission.Claimed = true;
			DetachedSingleton<MissionManager>.Instance.AssignGlobalMissions();
		}
		else
		{
			DetachedSingleton<MissionManager>.Instance.RemoveDailyMission(mission);
		}
		UpdateMissions(true);
		Singleton<PlayerInfoScript>.Instance.Save();
	}

	private void UpdateExpeditions()
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		if (num >= Singleton<PlayerInfoScript>.Instance.SaveData.ExpeditionRefreshTime)
		{
			DetachedSingleton<ExpeditionManager>.Instance.AssignNewExpeditions();
		}
		int num2 = (int)(Singleton<PlayerInfoScript>.Instance.SaveData.ExpeditionRefreshTime - num);
		ExpeditionTimerLabel.text = KFFLocalization.Get("!!TIME_VAL_LEFT").Replace("<val1>", PlayerInfoScript.FormatTimeString(num2, true));
		Singleton<ExpeditionStartController>.Instance.SetTimeUntilRefresh(num2);
		int completed;
		int total;
		DetachedSingleton<ExpeditionManager>.Instance.CompletedOrAvailableExpeditionCount(out completed, out total);
		if (total != mDisplayedExpeditionCount)
		{
			if (total > 0)
			{
				ExpeditionStackValue.transform.SetParentActive(true);
				ExpeditionStackValue.text = total.ToString();
			}
			else
			{
				ExpeditionStackValue.transform.SetParentActive(false);
			}
			mDisplayedExpeditionCount = total;
		}
		if (completed > mDisplayedCompletedExpeditionCount)
		{
			Singleton<CornerNotificationPopupController>.Instance.Show(CornerNotificationPopupController.PopupTypeEnum.ExpeditionsComplete, completed);
		}
		mDisplayedCompletedExpeditionCount = completed;
	}

	public void InitializeBuildings()
	{
		TownBuildingScript[] componentsInChildren = Singleton<TownController>.Instance.gameObject.GetComponentsInChildren<TownBuildingScript>();
		TownBuildingScript[] array = componentsInChildren;
		foreach (TownBuildingScript townBuildingScript in array)
		{
			townBuildingScript.Init();
			GameObject gameObject = Singleton<TownHudController>.Instance.transform.InstantiateAsChild(UIBar);
			FrontEndBuildingUIBar component = gameObject.GetComponent<FrontEndBuildingUIBar>();
			component.mBuilding = townBuildingScript;
			component.Init();
			UIBarObjects.Add(component);
		}
		RefreshBuildingLocks();
		PlayRandomBadgeGlint();
	}

	private void PlayRandomBadgeGlint()
	{
		int num = 0;
		int num2 = UnityEngine.Random.Range(0, UIBarObjects.Count);
		while (num < UIBarObjects.Count)
		{
			num++;
			if (num2 == _GlintIndex || !UIBarObjects[num2].gameObject.activeSelf)
			{
				num2++;
				num2 %= UIBarObjects.Count;
				continue;
			}
			_GlintIndex = num2;
			float time = UnityEngine.Random.Range(1, 5);
			CancelInvoke("PlayBadgeGlint");
			Invoke("PlayBadgeGlint", time);
			break;
		}
	}

	private void PlayBadgeGlint()
	{
		UIBarObjects[_GlintIndex].GlintAnim.PlayOnce();
		PlayRandomBadgeGlint();
	}

	public void RefreshBuildingLocks(bool forceLock = false)
	{
		foreach (FrontEndBuildingUIBar uIBarObject in UIBarObjects)
		{
			if (forceLock)
			{
				uIBarObject.gameObject.SetActive(false);
			}
			else
			{
				uIBarObject.gameObject.SetActive(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(uIBarObject.mBuilding.BuildingId));
			}
		}
		ExpeditionsButton.SetActive(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Expedition"));
	}

	public void HideUIBar(bool hide)
	{
		foreach (FrontEndBuildingUIBar uIBarObject in UIBarObjects)
		{
			uIBarObject.gameObject.SetActive(!hide);
		}
	}

	public void ReturnToTownView()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		Singleton<MouseOrbitCamera>.Instance.UnZoom();
		CheckPromoBanner();
		ShowMainHUD();
	}

	public void ShowMainHUD()
	{
		ShowHUD.Play();
		for (int i = 0; i < UIBarObjects.Count; i++)
		{
			FrontEndBuildingUIBar component = UIBarObjects[i].GetComponent<FrontEndBuildingUIBar>();
			if (component != null && Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(component.mBuilding.BuildingId))
			{
				UIBarObjects[i].ShowTween.Play();
				UIBarObjects[i].TryToShowAttract();
			}
		}
		ExpeditionsButton.SetActive(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Expedition"));
		if (UIBarObjects.Count > 0)
		{
			PlayRandomBadgeGlint();
		}
		EnablePromoBanner();
	}

	public void HideMainHUD()
	{
		promoBannerObject.SetActive(false);
		ExpeditionsButton.SetActive(Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Expedition"));
		HideHUD.Play();
		for (int i = 0; i < UIBarObjects.Count; i++)
		{
			FrontEndBuildingUIBar component = UIBarObjects[i].GetComponent<FrontEndBuildingUIBar>();
			if (component != null && Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(component.mBuilding.BuildingId))
			{
				UIBarObjects[i].HideTween.Play();
			}
		}
		CancelInvoke("PlayBadgeGlint");
	}

	public void OnClickSettings()
	{
		Singleton<TownSettingsController>.Instance.Show();
	}

	public void OnClickCustomize()
	{
		Singleton<PlayerCustomizationController>.Instance.ShowDefault();
	}

	public void ShowLockedBuilding(TownBuildingData building)
	{
		PlayerRankData playerRankData = PlayerRankDataManager.Instance.UnlockRanks.Find((PlayerRankData m) => m.UnlockType == UnlockTypeEnum.Building && m.UnlockId == building.ID);
		if (playerRankData != null)
		{
			BuildingLockedTween.Play();
			BuildingLockedLabel.text = KFFLocalization.Get("!!BUILDING_LOCKED_INFO").Replace("<val1>", playerRankData.Level.ToString());
		}
	}

	public void ShowGachaAttractionPointer()
	{
		foreach (FrontEndBuildingUIBar uIBarObject in UIBarObjects)
		{
			if (uIBarObject != null && uIBarObject.isActiveAndEnabled)
			{
				uIBarObject.TryToShowAttract();
			}
		}
	}

	public void HandleStoreButtonPress()
	{
		TownBuildingScript component = Singleton<TownController>.Instance.GetBuildingObject("TBuilding_Store").GetComponent<TownBuildingScript>();
		component.SendMessage("OnClick");
	}

	public void HandleDailyChestButtonPress()
	{
		Singleton<GachaScreenController>.Instance.DailyChestID = _DailyGachaSlotData.ID;
		TownBuildingScript component = Singleton<TownController>.Instance.GetBuildingObject("TBuilding_Gacha").GetComponent<TownBuildingScript>();
		component.SendMessage("OnClick");
	}

	public void HandleTreasureCaveButtonPress()
	{
		TownBuildingScript component = Singleton<TownController>.Instance.GetBuildingObject("TBuilding_Gacha").GetComponent<TownBuildingScript>();
		component.SendMessage("OnClick");
	}

	public void HandleDailyDungeonButtonPress()
	{
		if (_DailyDungeonLeague != null)
		{
			Singleton<QuestSelectController>.Instance.DailyDungeonLeagueID = _DailyDungeonLeague.League.ID;
		}
		TownBuildingScript component = Singleton<TownController>.Instance.GetBuildingObject("TBuilding_Dungeon").GetComponent<TownBuildingScript>();
		component.SendMessage("OnClick");
	}

	private void CheckPromoBanner()
	{
		PromoBannerController component = promoBannerObject.GetComponent<PromoBannerController>();
		if (component.IsInitialized())
		{
			component.CheckBannerStatus();
		}
	}

	private void SetScheduledUIElements()
	{
		TownScheduleData currentScheduledTownData = TownScheduleDataManager.Instance.GetCurrentScheduledTownData();
		for (int i = 0; i < DeepLinkButtonBgs.Length; i++)
		{
			DeepLinkButtonBgs[i].spriteName = currentScheduledTownData.DeepLinkButtonBgSprite;
		}
	}

	private void EnablePromoBanner()
	{
		PromoBannerController component = promoBannerObject.GetComponent<PromoBannerController>();
		if (component.IsInitialized())
		{
			promoBannerObject.SetActive(true);
			return;
		}
		promoBannerObject.SetActive(false);
		bool flag = false;
		List<EventBannersData> database = EventBannersDataManager.Instance.GetDatabase();
		List<EventTemplateData> database2 = EventTemplateDataManager.Instance.GetDatabase();
		foreach (EventBannersData item in database)
		{
			if (!EventBannersDataManager.Instance.CheckBannerDates(item))
			{
				continue;
			}
			int intEventTemplateId = item.GetIntEventTemplateId();
			foreach (EventTemplateData item2 in database2)
			{
				if (intEventTemplateId == item2.GetIntEventTemplateId() && EventTemplateDataManager.Instance.CheckEventCondition(item2))
				{
					if (!flag)
					{
						promoBannerObject.SetActive(true);
						flag = true;
					}
					component.LoadBannerData(item2, item);
				}
			}
		}
		if (flag)
		{
			component.Init();
			promoBannerObject.SetActive(true);
		}
	}
}
