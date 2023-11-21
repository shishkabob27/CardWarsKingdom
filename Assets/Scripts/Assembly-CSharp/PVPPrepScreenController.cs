using System.Collections.Generic;
using UnityEngine;

public class PVPPrepScreenController : Singleton<PVPPrepScreenController>
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public float CountdownTime;

	public UILabel HardCurrencyLabel;

	public UILabel SoftCurrencyLabel;

	public UILabel PlayerStamina;

	public UILabel PlayerStaminaTimer;

	public UILabel EnergyCost;

	public LeagueBadge PlayerBadge;

	public LeagueBadge OpponentBadge;

	public UILabel Countdown;

	public GameObject ReadyBlockingCollider;

	public UILabel LevelSetNotificationLabel;

	public UITexture[] PIPIconTargets = new UITexture[2];

	public Transform[] MyCreatureNodes;

	public UILabel MyPlayerNameLabel;

	public UILabel TeamCost;

	public UILabel TeamName;

	public UILabel TitleLabel;

	public UILabel LeaderName;

	public GameObject CancelButton;

	public GameObject BackButton;

	public GameObject PressToSearchLabel;

	public GameObject HeartButtonDecoration;

	public GameObject CheckmarkButtonDecoration;

	public UITexture BackgroundTexture;

	private List<GameObject> mMySpawnedCreatures = new List<GameObject>();

	private bool mWaitingForOpponentStartData;

	private bool mOpponentStartDataReceived;

	private float mMatchStartCountdown = -1f;

	private PvpMode mMode;

	private HelperItem mAlly;

	private int mLastCountdownVal;

	private bool mActive;

	public bool matchFound;

	public float syncTime;

	public ParticleSystem CountdownVFX;

	public GameObject SwordShieldAnimation;

	[Header("Tweens")]
	public UITweenController ShowTween;

	public UITweenController ShowAllyTween;

	public UITweenController HideTween;

	public UITweenController ExceededTeamCostTween;

	public UITweenController ConsumeTicketTween;

	public UITweenController ShowConnectingTween;

	public UITweenController HideConnectingTween;

	public UITweenController OpponentFoundTween;

	public UITweenController ShowFlagsAndCounterTween;

	public UITweenController FriendMatchStartingTween;

	public UITweenController CountdownPulseTween;

	public UITweenController ReadyTween;

	private bool mSearching;

	private bool mBackButtonPressed;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public bool InFriendMatchLobby()
	{
		return mActive && mMode == PvpMode.Friend;
	}

	public void Show(PvpMode mode, string allyName, bool inShouldShowBackButton = false)
	{
		mActive = true;
		ReadyBlockingCollider.SetActive(false);
		SwordShieldAnimation.SetActive(false);
		LevelSetNotificationLabel.text = string.Empty;
		Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode = true;
		Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode = mode == PvpMode.Ranked;
		mMode = mode;
		BackButton.SetActive(inShouldShowBackButton);
		EnergyCost.text = MiscParams.PvpStaminaMatchCost.ToString();
		RefreshMyLoadout();
		mWaitingForOpponentStartData = false;
		mOpponentStartDataReceived = false;
		mMatchStartCountdown = -1f;
		switch (mode)
		{
		case PvpMode.Ranked:
			TitleLabel.text = KFFLocalization.Get("!!RANKED_MATCH");
			break;
		case PvpMode.Unranked:
			TitleLabel.text = KFFLocalization.Get("!!UNRANKED_MATCH");
			break;
		case PvpMode.Friend:
			TitleLabel.text = KFFLocalization.Get("!!ALLY_MATCH");
			break;
		}
		if (mode == PvpMode.Friend)
		{
			ShowAllyTween.Play();
			PressToSearchLabel.SetActive(false);
			HeartButtonDecoration.SetActive(false);
			CheckmarkButtonDecoration.SetActive(true);
		}
		else
		{
			ShowTween.Play();
			PressToSearchLabel.SetActive(true);
			HeartButtonDecoration.SetActive(true);
			CheckmarkButtonDecoration.SetActive(false);
		}
		Singleton<PlayerInfoScript>.Instance.CheckPvpSeasonStatus();
	}

	private void RefreshMyLoadout()
	{
		MyPlayerNameLabel.text = Singleton<PlayerInfoScript>.Instance.PvPData.PlayerName;
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		Singleton<FrontEndPIPController>.Instance.ShowModelPortraits(new LeaderData[1] { currentLoadout.Leader.SelectedSkin }, new UIWidget[1] { PIPIconTargets[0] });
		foreach (GameObject mMySpawnedCreature in mMySpawnedCreatures)
		{
			NGUITools.Destroy(mMySpawnedCreature);
		}
		mMySpawnedCreatures.Clear();
		InventoryTile.ClearDelegates(true);
		for (int i = 0; i < currentLoadout.CreatureSet.Count; i++)
		{
			if (currentLoadout.CreatureSet[i] != null)
			{
				GameObject gameObject = MyCreatureNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
				gameObject.ChangeLayer(base.gameObject.layer);
				InventoryTile component = gameObject.GetComponent<InventoryTile>();
				component.Populate(currentLoadout.CreatureSet[i]);
				component.ShowRarityFrameMini();
				mMySpawnedCreatures.Add(gameObject);
			}
		}
		int selectedLoadout = Singleton<PlayerInfoScript>.Instance.SaveData.SelectedLoadout;
		TeamName.text = Singleton<PlayerInfoScript>.Instance.GetTeamName(selectedLoadout);
		int teamCost = currentLoadout.GetTeamCost();
		int teamCost2 = Singleton<PlayerInfoScript>.Instance.RankData.TeamCost;
		TeamCost.text = KFFLocalization.Get("!!TEAM_WEIGHT") + "  " + teamCost + " / " + teamCost2;
		LeaderName.text = currentLoadout.Leader.Form.Name;
		BackgroundTexture.ReplaceTexture(currentLoadout.Leader.Form.VSScreenBackground);
		if (teamCost > teamCost2)
		{
			ExceededTeamCostTween.Play();
		}
		else
		{
			ExceededTeamCostTween.StopAndReset();
		}
		RefreshMyLeagueBadge(true);
	}

	private void RefreshMyLeagueBadge(bool includeNameAndFlag)
	{
		PlayerBadge.PopulateMyData();
		PlayerBadge.ShowNameAndFlag(includeNameAndFlag);
	}

	private void RefreshOpponentLeagueBadge(bool includeNameAndFlag)
	{
		PvPGameStateData pvPData = Singleton<PlayerInfoScript>.Instance.PvPData;
		if (pvPData.OpponentPortraitData.ID == "Facebook")
		{
			OpponentBadge.PopulateOtherPlayerData(pvPData.OpponentName, pvPData.OpponentPortrait, pvPData.OpponentLevel, pvPData.OpponentBestLevel, Singleton<CountryFlagManager>.Instance.TextureForCountryCode(Singleton<OnlinePvPManager>.Instance.GetOpponentCountryCode()));
		}
		else if (pvPData.OpponentPortraitData.ID == "Default")
		{
			OpponentBadge.PopulateOtherPlayerData(pvPData.OpponentName, pvPData.OpponentLoadout.Leader.SelectedSkin.PortraitTexture, pvPData.OpponentLevel, pvPData.OpponentBestLevel, Singleton<CountryFlagManager>.Instance.TextureForCountryCode(Singleton<OnlinePvPManager>.Instance.GetOpponentCountryCode()));
		}
		else
		{
			OpponentBadge.PopulateOtherPlayerData(pvPData.OpponentName, pvPData.OpponentPortraitData.Texture, pvPData.OpponentLevel, pvPData.OpponentBestLevel, Singleton<CountryFlagManager>.Instance.TextureForCountryCode(Singleton<OnlinePvPManager>.Instance.GetOpponentCountryCode()));
		}
	}

	public void OnClickNextLoadout()
	{
		Singleton<PlayerInfoScript>.Instance.GoToNextLoadout();
		RefreshMyLoadout();
	}

	public void OnClickPrevLoadout()
	{
		Singleton<PlayerInfoScript>.Instance.GoToPrevLoadout();
		RefreshMyLoadout();
	}

	public void OnClickPlay()
	{
		mSearching = false;
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		if (!MiscParams.PvpEnable)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(KFFLocalization.Get("!!GAME_ERROR_CONTACTING"), KFFLocalization.Get("!!PVP_UNDER_MAINTENANCE"), true);
			return;
		}
		if (!currentLoadout.IsUsable())
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!NEED_CREATURE"));
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			return;
		}
		if (currentLoadout.GetTeamCost() > Singleton<PlayerInfoScript>.Instance.RankData.TeamCost)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!EXCEEDS_WEIGHT"));
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			return;
		}
		if (DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Pvp) < MiscParams.PvpStaminaMatchCost)
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!NO_PVP_STAMINA_BUY"), KFFLocalization.Get("!!NO_PVP_STAMINA_NOBUY"), MiscParams.StaminaRefillCost, RefillStaminaAndTryAgain);
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			return;
		}
		Singleton<PlayerInfoScript>.Instance.StateData.CurrentLoadout = currentLoadout;
		if (mMode == PvpMode.Friend)
		{
			ReadyBlockingCollider.SetActive(true);
			ReadyTween.Play();
			mWaitingForOpponentStartData = true;
			Singleton<MultiplayerMessageHandler>.Instance.SendMatchStartData(Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout());
		}
		else
		{
			mSearching = true;
			ShowConnectingTween.Play();
			Singleton<MultiplayerMessageHandler>.Instance.StartMatchmaking();
		}
	}

	public void OnConnectionComplete(bool amIPrimary)
	{
		Singleton<PlayerInfoScript>.Instance.PvPData.AmIPrimary = amIPrimary;
		mWaitingForOpponentStartData = true;
		mSearching = false;
		Singleton<MultiplayerMessageHandler>.Instance.SendMatchStartData(Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout());
	}

	public void OnOpponentDataReceived()
	{
		mOpponentStartDataReceived = true;
		mSearching = false;
	}

	public void OnClickCancelConnect()
	{
		HideConnectingTween.Play();
		mWaitingForOpponentStartData = false;
		mOpponentStartDataReceived = false;
		mSearching = false;
		Singleton<MultiplayerMessageHandler>.Instance.CancelMatchmaking();
	}

	private void LoadBattleScene()
	{
		Singleton<FrontEndPIPController>.Instance.UnloadModels(false);
		Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper = null;
		UICamera.UnlockInput();
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		DetachedSingleton<SceneFlowManager>.Instance.LoadBattleScene();
	}

	private void RefillStaminaAndTryAgain()
	{
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(MiscParams.StaminaRefillCost, "stamina refill", UserActionCallback);
		mNextFunction = StaminaRefillExecute;
	}

	public void UserActionCallback(PlayerSaveData.ActionResult result)
	{
		if (result.success)
		{
			mUserActionProceed = NextAction.PROCEED;
		}
		else
		{
			mUserActionProceed = NextAction.ERROR;
		}
	}

	private void StaminaRefillExecute()
	{
		DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		Singleton<BuyStaminaPopupController>.Instance.Show(OnClickPlay);
	}

	public void OnClickEditDeck()
	{
		Singleton<EditDeckController>.Instance.Show(RefreshMyLoadout);
		Singleton<FrontEndPIPController>.Instance.HideModelPortrait();
	}

	public void OnClickMyLeader()
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		Singleton<LeaderDetailsController>.Instance.Show(currentLoadout.Leader, OnLeaderPopupClosed);
	}

	private void OnLeaderPopupClosed()
	{
		RefreshMyLoadout();
	}

	private void Update()
	{
		UpdateTimers();
		RefreshCurrency();
		if (mWaitingForOpponentStartData && mOpponentStartDataReceived && mMatchStartCountdown == -1f)
		{
			PvPGameStateData pvPData = Singleton<PlayerInfoScript>.Instance.PvPData;
			RefreshMyLeagueBadge(true);
			UICamera.LockInput();
			if (mMode == PvpMode.Friend)
			{
				FriendMatchStartingTween.Play();
				SwordShieldAnimation.SetActive(true);
				ShowFlagsAndCounterTween.Play();
				ReadyBlockingCollider.SetActive(false);
				RefreshOpponentLeagueBadge(true);
			}
			else
			{
				OpponentFoundTween.Play();
				SwordShieldAnimation.SetActive(true);
				ShowFlagsAndCounterTween.Play();
				RefreshOpponentLeagueBadge(true);
			}
			DetachedSingleton<StaminaManager>.Instance.ConsumeStamina(StaminaType.Pvp, MiscParams.PvpStaminaMatchCost);
			Singleton<MultiplayerMessageHandler>.Instance.OnStartBattle();
			if (Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode)
			{
				Singleton<PlayerInfoScript>.Instance.SaveData.RankedPvpMatchStarted = true;
			}
			pvPData.HistoryEntryForThisMatch = BattleHistoryLocalSavesManager.Instance.SaveNewBattleHistory();
			Singleton<PlayerInfoScript>.Instance.Save();
			mMatchStartCountdown = CountdownTime;
			mLastCountdownVal = -1;
		}
		if (mMatchStartCountdown > 0f)
		{
			if (!OpponentFoundTween.AnyTweenPlaying())
			{
				mMatchStartCountdown -= Time.deltaTime;
			}
			if (mMatchStartCountdown < 0f)
			{
				BattleHistoryLocalSavesManager.Instance.CachedOpponentFlag = Singleton<OnlinePvPManager>.Instance.GetOpponentCountryCode();
				mMatchStartCountdown = -1f;
				LoadBattleScene();
			}
			int num = (int)Mathf.Ceil(Mathf.Max(0f, mMatchStartCountdown));
			Countdown.text = num.ToString();
			if (num != mLastCountdownVal)
			{
				CountdownPulseTween.Play();
				mLastCountdownVal = num;
				CountdownVFX.Play();
			}
		}
		if (!mWaitForUserAction)
		{
			return;
		}
		if (mUserActionProceed == NextAction.PROCEED)
		{
			Singleton<BusyIconPanelController>.Instance.Hide();
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			if (mNextFunction != null)
			{
				mNextFunction();
			}
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
		}
		if (mUserActionProceed == NextAction.ERROR)
		{
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			Singleton<BusyIconPanelController>.Instance.Hide();
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), OnCloseServerAccessErrorPopup);
		}
	}

	private void RefreshCurrency()
	{
		SoftCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
		HardCurrencyLabel.text = Singleton<PlayerInfoScript>.Instance.SaveData.HardCurrency.ToString();
	}

	private void OnCloseServerAccessErrorPopup()
	{
	}

	private void UpdateTimers()
	{
		if (matchFound)
		{
			syncTime += Time.deltaTime;
		}
		if (PlayerStamina.gameObject.activeInHierarchy)
		{
			int currentStamina;
			int maxStamina;
			int secondsUntilNextStamina;
			DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Pvp, out currentStamina, out maxStamina, out secondsUntilNextStamina);
			PlayerStamina.text = currentStamina + " / " + maxStamina;
			PlayerStaminaTimer.text = ((secondsUntilNextStamina <= 0) ? string.Empty : PlayerInfoScript.BuildTimerString(secondsUntilNextStamina));
		}
	}

	public void OnBackClicked()
	{
		mBackButtonPressed = true;
		OnCloseClicked();
	}

	public void OnCloseClicked()
	{
		Singleton<MouseOrbitCamera>.Instance.CheckTiltCamSettingBeforeTutorial();
		if (mMode == PvpMode.Friend)
		{
			Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!CONFIRM_LEAVE_PVP_LOBBY"), OnConfirmClose, OnCancelClose);
		}
		else
		{
			OnConfirmClose();
		}
	}

	private void OnConfirmClose()
	{
		mActive = false;
		if (mMode == PvpMode.Friend)
		{
			Singleton<MultiplayerMessageHandler>.Instance.SendLeaveGame("leave");
		}
		Singleton<FrontEndPIPController>.Instance.HideModelPortrait();
		HideTween.Play();
		if (!mBackButtonPressed)
		{
			Singleton<PvPModeSelectController>.Instance.Hide();
			return;
		}
		Singleton<PvPModeSelectController>.Instance.ShowElements();
		mBackButtonPressed = false;
	}

	private void OnCancelClose()
	{
		ShowAllyTween.ReattachBackButtonTarget();
	}

	public void OnFriendLeft()
	{
		if (mActive)
		{
			mActive = false;
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!OTHER_PLAYER_LEFT"), OnConfirmPlayerLeft);
		}
	}

	private void OnConfirmPlayerLeft()
	{
		Singleton<FrontEndPIPController>.Instance.HideModelPortrait();
		HideTween.Play();
		MenuStackManager.RemoveTopItemFromStack(true);
	}

	public void Unload()
	{
		Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode = false;
		foreach (GameObject mMySpawnedCreature in mMySpawnedCreatures)
		{
			NGUITools.Destroy(mMySpawnedCreature);
		}
		mMySpawnedCreatures.Clear();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (mSearching)
		{
			OnClickCancelConnect();
		}
	}

	public void DebugFriend()
	{
		FriendMatchStartingTween.Play();
		SwordShieldAnimation.SetActive(true);
		ShowFlagsAndCounterTween.Play();
		ReadyBlockingCollider.SetActive(false);
	}

	public void DebugMatch()
	{
		OpponentFoundTween.Play();
		SwordShieldAnimation.SetActive(true);
	}
}
