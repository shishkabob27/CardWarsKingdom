using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultsController : Singleton<BattleResultsController>
{
	private enum RewardSlots
	{
		RankXp,
		SoftCurrency,
		SocialCurrency,
		HardCurrency,
		Trophies,
		Count
	}

	private enum State
	{
		Idle,
		Intro,
		Stars,
		RewardsAppear,
		Currency,
		Wait,
		Drops,
		Final
	}

	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public float StateChangeDelay;

	public float CurrencyTickTime;

	public float RareCurrencyTickRate;

	public float LootOpenDelay;

	public float RewardAppearDelay;

	public UITweenController ShowCoolGuyBannerTween;

	public UITweenController ShowDweebBannerTween;

	public UITweenController ShowVictoryTween;

	public AnimatedBannerScript BannerAnim;

	public UITweenController VictoryBannerBGTween;

	public UITweenController FailBannerBGTween;

	public UITweenController HideFailBannerTween;

	public UITweenController ShowPvPVictoryTween;

	public UITweenController ShowLevelUpIconTween;

	public UITweenController ShowBlackFadeTween;

	public UITweenController FinalTween;

	public UITweenController ShowRewardLabelTween;

	public UITweenController ShowRewardGridTween;

	public UITweenController[] ShowRewardSlotTweens = new UITweenController[5];

	public UITweenController ShowRewardLootTween;

	public UITweenController ShowRewardTotalsTween;

	public UITweenController RewardTallyFinishedTween;

	public UITweenController[] StarGetTweens = new UITweenController[3];

	public UITweenController ShowTipTween;

	public UITweenController ShowCreaturesLostTween;

	public UITweenController HideRewardsDuringGacha;

	public UITweenController ShowRewardsAfterGacha;

	public UILabel[] ReceivedRewardAmounts = new UILabel[5];

	public UILabel[] TotalRewardAmounts = new UILabel[5];

	public GameObject[] ReceivedRewardObjects = new GameObject[5];

	public GameObject[] TotalRewardObjects = new GameObject[5];

	public UIGrid ReceivedRewardsGrid;

	public UIGrid TotalRewardsGrid;

	public UISprite ExpBar;

	public UILabel ExpLevel;

	public UIGrid DropsGrid;

	public Transform DropsGridParent;

	public UILabel AvailableStamina;

	public UILabel RetryStaminaCost;

	public UILabel TipLabel;

	public UILabel CreatureLostLabel;

	public GameObject[] WinLoseBanner;

	public UIGrid ButtonsGrid;

	public GameObject ReplayButton;

	public GameObject TownButton;

	public GameObject WorldButton;

	public GameObject NextRandomBattleButton;

	public Texture2D[] EvoMisteryTextures;

	private State mState;

	private int mCurrentRewardType;

	private RewardManager.QuestRewards mQuestRewards;

	private float[] mTickRates = new float[5];

	private int[] mAmountsToGive = new int[5];

	private int[] mCurrentRewardAmounts = new int[5];

	private QuestData mNextQuest;

	private Vector3 mLootStartPos;

	private XPLevelData mCurrentLevelData;

	private List<InventoryTile> mLoot = new List<InventoryTile>();

	private float mStateDelay;

	private bool mShowingLevelUp;

	private int mLootOpenIndex;

	private float mTickTimer;

	private bool mRewardsAppearing;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	public UILabel PrintDebugState;

	private bool mCoinLoopSoundStarted;

	private bool mExpLoopSoundStarted;

	private bool Skipped;

	private bool ToMap;

	private bool ToTown;

	public bool Showing { get; private set; }

	private void Awake()
	{
		mLootStartPos = DropsGridParent.localPosition;
	}

	private void Update()
	{
		RefreshStamina();
		UpdateRewardsState();
		UpdateRewardValues();
		if (RetryStaminaCost.gameObject.activeInHierarchy)
		{
			UpdateRetryStaminaCost();
		}
		PrintDebugState.text = mState.ToString();
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

	public void ShowVictory()
	{
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_Badlands");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_CandyKingdom");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_FireKingdom");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_IceKingdom");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_LumpySpace");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_MarcelinesCave");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_OilRig");
		Singleton<SLOTAudioManager>.Instance.StopSound("SFX_BoardAmb_TreeFort");
		SetState(State.Intro);
		ShowStars();
		if (ShowVictoryTween.gameObject.activeSelf)
		{
			ShowVictoryTween.Play();
		}
		Showing = true;
		GameObject[] receivedRewardObjects = ReceivedRewardObjects;
		foreach (GameObject gameObject in receivedRewardObjects)
		{
			gameObject.SetActive(false);
		}
		GameObject[] totalRewardObjects = TotalRewardObjects;
		foreach (GameObject gameObject2 in totalRewardObjects)
		{
			gameObject2.SetActive(false);
		}
	}

	public void PlayBannerAnim()
	{
		BannerAnim.TriggerBannerAnim();
	}

	public void PopulateRewards()
	{
		PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
		mCurrentLevelData = Singleton<PlayerInfoScript>.Instance.RankXpLevelData;
		mCurrentRewardAmounts[1] = saveData.SoftCurrency;
		mCurrentRewardAmounts[2] = saveData.PvPCurrency;
		mCurrentRewardAmounts[3] = saveData.HardCurrency;
		mCurrentRewardAmounts[0] = saveData.RankXP;
		mQuestRewards = RewardManager.AccumulateRewards(true);
		mAmountsToGive[1] = mQuestRewards.SoftCurrency;
		mAmountsToGive[2] = mQuestRewards.SocialCurrency;
		mAmountsToGive[3] = mQuestRewards.HardCurrency;
		mAmountsToGive[0] = mQuestRewards.XP;
		Vector3 vector = mLootStartPos;
		for (int i = 0; i < 5; i++)
		{
			if (i == 2 || i == 3 || i == 4)
			{
				mTickRates[i] = RareCurrencyTickRate;
			}
			else
			{
				mTickRates[i] = (float)mAmountsToGive[i] / CurrencyTickTime;
			}
			if (mAmountsToGive[i] <= 0)
			{
				vector.y += ReceivedRewardsGrid.cellHeight;
			}
		}
		SendKPITrack();
		UpdateRewardValues();
	}

	private void SendKPITrack()
	{
		if (mAmountsToGive[0] >= mCurrentLevelData.mRemainingXPToLevelUp)
		{
			string value = (mCurrentLevelData.mCurrentLevel + 1).ToString();
			string upsightEvent = "Player.RankUp";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("newRank", value);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null && !Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.Fake)
		{
			string upsightEvent2 = "Player.Help";
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			string helperID = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.HelperID;
			string value2 = Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.HelperCreature.Creature.ToString();
			int num = 0;
			num = ((Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.IsAlly != 1) ? MiscParams.HelpPointForExplorer : MiscParams.HelpPointForAlly);
			dictionary2.Add("helper", helperID);
			dictionary2.Add("creatureReceived", value2);
			dictionary2.Add("wishboneReceived", num);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent2, dictionary2);
		}
		string iD = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.ID;
		if (mAmountsToGive[1] > 0)
		{
			string value3 = mAmountsToGive[1].ToString();
			string upsightEvent3 = "Economy.CoinEnter.Quest";
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.Add("questID", iD);
			dictionary3.Add("amount", value3);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent3, dictionary3);
		}
		if (mAmountsToGive[2] > 0)
		{
			string value4 = mAmountsToGive[2].ToString();
			string upsightEvent4 = "Economy.WishboneEnter.Quest";
			Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
			dictionary4.Add("questID", iD);
			dictionary4.Add("amount", value4);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent4, dictionary4);
		}
		if (mAmountsToGive[3] > 0)
		{
			string value5 = mAmountsToGive[3].ToString();
			string upsightEvent5 = "Economy.GemEnter.Quest";
			Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
			dictionary5.Add("questID", iD);
			dictionary5.Add("amount", value5);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent5, dictionary5);
		}
		int num2 = 0;
		foreach (InventorySlotItem item in mQuestRewards.XPMatsLooted)
		{
			num2++;
			string iD2 = item.XPMaterial.ID;
			string upsightEvent6 = "Ingredients.Acquired";
			Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
			dictionary6.Add("ingredientID", iD2);
			dictionary6.Add("type", InventorySlotType.XPMaterial.ToString());
			dictionary6.Add("amount", "1");
			dictionary6.Add("source", "Quest");
			dictionary6.Add("sourceID", iD);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent6, dictionary6);
		}
		foreach (InventorySlotItem item2 in mQuestRewards.EvoMatsLooted)
		{
			num2++;
			string iD3 = item2.EvoMaterial.ID;
			string upsightEvent7 = "Ingredients.Acquired";
			Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
			dictionary7.Add("ingredientID", iD3);
			dictionary7.Add("type", InventorySlotType.EvoMaterial.ToString());
			dictionary7.Add("amount", "1");
			dictionary7.Add("source", "Quest");
			dictionary7.Add("sourceID", iD);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent7, dictionary7);
		}
		foreach (InventorySlotItem item3 in mQuestRewards.CreaturesLooted)
		{
			num2++;
			string upsightEvent8 = "Creatures.CreatureAcquired";
			string value6 = item3.Creature.ToString();
			Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
			dictionary8.Add("creatureID", value6);
			dictionary8.Add("acquisition", "Quest");
			dictionary8.Add("id", iD);
			Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent8, dictionary8);
		}
		Singleton<KFFUpsightVGController>.Instance.KPIBattleTrack(KFFUpsightVGController.BattleTrackProgress.QuestResult, KFFUpsightVGController.BattleTrackEvent.BattleWon);
		if (Singleton<PlayerInfoScript>.Instance.SaveData.IsInventorySpaceFull())
		{
			Singleton<KFFUpsightVGController>.Instance.KPIInventoryTrack(KFFUpsightVGController.InventoryTrackEvent.Full, Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.SetLoadout.ID, num2.ToString());
		}
	}

	private void RefreshStamina()
	{
		if (AvailableStamina.gameObject.activeInHierarchy)
		{
			AvailableStamina.text = DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests).ToString();
		}
	}

	private void SetState(State state)
	{
		mState = state;
		if (state >= State.Currency)
		{
			mStateDelay = StateChangeDelay;
		}
		mTickTimer = 0f;
	}

	private void UpdateRewardsState()
	{
		float deltaTime = Time.deltaTime;
		if (mShowingLevelUp || Singleton<GachaOpenSequencer>.Instance.Showing)
		{
			return;
		}
		if (mStateDelay > 0f)
		{
			mStateDelay -= deltaTime;
			if (!(mStateDelay <= 0f))
			{
				return;
			}
			mStateDelay = 0f;
		}
		if (mState == State.RewardsAppear)
		{
			if (!mRewardsAppearing)
			{
				SetState(State.Currency);
			}
		}
		else if (mState == State.Currency)
		{
			if (mCurrentRewardType == 0)
			{
				mCurrentLevelData = Singleton<PlayerInfoScript>.Instance.RankXpLevelDataAt(mCurrentRewardAmounts[0]);
			}
			mTickTimer += deltaTime;
			float num = 0f;
			int num2 = 1;
			if (mTickRates[mCurrentRewardType] != 0f)
			{
				num = 1f / mTickRates[mCurrentRewardType];
				num2 = (int)(mTickTimer / num);
			}
			if (num2 < 1)
			{
				return;
			}
			mTickTimer -= (float)num2 * num;
			if (Skipped || num2 > mAmountsToGive[mCurrentRewardType])
			{
				num2 = mAmountsToGive[mCurrentRewardType];
			}
			if (mCurrentRewardType == 0 && num2 > mCurrentLevelData.mXPToPassCurrentLevel)
			{
				num2 = mCurrentLevelData.mXPToPassCurrentLevel;
			}
			mAmountsToGive[mCurrentRewardType] -= num2;
			mCurrentRewardAmounts[mCurrentRewardType] += num2;
			if (mCurrentRewardType == 0 && mCurrentRewardAmounts[mCurrentRewardType] >= mCurrentLevelData.mXPToPassCurrentLevel)
			{
				Skipped = false;
				PlayCoinSound(false);
				ShowLevelUp();
			}
			if (mAmountsToGive[mCurrentRewardType] <= 0)
			{
				if (mCurrentRewardType == 0)
				{
					mCurrentLevelData = Singleton<PlayerInfoScript>.Instance.RankXpLevelDataAt(mCurrentRewardAmounts[0]);
				}
				mCurrentRewardType++;
				if (mCurrentRewardType >= 5)
				{
					Skipped = false;
					PlayCoinSound(false);
					SetState(State.Wait);
					RewardTallyFinishedTween.PlayWithCallback(RewardsEndTweenFinished);
				}
				else if (mCurrentRewardType == 0)
				{
					PlayCoinSound(false);
				}
			}
			else if (mCurrentRewardType == 0)
			{
				PlayCoinSound(false);
				PlayExpSound(true);
			}
			else
			{
				PlayExpSound(false);
				PlayCoinSound(true);
			}
		}
		else if (mState == State.Drops)
		{
			PlayCoinSound(false);
			PlayExpSound(false);
			if (!mLoot[mLootOpenIndex].OpenLoot())
			{
				mStateDelay = LootOpenDelay;
			}
			mLootOpenIndex++;
			if (mLootOpenIndex < mLoot.Count)
			{
				return;
			}
			foreach (InventoryTile item in mLoot)
			{
				item.GetComponent<Collider>().enabled = true;
			}
			SetState(State.Final);
		}
		else if (mState == State.Final)
		{
			PlayCoinSound(false);
			PlayExpSound(false);
			FinalTween.Play();
			RefreshStamina();
			SetState(State.Idle);
			if (Singleton<TutorialController>.Instance.IsAnyTutorialActive())
			{
				UICamera.UnlockInput();
			}
			if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
			{
				UICamera.UnlockInput();
			}
		}
	}

	private void PlayCoinSound(bool playing)
	{
		if (playing)
		{
			if (!mCoinLoopSoundStarted)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_CoinLoop2");
				mCoinLoopSoundStarted = true;
			}
		}
		else if (mCoinLoopSoundStarted)
		{
			Singleton<SLOTAudioManager>.Instance.StopSound("SFX_CoinLoop2");
			mCoinLoopSoundStarted = false;
		}
	}

	private void PlayExpSound(bool playing)
	{
		if (playing)
		{
			if (!mExpLoopSoundStarted)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("UI_ExpLoop");
				mExpLoopSoundStarted = true;
			}
		}
		else if (mExpLoopSoundStarted)
		{
			Singleton<SLOTAudioManager>.Instance.StopSound("UI_ExpLoop");
			mExpLoopSoundStarted = false;
		}
	}

	private void RewardsEndTweenFinished()
	{
		if (mLoot.Count > 0)
		{
			SetState(State.Drops);
		}
		else
		{
			SetState(State.Final);
		}
	}

	private void UpdateRewardValues()
	{
		if (mState == State.Idle)
		{
			return;
		}
		int num = ((!mShowingLevelUp) ? mCurrentLevelData.mXPEarnedWithinCurrentLevel : mCurrentLevelData.mTotalXPInCurrentLevel);
		float fillAmount = ((!mShowingLevelUp) ? mCurrentLevelData.mPercentThroughCurrentLevel : 1f);
		for (int i = 0; i < 5; i++)
		{
			if (i == 0)
			{
				ReceivedRewardAmounts[i].text = mAmountsToGive[i] + " " + KFFLocalization.Get("!!EXP");
				TotalRewardAmounts[i].text = num + " / " + mCurrentLevelData.mTotalXPInCurrentLevel + " " + KFFLocalization.Get("!!EXP");
			}
			else
			{
				ReceivedRewardAmounts[i].text = mAmountsToGive[i].ToString();
				TotalRewardAmounts[i].text = mCurrentRewardAmounts[i].ToString();
			}
		}
		ExpBar.fillAmount = fillAmount;
		ExpLevel.text = KFFLocalization.Get("!!RANK") + " " + mCurrentLevelData.mCurrentLevel;
	}

	public void SkipRewardStates()
	{
		Skipped = true;
	}

	public void ShowStars()
	{
		if (ShowCreaturesLostTween.gameObject.activeSelf)
		{
			ShowCreaturesLostTween.PlayWithCallback(OnCreatureLostFinished);
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_PostMatchText");
		int deadCreatureCount = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User).GetDeadCreatureCount();
		if (deadCreatureCount == 1)
		{
			CreatureLostLabel.text = KFFLocalization.Get("!!1_CREATURE_LOST");
		}
		else
		{
			CreatureLostLabel.text = KFFLocalization.Get("!!X_CREATURES_LOST").Replace("<val1>", deadCreatureCount.ToString());
		}
	}

	private void OnCreatureLostFinished()
	{
		if (mQuestRewards.Stars > 0)
		{
			SetState(State.Stars);
			for (int i = 0; i < mQuestRewards.Stars; i++)
			{
				if (i == mQuestRewards.Stars - 1)
				{
					StarGetTweens[i].PlayWithCallback(OnStarsFinished);
				}
				else
				{
					StarGetTweens[i].Play();
				}
			}
		}
		else
		{
			OnStarsFinished();
		}
	}

	private void OnStarsFinished()
	{
		ShowRewardGridTween.Play();
		if (ShowRewardLabelTween.gameObject.activeSelf)
		{
			ShowRewardLabelTween.Play();
		}
		mCurrentRewardType = 0;
		for (int i = 0; i < 5; i++)
		{
			ReceivedRewardObjects[i].SetActive(mAmountsToGive[i] > 0);
			TotalRewardObjects[i].SetActive(mAmountsToGive[i] > 0);
		}
		ReceivedRewardsGrid.Reposition();
		TotalRewardsGrid.Reposition();
		GameObject[] receivedRewardObjects = ReceivedRewardObjects;
		foreach (GameObject gameObject in receivedRewardObjects)
		{
			gameObject.SetActive(false);
		}
		mLootOpenIndex = 0;
		mLoot.Clear();
		Singleton<AnalyticsManager>.Instance.LogQuestWon(Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.ID);
		InventoryTile.ClearDelegates(true);
		foreach (InventorySlotItem item in mQuestRewards.CreaturesLooted)
		{
			InventoryTile component = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component.Populate(item);
			component.SetLootMode();
			component.GetComponent<Collider>().enabled = false;
			component.RarityStarGroup.SetActive(false);
			component.HideRarityFrame();
			mLoot.Add(component);
		}
		foreach (InventorySlotItem item2 in mQuestRewards.EvoMatsLooted)
		{
			InventoryTile component2 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component2.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component2.Populate(item2);
			component2.SetLootMode();
			component2.GetComponent<Collider>().enabled = false;
			mLoot.Add(component2);
		}
		foreach (InventorySlotItem item3 in mQuestRewards.XPMatsLooted)
		{
			InventoryTile component3 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component3.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component3.Populate(item3);
			component3.SetLootMode();
			component3.GetComponent<Collider>().enabled = false;
			mLoot.Add(component3);
		}
		foreach (InventorySlotItem item4 in mQuestRewards.CardsLooted)
		{
			InventoryTile component4 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component4.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component4.Populate(item4);
			component4.SetLootMode();
			component4.GetComponent<Collider>().enabled = false;
			mLoot.Add(component4);
		}
		foreach (GachaSlotData item5 in mQuestRewards.KeysLooted)
		{
			InventoryTile component5 = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component5.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component5.Populate(item5);
			component5.SetLootMode(true, item5.KeyRarity);
			component5.GetComponent<Collider>().enabled = false;
			mLoot.Add(component5);
		}
		DropsGrid.Reposition();
		DropsGrid.gameObject.SetActive(false);
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		UpdateRetryStaminaCost();
		int num = QuestDataManager.Instance.GetIndex(currentActiveQuest) + 1;
		List<QuestData> database = QuestDataManager.Instance.GetDatabase();
		if (num < database.Count)
		{
			mNextQuest = database[num];
		}
		else
		{
			mNextQuest = null;
		}
		bool flag = false;
		for (int k = Singleton<PlayerInfoScript>.Instance.SaveData.PlayersLastSavedLevel + 1; k <= Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel; k++)
		{
			PlayerRankData data = PlayerRankDataManager.Instance.GetData(k - 1);
			if (data.UnlockType != 0)
			{
				flag = true;
				break;
			}
		}
		NextRandomBattleButton.SetActive(false);
		if (flag || currentActiveQuest.BroadcastWin)
		{
			ReplayButton.SetActive(false);
			WorldButton.SetActive(false);
			TownButton.SetActive(true);
		}
		else
		{
			TownButton.SetActive(false);
			WorldButton.SetActive(true);
			if (currentActiveQuest.IsRandomDungeonBattle)
			{
				ReplayButton.SetActive(false);
				NextRandomBattleButton.SetActive(!currentActiveQuest.IsFinalRandomBattle);
				if (mQuestRewards.Stars < 3)
				{
					Singleton<PlayerInfoScript>.Instance.StateData.PerfectRandomDungeonRun = false;
				}
			}
			else if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null)
			{
				ReplayButton.SetActive(false);
			}
			else
			{
				ReplayButton.SetActive(true);
			}
		}
		ButtonsGrid.Reposition();
		mRewardsAppearing = true;
		StartCoroutine(ShowRewardsCo());
		SetState(State.RewardsAppear);
	}

	private void UpdateRetryStaminaCost()
	{
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		bool bonusActive;
		QuestBonusType bonusType;
		string timeText;
		currentActiveQuest.League.GetBonusStatus(out bonusActive, out bonusType, out timeText);
		int num = ((!bonusActive || bonusType != QuestBonusType.ReducedStamina) ? currentActiveQuest.StaminaCost : currentActiveQuest.ReducedStaminaCost);
		RetryStaminaCost.text = num.ToString();
	}

	private IEnumerator ShowRewardsCo()
	{
		if (mLoot.Count > 0)
		{
			foreach (InventoryTile thisTile in mLoot)
			{
				thisTile.HideRarityFrame();
			}
		}
		for (int i = 0; i < ShowRewardSlotTweens.Length; i++)
		{
			if (mAmountsToGive[i] > 0)
			{
				ShowRewardSlotTweens[i].Play();
				yield return new WaitForSeconds(RewardAppearDelay);
			}
		}
		if (mLoot.Count > 0)
		{
			ShowLoot();
		}
		else
		{
			LastRewardAppearTweenDone();
		}
	}

	private void ShowLoot()
	{
		ShowRewardLootTween.PlayWithCallback(LastRewardAppearTweenDone);
	}

	private void LastRewardAppearTweenDone()
	{
		mRewardsAppearing = false;
	}

	private void ShowLevelUp()
	{
		mShowingLevelUp = false;
	}

	public void LevelUpFinished()
	{
		mShowingLevelUp = false;
	}

	public void OnClickNextPage()
	{
		FinalTween.Play();
	}

	public void OnClickReplay()
	{
		if (Singleton<PlayerInfoScript>.Instance.SaveData.IsInventorySpaceFull() && Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("TBuilding_Sell"))
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!INVENTORY_FULL_BUY").Replace("<val2>", MiscParams.InventorySpacePerPurchase.ToString()), KFFLocalization.Get("!!INVENTORY_FULL_NOBUY"), MiscParams.InventorySpacePurchaseCost, PurchaseSlotsAndReplay, BuyInventoryDenied);
			return;
		}
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		bool bonusActive;
		QuestBonusType bonusType;
		string timeText;
		currentActiveQuest.League.GetBonusStatus(out bonusActive, out bonusType, out timeText);
		int num = ((!bonusActive || bonusType != QuestBonusType.ReducedStamina) ? currentActiveQuest.StaminaCost : currentActiveQuest.ReducedStaminaCost);
		if (DetachedSingleton<StaminaManager>.Instance.GetStamina(StaminaType.Quests) < num)
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!NO_STAMINA_BUY"), KFFLocalization.Get("!!NO_STAMINA_NOBUY"), MiscParams.StaminaRefillCost, RefillStaminaAndReplay);
			return;
		}
		Singleton<KFFUpsightVGController>.Instance.KPIBattleTrack(KFFUpsightVGController.BattleTrackProgress.QuestStart, KFFUpsightVGController.BattleTrackEvent.BattleRetry);
		DetachedSingleton<StaminaManager>.Instance.ConsumeStamina(StaminaType.Quests, num);
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		Singleton<PlayerInfoScript>.Instance.LogTeamUsedAnalytics(false);
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
		{
			DetachedSingleton<SceneFlowManager>.Instance.LoadBattleScene();
		});
	}

	public void BuyInventoryDenied()
	{
		Singleton<KFFUpsightVGController>.Instance.KPIInventoryTrack(KFFUpsightVGController.InventoryTrackEvent.FullModal, Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.SetLoadout.ID, string.Empty);
	}

	private void PurchaseSlotsAndReplay()
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
		Singleton<BuyInventoryPopupController>.Instance.Show(OnClickReplay);
	}

	private void RefillStaminaAndReplay()
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
		mNextFunction = RefillStaminaExecute;
	}

	private void RefillStaminaExecute()
	{
		DetachedSingleton<StaminaManager>.Instance.RefillStamina();
		Singleton<BuyStaminaPopupController>.Instance.Show(OnClickReplay);
	}

	public void OnClickMap()
	{
		GameStateData stateData = Singleton<PlayerInfoScript>.Instance.StateData;
		ToMap = true;
		if (stateData.SelectedHelper != null && !stateData.SelectedHelper.Fake)
		{
			ShowAllyInvite();
		}
		else
		{
			ProceedToFrontEnd();
		}
	}

	public void OnClickTown()
	{
		GameStateData stateData = Singleton<PlayerInfoScript>.Instance.StateData;
		ToTown = true;
		if (!stateData.MultiplayerMode && stateData.SelectedHelper != null && !stateData.SelectedHelper.Fake)
		{
			ShowAllyInvite();
		}
		else
		{
			ProceedToFrontEnd();
		}
	}

	public void OnClickNextRandomBattle()
	{
		GameStateData stateData = Singleton<PlayerInfoScript>.Instance.StateData;
		QuestData questData = new QuestData();
		questData.CreateRandomQuest(stateData.CurrentActiveQuest.RandomBattleIndex + 1, stateData.CurrentActiveQuest.RandomDungeonReward);
		questData.FinalizeRandomQuest();
		stateData.CurrentActiveQuest = questData;
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
		{
			DetachedSingleton<SceneFlowManager>.Instance.LoadBattleScene();
		});
	}

	private void ShowAllyInvite()
	{
		HideRewardsDuringGacha.Play();
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper.IsAlly == 1)
		{
			string title = "Help Reward";
			int helpPointForAlly = MiscParams.HelpPointForAlly;
			string body = string.Format(KFFLocalization.Get("!!HELPER_REWARD"), helpPointForAlly, KFFLocalization.Get("!!CURRENCY_PVP"));
			Singleton<HelperRequestController>.Instance.ShowHelpRewardConfirm(title, body, SendHelpReward);
		}
		else
		{
			string title2 = KFFLocalization.Get("!!REGISTER_HELPER");
			int helpPointForExplorer = MiscParams.HelpPointForExplorer;
			string text = string.Format(KFFLocalization.Get("!!HELPER_REWARD"), helpPointForExplorer, KFFLocalization.Get("!!CURRENCY_PVP"));
			text = text + " " + KFFLocalization.Get("!!HELPER_FRIENDSUGGEST");
			Singleton<HelperRequestController>.Instance.ShowAllyInvite(title2, text, SendAllyInviteAndReward, SendHelpReward);
		}
	}

	private void SendAllyInviteAndReward()
	{
		Singleton<HelperRequestController>.Instance.ShouldSendAllyInvite = true;
		Singleton<HelperRequestController>.Instance.SendAllyInviteAndReward();
		ShowRewardsAfterGacha.Play();
	}

	private void SendHelpReward()
	{
		Singleton<HelperRequestController>.Instance.ShouldSendAllyInvite = false;
		Singleton<HelperRequestController>.Instance.SendAllyInviteAndReward();
		ShowRewardsAfterGacha.Play();
	}

	public void ProceedToFrontEnd()
	{
		if (Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") || Singleton<TutorialController>.Instance.IsFTUETutorialActive())
		{
			ProceedToFrontEndAfterTip();
		}
		else
		{
			ShowVictoryTip();
		}
	}

	private void ShowVictoryTip()
	{
		TipEntry randomTip = TipsDataManager.Instance.GetRandomTip(TipEntry.TipContext.Victory);
		if (randomTip != null)
		{
			ShowTipTween.Play();
			TipLabel.text = randomTip.Text;
		}
		else
		{
			ProceedToFrontEndAfterTip();
		}
	}

	public void ProceedToFrontEndAfterTip()
	{
		Singleton<SLOTMusic>.Instance.StopMusic(0.5f);
		SceneFlowManager.ReturnLocation returnLocation = SceneFlowManager.ReturnLocation.NotSet;
		if (ToMap)
		{
			returnLocation = Singleton<PlayerInfoScript>.Instance.GetCurrentReturnLocation();
		}
		else if (ToTown)
		{
			returnLocation = SceneFlowManager.ReturnLocation.Town;
		}
		Singleton<ScreenFadeController>.Instance.ShowLoadScreen(delegate
		{
			StartCoroutine(ChangeScene(returnLocation));
		});
	}

	public void OnClickTower()
	{
	}

	private IEnumerator ChangeScene(SceneFlowManager.ReturnLocation returnLoc)
	{
		yield return new WaitForSeconds(0.6f);
		Singleton<DWBattleLane>.Instance.ClearPooledData();
		DetachedSingleton<SceneFlowManager>.Instance.LoadFrontEndScene(returnLoc);
		yield return null;
	}

	public void AndroidBackButtonPressed()
	{
		if (TownButton.activeInHierarchy)
		{
			TownButton.SendMessage("OnClick");
		}
		else if (WorldButton.activeInHierarchy)
		{
			WorldButton.SendMessage("OnClick");
		}
	}

	public void HideWinLoseBanner()
	{
		GameObject[] winLoseBanner = WinLoseBanner;
		foreach (GameObject gameObject in winLoseBanner)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
