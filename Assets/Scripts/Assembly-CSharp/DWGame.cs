using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class DWGame : Singleton<DWGame>
{
	public enum TurnActions
	{
		PlayCard,
		PlayMonster,
		Attack
	}

	private GameState mGameState;

	private bool mRevivingPlayerThisTurn;

	public List<InventorySlotItem> TempUserSet = new List<InventorySlotItem>();

	public List<InventorySlotItem> TempAISet = new List<InventorySlotItem>();

	public Loadout UserLoadout;

	public Loadout OpLoadout;

	private static int mAutoRestartCount;

	private BoardState MasterBoardState;

	public bool IsTutorialSetup;

	private IAbilitySource PendingEffectSource;

	private SelectionType PendingSelectionType;

	private GameObject mLoadingCreatureModel;

	private bool mUserWonCoinFlip;

	private bool mStateSwitchInputLock;

	private ObscuredFloat mMultiplayerTimeLeft = -1f;

	private ObscuredFloat mSaveMultiplayerTimeLeft = -1f;

	public int turnNumber;

	public float battleDuration;

	public bool battleStarted;

	public bool turnStarted;

	public float turnDuration;

	public List<TurnActions> turnActions = new List<TurnActions>();

	private bool IsAwakeDone;

	private bool IntroStarted;

	private bool DeployEnemyDone;

	private bool P1ActionStarted;

	private bool P1SelectLaneStarted;

	private bool P1AttackStarted;

	private bool DeployCreatureStart;

	public bool LostDuringMyTurn { get; set; }

	public bool SelectingLane { get; set; }

	public bool WaitingForCreatureDeployAfterRevive { get; private set; }

	public bool IsSetUp { get; set; }

	public BoardState CurrentBoardState
	{
		get
		{
			return MasterBoardState;
		}
	}

	public int MultiplayerSecondsLeft
	{
		get
		{
			return (int)(float)mMultiplayerTimeLeft;
		}
	}

	public bool InDeploymentPhase()
	{
		return MasterBoardState.IsDeployment;
	}

	public bool IsFirstTurnAfterRevive()
	{
		return mRevivingPlayerThisTurn;
	}

	public void StopMultiplayerTimer()
	{
		mSaveMultiplayerTimeLeft = mMultiplayerTimeLeft;
		mMultiplayerTimeLeft = -1f;
	}

	public void ResumeMultiplayerTimer()
	{
		mMultiplayerTimeLeft = mSaveMultiplayerTimeLeft;
	}

	public void Debug5SecondMultiplayerTimer()
	{
		mMultiplayerTimeLeft = 5f;
	}

	private void Awake()
	{
		StartCoroutine(DWGameAwake());
	}

	private IEnumerator DWGameAwake()
	{
		while (!SessionManager.Instance.IsLoadDataDone())
		{
			yield return null;
		}
		while (!Singleton<PlayerInfoScript>.Instance.IsInitialized)
		{
			yield return null;
		}
		yield return StartCoroutine(Singleton<AIManager>.Instance.CreatePool());
		MasterBoardState = BoardState.Create();
		IsAwakeDone = true;
	}

	private void OnDestroy()
	{
		if (MasterBoardState != null)
		{
			BoardState.Destroy(MasterBoardState);
		}
	}

	public void InitPlayer(PlayerType idx, Loadout loadout, bool reviving = false)
	{
		loadout.Leader.Form.ParseKeywords();
		foreach (InventorySlotItem item in loadout.CreatureSet)
		{
			if (item != null)
			{
				item.Creature.Form.ParseKeywords();
			}
		}
		MasterBoardState.InitPlayer(idx, loadout, reviving);
	}

	private void OnApplicationQuit()
	{
        if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
        {
			Instance.turnNumber = 0;
			Instance.battleDuration = 0f;
        }
    }

	public void Setup()
	{
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		if (instance.StateData.MultiplayerMode)
		{
			instance.StateData.CurrentActiveQuest = QuestDataManager.Instance.GetData("PVP");
		}
		else if (instance.StateData.CurrentActiveQuest == null)
		{
			instance.StateData.CurrentActiveQuest = QuestDataManager.Instance.GetData(0);
		}
		QuestData currentActiveQuest = instance.StateData.CurrentActiveQuest;
		if (instance.StateData.CurrentLoadout != null)
		{
			UserLoadout = instance.StateData.CurrentLoadout.Copy();
		}
		if (UserLoadout == null)
		{
			UserLoadout = instance.GetCurrentLoadout().Copy();
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.SelectedHelper != null)
		{
			InventorySlotItem helperCreature = Singleton<PlayerInfoScript>.Instance.StateData.HelperCreature;
			UserLoadout.CreatureSet[MiscParams.CreaturesOnTeam - 1] = helperCreature;
		}
		if (!instance.StateData.MultiplayerMode)
		{
			OpLoadout = BuildOpponentLoadout(currentActiveQuest);
		}
		else
		{
			OpLoadout = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentLoadout;
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && !Singleton<PlayerInfoScript>.Instance.PvPData.AmIPrimary)
		{
			InitPlayer(PlayerType.Opponent, OpLoadout);
			InitPlayer(PlayerType.User, UserLoadout);
		}
		else
		{
			InitPlayer(PlayerType.User, UserLoadout);
			InitPlayer(PlayerType.Opponent, OpLoadout);
		}
		Singleton<BattleHudController>.Instance.PopulateHeros(Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(0), Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(1));
		RewardManager.Init(currentActiveQuest, OpLoadout);
		MasterBoardState.Setup();
		IsSetUp = true;
	}

	public IEnumerator SetupIntroBattleBoard()
	{
		IsTutorialSetup = true;
		DetachedSingleton<MissionManager>.Instance.ResetBattle();
		Singleton<AIManager>.Instance.ResetCreatureIndex();
		TutorialBoardData boardData = Singleton<TutorialController>.Instance.GetTutorialBoard();
		PlayerState user = MasterBoardState.GetPlayerState(PlayerType.User);
		PlayerState opp = MasterBoardState.GetPlayerState(PlayerType.Opponent);
		foreach (TutorialBoardEntry entry in boardData.Entries)
		{
			if (entry.whichPlayer == PlayerType.User)
			{
				CreatureState creature2 = user.DeploymentList.Find((CreatureState m) => m.Data.Form.ID == entry.CreatureID);
				if (creature2 != null)
				{
					if (entry.isOut)
					{
						DeployCreatureFromCard(entry.whichPlayer, creature2.Data, 0);
						int newHP2 = Mathf.RoundToInt(Mathf.Ceil((float)creature2.MaxHP * entry.HPFactor));
						creature2.SetHP(newHP2);
					}
					else
					{
						Singleton<HandCardController>.Instance.ShowCreatureCardDraw(PlayerType.User, creature2.Data);
					}
				}
			}
			else
			{
				if (entry.whichPlayer != PlayerType.Opponent)
				{
					continue;
				}
				CreatureState creature = opp.DeploymentList.Find((CreatureState m) => m.Data.Form.ID == entry.CreatureID);
				if (creature != null)
				{
					if (entry.isOut)
					{
						DeployCreatureFromCard(entry.whichPlayer, creature.Data, 0);
						int newHP = Mathf.RoundToInt(Mathf.Ceil((float)creature.MaxHP * entry.HPFactor));
						creature.SetHP(newHP);
					}
					else
					{
						Singleton<HandCardController>.Instance.ShowCreatureCardDraw(PlayerType.Opponent, creature.Data);
					}
				}
			}
		}
		Singleton<DWGameCamera>.Instance.InitPIPCams();
		Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
		Singleton<BattleHudController>.Instance.HideBattleButtonTween.Play();
		Singleton<HandCardController>.Instance.HideHand();
		Singleton<HandCardController>.Instance.HideOpponentTween.Play();
		SetGameState(GameState.P1StartTurn);
		yield return null;
		MasterBoardState.IsDeployment = false;
		MasterBoardState.ClearFirstTurnFlag();
		Singleton<DWGameCamera>.Instance.MoveCameraToP2Setup();
		while (!Singleton<DWGameMessageHandler>.Instance.IsEffectDone())
		{
			yield return null;
		}
		IsTutorialSetup = false;
		if (LoadingScreenController.GetInstance() != null)
		{
			LoadingScreenController.GetInstance().HideLoading(OnHideLoadingFinished);
		}
	}

	private Loadout BuildOpponentLoadout(QuestData questData)
	{
		Loadout loadout = new Loadout();
		loadout.Leader = new LeaderItem(questData.Opponent);
		for (int i = 0; i < questData.SetLoadout.Entries.Count; i++)
		{
			loadout.CreatureSet.Add(new InventorySlotItem(questData.SetLoadout.Entries[i].BuildCreatureItem()));
		}
		int num = questData.EnemyCount - questData.SetLoadout.Entries.Count;
		for (int j = 0; j < num; j++)
		{
			QuestLoadoutEntry randomEntry = questData.RandomLoadout.GetRandomEntry();
			loadout.CreatureSet.Add(new InventorySlotItem(randomEntry.BuildCreatureItem()));
		}
		return loadout;
	}

	public void StartTurn(PlayerType player)
	{
		DetachedSingleton<CustomAIManager>.Instance.SetSpecialRulesThisTurn(Singleton<DWGame>.Instance.CurrentBoardState, player);
		MasterBoardState.StartTurn(player);
		ProcessMessages();
		while (AttackProgress.AttacksInProgress)
		{
			MasterBoardState.UpdateAttackState(player);
			ProcessMessages();
		}
	}

	public int DetermineCost(PlayerType player, CardData card)
	{
		return card.Cost;
	}

	public PlayerState.CanPlayResult CanPlay(PlayerType player, CardData card, PlayerType targetPlayer = null, int LaneIndex = -1)
	{
		return MasterBoardState.CanPlay(player, card, targetPlayer, LaneIndex);
	}

	public PlayerState.CanPlayResult CanPlay(PlayerType player, CreatureItem creature)
	{
		if (InDeploymentPhase())
		{
			return PlayerState.CanPlayResult.CanPlay;
		}
		if (MasterBoardState.CanDeploy(player, creature))
		{
			return PlayerState.CanPlayResult.CanPlay;
		}
		return PlayerState.CanPlayResult.NotEnoughAP;
	}

	public List<LaneState> GetOccupiedLanes()
	{
		return MasterBoardState.GetOccupiedLanes();
	}

	public List<LaneState> GetFirstTargetList(PlayerType WhichPlayer, CardData Card)
	{
		return MasterBoardState.GetFirstTargetList(WhichPlayer, Card);
	}

	public List<LaneState> GetSecondTargetList(PlayerType WhichPlayer, CardData Card)
	{
		return MasterBoardState.GetSecondTargetList(WhichPlayer, Card);
	}

	public int GetCreatureCount(PlayerType WhichPlayer)
	{
		return MasterBoardState.GetCreatureCount(WhichPlayer);
	}

	public void SetTarget(PlayerType WhichPlayer, CreatureState Target)
	{
		MasterBoardState.SetTarget(WhichPlayer, Target);
	}

	public void SetTargetByLane(PlayerType WhichPlayer, int laneIndex)
	{
		PlayerType idx = ((WhichPlayer != PlayerType.User) ? PlayerType.User : PlayerType.Opponent);
		CreatureState creature = MasterBoardState.GetCreature(idx, laneIndex);
		MasterBoardState.SetTarget(WhichPlayer, creature);
	}

	public void PlayActionCard(PlayerType WhichPlayer, CardData Card, PlayerType targetPlayer, int LaneIndex1 = -1)
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && WhichPlayer == PlayerType.User)
		{
			int targetLane = CurrentBoardState.GetPlayerState(PlayerType.User).GetTargetLane();
			Singleton<MultiplayerMessageHandler>.Instance.SendCardPlay(Card, targetPlayer, LaneIndex1, targetLane);
		}
		MasterBoardState.PlayActionCard(WhichPlayer, Card, targetPlayer, LaneIndex1);
		Card.AlreadySeen = true;
	}

	public void DeployCreatureFromCard(PlayerType player, CreatureItem creature, int laneIndex, bool fromPullCard = false)
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && player == PlayerType.User)
		{
			Singleton<MultiplayerMessageHandler>.Instance.SendCreaturePlay(creature, laneIndex);
		}
		CreatureState creatureState;
		if (fromPullCard)
		{
			creatureState = MasterBoardState.GetPlayerState(player).GetCreature(creature);
		}
		else
		{
			creatureState = MasterBoardState.DeployCreature(player, creature, laneIndex);
			if (player == PlayerType.User)
			{
				WaitingForCreatureDeployAfterRevive = false;
			}
		}
		DWBattleLaneObject dWBattleLaneObject = Singleton<DWBattleLane>.Instance.AddLaneObject(creatureState, laneIndex);
		Vector3 size = dWBattleLaneObject.LaneCollider.size;
		size.x = creature.Form.Width;
		if (size.x < Singleton<DWBattleLane>.Instance.MinimumCreatureWidth)
		{
			size.x = Singleton<DWBattleLane>.Instance.MinimumCreatureWidth;
		}
		dWBattleLaneObject.LaneCollider.size = size;
		GameObject value = null;
		Singleton<DWBattleLane>.Instance.CreaturePool.TryGetValue(creature, out value);
		value.transform.parent = dWBattleLaneObject.transform;
		value.transform.localPosition = Vector3.zero;
		if (player == PlayerType.Opponent && creature.QuestLoadoutEntryData != null)
		{
			float creatureScale = creature.QuestLoadoutEntryData.CreatureScale;
			value.transform.localScale = Vector3.one * creatureScale;
			if (creatureScale > 1f)
			{
				Vector3 localPosition = value.transform.localPosition;
				localPosition.z += Singleton<DWBattleLane>.Instance.MoveScaledUpCreaturesBackFactor * (creatureScale - 1f);
				value.transform.localPosition = localPosition;
			}
		}
		else
		{
			value.transform.localScale = Vector3.one;
		}
		float y = ((player != PlayerType.Opponent) ? 0f : 180f);
		value.transform.localRotation = Quaternion.Euler(new Vector3(0f, y, 0f));
		dWBattleLaneObject.CreatureObject = value;
		Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
		if (!IsTutorialSetup)
		{
			StartCoroutine(StartSummonAnim(value, creatureState));
			StartCoroutine(TriggerSpawnCreatureEffects(creatureState));
		}
		else
		{
			value.SetActive(true);
			dWBattleLaneObject.ShadowBlob.Spawn(value, creatureState.Data);
		}
		Singleton<BattleHudController>.Instance.SpawnHPBar(dWBattleLaneObject);
		creature.Form.AlreadySeen = true;
	}

	public IEnumerator TriggerSpawnCreatureEffects(CreatureState creatureState)
	{
		while (Singleton<PauseController>.Instance.Paused)
		{
			yield return null;
		}
		GameObject creatureObj = Singleton<DWBattleLane>.Instance.GetCreatureObject(creatureState);
		DWBattleLaneObject laneObj = Singleton<DWBattleLane>.Instance.GetLaneObject(creatureState);
		laneObj.RevertMaterials();
		GameObject objData2 = null;
		Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureState.Data.Form.PersistentVFX, out objData2);
		if (objData2 == null)
		{
			objData2 = Resources.Load("VFX/Creatures/" + creatureState.Data.Form.PersistentVFX, typeof(GameObject)) as GameObject;
		}
		if (objData2 != null)
		{
			List<Transform> trs = FindAllInChildren(creatureObj.transform, creatureState.Data.Form.PersistentVFXAttachBone);
			foreach (Transform tr in trs)
			{
				tr.InstantiateAsChild(objData2);
			}
		}
		objData2 = null;
		Singleton<DWBattleLane>.Instance.CreatureVFXPool.TryGetValue(creatureState.Data.Form.RezInVFX, out objData2);
		if (objData2 == null)
		{
			objData2 = Resources.Load("VFX/Creatures/" + creatureState.Data.Form.RezInVFX, typeof(GameObject)) as GameObject;
		}
		if (objData2 != null)
		{
			GameObject fxObj = laneObj.transform.InstantiateAsChild(objData2);
			AudioSource sound = fxObj.GetComponent<AudioSource>();
			if (sound != null && sound.clip != null)
			{
				Singleton<SLOTAudioManager>.Instance.PlaySound("battle/SFX_Rez_In");
			}
		}
		if ((bool)laneObj.ShadowBlob)
		{
			laneObj.ShadowBlob.Spawn(creatureObj, creatureState.Data);
		}
	}

	public IEnumerator StartSummonAnim(GameObject creatureObj, CreatureState creatureState)
	{
		while (Singleton<PauseController>.Instance.Paused)
		{
			yield return null;
		}
		CardProgress.Instance.State = CardState.Intro;
		if (InDeploymentPhase() && GetCurrentGameState().IsP1Turn())
		{
			Singleton<BattleHudController>.Instance.HideDeployTween.Play();
		}
		Singleton<DWGameCamera>.Instance.MoveCameraToCreatureSummon(creatureState);
		Singleton<DWGameCamera>.Instance.Battle3DUICam.enabled = false;
		yield return new WaitForSeconds(0.4f);
		creatureObj.SetActive(true);
		Animator anim = creatureObj.GetComponentInChildren<Animator>();
		if (anim != null)
		{
			anim.speed = 1f;
		}
		if (creatureState.Data.Form.IntroSound != null)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound("creature/" + creatureState.Data.Form.IntroSound);
		}
		yield return new WaitForSeconds(1f);
		CardProgress.Instance.State = CardState.Idle;
		Singleton<DWGameCamera>.Instance.Battle3DUICam.enabled = true;
		Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
	}

	private Transform FindInChildren(Transform tr, string childName)
	{
		Transform[] componentsInChildren = tr.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains(childName))
			{
				return transform;
			}
		}
		return null;
	}

	public List<Transform> FindAllInChildren(Transform tr, string childName)
	{
		List<Transform> list = new List<Transform>();
		if (childName == string.Empty)
		{
			list.Add(tr);
			return list;
		}
		Transform[] componentsInChildren = tr.gameObject.GetComponentsInChildren<Transform>();
		return componentsInChildren.FindAll((Transform m) => m.name.Contains(childName));
	}

	public void AddMessage(GameMessage Message)
	{
		MasterBoardState.AddMessage(Message);
	}

	public void ProcessMessages()
	{
		MasterBoardState.RecordMessages();
		List<GameMessage> list = MasterBoardState.ProcessMessages();
		if (list.Count > 0)
		{
			Singleton<DWGameMessageHandler>.Instance.ProcessMessages(list);
			MasterBoardState.ClearProcessedMessageList();
		}
	}

	public void Update()
	{
		if (!IsAwakeDone)
		{
			return;
		}
		if (battleStarted)
		{
			battleDuration += Time.deltaTime;
		}
		if (turnStarted)
		{
			turnDuration += Time.deltaTime;
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && (float)mMultiplayerTimeLeft >= 0f)
		{
			mMultiplayerTimeLeft = (float)mMultiplayerTimeLeft - Time.unscaledDeltaTime;
			if ((float)mMultiplayerTimeLeft <= 0f)
			{
				mMultiplayerTimeLeft = -1f;
				StartCoroutine(EndTurnOnMultiplayerTimeUp());
			}
		}
		UpdateGameState();
		if (CardProgress.Instance.State != 0 && CardProgress.Instance.State != CardState.Intro)
		{
			MasterBoardState.UpdateActionCard(CardProgress.Instance.UsingPlayer);
		}
		if (AttackProgress.AttacksInProgress)
		{
			MasterBoardState.UpdateAttackState(AttackProgress.Attacks[0].UsingPlayer);
		}
		ProcessMessages();
		if (mGameState == GameState.P1Turn)
		{
			Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(UserLoadout.Leader.Form, VOEvent.Idle);
		}
	}

	private IEnumerator EndTurnOnMultiplayerTimeUp()
	{
		UICamera.LockInput();
		while (!Singleton<DWGameMessageHandler>.Instance.IsEffectDone())
		{
			yield return null;
		}
		if (SelectingLane)
		{
			Singleton<DWBattleLane>.Instance.CancelTargetSelection();
		}
		Singleton<CreatureInfoPopup>.Instance.HideIfShowing();
		Singleton<HandCardController>.Instance.CancelCardDrag();
		Singleton<DWBattleLane>.Instance.CancelDragAttack();
		Singleton<HandCardController>.Instance.UnzoomCard();
		Singleton<BattleHudController>.Instance.CloseLeaderPopupIfShowing(true);
		Singleton<DWBattleLane>.Instance.EndFreeCam();
		yield return StartCoroutine(Singleton<BattleHudController>.Instance.TimeUpTween.PlayAsCoroutine());
		PlayerState player = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		if (player.GetCreatureCount() == 0 && player.CanDeploy())
		{
			List<CardPrefabScript> creatureCards = Singleton<HandCardController>.Instance.GetCreatureCards();
			CardPrefabScript randomCreatureCard = creatureCards[Random.Range(0, creatureCards.Count)];
			DeployCreatureFromCard(PlayerType.User, randomCreatureCard.Creature, 0);
			yield return StartCoroutine(randomCreatureCard.ShowPlayAnim());
			while (!Singleton<DWGameMessageHandler>.Instance.IsEffectDone())
			{
				yield return null;
			}
		}
		if (!Singleton<DWGame>.Instance.InDeploymentPhase())
		{
			Singleton<DWGame>.Instance.EndPlayerTurn();
		}
		Singleton<BattleHudController>.Instance.ClearPvpTimer(true);
		UICamera.UnlockInput();
	}

	private void UpdateGameState()
	{
		if (mStateSwitchInputLock)
		{
			UICamera.UnlockInput();
			mStateSwitchInputLock = false;
		}
		switch (mGameState)
		{
		case GameState.LoadingData:
			if (SessionManager.Instance.IsLoadDataDone() && Singleton<PlayerInfoScript>.Instance.IsInitialized)
			{
				SetGameState(GameState.LoadingLevel);
			}
			break;
		case GameState.LoadingLevel:
			StartCoroutine(LoadLevel());
			SetGameState(GameState.WaitForLevelLoad);
			break;
		case GameState.WaitForLevelLoad:
			break;
		case GameState.Intro:
			if (LoadingScreenController.GetInstance() != null)
			{
				LoadingScreenController.GetInstance().HideLoading(OnHideLoadingFinished);
			}
			DetachedSingleton<MissionManager>.Instance.ResetBattle();
			Singleton<AIManager>.Instance.ResetCreatureIndex();
			Singleton<BattleIntroController>.Instance.LaunchBattleIntro();
			SetGameState(GameState.WaitForIntro);
			Singleton<CharacterAnimController>.Instance.StopCharacterFidgets();
			if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				Singleton<PlayerInfoScript>.Instance.Save();
			}
			break;
		case GameState.WaitForIntro:
			break;
		case GameState.Waiting:
			break;
		case GameState.WaitingResultSequence:
			if (!Singleton<PvpBattleResultsController>.Instance.Showing)
			{
				SetGameState(GameState.Waiting);
			}
			break;
		case GameState.FirstTurnCoinFlip:
		{
			SetGameState(GameState.Waiting);
			UICamera.LockInput();
			Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
			Singleton<BattleHudController>.Instance.ShowTrayTween.Play();
			Singleton<PauseController>.Instance.ShowButton();
			Singleton<QuickMessageController>.Instance.ShowButton();
			bool multiplayerMode = Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode;
			QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
			if (Singleton<TutorialController>.Instance.IsFTUETutorialActive())
			{
				mUserWonCoinFlip = true;
			}
			else if (!multiplayerMode && currentActiveQuest.ForceFirstPlayer == 0)
			{
				mUserWonCoinFlip = true;
			}
			else if (!multiplayerMode && currentActiveQuest.ForceFirstPlayer == 1)
			{
				mUserWonCoinFlip = false;
			}
			else if (multiplayerMode)
			{
				mUserWonCoinFlip = Singleton<PlayerInfoScript>.Instance.PvPData.WonInitialCoinFlip;
			}
			else
			{
				mUserWonCoinFlip = Random.Range(0, 2) == 0;
			}
			Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
			if (mUserWonCoinFlip)
			{
				Singleton<BattleHudController>.Instance.PlayerWonCoinFlipTween.PlayWithCallback(OnCoinFlipTweenFinished);
			}
			else
			{
				Singleton<BattleHudController>.Instance.PlayerLostCoinFlipTween.PlayWithCallback(OnCoinFlipTweenFinished);
			}
			Singleton<DWBattleLane>.Instance.ShowEnvVFXObj(false);
			break;
		}
		case GameState.DealCreatureCards:
		{
			UICamera.LockInput();
			Singleton<DWGameCamera>.Instance.RenderP1Character(false);
			List<InventorySlotItem> list = UserLoadout.CreatureSet.FindAll((InventorySlotItem m) => m != null);
			foreach (InventorySlotItem item in list)
			{
				Singleton<HandCardController>.Instance.ShowCreatureCardDraw(PlayerType.User, item.Creature);
			}
			if (!mRevivingPlayerThisTurn)
			{
				List<InventorySlotItem> list2 = OpLoadout.CreatureSet.FindAll((InventorySlotItem m) => m != null);
				foreach (InventorySlotItem item2 in list2)
				{
					Singleton<HandCardController>.Instance.ShowCreatureCardDraw(PlayerType.Opponent, item2.Creature);
				}
			}
			SetGameState(GameState.DealCreatureCardsWait);
			break;
		}
		case GameState.DealCreatureCardsWait:
			if (Singleton<HandCardController>.Instance.CardEventsInProgress())
			{
				break;
			}
			UICamera.UnlockInput();
			if (mRevivingPlayerThisTurn)
			{
				if (LostDuringMyTurn)
				{
					MasterBoardState.GetPlayerState(PlayerType.User).RefillActionPoints();
					SetGameState(GameState.P1StartTurn);
					LostDuringMyTurn = false;
				}
				else
				{
					SetGameState(GameState.P2EndTurn);
				}
			}
			else
			{
				battleStarted = true;
				if (mUserWonCoinFlip)
				{
					SetGameState(GameState.P1StartTurn);
				}
				else
				{
					SetGameState(GameState.P2StartTurn);
				}
			}
			break;
		case GameState.P1StartTurn:
			UICamera.LockInput();
			Singleton<PauseController>.Instance.BlockInput(false);
			if (MasterBoardState.IsFirstTurn() && Singleton<TutorialController>.Instance.IsFTUETutorialActive())
			{
				Singleton<TutorialController>.Instance.AdvanceTutorialState();
			}
			Singleton<DWBattleLane>.Instance.SetLaneColliders(true);
			if (InDeploymentPhase())
			{
				Singleton<BattleHudController>.Instance.OnStartP1Turn();
			}
			Singleton<CharacterAnimController>.Instance.TriggerHeroThinking2(PlayerType.User);
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				mMultiplayerTimeLeft = (int)CurrentBoardState.GetPlayerState(PlayerType.User).CurrentPvpTimeLimit;
			}
			StartTurn(PlayerType.User);
			SetGameState(GameState.P1Turn);
			break;
		case GameState.P1Turn:
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				Singleton<MultiplayerMessageHandler>.Instance.CheckPlayerLeft();
			}
			break;
		case GameState.P1EndTurn:
			mRevivingPlayerThisTurn = false;
			if (!InDeploymentPhase())
			{
				if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
				{
					Singleton<MultiplayerMessageHandler>.Instance.SendEndTurn();
				}
				Singleton<BattleHudController>.Instance.EndTurnTween.Play();
			}
			EndTurn(PlayerType.User);
			SetGameState(GameState.P2StartTurn);
			break;
		case GameState.P2StartTurn:
			Singleton<PauseController>.Instance.BlockInput(true);
			if (InDeploymentPhase())
			{
				Singleton<BattleHudController>.Instance.OnStartP2Turn();
			}
			StartTurn(PlayerType.Opponent);
			if (!InDeploymentPhase())
			{
				Singleton<CharacterAnimController>.Instance.TriggerHeroThinking2(PlayerType.Opponent);
			}
			if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				if (Singleton<TutorialController>.Instance.OverrideAIInCurrentState())
				{
					Singleton<TutorialController>.Instance.BuildAIPlan();
				}
				else
				{
					Singleton<AIManager>.Instance.StartPlanning(PlayerType.Opponent);
				}
			}
			StartCoroutine(Singleton<DWBattleLane>.Instance.ReadP2Actions());
			SetGameState(GameState.P2Turn);
			break;
		case GameState.P2Turn:
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				Singleton<MultiplayerMessageHandler>.Instance.CheckPlayerLeft();
			}
			break;
		case GameState.P2EndTurn:
			EndTurn(PlayerType.Opponent, IsFirstTurnAfterRevive());
			SetGameState(GameState.P1StartTurn);
			break;
		case GameState.LootCollect:
			break;
		case GameState.P2Defeated:
			Singleton<DWBattleLane>.Instance.ShowEnvVFXObj(true);
			battleStarted = false;
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				mMultiplayerTimeLeft = -1f;
				Singleton<BattleHudController>.Instance.ClearPvpTimer(false);
			}
			Singleton<DWGameCamera>.Instance.RenderP1Character(true);
			Singleton<CharacterAnimController>.Instance.StopCharacterFidgets();
			Singleton<BattleHudController>.Instance.OnGameEnd();
			Singleton<HandCardController>.Instance.HideHand();
			DetachedSingleton<ConditionalTutorialController>.Instance.EndConditionalBlock();
			SetGameState(GameState.P2DefeatedWaiting);
			StartCoroutine(SetGameStateWithDelay(GameState.PlayerVictory, 0.4f));
			break;
		case GameState.P2DefeatedWaiting:
			break;
		case GameState.P1Defeated:
			Singleton<DWBattleLane>.Instance.ShowEnvVFXObj(true);
			battleStarted = false;
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				mMultiplayerTimeLeft = -1f;
				Singleton<BattleHudController>.Instance.ClearPvpTimer(false);
			}
			Singleton<DWBattleLane>.Instance.PlayDefeatedAnim(PlayerType.User);
			Singleton<DWGameCamera>.Instance.RenderP1Character(true);
			Singleton<DWGameCamera>.Instance.MoveCameraToP1Defeated();
			Singleton<CharacterAnimController>.Instance.StopCharacterFidgets();
			Singleton<BattleHudController>.Instance.OnGameEnd();
			Singleton<HandCardController>.Instance.HideHand();
			Singleton<BattleIntroController>.Instance.PlayLoserBanner();
			Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(UserLoadout.Leader.Form, VOEvent.Lose);
			SetGameState(GameState.P1DefeatedWaiting);
			break;
		case GameState.P1DefeatedWaiting:
			break;
		case GameState.PlayerVictory:
			Singleton<DWBattleLane>.Instance.ShowEnvVFXObj(true);
			Singleton<PauseController>.Instance.BlockInput(false);
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				SetGameState(GameState.WaitingResultSequence);
				Singleton<PvPFinishController>.Instance.Finish(true);
				Singleton<PvpBattleResultsController>.Instance.ShowResultsSequence();
			}
			else
			{
				SetGameState(GameState.Waiting);
				Singleton<BattleIntroController>.Instance.PlayWinnerCamera(PlayerType.User);
			}
			Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(UserLoadout.Leader.Form, VOEvent.Win);
			if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
			{
				Singleton<TutorialController>.Instance.AdvanceTutorialState();
			}
			break;
		case GameState.EnemyVictory:
			Singleton<DWBattleLane>.Instance.ShowEnvVFXObj(true);
			Singleton<PauseController>.Instance.BlockInput(false);
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				Singleton<PvPFinishController>.Instance.Finish(false);
			}
			Singleton<DWBattleLane>.Instance.PlayWinnerAnim(PlayerType.Opponent);
			Singleton<DWGameCamera>.Instance.MoveCameraToP2Winner();
			if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				SetGameState(GameState.WaitingResultSequence);
				Singleton<PvpBattleResultsController>.Instance.ShowResultsSequence();
			}
			else
			{
				SetGameState(GameState.Waiting);
				Singleton<BattleResultsFailController>.Instance.Show();
			}
			break;
		case GameState.RevivePlayer:
			SetGameState(GameState.Waiting);
			mRevivingPlayerThisTurn = true;
			Singleton<HandCardController>.Instance.ClearPlayerHand();
			StartCoroutine(Singleton<SLOTMusic>.Instance.PlayBattleMusic());
			WaitingForCreatureDeployAfterRevive = true;
			InitPlayer(PlayerType.User, UserLoadout, true);
			MasterBoardState.GetPlayerState(PlayerType.User).Setup();
			StartCoroutine(ShowRevive());
			Singleton<DWBattleLane>.Instance.ShowEnvVFXObj(false);
			break;
		case GameState.EndGameWait:
			break;
		}
	}

	public void SkipCoinFlipForIntroBattle()
	{
		Singleton<TutorialController>.Instance.AdvanceTutorialState();
		Singleton<DWBattleLane>.Instance.RepositionLaneObjects();
		Singleton<DWGameCamera>.Instance.RenderP1Character(false);
		Singleton<DWGameCamera>.Instance.MoveCameraToP2Setup();
		mUserWonCoinFlip = true;
		SetGameState(GameState.Waiting);
	}

	private void OnHideLoadingFinished()
	{
		if (Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle"))
		{
			Singleton<TutorialController>.Instance.AdvanceTutorialState();
		}
	}

	private IEnumerator ShowRevive()
	{
		Singleton<CharacterAnimController>.Instance.PlayHeroAnim(PlayerType.User, CharAnimType.Revive);
		BattleCharacterAnimState animState = Singleton<CharacterAnimController>.Instance.playerAnimState[PlayerType.User];
		bool isDone = false;
		float timePast = 0f;
		while (!isDone)
		{
			timePast += Time.deltaTime;
			if (animState.GetCurrentAnimType() == CharAnimType.Revive)
			{
				if (animState.IsCurrentAnimDone())
				{
					isDone = true;
				}
				else
				{
					yield return null;
				}
			}
			else if (timePast >= 2.1f)
			{
				isDone = true;
			}
			else
			{
				yield return null;
			}
		}
		Singleton<BattleHudController>.Instance.ShowTrayTween.Play();
		Singleton<PauseController>.Instance.ShowButton();
		Singleton<QuickMessageController>.Instance.ShowButton();
		Singleton<HandCardController>.Instance.ShowHand();
		Singleton<HandCardController>.Instance.ShowOpponentTween.Play();
		Singleton<CharacterAnimController>.Instance.HideHandCards(false);
		Singleton<CharacterAnimController>.Instance.ForceIdleForBoth();
		Singleton<CharacterAnimController>.Instance.StopCharacterFidgets(false);
		Singleton<DWGameCamera>.Instance.MoveCameraToP1Setup();
		yield return new WaitForSeconds(0.5f);
		SetGameState(GameState.DealCreatureCards);
	}

	public GameState GetCurrentGameState()
	{
		return mGameState;
	}

	private IEnumerator SetGameStateWithDelay(GameState state, float delay)
	{
		yield return new WaitForSeconds(delay);
		SetGameState(state);
	}

	public void SetGameState(GameState state)
	{
		if (!CurrentBoardState.IsDeployment)
		{
			switch (state)
			{
			case GameState.P1StartTurn:
				turnNumber++;
				turnDuration = 0f;
				turnActions.Clear();
				turnStarted = true;
				break;
			case GameState.P1EndTurn:
				turnStarted = false;
				break;
			case GameState.P1Defeated:
			case GameState.P2Defeated:
				if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && turnStarted)
				{
					turnStarted = false;
					turnDuration = 0f;
					turnActions.Clear();
				}
				break;
			}
		}
		mGameState = state;
		Singleton<SLOTAudioManager>.Instance.SetVOEventCooldown(VOEvent.Idle);
		if (!mStateSwitchInputLock)
		{
			mStateSwitchInputLock = true;
			UICamera.LockInput();
		}
	}

	private void OnCoinFlipTweenFinished()
	{
		if (mGameState != GameState.P1Defeated && mGameState != GameState.P1DefeatedWaiting)
		{
			SetGameState(GameState.DealCreatureCards);
		}
	}

	public bool IsGameOver()
	{
		return mGameState == GameState.P1Defeated || mGameState == GameState.P2Defeated || mGameState == GameState.P1DefeatedWaiting || mGameState == GameState.P2DefeatedWaiting;
	}

	public void RemoveCardFromHand(PlayerType player, CardData card)
	{
		MasterBoardState.RemoveCardFromHand(player, card);
		if (player != PlayerType.User)
		{
		}
	}

	public void DiscardCard(PlayerType player, CardData card)
	{
		MasterBoardState.DiscardCard(player, card);
	}

	public void DiscardHand(PlayerType player)
	{
		MasterBoardState.DiscardHand(player);
	}

	public void MoveCardFromDiscardToDraw(PlayerType player)
	{
		MasterBoardState.MoveCardFromDiscardToDraw(player);
	}

	public void Reshuffle(PlayerType player)
	{
		MasterBoardState.Reshuffle(player);
	}

	public void DrawHeroCard(PlayerType player)
	{
		MasterBoardState.DrawHeroCard(player);
	}

	public void DrawCreatureCard(PlayerType player, int lane)
	{
		MasterBoardState.DrawCreatureCard(player, lane);
	}

	public void DragAttack(PlayerType player, int attackLane, int targetLane)
	{
		MasterBoardState.DragAttack(player, attackLane, targetLane);
	}

	public void DoResultBleed(PlayerType player)
	{
	}

	public void EndTurn(PlayerType player, bool reviving = false)
	{
		if (KFFLODManager.IsLowEndDevice())
		{
			Resources.UnloadUnusedAssets();
		}
		MasterBoardState.EndTurn(player, reviving);
	}

	public bool HasLegalMove(PlayerType player)
	{
		return MasterBoardState.HasLegalPlay(player);
	}

	public bool IsDrawPileEmpty(PlayerType idx)
	{
		return MasterBoardState.IsDrawPileEmpty(idx);
	}

	public bool IsDiscardPileEmpty(PlayerType idx)
	{
		return MasterBoardState.IsDiscardPileEmpty(idx);
	}

	public bool IsMarkedForDeath(PlayerType player, int lane)
	{
		CreatureState creature = MasterBoardState.GetCreature(player, lane);
		if (creature == null)
		{
			return true;
		}
		return false;
	}

	public LeaderData GetCharacter(PlayerType player)
	{
		return GetLeader(player).SelectedSkin;
	}

	public int GetActionPoints(PlayerType idx)
	{
		return MasterBoardState.GetActionPoints(idx);
	}

	public LaneState GetLaneState(PlayerType idx, int LaneIndex)
	{
		return MasterBoardState.GetLaneState(idx, LaneIndex);
	}

	public CreatureState GetCreature(PlayerType idx, int LaneIndex)
	{
		return MasterBoardState.GetCreature(idx, LaneIndex);
	}

	public List<CreatureState> GetCreatures(PlayerType idx)
	{
		return MasterBoardState.GetCreatures(idx);
	}

	public List<LaneState> GetLanes(PlayerType idx)
	{
		return MasterBoardState.GetLanes(idx);
	}

	public List<CardData> GetHand(PlayerType idx)
	{
		return MasterBoardState.GetHand(idx);
	}

	public List<CardData> GetDrawPile(PlayerType idx)
	{
		return MasterBoardState.GetDrawPile(idx);
	}

	public List<CardData> GetDiscardPile(PlayerType idx)
	{
		return MasterBoardState.GetDiscardPile(idx);
	}

	public LeaderItem GetLeader(PlayerType idx)
	{
		return MasterBoardState.GetLeader(idx);
	}

	public int GetHandCount(PlayerType idx)
	{
		return MasterBoardState.GetHandCount(idx);
	}

	public bool IsCreatureTargetRestricted(CreatureState creature)
	{
		List<CreatureState> list = GetCreatures(creature.Owner.Type).FindAll((CreatureState m) => m.HasBravery);
		if (list.Count > 0 && !list.Contains(creature))
		{
			return true;
		}
		return false;
	}

	private IEnumerator CreateCharacters()
	{
		yield return null;
		QuestData qd = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		for (int i = 0; i < 2; i++)
		{
			LeaderData CurrentCharacter = GetCharacter(i);
			GameObject NewCharacter = null;
			Transform tr = ((i != 0) ? Singleton<DWBattleLane>.Instance.P2CharacterPos : Singleton<DWBattleLane>.Instance.P1CharacterPos);
			yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadLeaderResources(CurrentCharacter, delegate(GameObject loadedObjData)
			{
				//NewCharacter = SLOTGame.InstantiateFX(loadedObjData, tr.position, tr.rotation) as GameObject;
				GameObject resource = Resources.Load("Characters/" + CurrentCharacter.Prefab + "/" + CurrentCharacter.Prefab, typeof(GameObject)) as GameObject;
				NewCharacter = Instantiate(resource, tr.position, tr.rotation) as GameObject;
			}));
			if (NewCharacter == null)
			{
			}
			OffsetCharacterObject(offsetX: (i != 0) ? (0f - CurrentCharacter.CharacterOffsetX) : CurrentCharacter.CharacterOffsetX, offsetY: CurrentCharacter.CharacterOffsetY, tr: tr, chair: NewCharacter.transform);
			Singleton<DWBattleLane>.Instance.StoreCharacterObj(i, NewCharacter);
			if (i == 1)
			{
				NewCharacter.ChangeLayer(LayerMask.NameToLayer("PIP2"));
			}
			Animator anim = NewCharacter.GetComponent<Animator>();
			Singleton<CharacterAnimController>.Instance.AddPlayer(anim);
			Singleton<CharacterAnimController>.Instance.playerData[i] = CurrentCharacter;
			BattleCharacterAnimState animState = NewCharacter.GetComponent<BattleCharacterAnimState>();
			animState.player = (PlayerType)i;
			animState.Init(CurrentCharacter);
			string chairName = ((i != (int)PlayerType.User) ? "Chair_P2" : "Chair_P1");
			Transform chairObj = Singleton<DWBattleLane>.Instance.GetChairObj(chairName);
			if (CurrentCharacter.UseChair)
			{
				OffsetCharacterObject(offsetX: (i != (int)PlayerType.User) ? (0f - CurrentCharacter.ChairOffsetX) : CurrentCharacter.ChairOffsetX, offsetY: CurrentCharacter.ChairOffsetY, tr: tr, chair: chairObj.transform);
				Transform[] transforms = chairObj.GetComponentsInChildren<Transform>();
				Transform[] array = transforms;
				foreach (Transform trs in array)
				{
					if (trs.name == "Chair_Shadow")
					{
						trs.position = new Vector3(trs.position.x, -49f, trs.position.z);
					}
				}
			}
			else
			{
				chairObj.gameObject.SetActive(false);
			}
		}
		Singleton<CharacterAnimController>.Instance.SetupCharacters();
		yield return null;
	}

	private void OffsetCharacterObject(Transform tr, Transform chair, float offsetX, float offsetY)
	{
		Vector3 vector2 = (chair.position = new Vector3(tr.position.x + offsetX, tr.position.y + offsetY, 0f));
		chair.rotation = tr.rotation;
	}

	private GameObject SpawnCharacterObject(Transform tr, string prefabName, float offsetX, float offsetY)
	{
		GameObject gameObject = null;
		Vector3 position = new Vector3(tr.position.x + offsetX, tr.position.y + offsetY, 0f);
		Object @object = Singleton<SLOTResourceManager>.Instance.LoadResource(prefabName);
		if (@object != null)
		{
			gameObject = SLOTGame.InstantiateFX(@object, position, tr.rotation) as GameObject;
			gameObject.transform.parent = tr;
		}
		return gameObject;
	}

	private IEnumerator LoadLevel()
	{
		yield return null;
		Setup();
		yield return null;
		PlayerInfoScript pInfo = Singleton<PlayerInfoScript>.Instance;
		QuestData qData = pInfo.StateData.CurrentActiveQuest;
		DetachedSingleton<CustomAIManager>.Instance.ParseAIData(qData.CustomAI);
		int creatureCount = 0;
		foreach (InventorySlotItem creature2 in UserLoadout.CreatureSet)
		{
			if (creature2 != null)
			{
				creatureCount++;
			}
		}
		foreach (InventorySlotItem creature in OpLoadout.CreatureSet)
		{
			if (creature != null)
			{
				creatureCount++;
			}
		}
		int totalResourceCount = 2 * creatureCount + 4;
		Singleton<SLOTResourceManager>.Instance.StartResourceLoadProgress(totalResourceCount);
		bool inIntroBattle = Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle");
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadEnvironmentResources(qData, delegate(Object loadedObjData)
		{
			//GameObject gameObject2 = SLOTGame.InstantiateFX(loadedObjData) as GameObject;
			GameObject resource = Resources.Load("Environment/" + qData.LevelPrefab + "/" + qData.LevelPrefab, typeof(GameObject)) as GameObject;
			GameObject gameObject2 = Instantiate(resource);
			gameObject2.transform.parent = Singleton<DWBattleLane>.Instance.transform;
			Singleton<DWBattleLane>.Instance.EnvironmentObj = gameObject2;
		}));
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadGameBoardResources(qData, delegate(Object loadedObjData)
		{
			//GameObject gameObject = SLOTGame.InstantiateFX(loadedObjData) as GameObject;
			GameObject resource = Resources.Load("GameBoard/" + qData.BoardPrefab + "/" + qData.BoardPrefab, typeof(GameObject)) as GameObject;
			GameObject gameObject2 = Instantiate(resource);
			gameObject2.transform.parent = Singleton<DWBattleLane>.Instance.transform;
			Singleton<DWBattleLane>.Instance.BoardObj = gameObject2;
			if (!inIntroBattle)
			{
				gameObject2.SetActive(false);
			}
		}));
		yield return StartCoroutine(CreateCharacters());
		yield return StartCoroutine(PoolCreatureObjects());
		yield return StartCoroutine(Singleton<SLOTMusic>.Instance.PlayBattleMusic());
		if (inIntroBattle)
		{
			yield return StartCoroutine(SetupIntroBattleBoard());
		}
		if (inIntroBattle)
		{
			Singleton<SLOTResourceManager>.Instance.StartAssetBundlePreload(SLOTResourceManager.PreloadBundlesPoint.IntroBattle);
		}
		else if (Singleton<TutorialController>.Instance.IsBlockActive("Q1"))
		{
			Singleton<SLOTResourceManager>.Instance.StartAssetBundlePreload(SLOTResourceManager.PreloadBundlesPoint.Q1);
		}
		else if (Singleton<TutorialController>.Instance.IsBlockActive("Q2"))
		{
			Singleton<SLOTResourceManager>.Instance.StartAssetBundlePreload(SLOTResourceManager.PreloadBundlesPoint.Q2);
		}
		if (!inIntroBattle)
		{
			SetGameState(GameState.Intro);
			StartCoroutine(Singleton<DWBattleLane>.Instance.ShowBoardAnim());
		}
		else
		{
			Singleton<DWBattleLane>.Instance.BoardHologram.SetActive(false);
		}
	}
	public IEnumerator RestoreCreatureObjectsPool()
	{
		List<CreatureItem> toRepool = new List<CreatureItem>();
		foreach (CreatureItem key in Singleton<DWBattleLane>.Instance.CreaturePool.Keys)
		{
			if (Singleton<DWBattleLane>.Instance.CreaturePool[key] == null)
			{
				toRepool.Add(key);
			}
		}
		for (int i = 0; i < toRepool.Count; i++)
		{
			Singleton<DWBattleLane>.Instance.CreaturePool.Remove(toRepool[i]);
			yield return StartCoroutine(PoolCreatureData(toRepool[i]));
		}
		yield return null;
	}

	private IEnumerator PoolCreatureObjects()
	{
		Singleton<DWBattleLane>.Instance.PoolLaneObjects();
		yield return null;
		foreach (InventorySlotItem creature2 in UserLoadout.CreatureSet)
		{
			if (creature2 != null)
			{
				yield return StartCoroutine(PoolCreatureData(creature2.Creature));
			}
		}
		yield return null;
		foreach (InventorySlotItem creature in OpLoadout.CreatureSet)
		{
			if (creature != null)
			{
				yield return StartCoroutine(PoolCreatureData(creature.Creature));
			}
		}
		yield return null;
	}

	private IEnumerator PoolCreatureData(CreatureItem creature)
	{
		if (creature == null)
		{
			yield break;
		}
		Object objData = null;
		Texture2D tex = null;
		yield return StartCoroutine(Singleton<SLOTResourceManager>.Instance.LoadCreatureResources(creature.Form, delegate(GameObject loadedObjData, Texture2D loadedTexture)
		{
			objData = loadedObjData;
			tex = loadedTexture;
		}));
		GameObject resource = Resources.Load("Creatures/" + creature.Form.Prefab + "/" + creature.Form.Prefab, typeof(GameObject)) as GameObject;
		GameObject creatureObj = Instantiate(resource) as GameObject;

		Texture2D resourceTex = Resources.Load("Creatures/" + creature.Form.Prefab + "/Textures/" + creature.Faction + "/" + creature.Form.PrefabTexture, typeof(Texture2D)) as Texture2D;

		Singleton<DWBattleLane>.Instance.CreaturePool.Add(creature, creatureObj);
		if (tex != null)
		{
			Renderer[] mats = creatureObj.GetComponentsInChildren<Renderer>(true);
			Renderer[] array = mats;
			foreach (Renderer mat in array)
			{
				mat.material.mainTexture = resourceTex;
			}
		}
		if (creatureObj != null)
		{
			creatureObj.GetComponentInChildren<Animator>().StartPlayback();
			creatureObj.GetComponentInChildren<Animator>().StopPlayback();
			creatureObj.SetActive(false);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.HitVFX))
		{
			PoolCreatureVFX(creature.Form.HitVFX);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.ShootVFX))
		{
			PoolCreatureVFX(creature.Form.ShootVFX);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.WeaponTrailVFX))
		{
			PoolCreatureVFX(creature.Form.WeaponTrailVFX);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.AttackChargeVFX))
		{
			PoolCreatureVFX(creature.Form.AttackChargeVFX);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.CritHitVFX))
		{
			PoolCreatureVFX(creature.Form.CritHitVFX);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.PersistentVFX))
		{
			PoolCreatureVFX(creature.Form.PersistentVFX);
		}
		if (!Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(creature.Form.RezInVFX))
		{
			PoolCreatureVFX(creature.Form.RezInVFX);
		}
	}

	public void PoolCreatureVFX(string path)
	{
		if (!string.IsNullOrEmpty(path) && !Singleton<DWBattleLane>.Instance.CreatureVFXPool.ContainsKey(path))
		{
			GameObject gameObject = Singleton<SLOTResourceManager>.Instance.LoadResource("VFX/Creatures/" + path) as GameObject;
			if (gameObject != null)
			{
				Singleton<DWBattleLane>.Instance.CreatureVFXPool.Add(path, gameObject);
				GameObject obj = Object.Instantiate(gameObject);
				Object.Destroy(obj);
			}
		}
	}

	public void EndPlayerTurn()
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			mMultiplayerTimeLeft = -1f;
		}
		SetGameState(GameState.P1EndTurn);
	}

	public void RevivePlayer()
	{
		StartCoroutine(RestoreCreatureObjectsPool());
		SetGameState(GameState.RevivePlayer);
	}

	public bool FindKeywordForTutorial(string keywordIDString, out CardPrefabScript foundCard, out StatusIconItem foundStatusIcon)
	{
		foundCard = null;
		foundStatusIcon = null;
		string[] array = keywordIDString.Split('+');
		foreach (CardPrefabScript handCard in Singleton<HandCardController>.Instance.GetHandCards())
		{
			if (handCard.Card != null && handCard.Card.HasKeywords(array))
			{
				foundCard = handCard;
				return true;
			}
		}
		string statusKeyword = array[array.Length - 1];
		CreatureBuffBar[] componentsInChildren = Singleton<BattleHudController>.Instance.GetComponentsInChildren<CreatureBuffBar>();
		DWBattleLaneObject lane;
		foreach (DWBattleLaneObject item in Singleton<DWBattleLane>.Instance.BattleLaneObjects[0])
		{
			lane = item;
			CreatureBuffBar creatureBuffBar = componentsInChildren.Find((CreatureBuffBar m) => m.mCreature == lane.Creature);
			StatusIconItem statusIconItem = creatureBuffBar.FindStatusIcon(statusKeyword);
			if (statusIconItem != null)
			{
				foundStatusIcon = statusIconItem;
				return true;
			}
		}
		return false;
	}
}
