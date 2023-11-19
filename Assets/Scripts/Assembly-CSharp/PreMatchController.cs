using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreMatchController : Singleton<PreMatchController>
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public Color[] TutorialTeamColors;

	public UITweenController ShowTween;

	public UITweenController ExceededTeamCostTween;

	public UITweenController ConsumeTicketTween;

	public UITweenController ShowHelperBeingDraggedTween;

	public UITweenController HideHelperBeingDraggedTween;

	public UITweenController BattleStartTween;

	public UITweenController PulseTutorialButtonsTween;

	public UITweenController FadeInHelperButtonTween;

	public UITweenController FadeOutHelperButtonTween;

	public UITexture[] PIPIconTargets = new UITexture[2];

	public UITexture[] PlayerBackgroundTextures = new UITexture[2];

	public Transform[] MyCreatureNodes;

	public Transform[] EnemyCreatureNodes;

	public UILabel StaminaCost;

	public UILabel PlayerStamina;

	public UILabel PlayerStaminaTimer;

	public GameObject EditDeckButton;

	public UILabel QuestName;

	public UISprite IconVS;

	public UILabel TeamCost;

	public UILabel TeamName;

	public UILabel[] LeaderNames = new UILabel[2];

	public GameObject AddHelperButton;

	public GameObject RemoveHelperButton;

	public GameObject HelperBlockingSlot;

	public GameObject StandardQuestParent;

	public GameObject RandomQuestParent;

	public UILabel RandomQuestFloor;

	public UILabel RandomQuestReward;

	public UILabel RandomQuestBattles;

	public GameObject StartButton;

	public GameObject RequiredCreatureParent;

	public GameObject RequiredCreatureSpawnNode;

	public GameObject CloseButton;

	public UILabel TeamDescription;

	public UISprite TeamDescriptionBox;

	private List<GameObject> mMySpawnedCreatures = new List<GameObject>();

	private List<GameObject> mEnemySpawnedCreatures = new List<GameObject>();

	private bool mDeclinedHelper;

	private GameObject mSpawnedRequiredCreature;

	private bool shouldShowHelperTile;

	private bool isHelperTileFadedOut;

	public BoxCollider HelperSlot;

	public bool mDeclineSlotPurchase;

	private string opponentBGTexture;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public void Show()
	{
		AddHelperButton.SetActive(Singleton<PlayerInfoScript>.Instance.CanUseHelper());
		mDeclinedHelper = false;
		mDeclineSlotPurchase = false;
		ShowTween.Play();
		bool flag = Singleton<TutorialController>.Instance.IsFTUETutorialActive();
		if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
		{
			PulseTutorialButtonsTween.Play();
		}
		Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper = null;
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		StaminaCost.text = string.Empty;
		QuestName.text = currentActiveQuest.LevelName;
		EditDeckButton.SetActive(Singleton<PlayerInfoScript>.Instance.CanEditDeck());
		IconVS.spriteName = currentActiveQuest.VSIcon;
		RefreshLoadouts(true);
		Update();
		EditDeckButton.SetActive(!flag);
		CloseButton.SetActive(!flag);
		if (DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests) < currentActiveQuest.StaminaCost && !Singleton<TutorialController>.Instance.IsAnyTutorialActive())
		{
			Singleton<TutorialController>.Instance.StartTutorialBlockIfNotComplete("Stamina");
		}
		shouldShowHelperTile = Singleton<PlayerInfoScript>.Instance.CanUseHelper();
		isHelperTileFadedOut = false;
	}

	public void RefreshLoadouts(bool isInit = false)
	{
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		if (currentActiveQuest.League.ID == "DailyRandom")
		{
			RefreshMyLoadout(true, isInit);
			StandardQuestParent.SetActive(false);
			RandomQuestParent.SetActive(true);
			RandomDungeonFloorData currentData = RandomDungeonFloorDataManager.Instance.GetCurrentData();
			RandomQuestFloor.text = KFFLocalization.Get("!!RANDOM_DUNGEON_FLOOR_X").Replace("<val1>", Singleton<PlayerInfoScript>.Instance.SaveData.RandomDungeonLevel.ToString());
			RandomQuestReward.text = currentActiveQuest.RandomDungeonReward.RewardLabel();
			RandomQuestBattles.text = KFFLocalization.Get("!!X_BATTLES").Replace("<val1>", currentData.Battles.ToString());
		}
		else
		{
			RefreshMyLoadout(false, isInit);
			StandardQuestParent.SetActive(true);
			RandomQuestParent.SetActive(false);
			RefreshEnemyLoadout();
			LeaderData[] leaders = new LeaderData[2]
			{
				Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader.SelectedSkin,
				currentActiveQuest.Opponent
			};
			Singleton<FrontEndPIPController>.Instance.ShowModelPortraits(leaders, PIPIconTargets);
		}
		HideHelperSlot(false);
	}

	private void RefreshMyLoadout(bool loadModel, bool isInit = false)
	{
		if (isInit && Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
		{
			Singleton<PlayerInfoScript>.Instance.SetDefaultLoadout();
		}
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		Loadout currentLoadout2 = Singleton<PlayerInfoScript>.Instance.StateData.CurrentLoadout;
		if (loadModel)
		{
			Singleton<FrontEndPIPController>.Instance.ShowModelPortraits(new LeaderData[1] { currentLoadout.Leader.SelectedSkin }, new UIWidget[1] { PIPIconTargets[0] });
		}
		foreach (GameObject mMySpawnedCreature in mMySpawnedCreatures)
		{
			NGUITools.Destroy(mMySpawnedCreature);
		}
		mMySpawnedCreatures.Clear();
		if (mSpawnedRequiredCreature != null)
		{
			NGUITools.Destroy(mSpawnedRequiredCreature);
			mSpawnedRequiredCreature = null;
		}
		bool clickable = !Singleton<TutorialController>.Instance.IsBlockActive("Q1");
		InventoryTile.ClearDelegates(clickable);
		for (int i = 0; i < currentLoadout.CreatureSet.Count; i++)
		{
			InventorySlotItem inventorySlotItem = null;
			inventorySlotItem = currentLoadout.CreatureSet[i];
			if (inventorySlotItem != null)
			{
				GameObject gameObject = MyCreatureNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
				gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				gameObject.ChangeLayer(gameObject.transform.parent.gameObject.layer);
				InventoryTile component = gameObject.GetComponent<InventoryTile>();
				component.Populate(inventorySlotItem);
				component.ShowRarityFrameMini();
				mMySpawnedCreatures.Add(gameObject);
			}
		}
		int selectedLoadout = Singleton<PlayerInfoScript>.Instance.SaveData.SelectedLoadout;
		TeamName.text = Singleton<PlayerInfoScript>.Instance.GetTeamName(selectedLoadout);
		UpdateTeamCost();
		CheckCreatureRequirement();
		if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
		{
			TeamDescription.parent.gameObject.SetActive(true);
			TeamDescription.text = KFFLocalization.Get("!!TUT_TEAM_DESC_" + (selectedLoadout + 1));
			TeamDescription.color = TutorialTeamColors[selectedLoadout];
			TeamDescriptionBox.color = TutorialTeamColors[selectedLoadout];
		}
		else
		{
			TeamDescription.parent.gameObject.SetActive(false);
		}
	}

	private void UpdateTeamCost()
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		int teamCost = currentLoadout.GetTeamCost(HasHelper());
		int teamCost2 = Singleton<PlayerInfoScript>.Instance.RankData.TeamCost;
		TeamCost.text = KFFLocalization.Get("!!TEAM_WEIGHT") + "  " + teamCost + " / " + teamCost2;
		LeaderNames[0].text = currentLoadout.Leader.Form.Name;
		bool shouldUnload = true;
		if (opponentBGTexture != null && currentLoadout.Leader.Form.VSScreenBackground == opponentBGTexture)
		{
			shouldUnload = false;
		}
		PlayerBackgroundTextures[0].ReplaceTexture(currentLoadout.Leader.Form.VSScreenBackground, shouldUnload);
		if (teamCost > teamCost2)
		{
			ExceededTeamCostTween.Play();
		}
		else
		{
			ExceededTeamCostTween.StopAndReset();
		}
	}

	public void CheckCreatureRequirement()
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		if (currentActiveQuest.RequiredCreature != null && !currentLoadout.ContainsCreatureOrEvo(currentActiveQuest.RequiredCreature, HasHelper()))
		{
			StartButton.SetActive(false);
			RequiredCreatureParent.SetActive(true);
			mSpawnedRequiredCreature = RequiredCreatureSpawnNode.transform.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
			InventoryTile component = mSpawnedRequiredCreature.GetComponent<InventoryTile>();
			component.PopulateAndForceDisplay(currentActiveQuest.RequiredCreature);
			return;
		}
		if (mSpawnedRequiredCreature != null)
		{
			NGUITools.Destroy(mSpawnedRequiredCreature);
			mSpawnedRequiredCreature = null;
		}
		StartButton.SetActive(true);
		RequiredCreatureParent.SetActive(false);
	}

	private void RefreshEnemyLoadout()
	{
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		if (currentActiveQuest == null)
		{
			return;
		}
		InventorySlotItem[] preMatchCreatures = currentActiveQuest.GetPreMatchCreatures();
		foreach (GameObject mEnemySpawnedCreature in mEnemySpawnedCreatures)
		{
			NGUITools.Destroy(mEnemySpawnedCreature);
		}
		mEnemySpawnedCreatures.Clear();
		bool clickable = !Singleton<TutorialController>.Instance.IsBlockActive("Q1");
		InventoryTile.ClearDelegates(clickable);
		for (int i = 0; i < preMatchCreatures.Length; i++)
		{
			if (preMatchCreatures[i] != null)
			{
				GameObject gameObject = EnemyCreatureNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile);
				gameObject.ChangeLayer(gameObject.transform.parent.gameObject.layer);
				gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				InventoryTile component = gameObject.GetComponent<InventoryTile>();
				component.Populate(preMatchCreatures[i]);
				component.DisableCardEditing = true;
				component.ShowRarityFrameMini();
				mEnemySpawnedCreatures.Add(gameObject);
			}
		}
		LeaderNames[1].text = currentActiveQuest.Opponent.Name;
		opponentBGTexture = currentActiveQuest.Opponent.VSScreenBackground;
		PlayerBackgroundTextures[1].ReplaceTexture(currentActiveQuest.Opponent.VSScreenBackground);
	}

	public void OnCloseClicked()
	{
		Singleton<FrontEndPIPController>.Instance.HideModelPortrait();
		OnClickRemoveHelper();
		Singleton<QuestSelectController>.Instance.OnReturnShowBossFX();
	}

	public void Unload()
	{
		foreach (GameObject mMySpawnedCreature in mMySpawnedCreatures)
		{
			NGUITools.Destroy(mMySpawnedCreature);
		}
		mMySpawnedCreatures.Clear();
		foreach (GameObject mEnemySpawnedCreature in mEnemySpawnedCreatures)
		{
			NGUITools.Destroy(mEnemySpawnedCreature);
		}
		mEnemySpawnedCreatures.Clear();
		if (mSpawnedRequiredCreature != null)
		{
			NGUITools.Destroy(mSpawnedRequiredCreature);
			mSpawnedRequiredCreature = null;
		}
	}

	public void OnClickNextLoadout()
	{
		Singleton<PlayerInfoScript>.Instance.GoToNextLoadout(Singleton<TutorialController>.Instance.IsFTUETutorialActive());
		RefreshMyLoadout(true);
	}

	public void OnClickPrevLoadout()
	{
		Singleton<PlayerInfoScript>.Instance.GoToPrevLoadout(Singleton<TutorialController>.Instance.IsFTUETutorialActive());
		RefreshMyLoadout(true);
	}

	public void OnClickPlay()
	{
		GameStateData stateData = Singleton<PlayerInfoScript>.Instance.StateData;
		QuestData currentActiveQuest = stateData.CurrentActiveQuest;
		bool bonusActive;
		QuestBonusType bonusType;
		string timeText;
		currentActiveQuest.League.GetBonusStatus(out bonusActive, out bonusType, out timeText);
		if (bonusActive)
		{
			stateData.ActiveQuestBonus = bonusType;
		}
		else
		{
			stateData.ActiveQuestBonus = QuestBonusType.None;
		}
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		if (!currentLoadout.IsUsable())
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!NEED_CREATURE"));
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			return;
		}
		if (currentLoadout.GetTeamCost(HasHelper()) > Singleton<PlayerInfoScript>.Instance.RankData.TeamCost)
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!EXCEEDS_WEIGHT"));
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			return;
		}
		if (Singleton<PlayerInfoScript>.Instance.SaveData.IsInventorySpaceFull() && Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Sell") && !mDeclineSlotPurchase)
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_FULL_BUY").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), KFFLocalization.Get("!!INVENTORY_FULL_NOBUY"), MiscParams.InventorySpacePurchaseCost, PurchaseSlotsAndTryAgain, BuyInventoryDenied);
			Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
			return;
		}
		bool flag = true;
		int num = ((stateData.ActiveQuestBonus != QuestBonusType.ReducedStamina) ? currentActiveQuest.StaminaCost : currentActiveQuest.ReducedStaminaCost);
		if (DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests) < num)
		{
			if (!Singleton<TutorialController>.Instance.IsFTUETutorialActive())
			{
				if (num > Singleton<PlayerInfoScript>.Instance.RankData.Stamina)
				{
					Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!NO_STAMINA_RANKUP"));
				}
				else
				{
					Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!NO_STAMINA_BUY"), KFFLocalization.Get("!!NO_STAMINA_NOBUY"), MiscParams.StaminaRefillCost, RefillStaminaAndTryAgain);
				}
				Singleton<SLOTAudioManager>.Instance.PlayErrorSound();
				return;
			}
			flag = false;
		}
		if (Singleton<PlayerInfoScript>.Instance.CanUseHelper() && !HasHelper() && !mDeclinedHelper)
		{
			Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, KFFLocalization.Get("!!NO_HELPER_MESSAGE"), DeclineHelperAndTryAgain, OnClickAddHelper);
			return;
		}
		UICamera.LockInput();
		Singleton<KFFUpsightVGController>.Instance.KPIBattleTrack(KFFUpsightVGController.BattleTrackProgress.QuestStart, KFFUpsightVGController.BattleTrackEvent.BattleStart);
		if (flag)
		{
			ConsumeTicketTween.Play();
			DetachedSingleton<StaminaManager>.Instance.ConsumeStamina(StaminaType.Quests, num);
		}
		stateData.CurrentLoadout = currentLoadout;
		if (currentActiveQuest.League.QuestLine == QuestLineEnum.Special)
		{
			if (currentActiveQuest.League.ID == "DailyRandom")
			{
				currentActiveQuest.FinalizeRandomQuest();
				stateData.PerfectRandomDungeonRun = true;
				stateData.NoReviveRandomDungeonRun = true;
			}
			else
			{
				Singleton<PlayerInfoScript>.Instance.OnSpecialQuestAttempted(currentActiveQuest.ID);
				if (stateData.DungeonMapQuest)
				{
					Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeDungeonMap(currentActiveQuest.ID);
				}
			}
		}
		Singleton<AnalyticsManager>.Instance.LogQuestStart(currentActiveQuest.ID);
		Singleton<PlayerInfoScript>.Instance.LogTeamUsedAnalytics(false);
		RewardManager.UpdatePvPInfo();
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null)
		{
			if (Singleton<PlayerInfoScript>.Instance.StateData.UsedHelperList == null)
			{
				Singleton<PlayerInfoScript>.Instance.StateData.UsedHelperList = new List<string>();
			}
			if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.IsAlly == 0)
			{
				Singleton<PlayerInfoScript>.Instance.StateData.UsedHelperList.Add(Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.HelperID);
			}
		}
		Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(currentLoadout.Leader.Form, VOEvent.GameStart);
		StartCoroutine(PlayStartAnimInPrep());
	}

	private void DeclineHelperAndTryAgain()
	{
		mDeclinedHelper = true;
		OnClickPlay();
	}

	private IEnumerator PlayStartAnimInPrep()
	{
		Singleton<SLOTAudioManager>.Instance.PlaySound("UI_VS_Screen_Start");
		Singleton<FrontEndPIPController>.Instance.PlayBattleStartAnim();
		yield return new WaitForSeconds(2.5f);
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(LoadBattleScene);
	}

	private void LoadBattleScene()
	{
		BattleStartTween.StopAndReset();
		DetachedSingleton<SceneFlowManager>.Instance.LoadBattleScene();
	}

	public void BuyInventoryDenied()
	{
		Singleton<KFFUpsightVGController>.Instance.KPIInventoryTrack(KFFUpsightVGController.InventoryTrackEvent.FullModal, Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.SetLoadout.ID, string.Empty);
	}

	private void PurchaseSlotsAndTryAgain()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		string value = MiscParams.InventorySpacePurchaseCost.ToString();
		string upsightEvent = "Economy.GemExit.IncreaseInventory";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("cost", value);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
		Singleton<KFFUpsightVGController>.Instance.KPIInventoryTrack(KFFUpsightVGController.InventoryTrackEvent.FullModal, Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.SetLoadout.ID, string.Empty, KFFUpsightVGController.InventoryModalAction.Purchased);
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		saveData.ConsumeHardCurrency2(MiscParams.InventorySpacePurchaseCost, "inventory space", UserActionCallback);
		mNextFunction = InventorySpaceExecute;
	}

	private void DenyPurchaseSlots()
	{
		mDeclineSlotPurchase = true;
		OnClickPlay();
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

	private void InventorySpaceExecute()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		saveData.AddEmptyInventorySlots(MiscParams.InventorySpacePerPurchase);
		Singleton<BuyInventoryPopupController>.Instance.Show(OnClickPlay);
	}

	private void RefillStaminaAndTryAgain()
	{
		string value = MiscParams.StaminaRefillCost.ToString();
		string upsightEvent = "Economy.GemExit.ReplenishHearts";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("cost", value);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		Singleton<PlayerInfoScript>.Instance.SaveData.ConsumeHardCurrency2(MiscParams.StaminaRefillCost, "stamina refill", UserActionCallback);
		mNextFunction = StaminaRefillExecute;
	}

	private void StaminaRefillExecute()
	{
		DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		Singleton<BuyStaminaPopupController>.Instance.Show(OnClickPlay);
	}

	public unsafe void OnClickEditDeck()
	{
		Singleton<FrontEndPIPController>.Instance.HideModelPortrait();
		HideHelperSlot(true);
		Singleton<EditDeckController>.Instance.Show(new EditDeckController.Callback(this, (IntPtr)__ldftn(PreMatchController.RefreshLoadouts)));
	}

	public void OnClickMyLeader()
	{
		Loadout currentLoadout = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout();
		Singleton<LeaderDetailsController>.Instance.Show(currentLoadout.Leader, OnLeaderPopupClosed);
	}

	private void OnLeaderPopupClosed()
	{
		RefreshMyLoadout(false);
	}

	public void OnClickOpponentLeader()
	{
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		Singleton<LeaderDetailsController>.Instance.Show(currentActiveQuest.Opponent);
	}

	private void Update()
	{
		HelperSlot.gameObject.SetActive(shouldShowHelperTile);
		UpdateTimers();
		HelperBlockingSlot.SetActive(HasHelper());
		RemoveHelperButton.SetActive(HasHelper());
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

	private void OnCloseServerAccessErrorPopup()
	{
	}

	private void UpdateTimers()
	{
		if (PlayerStamina.gameObject.activeInHierarchy)
		{
			int currentStamina;
			int maxStamina;
			int secondsUntilNextStamina;
			DetachedSingleton<StaminaManager>.Instance.GetStaminaInfo(StaminaType.Quests, out currentStamina, out maxStamina, out secondsUntilNextStamina);
			PlayerStamina.text = currentStamina + " / " + maxStamina;
			PlayerStaminaTimer.text = ((secondsUntilNextStamina <= 0) ? string.Empty : PlayerInfoScript.BuildTimerString(secondsUntilNextStamina));
			QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
			bool bonusActive;
			QuestBonusType bonusType;
			string timeText;
			currentActiveQuest.League.GetBonusStatus(out bonusActive, out bonusType, out timeText);
			int num = ((!bonusActive || bonusType != QuestBonusType.ReducedStamina) ? currentActiveQuest.StaminaCost : currentActiveQuest.ReducedStaminaCost);
			StaminaCost.text = num.ToString();
		}
	}

	public void OnClickAddHelper()
	{
		Singleton<PreMatchHelperSelectController>.Instance.Show();
	}

	public void OnClickRemoveHelper()
	{
		Singleton<PreMatchHelperSelectController>.Instance.SetHelperToLoadout(null);
		if (Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile != null)
		{
			Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile.gameObject.name += "_ToBeRemoved";
			NGUITools.Destroy(Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile.gameObject);
		}
		Singleton<PreMatchHelperSelectController>.Instance.SelectedHelperCreatureTile = null;
	}

	public void PlayTweenForSetHelperCreature(CreatureItem creature)
	{
		foreach (GameObject mMySpawnedCreature in mMySpawnedCreatures)
		{
			InventoryTile component = mMySpawnedCreature.GetComponent<InventoryTile>();
			if (component.InventoryItem.Creature == creature)
			{
				component.SetCreatureToTarget();
			}
		}
	}

	public void HideHelperSlot(bool hide)
	{
		if (isHelperTileFadedOut != hide)
		{
			if (hide)
			{
				isHelperTileFadedOut = true;
				FadeOutHelperButtonTween.Play();
			}
			else
			{
				isHelperTileFadedOut = false;
				FadeInHelperButtonTween.Play();
			}
		}
	}

	public void TriggerTweenForDraggingHelper()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.GetCurrentScene() == SceneFlowManager.Scene.FrontEnd)
		{
			Singleton<PreMatchController>.Instance.ShowHelperBeingDraggedTween.Play();
		}
	}

	public void StopTweenForDraggingHelper()
	{
		HideHelperBeingDraggedTween.Play();
		ShowHelperBeingDraggedTween.StopAndReset();
	}

	private bool HasHelper()
	{
		return Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature != null;
	}

	public void OnHelperChanged()
	{
		CheckCreatureRequirement();
		UpdateTeamCost();
	}
}
