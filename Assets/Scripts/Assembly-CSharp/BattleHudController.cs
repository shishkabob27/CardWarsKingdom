using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHudController : Singleton<BattleHudController>
{
	public delegate void CardSelectCallback(CardData selectedCard);

	public float MaxLootGridWidth;

	public GameObject LootGridPrefab;

	public UITweenController ShowTrayTween;

	public UITweenController ShowCardSelectorTween;

	public UITweenController HideCardSelectorTween;

	public UITweenController P1SelectLaneStart;

	public UITweenController P1SelectLaneEnd;

	public UITweenController EndTurnTween;

	public UITweenController EndGameTween;

	public UITweenController StartActionTween;

	public UITweenController EndActionTween;

	public UITweenController ShowEndTurnButtonTween;

	public UITweenController P1TurnBannerTween;

	public UITweenController StartAttackBannerTween;

	public UITweenController P2TurnBannerTween;

	public UITweenController StartDefenseBannerTween;

	public UITweenController NotEnoughActionPointBannerTween;

	public UITweenController ShowTapTheEggToCollectBannerTween;

	public UITweenController HideTapTheEggToCollectBannerTween;

	public UITweenController ShowNextWaveBannerTween;

	public UITweenController AttackSequenceShowTween;

	public UITweenController LootOpenStartTween;

	public UITweenController ShowOpponentTween;

	public UITweenController HideOpponentTween;

	public UITweenController HideHudForInfoPopupTween;

	public UITweenController ShowHudAfterInfoPopupTween;

	public UITweenController EnergyCostBarTween;

	public UITweenController EnergyCostBarInsufficientTween;

	public UITweenController PlayerWonCoinFlipTween;

	public UITweenController PlayerLostCoinFlipTween;

	public UITweenController[] EnergySpendTweens = new UITweenController[2];

	public UITweenController ScreenFlashTween;

	public UITweenController EndTurnAvailableTween;

	public UITweenController HideBattleButtonTween;

	public UITweenController ShowBattleButtonTween;

	public UITweenController ShowDeployTween;

	public UITweenController HideDeployTween;

	public UITweenController EnemyThinkingTween;

	public UITweenController BattleFinishedTween;

	public UITweenController ShowLeaderPopupTween;

	public UITweenController HideLeaderPopupTween;

	public UITweenController HeroCardDrawTween;

	public UITweenController ShowPvpTimeTween;

	public UITweenController HidePvpTimeTween;

	public UITweenController TimeUpTween;

	public UITweenController MyChatMessageTween;

	public UITweenController OpponentChatMessageTween;

	public UITweenController DragToDrawBannerTween;

	public UITweenController ShowAIDialogTween;

	public UITweenController HideAIDialogTween;

	public UILabel MultiplayerTimerLabel;

	public GameObject NextWaveBannerObj;

	public HeroPortraitScript[] HeroPortraits;

	public GameObject DeployCardPrefab;

	public GameObject DeployCardsParent;

	public List<DeployDragDropItem> DeployCards;

	public GameObject OpponentZoomCardSpawnParent;

	public UIStreamingGrid CardSelectorGrid;

	public UILabel[] MagicPoints;

	public UILabel MagicSpendCount;

	public GameObject TopBarParent;

	public GameObject EnemyThinkingObject;

	public GameObject EndTurnFinger;

	public UIWidget UIEffectTargetDeck;

	public UIWidget UIEffectTargetDiscardPile;

	public UIWidget[] UIEffectTargets;

	public Transform TargetReticle;

	public UITweenController ShowTargetReticle;

	public UITweenController HideTargetReticle;

	public GameObject HPBarPrefab;

	public GameObject BuffBarPrefab;

	public UIPanel HPBarUIPanel;

	public GameObject ShowDamagePrefab;

	public UILabel HardCurrencyCount;

	public UILabel CardErrorLabel;

	public GameObject EndTurnGlow;

	public UIGrid LootCollectGrid;

	public UITexture LeaderPopupPortrait;

	public Transform LeaderPopupCardParent;

	private GameObject[] LeaderPopupCardNodes = new GameObject[5];

	public UILabel LeaderPopupName;

	public UILabel LeaderPopupAge;

	public UILabel LeaderPopupHeight;

	public UILabel LeaderPopupWeight;

	public UILabel LeaderPopupSpecies;

	public UILabel LeaderPopupQuote;

	public float LootFlyTime = 2f;

	public iTween.EaseType LootFlyEaseType = iTween.EaseType.linear;

	public float LootFlyAmplitudeX = 0.1f;

	public float ActionPointTickUpSpeed;

	public float ActionPointTickDownSpeed;

	public UIWidget[] PIPIconTargets;

	public GameObject[] CardReleaseFadeFXs;

	public GameObject CardDrawFX;

	private CardSelectCallback mCardSelectorCallback;

	private UIStreamingGridDataSource<CardData> mCardSelectorGridDataSource = new UIStreamingGridDataSource<CardData>();

	private bool mIsFirstTurn;

	private float[] mDisplayedMana = new float[2];

	private int[] mTargetMana = new int[2];

	private int[] mShowManaCost = new int[2];

	private bool mTurnIntroFinished = true;

	public UITweenController LootCreatureTween;

	public UITweenController LootSoftCurrencyTween;

	public GameObject PowerUpVFXFly;

	public GameObject PowerUpVFXAdd;

	public float PowerUpVFXFlyTime = 1f;

	public Vector3 CardDroppedPos;

	private bool mShowingPvpTimer;

	public UILabel MyChatLabel;

	public UILabel OpponentChatLabel;

	public UILabel AIDialogLabel;

	public Transform ThoughtBubbleParent;

	public List<UILabel> SubtitleLabels = new List<UILabel>();

	private List<CardPrefabScript> mSpawnedLeaderCards = new List<CardPrefabScript>();

	public List<CreatureHPBar> HPBars = new List<CreatureHPBar>();

	public List<CreatureBuffBar> BuffBars = new List<CreatureBuffBar>();

	private bool mIsP1BannerTweenPlaying;

	private bool mIsP2BannerTweenPlaying;

	private int mShowingLeaderPopup = -1;

	public GameObject TargetedCreatureObject { get; set; }

	public CreatureState TargetedCreature { get; set; }

	public bool ShowingWaveBanner { get; set; }

	public bool IsTurnIntroFinished()
	{
		return mTurnIntroFinished;
	}

	private void Awake()
	{
		MagicSpendCount.text = string.Empty;
		for (int i = 0; i < LeaderPopupCardNodes.Length; i++)
		{
			LeaderPopupCardNodes[i] = LeaderPopupCardParent.Find("ActionCardSpawn_0" + (i + 1)).gameObject;
		}
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			MultiplayerTimerLabel.gameObject.SetActive(false);
		}
		if (!(KFFLocalization.ReturnLang().ToString() == "EN"))
		{
			return;
		}
		foreach (UILabel subtitleLabel in SubtitleLabels)
		{
			subtitleLabel.text = string.Empty;
		}
		foreach (UILabel subtitleLabel2 in Singleton<BattleIntroController>.Instance.SubtitleLabels)
		{
			subtitleLabel2.text = string.Empty;
		}
	}

	private void Start()
	{
		if (!Singleton<TutorialController>.Instance.IsBlockComplete("IntroBattle"))
		{
			TopBarParent.SetActive(false);
		}
		else
		{
			TopBarParent.SetActive(true);
		}
	}

	private void Update()
	{
		if (TargetedCreatureObject != null)
		{
			Vector3 position = Camera.main.WorldToScreenPoint(TargetedCreatureObject.transform.position);
			TargetReticle.position = Singleton<DWGameCamera>.Instance.BattleUICam.ScreenToWorldPoint(position);
		}
		for (int i = 0; i < 2; i++)
		{
			if (i != 1 || !Singleton<DWBattleLane>.Instance.ShowingAiAttackArrow)
			{
				mDisplayedMana[i] = mDisplayedMana[i].TickTowards(mTargetMana[i], ActionPointTickUpSpeed, ActionPointTickDownSpeed);
				UpdateManaBar(i);
			}
		}
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			return;
		}
		int multiplayerSecondsLeft = Singleton<DWGame>.Instance.MultiplayerSecondsLeft;
		if (multiplayerSecondsLeft <= MiscParams.MultiplayerTimeLimitShowAt && multiplayerSecondsLeft >= 0)
		{
			if (!mShowingPvpTimer)
			{
				mShowingPvpTimer = true;
				ShowPvpTimeTween.Play();
			}
			MultiplayerTimerLabel.text = (multiplayerSecondsLeft + 1).ToString();
		}
	}

	public void ClearPvpTimer(bool timeUp)
	{
		if (!timeUp && mShowingPvpTimer)
		{
			HidePvpTimeTween.Play();
		}
		mShowingPvpTimer = false;
	}

	private void UpdateManaBar(int player)
	{
		if (Singleton<DWGame>.Instance.CurrentBoardState == null)
		{
			return;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(new PlayerType(player));
		if (playerState != null)
		{
			int num = playerState.EffectiveCurrentActionPoints;
			if (num > 0 && ((int)mDisplayedMana[player] > 0 || !Singleton<DWGame>.Instance.InDeploymentPhase()))
			{
				MagicPoints[player].text = ((int)mDisplayedMana[player]).ToString();
			}
			else
			{
				MagicPoints[player].text = string.Empty;
			}
		}
	}

	public void SpawnHPBar(DWBattleLaneObject laneObject)
	{
		Transform parent = base.transform;
		if (HPBarUIPanel != null)
		{
			parent = HPBarUIPanel.transform;
		}
		GameObject gameObject = parent.InstantiateAsChild(BuffBarPrefab);
		CreatureBuffBar component = gameObject.GetComponent<CreatureBuffBar>();
		component.Init(laneObject.Creature);
		GameObject gameObject2 = laneObject.transform.InstantiateAsChild(HPBarPrefab);
		CreatureHPBar component2 = gameObject2.GetComponent<CreatureHPBar>();
		component2.Init(laneObject.Creature);
		if (laneObject.Creature.Data.QuestLoadoutEntryData != null)
		{
			float num = laneObject.Creature.Data.QuestLoadoutEntryData.CreatureScale - 1f;
			component2.transform.localScale *= 1f + num / 2f;
		}
		laneObject.HealthBar = component2;
	}

	public CreatureHPBar GetHPBar(CreatureState creature)
	{
		DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(creature);
		if (laneObject == null)
		{
			return null;
		}
		return laneObject.HealthBar;
	}

	public CreatureBuffBar GetBuffBar(CreatureState creature)
	{
		CreatureBuffBar[] componentsInChildren = GetComponentsInChildren<CreatureBuffBar>();
		CreatureBuffBar[] array = componentsInChildren;
		foreach (CreatureBuffBar creatureBuffBar in array)
		{
			if (creatureBuffBar.mCreature == creature)
			{
				return creatureBuffBar;
			}
		}
		return null;
	}

	public GameObject GetStatusEffectIcon(string effectId)
	{
		CreatureBuffBar[] componentsInChildren = GetComponentsInChildren<CreatureBuffBar>();
		CreatureBuffBar[] array = componentsInChildren;
		foreach (CreatureBuffBar creatureBuffBar in array)
		{
			GameObject statusEffectObject = creatureBuffBar.GetStatusEffectObject(effectId);
			if (statusEffectObject != null)
			{
				return statusEffectObject;
			}
		}
		return null;
	}

	public void OnStartP1Turn()
	{
		mTurnIntroFinished = false;
		mIsFirstTurn = Singleton<DWGame>.Instance.CurrentBoardState.IsFirstTurn();
		UpdateActionPointsAtTurnStart(PlayerType.User);
		UpdateTopBar();
		mTurnIntroFinished = true;
		if (!Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			if (Singleton<DWGame>.Instance.InDeploymentPhase())
			{
				if (!Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") && !Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
				{
					ShowDeployTween.Play();
				}
			}
			else
			{
				ShowEndTurnButtonTween.Play();
			}
			if (Singleton<DWGame>.Instance.InDeploymentPhase() || (mIsFirstTurn && !Singleton<TutorialController>.Instance.IsBlockComplete("IntroBattle")))
			{
				P1TurnBannerShown();
				return;
			}
			mIsP1BannerTweenPlaying = true;
			P1TurnBannerTween.PlayWithCallback(P1TurnBannerShown);
		}
		else
		{
			P1TurnBannerShown();
		}
	}

	public void PlayFloopBanner()
	{
		StartCoroutine(PlayFloopBannerAfterDelay());
	}

	public IEnumerator PlayFloopBannerAfterDelay()
	{
		yield return new WaitForSeconds(1f);
		HeroCardDrawTween.Play();
	}

	public void PlayNextWaveBanner(int n)
	{
		ShowingWaveBanner = true;
		UILabel[] componentsInChildren = NextWaveBannerObj.GetComponentsInChildren<UILabel>(true);
		string text = string.Format(KFFLocalization.Get("!!WAVE"), n + 1);
		UILabel[] array = componentsInChildren;
		foreach (UILabel uILabel in array)
		{
			uILabel.text = text;
		}
		ShowNextWaveBannerTween.PlayWithCallback(WaveBannerFinished);
	}

	private void WaveBannerFinished()
	{
		ShowingWaveBanner = false;
	}

	public void ShowTapToCollectBanner()
	{
		ShowTapTheEggToCollectBannerTween.Play();
	}

	public void HideTapToCollectBanner()
	{
		HideTapTheEggToCollectBannerTween.Play();
	}

	private void P1TurnBannerShown()
	{
		if (mIsFirstTurn)
		{
			Singleton<TutorialController>.Instance.AdvanceIfOnState("Q3_Start");
			Singleton<TutorialController>.Instance.AdvanceIfOnState("Q2_Start");
			Singleton<TutorialController>.Instance.AdvanceIfOnState("Q1_Start");
		}
		UICamera.UnlockInput();
		mIsP1BannerTweenPlaying = false;
	}

	public void OnStartAttackSequence()
	{
		AttackSequenceShowTween.Play();
	}

	public void ShowErrorReason(string message)
	{
		NotEnoughActionPointBannerTween.Play();
		CardErrorLabel.text = message;
	}

	public void OnStartP2Turn()
	{
		UpdateActionPointsAtTurnStart(PlayerType.Opponent);
		if (!Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			mIsP2BannerTweenPlaying = true;
			P2TurnBannerTween.PlayWithCallback(P2TurnBannerShown);
		}
	}

	public bool IsBannerTweenPlaying()
	{
		return mIsP1BannerTweenPlaying || mIsP2BannerTweenPlaying;
	}

	private void P2TurnBannerShown()
	{
		mIsP2BannerTweenPlaying = false;
	}

	public void OnOpponentActionTaken()
	{
		Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
		EnemyThinkingObject.SetActive(false);
	}

	public void OnStartAction()
	{
	}

	public void OnP1ActionEndAction()
	{
	}

	public void OnP2ActionEndAction()
	{
	}

	public void OnP1SelectLaneStart(CardData card)
	{
		P1SelectLaneStart.Play();
		Singleton<HandCardController>.Instance.HideHand();
	}

	public void OnP1SelectLaneEnd()
	{
		P1SelectLaneEnd.Play();
	}

	public void OnPlayerVictory()
	{
	}

	private void UpdateActionPointsAtTurnStart(PlayerType player)
	{
	}

	public void AdjustActionPoints(PlayerType player, int amount)
	{
		mTargetMana[(int)player] += amount;
		if (amount < 0)
		{
			EnergySpendTweens[(int)player].Play();
		}
	}

	public void BattleButtonPressed()
	{
		if (!Singleton<DWBattleLane>.Instance.LootObjectsToCollect() && !Singleton<DWGame>.Instance.IsGameOver() && Singleton<DWGame>.Instance.GetCurrentGameState() != GameState.EndGameWait && (!Singleton<TutorialController>.Instance.IsFTUETutorialActive() || !DetachedSingleton<ConditionalTutorialController>.Instance.CheckDidNothing()))
		{
			PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
			if (playerState.GetCreatureCount() == 0 && playerState.CanDeploy())
			{
				Singleton<BattleHudController>.Instance.ShowErrorReason(KFFLocalization.Get("!!MUST_PLAY_CREATURE"));
				return;
			}
			Singleton<DWGame>.Instance.EndPlayerTurn();
			ClearPvpTimer(false);
		}
	}

	public void ShowCreatureDamage(CreatureState creature, int rawAmount, int cappedAmount, bool isCrit, DamageType type)
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		if (!(creatureObject == null))
		{
			GameObject gameObject = SLOTGame.InstantiateFX(ShowDamagePrefab) as GameObject;
			ShowDamagePopup component = gameObject.GetComponent<ShowDamagePopup>();
			Vector3 position = creatureObject.transform.position;
			position.y += 2f;
			component.Init(rawAmount, position, type, isCrit);
			GetHPBar(creature).ShowDamage(cappedAmount);
		}
	}

	public void ShowCreatureHealing(CreatureState creature, int rawAmount, int cappedAmount)
	{
		GameObject creatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		GameObject gameObject = base.transform.InstantiateAsChild(ShowDamagePrefab);
		ShowDamagePopup component = gameObject.GetComponent<ShowDamagePopup>();
		Vector3 position = creatureObject.transform.position;
		position.y += 2f;
		component.Init(rawAmount, position, DamageType.Healing);
		GetHPBar(creature).ShowHealing(cappedAmount);
	}

	public static Vector3 GetWorldPos(Vector2 screenPos, float worldHeight = 0f)
	{
		Ray ray = Singleton<DWGameCamera>.Instance.MainCam.ScreenPointToRay(screenPos);
		float distance = (0f - (ray.origin.y - worldHeight)) / ray.direction.y;
		return ray.GetPoint(distance);
	}

	public void PopulateHeros(PlayerState user, PlayerState opponent)
	{
		HeroPortraits[0].Init(user);
		HeroPortraits[1].Init(opponent);
	}

	public void ShowCardSelector(List<CardData> cardList, CardSelectCallback callback)
	{
	}

	public void OnCardSelected(CardData cardData)
	{
		if (mCardSelectorCallback != null)
		{
			mCardSelectorCallback(cardData);
			mCardSelectorCallback = null;
		}
		HideCardSelectorTween.Play();
	}

	public void CleanUpCardSelector()
	{
		mCardSelectorGridDataSource.Clear();
	}

	public static DWBattleLaneObject GetHoveredLane(CardData card)
	{
		if (card.TargetType1 == SelectionType.None)
		{
			return null;
		}
		List<LaneState> firstTargetList = Singleton<DWGame>.Instance.GetFirstTargetList(PlayerType.User, card);
		return GetHoveredLane(firstTargetList);
	}

	public static DWBattleLaneObject GetHoveredLane()
	{
		List<LaneState> occupiedLanes = Singleton<DWGame>.Instance.GetOccupiedLanes();
		return GetHoveredLane(occupiedLanes);
	}

	private static DWBattleLaneObject GetHoveredLane(List<LaneState> targets)
	{
		Vector3 mousePosition = Input.mousePosition;
		Vector3 worldPos = GetWorldPos(mousePosition, 0f);
		DWBattleLaneObject dWBattleLaneObject = null;
		float num = 0f;
		foreach (LaneState target in targets)
		{
			DWBattleLaneObject laneObject = Singleton<DWBattleLane>.Instance.GetLaneObject(target.Creature);
			if (UICamera.ColliderRestrictionList.Count <= 0 || UICamera.ColliderRestrictionList.Contains(laneObject.gameObject))
			{
				float magnitude = (laneObject.CreatureObject.transform.position - worldPos).magnitude;
				if (dWBattleLaneObject == null || magnitude < num)
				{
					dWBattleLaneObject = laneObject;
					num = magnitude;
				}
			}
		}
		return dWBattleLaneObject;
	}

	public void UpdateTopBar()
	{
	}

	public void UpdateLootCount()
	{
	}

	public void SetTargetReticle(CreatureState creature)
	{
	}

	public void ClearTargetReticle()
	{
	}

	public void OnCreatureDied(CreatureState creature)
	{
	}

	public void DisplayPredictedValueOnHover(CardData card)
	{
		Singleton<DWBattleLane>.Instance.PulsePlayerAttackValues(card.AttackBase);
	}

	public void StopDisplayPredictedValue()
	{
		CreatureBuffBar[] componentsInChildren = GetComponentsInChildren<CreatureBuffBar>();
		foreach (CreatureBuffBar creatureBuffBar in componentsInChildren)
		{
			creatureBuffBar.StopBlinkPredictedBuff();
		}
		Singleton<DWBattleLane>.Instance.PulsePlayerAttackValues(AttackBase.None);
	}

	public void OnGameEnd()
	{
		EndGameTween.Play();
		Singleton<HandCardController>.Instance.HideOpponentTween.Play();
		if (Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			HideDeployTween.Play();
		}
	}

	public void ShowDragActionCost(int amount)
	{
		ShowCost(amount);
	}

	public void HideDragActionCost()
	{
		HideCost();
	}

	public void ShowCost(int cost)
	{
		mShowManaCost[0] = cost;
		MagicSpendCount.text = "-" + cost;
		UpdateManaBar(0);
		if (cost > mTargetMana[0])
		{
			EnergyCostBarTween.StopAndReset();
			EnergyCostBarInsufficientTween.Play();
		}
		else
		{
			EnergyCostBarInsufficientTween.StopAndReset();
			EnergyCostBarTween.Play();
		}
	}

	public void HideCost()
	{
		mShowManaCost[0] = 0;
		MagicSpendCount.text = string.Empty;
		UpdateManaBar(0);
	}

	public void PlayWhiteScreenFlashTween()
	{
		Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_CreatureDeath");
	}

	public Transform GetNextLootContainer()
	{
		Transform transform = LootCollectGrid.gameObject.InstantiateAsChild(LootGridPrefab).transform;
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
		float num = MaxLootGridWidth / (float)LootCollectGrid.transform.childCount;
		if (LootCollectGrid.cellWidth > num)
		{
			LootCollectGrid.cellWidth = num;
		}
		LootCollectGrid.Reposition();
		List<Transform> childList = LootCollectGrid.GetChildList();
		for (int i = 0; i < childList.Count; i++)
		{
			Transform transform2 = childList[i].Find("LootSpawnNode");
			if (transform2 != null)
			{
				transform2.transform.localPosition = new Vector3(0f, 0f, -50 * i);
				transform2.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
			}
		}
		return transform.Find("LootSpawnNode");
	}

	public void OnClickUserLeader()
	{
		OnClickLeaderPopup(PlayerType.User);
	}

	public void OnClickOpponentLeader()
	{
		OnClickLeaderPopup(PlayerType.Opponent);
	}

	private void OnClickLeaderPopup(PlayerType player)
	{
		if (!Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") && !Singleton<TutorialController>.Instance.IsFTUETutorialActive() && !Singleton<DWGame>.Instance.SelectingLane && !Singleton<DWBattleLane>.Instance.LootObjectsToCollect() && Singleton<DWGameMessageHandler>.Instance.IsEffectDone() && !Singleton<DWGame>.Instance.IsGameOver())
		{
			mShowingLeaderPopup = player.IntValue;
			ShowLeaderPopupTween.Play();
			HideHudForInfoPopupTween.Play();
			Singleton<HandCardController>.Instance.HideHand();
			Singleton<HandCardController>.Instance.HideOpponentTween.Play();
			LeaderItem leader = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(player).Leader;
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(leader.SelectedSkin.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", LeaderPopupPortrait);
			LeaderPopupName.text = leader.Form.Name;
			LeaderPopupAge.text = leader.Form.FlvAge;
			LeaderPopupHeight.text = leader.Form.FlvHeight;
			LeaderPopupWeight.text = leader.Form.FlvWeight;
			LeaderPopupSpecies.text = leader.Form.FlvSpecies;
			LeaderPopupQuote.text = leader.Form.FlvQuote;
			for (int i = 0; i < leader.Form.ActionCards.Count; i++)
			{
				CardPrefabScript component = LeaderPopupCardNodes[i].InstantiateAsChild(Singleton<PrefabReferences>.Instance.Card).GetComponent<CardPrefabScript>();
				component.Mode = CardPrefabScript.CardMode.GeneralFrontEnd;
				component.Populate(leader.Form.ActionCards[i]);
				component.gameObject.ChangeLayer(base.gameObject.layer);
				component.AdjustDepth(1);
				mSpawnedLeaderCards.Add(component);
			}
		}
	}

	public void CloseLeaderPopupIfShowing(bool showHands = false)
	{
		if (mShowingLeaderPopup != -1)
		{
			HideLeaderPopupTween.Play();
			if (showHands)
			{
				OnClickCloseLeaderPopup();
			}
			else
			{
				mShowingLeaderPopup = -1;
			}
		}
	}

	public void OnClickCloseLeaderPopup()
	{
		mShowingLeaderPopup = -1;
		Singleton<HandCardController>.Instance.ShowHand();
		Singleton<HandCardController>.Instance.ShowOpponentTween.Play();
	}

	public void OnLeaderPopupClosed()
	{
		foreach (CardPrefabScript mSpawnedLeaderCard in mSpawnedLeaderCards)
		{
			NGUITools.Destroy(mSpawnedLeaderCard.gameObject);
		}
		mSpawnedLeaderCards.Clear();
	}

	public void ShowChatMessage(bool mine, QuickChatData data)
	{
		if (mine)
		{
			MyChatMessageTween.Play();
			MyChatLabel.text = data.Text;
		}
		else
		{
			OpponentChatMessageTween.Play();
			OpponentChatLabel.text = data.Text;
		}
	}

	public IEnumerator ShowAIDialog(string text, float time)
	{
		AIDialogLabel.text = KFFLocalization.Get(text);
		ShowAIDialogTween.Play();
		yield return new WaitForSeconds(time);
		yield return StartCoroutine(HideAIDialogTween.PlayAsCoroutine());
	}
}
