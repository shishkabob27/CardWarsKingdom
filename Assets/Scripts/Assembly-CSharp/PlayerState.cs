using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class PlayerState
{
	public enum CanPlayResult
	{
		CanPlay,
		NotEnoughAP,
		NoTarget,
		IsFrozen,
		IsMindless,
		MustPlayCreature,
		AIForbidden
	}

	public const int MAX_HAND = 10;

	public const int CARDS_TO_DEAL = 5;

	private ObscuredInt actionPoints;

	private ObscuredInt CurrentActionPoints;

	public BoardStats Statistics;

	public List<CreatureState> DeploymentList = new List<CreatureState>();

	public List<CardData> DrawPile = new List<CardData>();

	public List<CardData> DrawQueue = new List<CardData>();

	public List<CardData> Hand = new List<CardData>();

	public List<CardData> DiscardPile = new List<CardData>();

	public List<CreatureState> Graveyard = new List<CreatureState>();

	public List<CardData> PlayedCards = new List<CardData>();

	public List<LaneState> Lanes = new List<LaneState>();

	private CreatureState Target;

	private List<string> TutorialInitialHandOverride;

	public int ActionPoints
	{
		get
		{
			return actionPoints;
		}
		set
		{
			actionPoints = Math.Max(0, value);
		}
	}

	public ObscuredInt EffectiveCurrentActionPoints { get; private set; }

	public ObscuredInt CurrentPvpTimeLimit { get; set; }

	public BoardState Game { get; set; }

	public PlayerType Type { get; set; }

	public LeaderItem Leader { get; set; }

	public PlayerState Opponent
	{
		get
		{
			return Game.GetPlayerState(!Type);
		}
	}

	public ObscuredInt APMeter { get; set; }

	public int DamageDealt { get; set; }

	public static PlayerState Create(PlayerType WhichPlayer)
	{
		PlayerState playerState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(PlayerState)) as PlayerState;
		playerState.Init(WhichPlayer);
		return playerState;
	}

	private static PlayerState Create(PlayerState Source)
	{
		PlayerState playerState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(PlayerState)) as PlayerState;
		playerState.Init(Source);
		return playerState;
	}

	public static void Destroy(PlayerState State)
	{
		State.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(State);
	}

	public void Init(PlayerType WhichPlayer)
	{
		Statistics = default(BoardStats);
		Statistics.mFactionCount = new int[6];
		Statistics.mTypeCount = new int[16];
		int startAP = Singleton<TutorialController>.Instance.GetStartAP(WhichPlayer);
		if (startAP != -1)
		{
			CurrentActionPoints = startAP;
		}
		else
		{
			CurrentActionPoints = MiscParams.StartingActionPoints;
		}
		EffectiveCurrentActionPoints = CurrentActionPoints;
		CurrentPvpTimeLimit = MiscParams.MultiplayerTimeLimitStart;
		Type = WhichPlayer;
		LaneState laneState = null;
		for (int i = 0; i < MiscParams.CreaturesOnBoard; i++)
		{
			LaneState laneState2 = LaneState.Create();
			laneState2.Owner = this;
			laneState2.Index = Lanes.Count;
			if (laneState != null)
			{
				laneState2.AdjacentLanes.Add(laneState);
				laneState.AdjacentLanes.Add(laneState2);
			}
			laneState = laneState2;
			Lanes.Add(laneState2);
		}
		TutorialInitialHandOverride = Singleton<TutorialController>.Instance.GetCurrentForcedStartingHand(Type);
		Reset();
	}

	public void Init(PlayerState Source)
	{
		LaneState laneState = null;
		Type = Source.Type;
		Leader = Source.Leader;
		ActionPoints = Source.ActionPoints;
		CurrentActionPoints = Source.CurrentActionPoints;
		EffectiveCurrentActionPoints = Source.EffectiveCurrentActionPoints;
		APMeter = Source.APMeter;
		DamageDealt = Source.DamageDealt;
		DrawPile.AddRange(Source.DrawPile);
		DrawQueue.AddRange(Source.DrawQueue);
		Hand.AddRange(Source.Hand);
		DiscardPile.AddRange(Source.DiscardPile);
		foreach (LaneState lane in Source.Lanes)
		{
			LaneState laneState2 = lane.DeepCopy();
			laneState2.Owner = this;
			if (laneState2.Creature != null)
			{
				laneState2.Creature.Owner = this;
			}
			if (laneState != null)
			{
				laneState2.AdjacentLanes.Add(laneState);
				laneState.AdjacentLanes.Add(laneState2);
			}
			laneState2.Index = Lanes.Count;
			laneState = laneState2;
			Lanes.Add(laneState2);
		}
		if (Source.Target != null)
		{
			Target = Lanes[Source.Target.Lane.Index].Creature;
		}
		foreach (CreatureState item in Source.Graveyard)
		{
			CreatureState creatureState = item.DeepCopy();
			creatureState.Owner = this;
			creatureState.Lane = Lanes[item.Lane.Index];
			Graveyard.Add(creatureState);
		}
		foreach (CreatureState deployment in Source.DeploymentList)
		{
			CreatureState creatureState2 = deployment.DeepCopy();
			creatureState2.Owner = this;
			DeploymentList.Add(creatureState2);
		}
		Statistics = default(BoardStats);
		Statistics.mFactionCount = new int[6];
		Statistics.mTypeCount = new int[16];
		Statistics.mCardsPlayed = Source.Statistics.mCardsPlayed;
		Statistics.mHeroCardsPlayed = Source.Statistics.mHeroCardsPlayed;
		Statistics.mCardsDrawn = Source.Statistics.mCardsDrawn;
		Statistics.mHeroCardsDrawn = Source.Statistics.mHeroCardsDrawn;
		Statistics.mCreaturesKilledThisTurn = Source.Statistics.mCreaturesKilledThisTurn;
		Statistics.mCreaturesDeployedThisTurn = Source.Statistics.mCreaturesDeployedThisTurn;
		UpdateStatistics();
	}

	public void Clean()
	{
		Statistics.Reset();
		ActionPoints = 0;
		CurrentActionPoints = 0;
		EffectiveCurrentActionPoints = 0;
		APMeter = 0;
		DamageDealt = 0;
		BoardState boardState = null;
		Type = null;
		Leader = null;
		Target = null;
		DrawPile.Clear();
		DrawQueue.Clear();
		Hand.Clear();
		DiscardPile.Clear();
		PlayedCards.Clear();
		foreach (CreatureState item in Graveyard)
		{
			CreatureState.Destroy(item);
		}
		Graveyard.Clear();
		foreach (CreatureState deployment in DeploymentList)
		{
			CreatureState.Destroy(deployment);
		}
		DeploymentList.Clear();
		foreach (LaneState lane in Lanes)
		{
			LaneState.Destroy(lane);
		}
		Lanes.Clear();
	}

	public PlayerState DeepCopy()
	{
		return Create(this);
	}

	public void UpdateStatistics()
	{
		Statistics.Reset();
		Statistics.mDeadCreatureCount = Graveyard.Count;
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null)
			{
				Statistics.mCreatureCount++;
				Statistics.mFactionCount[(int)lane.Creature.Data.Faction]++;
				Statistics.mTypeCount[(int)lane.Creature.Data.Form.Type]++;
			}
		}
	}

	public int GetCounter(EffectMultiplier per, CreatureState Target)
	{
		UpdateStatistics();
		switch (per)
		{
		case EffectMultiplier.None:
			return 1;
		case EffectMultiplier.PerLivingAlly:
			return Statistics.mCreatureCount;
		case EffectMultiplier.PerLivingEnemy:
			return Opponent.Statistics.mCreatureCount;
		case EffectMultiplier.PerDeadAlly:
			return Statistics.mDeadCreatureCount;
		case EffectMultiplier.PerDeadEnemy:
			return Opponent.Statistics.mDeadCreatureCount;
		case EffectMultiplier.PerRed:
			return Statistics.mFactionCount[1];
		case EffectMultiplier.PerGreen:
			return Statistics.mFactionCount[2];
		case EffectMultiplier.PerBlue:
			return Statistics.mFactionCount[3];
		case EffectMultiplier.PerLight:
			return Statistics.mFactionCount[5];
		case EffectMultiplier.PerDark:
			return Statistics.mFactionCount[4];
		case EffectMultiplier.PerCardInHand:
			return Hand.Count;
		case EffectMultiplier.PerCardPlayed:
			return Statistics.mCardsPlayed;
		case EffectMultiplier.PerCardDrawn:
			return Statistics.mCardsDrawn;
		case EffectMultiplier.PerBuff:
			return (Target != null) ? Target.GetStatusTypeCount(StatusType.Buff) : 0;
		case EffectMultiplier.PerDebuff:
			return (Target != null) ? Target.GetStatusTypeCount(StatusType.Debuff) : 0;
		default:
			return 0;
		}
	}

	private void PopulateDrawQueue(bool firstDeal = false)
	{
		List<CardData> list = DrawPile.Copy();
		if (!firstDeal)
		{
			list.RemoveAll((CardData m) => m.OneShot);
		}
		while (list.Count > 0)
		{
			int index = KFFRandom.RandomIndex(list.Count);
			DrawQueue.Add(list[index]);
			list.RemoveAt(index);
		}
	}

	public void ShuffleDrawPile()
	{
		int num = DrawPile.Count * 7;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		while (num > 0)
		{
			while (num2 == num3)
			{
				num2 = KFFRandom.RandomIndex(DrawPile.Count);
				num3 = KFFRandom.RandomIndex(DrawPile.Count);
			}
			CardData value = DrawPile[num2];
			DrawPile[num2] = DrawPile[num3];
			DrawPile[num3] = value;
			num2 = num3;
			num--;
		}
	}

	public void Init(Loadout loadout, bool reviving)
	{
		DrawPile.Clear();
		DrawQueue.Clear();
		DiscardPile.Clear();
		Hand.Clear();
		int num = 0;
		foreach (InventorySlotItem item2 in loadout.CreatureSet)
		{
			if (item2 != null)
			{
				num++;
				item2.Creature.SetupPassiveAbilitySources();
				CreatureState item = CreatureState.Create(this, item2.Creature);
				DeploymentList.Add(item);
			}
		}
		Leader = loadout.Leader;
		foreach (CardData actionCard in Leader.Form.ActionCards)
		{
			if (actionCard != null)
			{
				DrawPile.Add(actionCard);
			}
		}
		PopulateDrawQueue(true);
		PopulateDrawQueue();
		if (!reviving)
		{
			APMeter = 0;
		}
	}

	public void Setup()
	{
		Reset();
		ShuffleDrawPile();
	}

	public void DrawCreatureCard(int LaneIndex)
	{
		CreatureState creature = Lanes[LaneIndex].Creature;
		creature.DrawCard();
		Statistics.mCardsDrawn++;
	}

	public void DrawHeroCard()
	{
		if (DrawPile.Count <= 0)
		{
			return;
		}
		string text = Singleton<TutorialController>.Instance.CheckTutorialLeaderCardDraw();
		CardData cardData;
		if (text != null)
		{
			cardData = CardDataManager.Instance.GetData(text);
		}
		else
		{
			cardData = DrawQueue[0];
			DrawQueue.RemoveAt(0);
			if (DrawQueue.Count <= DrawPile.Count)
			{
				PopulateDrawQueue();
			}
		}
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.DRAW_CARD;
		gameMessage.WhichPlayer = this;
		gameMessage.Card = cardData;
		Game.AddMessage(gameMessage);
		PlaceCardInHand(cardData);
		Statistics.mCardsDrawn++;
		Statistics.mHeroCardsDrawn++;
	}

	public void ForceDiscard(int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (Hand.Count > 0)
			{
				int index = KFFRandom.RandomIndex(Hand.Count);
				CardData card = Hand[index];
				GameMessage gameMessage = new GameMessage();
				gameMessage.Action = GameEvent.FORCE_DISCARD_CARD;
				gameMessage.WhichPlayer = this;
				gameMessage.Card = card;
				Game.AddMessage(gameMessage);
				DiscardFromHand(card);
			}
		}
	}

	public void ManualDiscard(CardData Card)
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.MANUAL_DISCARD_CARD;
		gameMessage.WhichPlayer = this;
		gameMessage.Card = Card;
		Game.AddMessage(gameMessage);
		DiscardFromHand(Card);
		if (MiscParams.ActionPointsPerDiscard > 0)
		{
			AddActionPoints(MiscParams.ActionPointsPerDiscard);
		}
	}

	public void PullCreatures(int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (DeploymentList.Count == 0)
			{
				break;
			}
			int creatureCount = GetCreatureCount();
			int index = KFFRandom.RandomIndex(DeploymentList.Count);
			int laneIndex = 0;
			if (creatureCount > 0)
			{
				laneIndex = KFFRandom.RandomIndex(creatureCount + 1);
			}
			DeployCreature(DeploymentList[index].Data, laneIndex, true);
		}
	}

	public void Reset()
	{
		Statistics.Reset();
		ResetForNewWave();
	}

	public void ResetForNewWave()
	{
		foreach (LaneState lane in Lanes)
		{
			lane.Creature = null;
		}
		Hand.Clear();
		DiscardPile.Clear();
		PlayedCards.Clear();
	}

	public void FillAPMeter(int amount)
	{
		if (!Singleton<TutorialController>.Instance.DisableLeaderBar() && (!Singleton<TutorialController>.Instance.DisableOpponentLeaderBar() || Type != PlayerType.Opponent))
		{
			if (Type == PlayerType.User && Singleton<TutorialController>.Instance.AutoFillLeaderBar())
			{
				amount = Leader.Form.APThreshold;
			}
			if (amount < 0 && -amount > (int)APMeter)
			{
				amount = -(int)APMeter;
			}
			APMeter = (int)APMeter + amount;
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.ADVANCE_APMETER;
			gameMessage.WhichPlayer = this;
			gameMessage.Amount = amount;
			Game.AddMessage(gameMessage);
		}
	}

	public void CheckFullAPMeter()
	{
		if ((int)APMeter >= Leader.Form.APThreshold)
		{
			if (Hand.Count < 10)
			{
				GameMessage gameMessage = new GameMessage();
				gameMessage.Action = GameEvent.APMETER_FULL;
				gameMessage.WhichPlayer = this;
				Game.AddMessage(gameMessage);
				DrawHeroCard();
				APMeter = 0;
				gameMessage = new GameMessage();
				gameMessage.Action = GameEvent.RESET_APMETER;
				gameMessage.WhichPlayer = this;
				Game.AddMessage(gameMessage);
			}
			else
			{
				APMeter = Leader.Form.APThreshold;
			}
		}
	}

	public void DrawCardFromDeploymentList(CreatureState Creature, CardData forcedCard)
	{
		if (TutorialInitialHandOverride != null && TutorialInitialHandOverride.Count > 0)
		{
			CardData data = CardDataManager.Instance.GetData(TutorialInitialHandOverride[0]);
			TutorialInitialHandOverride.RemoveAt(0);
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.INITIAL_DRAW_CARD;
			gameMessage.WhichPlayer = this;
			gameMessage.Creature = Creature;
			gameMessage.Card = data;
			Game.AddMessage(gameMessage);
			PlaceCardInHand(data);
		}
		else if (forcedCard != null)
		{
			GameMessage gameMessage2 = new GameMessage();
			gameMessage2.Action = GameEvent.INITIAL_DRAW_CARD;
			gameMessage2.WhichPlayer = this;
			gameMessage2.Creature = Creature;
			gameMessage2.Card = forcedCard;
			Game.AddMessage(gameMessage2);
			PlaceCardInHand(forcedCard);
		}
		else
		{
			Creature.DrawCard(true);
		}
	}

	public void StartTurn()
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.START_TURN;
		gameMessage.WhichPlayer = this;
		Game.AddMessage(gameMessage);
		Statistics.mCardsPlayed = 0;
		Statistics.mHeroCardsPlayed = 0;
		Statistics.mCardsDrawn = 0;
		Statistics.mHeroCardsDrawn = 0;
		Statistics.mCreaturesKilledThisTurn = 0;
		Statistics.mCreaturesDeployedThisTurn = 0;
		DamageDealt = 0;
		if (Singleton<DWGame>.Instance.IsTutorialSetup)
		{
			AddActionPoints(Singleton<TutorialController>.Instance.GetStartAP(PlayerType.User));
			Opponent.AddActionPoints(Singleton<TutorialController>.Instance.GetStartAP(PlayerType.Opponent));
		}
		else if (Game.IsFirstTurn() && !Game.IsDeployment)
		{
			AddActionPoints(MiscParams.StartingActionPoints);
			Opponent.AddActionPoints(MiscParams.StartingActionPoints);
		}
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null)
			{
				lane.Creature.StartTurn();
			}
		}
		if (!Game.IsFirstTurn() && !Game.IsFirstTurnAfterRevive())
		{
			return;
		}
		List<CardData> customForcedCards = GetCustomForcedCards();
		int a = 5;
		if (TutorialInitialHandOverride != null)
		{
			a = TutorialInitialHandOverride.Count;
		}
		else if (DetachedSingleton<CustomAIManager>.Instance.OverrideHandSize != -1)
		{
			a = DetachedSingleton<CustomAIManager>.Instance.OverrideHandSize;
		}
		a = Mathf.Max(a, customForcedCards.Count);
		for (int i = 0; i < a; i++)
		{
			CardData forcedCard = ((i >= customForcedCards.Count) ? null : customForcedCards[i]);
			int index = KFFRandom.RandomIndex(DeploymentList.Count);
			DrawCardFromDeploymentList(DeploymentList[index], forcedCard);
		}
		if (!Game.IsFirstTurn())
		{
			return;
		}
		customForcedCards = Opponent.GetCustomForcedCards();
		a = 5;
		if (Opponent.TutorialInitialHandOverride != null)
		{
			a = Opponent.TutorialInitialHandOverride.Count;
		}
		else if (DetachedSingleton<CustomAIManager>.Instance.OverrideHandSize != -1)
		{
			a = DetachedSingleton<CustomAIManager>.Instance.OverrideHandSize;
		}
		a = Mathf.Max(a, customForcedCards.Count);
		for (int j = 0; j < a; j++)
		{
			CardData forcedCard2 = ((j >= customForcedCards.Count) ? null : customForcedCards[j]);
			if (Singleton<DWGame>.Instance.IsTutorialSetup)
			{
				Opponent.DrawCardFromDeploymentList(null, forcedCard2);
				continue;
			}
			int index2 = KFFRandom.RandomIndex(Opponent.DeploymentList.Count);
			Opponent.DrawCardFromDeploymentList(Opponent.DeploymentList[index2], forcedCard2);
		}
	}

	private List<CardData> GetCustomForcedCards()
	{
		List<CardData> list = new List<CardData>();
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && Type == PlayerType.Opponent)
		{
			foreach (CustomAIManager.ParsedRow item in DetachedSingleton<CustomAIManager>.Instance.GetCustomActionsAtGameStart())
			{
				foreach (CustomAIManager.ParsedAction item2 in item.Actions.FindAll((CustomAIManager.ParsedAction m) => m is CustomAIScripts.StartWithCard))
				{
					CustomAIScripts.StartWithCard startWithCard = item2 as CustomAIScripts.StartWithCard;
					list.Add(startWithCard.Card);
				}
			}
			return list;
		}
		return list;
	}

	public void ProcessMessage(GameMessage Message)
	{
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null)
			{
				lane.Creature.ProcessMessage(Message);
			}
		}
		foreach (CreatureState item in Graveyard)
		{
			if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.SecondCreature == item)
			{
				item.ProcessMessage(Message);
			}
		}
		CompressLanes();
	}

	public bool CanPlayCard()
	{
		foreach (CardData item in Hand)
		{
			if (CanPlay(item) == CanPlayResult.CanPlay)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanAttack()
	{
		if (Game.IsDeployment)
		{
			return false;
		}
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null && !lane.Creature.IsFrozen && lane.Creature.AttackCost <= ActionPoints && !DetachedSingleton<CustomAIManager>.Instance.IsCreatureTargetForbidden(lane.Creature.Data))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanDeploy()
	{
		foreach (CreatureState deployment in DeploymentList)
		{
			if (CanDeploy(deployment.Data))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasLegalPlay()
	{
		return CanAttack() || CanDeploy();
	}

	private bool IsFree(List<LaneState> LaneList)
	{
		foreach (LaneState Lane in LaneList)
		{
			if (Lane.Creature != null && !Lane.Creature.IsFrozen)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsSmart(List<LaneState> LaneList)
	{
		foreach (LaneState Lane in LaneList)
		{
			if (Lane.Creature != null && !Lane.Creature.IsMindless)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanDeploy(CreatureItem Item, bool ignoreForbidden = false)
	{
		if (!ignoreForbidden && Type == PlayerType.Opponent && !Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && DetachedSingleton<CustomAIManager>.Instance.IsCreatureForbidden(Item))
		{
			return false;
		}
		if (Statistics.mCreatureCount >= MiscParams.CreaturesOnBoard)
		{
			return false;
		}
		if (Game.IsDeployment && Statistics.mCreatureCount == 0)
		{
			return true;
		}
		if ((Type == PlayerType.User && Singleton<TutorialController>.Instance.IgnoreEnergyCosts()) || Item.DeployCost <= ActionPoints)
		{
			return true;
		}
		return false;
	}

	public CanPlayResult CanPlay(CardData card, PlayerType WhichPlayer = null, int LaneIndex = -1, bool ignoreForbidden = false)
	{
		if (!ignoreForbidden && Type == PlayerType.Opponent && !Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && DetachedSingleton<CustomAIManager>.Instance.IsCardForbidden(card))
		{
			return CanPlayResult.AIForbidden;
		}
		if ((Type != PlayerType.User || !Singleton<TutorialController>.Instance.IgnoreEnergyCosts()) && ActionPoints < (int)card.Cost)
		{
			return CanPlayResult.NotEnoughAP;
		}
		if (GetCreatureCount() == 0)
		{
			return CanPlayResult.MustPlayCreature;
		}
		if (card.TargetType1 == SelectionType.None)
		{
			return CanPlayResult.CanPlay;
		}
		List<LaneState> firstTargetList = Game.GetFirstTargetList(Type, card);
		if (LaneIndex >= 0)
		{
			PlayerState playerState = Game.GetPlayerState(WhichPlayer);
			LaneState Lane = playerState.GetLaneState(LaneIndex);
			firstTargetList.RemoveAll((LaneState l) => l != Lane);
		}
		firstTargetList.RemoveAll((LaneState l) => l.Creature != null && l.Creature.Owner == this && DetachedSingleton<CustomAIManager>.Instance.IsCreatureTargetForbidden(l.Creature.Data));
		if (card.TargetType1 == SelectionType.Lane && firstTargetList.Count > 0)
		{
			if (!IsSmart(firstTargetList))
			{
				return CanPlayResult.IsMindless;
			}
			if (card.MaxConditionalAttacks() > 0 && !IsFree(firstTargetList))
			{
				return CanPlayResult.IsFrozen;
			}
			if (card.MaxConditionalAttacks() > 0 && GetTarget(null, AttackBase.None, AttackRange.All) == null)
			{
				return CanPlayResult.NoTarget;
			}
			if (card.TargetType2 == SelectionType.None)
			{
				return CanPlayResult.CanPlay;
			}
			if (card.TargetType2 == SelectionType.Lane && Game.GetSecondTargetList(Type, card).Count > 0)
			{
				return CanPlayResult.CanPlay;
			}
		}
		return CanPlayResult.NoTarget;
	}

	public void SpendActionPoints(int cost)
	{
		if (cost > 0)
		{
			if (cost > ActionPoints)
			{
				cost = ActionPoints;
			}
			ActionPoints -= cost;
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.SPEND_ACTION_POINTS;
			gameMessage.WhichPlayer = this;
			gameMessage.Amount = cost;
			Game.AddMessage(gameMessage);
		}
	}

	private void CheckZeroAP()
	{
		if (ActionPoints <= 0)
		{
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.ZERO_ACTION_POINTS;
			gameMessage.WhichPlayer = this;
			Game.AddMessage(gameMessage);
		}
	}

	public void LoseActionPoints(int cost)
	{
		if (cost > 0)
		{
			if (cost > ActionPoints)
			{
				cost = ActionPoints;
			}
			ActionPoints -= cost;
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.LOSE_ACTION_POINTS;
			gameMessage.WhichPlayer = this;
			gameMessage.Amount = cost;
			Game.AddMessage(gameMessage);
			CheckZeroAP();
		}
	}

	public void AddActionPoints(int amount)
	{
		if (amount > 0)
		{
			ActionPoints += amount;
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.GAIN_ACTION_POINTS;
			gameMessage.WhichPlayer = this;
			gameMessage.Amount = amount;
			Game.AddMessage(gameMessage);
		}
	}

	public CreatureState DeployCreature(CreatureItem Item, int LaneIndex, bool fromCardPull = false)
	{
		int num = DeploymentList.FindIndex((CreatureState m) => m.Data == Item);
		if (num == -1)
		{
			return null;
		}
		PushCreatures(LaneIndex);
		CreatureState creatureState = DeploymentList[num];
		LaneState laneState2 = (creatureState.Lane = Lanes[LaneIndex]);
		laneState2.Creature = creatureState;
		DeploymentList.Remove(creatureState);
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.CREATURE_DEPLOYED;
		gameMessage.WhichPlayer = this;
		gameMessage.Creature = creatureState;
		gameMessage.IsDrag = !fromCardPull;
		gameMessage.Lane = laneState2;
		Game.AddMessage(gameMessage);
		if (Item.QuestLoadoutEntryData != null)
		{
			if (Item.QuestLoadoutEntryData.DirectDamageImmune)
			{
				creatureState.ApplyStatus(StatusDataManager.Instance.GetData("DirectDamageImmune"), 1f, null);
			}
			if (Item.QuestLoadoutEntryData.PoisonImmune)
			{
				creatureState.ApplyStatus(StatusDataManager.Instance.GetData("PoisonImmune"), 1f, null);
			}
		}
		if (!fromCardPull)
		{
			SpendActionPoints(Item.DeployCost);
			if (Item.DeployCost > 0)
			{
				CheckZeroAP();
			}
		}
		UpdateStatistics();
		Statistics.mCreaturesDeployedThisTurn++;
		Statistics.mCardsPlayed++;
		return creatureState;
	}

	public void SwapCreature(CreatureState TargetCreature, LaneState TargetLane)
	{
		CreatureState creatureState = null;
		LaneState lane = null;
		if (TargetLane.Creature != null)
		{
			creatureState = TargetLane.Creature;
			lane = TargetCreature.Lane;
		}
		else
		{
			foreach (CreatureState item in Graveyard)
			{
				if (item.Lane == TargetLane)
				{
					item.Lane = TargetCreature.Lane;
				}
			}
		}
		RemoveCreature(TargetCreature.Lane);
		if (creatureState != null)
		{
			RemoveCreature(creatureState.Lane);
			PlaceCreature(creatureState, lane);
		}
		PlaceCreature(TargetCreature, TargetLane);
	}

	public void SetTarget(CreatureState TargetCreature)
	{
		Target = TargetCreature;
	}

	public int GetTargetLane()
	{
		if (Target != null)
		{
			return Target.Lane.Index;
		}
		return -1;
	}

	public CreatureState GetAutoTarget(CreatureState Attacker, AttackBase Base, AttackRange Range)
	{
		CreatureState creatureState = null;
		foreach (LaneState lane in Opponent.Lanes)
		{
			if (lane.Creature == null || (creatureState != null && (bool)creatureState.HasBravery && !lane.Creature.HasBravery))
			{
				continue;
			}
			if ((bool)lane.Creature.HasBravery && (creatureState == null || !creatureState.HasBravery))
			{
				creatureState = lane.Creature;
			}
			else if (creatureState == null || Attacker == null || lane.Creature.AssessThreat(Attacker, Base) > creatureState.AssessThreat(Attacker, Base))
			{
				int creatureCount = lane.Owner.GetCreatureCount();
				if (Range != AttackRange.Triple || creatureCount < 3 || (lane.Index != 0 && lane.Index != creatureCount - 1))
				{
					creatureState = lane.Creature;
				}
			}
		}
		return creatureState;
	}

	public CreatureState GetTarget(CreatureState Attacker, AttackBase Base, AttackRange EnemyGroup)
	{
		if (Target != null && Target.HP > 0)
		{
			return Target;
		}
		return GetAutoTarget(Attacker, Base, EnemyGroup);
	}

	public CreatureState GetRandomTarget(CreatureState Attacker)
	{
		CWList<CreatureState> cWList = new CWList<CreatureState>();
		bool flag = false;
		foreach (LaneState lane in Attacker.Owner.Opponent.Lanes)
		{
			if (lane.Creature == null)
			{
				continue;
			}
			if ((bool)lane.Creature.HasBravery)
			{
				if (!flag)
				{
					flag = true;
					cWList.Clear();
				}
				cWList.Add(lane.Creature);
			}
			else if (!flag)
			{
				cWList.Add(lane.Creature);
			}
		}
		return cWList.RandomItem();
	}

	public void Attack(CreatureState Attacker, CreatureState Target, AttackBase Base, AttackCause cause, float damageOverride = -1f)
	{
		AttackMessage(Attacker, GameEvent.ATTACK_START);
		CreatureState target = Target;
		foreach (StatusState statusEffect in Attacker.StatusEffects)
		{
			if (statusEffect.AutoMiss())
			{
				Attacker.AutoMiss = true;
			}
			if (statusEffect.AttackRandom())
			{
				target = GetRandomTarget(Attacker);
			}
		}
		foreach (StatusState statusEffect2 in Target.StatusEffects)
		{
			if (statusEffect2.Dodge(cause))
			{
				Attacker.AutoMiss = true;
			}
		}
		AttackMessage(Attacker, GameEvent.ATTACK_HIT_START);
		Attacker.Attack(target, Base, cause, null, damageOverride);
		Attacker.AutoMiss = false;
		AttackMessage(Attacker, GameEvent.ATTACK_HIT_END);
		AttackMessage(Attacker, GameEvent.ATTACK_END);
	}

	public void AttackAndDraw(int AttackIndex, int TargetIndex, AttackCause cause)
	{
		AttackProgress attackProgress = new AttackProgress();
		AttackProgress.Attacks.Add(attackProgress);
		CreatureState creature = Lanes[AttackIndex].Creature;
		attackProgress.NumAttacks = 1;
		foreach (StatusState statusEffect in creature.StatusEffects)
		{
			attackProgress.NumAttacks += statusEffect.BonusDragAttacks();
		}
		attackProgress.UsingPlayer = Type;
		attackProgress.Attacker = creature;
		attackProgress.TargetIndex = TargetIndex;
		attackProgress.Cause = cause;
		attackProgress.AttacksTaken = 0;
	}

	public void CompressLanes()
	{
		for (int i = 0; i < MiscParams.CreaturesOnBoard; i++)
		{
			if (Lanes[i].Creature != null)
			{
				continue;
			}
			for (int j = i + 1; j < MiscParams.CreaturesOnBoard; j++)
			{
				if (Lanes[j].Creature != null)
				{
					SwapCreature(Lanes[j].Creature, Lanes[i]);
					break;
				}
			}
		}
	}

	public void PushCreatures(int LaneIndex)
	{
		int mCreatureCount = Statistics.mCreatureCount;
		if (mCreatureCount < MiscParams.CreaturesOnBoard)
		{
			for (int num = mCreatureCount - 1; num >= LaneIndex; num--)
			{
				SwapCreature(Lanes[num].Creature, Lanes[num + 1]);
			}
		}
	}

	public static List<CreatureState> GetCreatureRange(PlayerState WhichPlayer, CreatureState BaseTarget, AttackRange Group)
	{
		List<CreatureState> list = new List<CreatureState>();
		if (BaseTarget != null && Group != AttackRange.All)
		{
			list.Add(BaseTarget);
		}
		switch (Group)
		{
		case AttackRange.Double:
			if (BaseTarget == null)
			{
				break;
			}
			{
				foreach (LaneState adjacentLane in BaseTarget.Lane.AdjacentLanes)
				{
					if (adjacentLane.Creature != null)
					{
						list.Add(adjacentLane.Creature);
						return list;
					}
				}
				return list;
			}
		case AttackRange.Triple:
			if (BaseTarget == null)
			{
				break;
			}
			{
				foreach (LaneState adjacentLane2 in BaseTarget.Lane.AdjacentLanes)
				{
					if (adjacentLane2.Creature != null)
					{
						list.Add(adjacentLane2.Creature);
					}
				}
				return list;
			}
		case AttackRange.All:
		{
			foreach (LaneState lane in WhichPlayer.Lanes)
			{
				if (lane.Creature != null)
				{
					list.Add(lane.Creature);
				}
			}
			return list;
		}
		}
		return list;
	}

	private void ApplyStatus(LaneState TargetLane, AttackRange Group, StatusData status, float varValue, CardData Card)
	{
		CreatureState baseTarget = null;
		PlayerState playerState = null;
		if (TargetLane != null)
		{
			baseTarget = TargetLane.Creature;
		}
		playerState = ((status.StatusType != StatusType.Buff) ? Opponent : this);
		List<CreatureState> creatureRange = GetCreatureRange(playerState, baseTarget, Group);
		foreach (CreatureState item in creatureRange)
		{
			item.ApplyStatus(status, varValue, Card, null, Card.RemoveStatusData);
		}
	}

	public LaneState GetTargetLane(CardData Card, int LaneIndex, PlayerType targetPlayer)
	{
		LaneState result = null;
		if (LaneIndex >= 0)
		{
			List<LaneState> firstTargetList = Game.GetFirstTargetList(Type, Card);
			result = firstTargetList.Find((LaneState l) => l.Index == LaneIndex && l.Owner.Type == targetPlayer);
		}
		return result;
	}

	public void AttackMessage(CreatureState Attacker, GameEvent Reason)
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = Reason;
		gameMessage.WhichPlayer = Attacker.Owner;
		gameMessage.Creature = Attacker;
		gameMessage.Lane = Attacker.Lane;
		Game.AddMessage(gameMessage);
	}

	public void PlayActionCard(PlayerType WhichPlayer, CardData Card, PlayerType targetPlayer, int LaneIndex)
	{
		CardProgress.Instance.UsingPlayer = WhichPlayer;
		CardProgress.Instance.TargetPlayer = targetPlayer;
		CardProgress.Instance.Card = Card;
		CardProgress.Instance.LaneIndex = LaneIndex;
		CardProgress.Instance.State = CardState.Init;
	}

	public int AICritComp(int currentChance)
	{
		float num = 1f;
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null)
			{
				num *= 1f - lane.Creature.DEX / 100f;
			}
		}
		float num2 = 1f - num;
		float num3 = (float)currentChance / 100f / num2;
		return (int)(num3 * 100f);
	}

	public void UpdateCardState()
	{
		CardProgress instance = CardProgress.Instance;
		switch (instance.State)
		{
		case CardState.Init:
			ProcessCardPlay(instance.Card, instance.LaneIndex, instance.TargetPlayer);
			instance.State = CardState.Buffs;
			break;
		case CardState.Buffs:
			ProcessCardStatusEffects(instance.Card, instance.LaneIndex, instance.TargetPlayer, StatusType.Buff);
			instance.State = CardState.AttackInit1;
			break;
		case CardState.AttackInit1:
			if (instance.Card.MaxConditionalAttacks() > 0)
			{
				instance.AttacksTaken = 0;
				CreatureState baseTarget = null;
				if (instance.LaneIndex >= 0)
				{
					baseTarget = Lanes[instance.LaneIndex].Creature;
				}
				instance.Attackers = GetCreatureRange(this, baseTarget, instance.Card.TargetGroup);
				instance.AttackerIdx = 0;
				int num = 0;
				while (num < instance.Attackers.Count)
				{
					baseTarget = instance.Attackers[num];
					if (baseTarget.StatusEffects.Contains((StatusState m) => m.PreventAttack()))
					{
						instance.Attackers.Remove(baseTarget);
					}
					else
					{
						num++;
					}
				}
				instance.State = CardState.AttackInit2;
			}
			else
			{
				instance.State = CardState.Debuffs;
			}
			break;
		case CardState.AttackInit2:
			if (instance.AttackerIdx < instance.Attackers.Count)
			{
				CreatureState creatureState3 = instance.Attackers[instance.AttackerIdx];
				if (instance.AttacksTaken == 0)
				{
					int multiplier = GetMultiplier(instance.Card, "AttackNum", creatureState3);
					AttackMessage(creatureState3, GameEvent.ATTACK_START);
					CreatureState target2 = GetTarget(creatureState3, instance.Card.AttackBase, instance.Card.Target2Group);
					if (target2 != null)
					{
						instance.NumAttacks = instance.Card.NumberOfAttacks.GetConditionalValue(new CardConditionData(Type, !Type, Game, creatureState3.Lane.Index, target2.Lane.Index), instance.Card) * multiplier;
					}
					else
					{
						instance.NumAttacks = instance.Card.MaxConditionalAttacks() * multiplier;
					}
				}
				if (instance.AttacksTaken < instance.NumAttacks && creatureState3.HP > 0 && !creatureState3.IsFrozen)
				{
					CreatureState creatureState4 = GetTarget(creatureState3, instance.Card.AttackBase, instance.Card.Target2Group);
					instance.Targets = GetCreatureRange(Opponent, creatureState4, instance.Card.Target2Group);
					instance.TargetIdx = 0;
					if (instance.Targets.Count > 0)
					{
						AttackMessage(creatureState3, GameEvent.ATTACK_HIT_START);
						foreach (StatusState statusEffect in creatureState3.StatusEffects)
						{
							if (statusEffect.AutoMiss())
							{
								creatureState3.AutoMiss = true;
							}
							if (statusEffect.AttackRandom())
							{
								creatureState4 = GetRandomTarget(creatureState3);
							}
						}
						foreach (StatusState statusEffect2 in creatureState4.StatusEffects)
						{
							if (statusEffect2.Dodge(AttackCause.Card))
							{
								creatureState3.AutoMiss = true;
							}
						}
						if (instance.Targets.Count > 1)
						{
							AttackMessage(creatureState3, GameEvent.AREA_ATTACK_START);
						}
						instance.AttacksTaken++;
						instance.State = CardState.Attacks;
					}
					else
					{
						AttackMessage(creatureState3, GameEvent.ATTACK_END);
						instance.State = CardState.Meta;
					}
				}
				else
				{
					AttackMessage(creatureState3, GameEvent.ATTACK_END);
					instance.AttacksTaken = 0;
					instance.AttackerIdx++;
				}
			}
			else
			{
				instance.State = CardState.Meta;
			}
			break;
		case CardState.Attacks:
		{
			if (instance.TargetIdx < instance.Targets.Count)
			{
				CreatureState creatureState = instance.Attackers[instance.AttackerIdx];
				CreatureState target = instance.Targets[instance.TargetIdx];
				bool flag = creatureState.Attack(target, instance.Card.AttackBase, AttackCause.Card, instance.Card, -1f);
				instance.TargetIdx++;
				break;
			}
			CreatureState creatureState2 = instance.Attackers[instance.AttackerIdx];
			creatureState2.AutoMiss = false;
			if (instance.Targets.Count > 1)
			{
				AttackMessage(creatureState2, GameEvent.AREA_ATTACK_END);
			}
			AttackMessage(creatureState2, GameEvent.ATTACK_HIT_END);
			instance.State = CardState.AttackInit2;
			break;
		}
		case CardState.Debuffs:
			ProcessCardStatusEffects(instance.Card, instance.LaneIndex, instance.TargetPlayer, StatusType.Debuff);
			instance.State = CardState.Meta;
			break;
		case CardState.Meta:
			ProcessCardMeta(instance.Card, instance.LaneIndex, instance.TargetPlayer);
			instance.State = CardState.Idle;
			break;
		case CardState.Intro:
			break;
		}
	}

	public void UpdateAttackState()
	{
		if (AttackProgress.Attacks.Count == 0)
		{
			return;
		}
		AttackProgress attackProgress = AttackProgress.Attacks[0];
		CreatureState attacker = attackProgress.Attacker;
		if (attackProgress.AttacksTaken == 0)
		{
			SpendActionPoints(attacker.AttackCost);
			AttackMessage(attacker, GameEvent.ATTACK_START);
		}
		if (attackProgress.TargetIndex != -1 && attackProgress.AttacksTaken < attackProgress.NumAttacks && attacker.HP > 0)
		{
			CreatureState creatureState = Opponent.Lanes[attackProgress.TargetIndex].Creature;
			if (creatureState == null && attackProgress.AttacksTaken > 0 && attackProgress.TargetIndex > 0)
			{
				creatureState = Opponent.Lanes[attackProgress.TargetIndex - 1].Creature;
			}
			foreach (StatusState statusEffect in attacker.StatusEffects)
			{
				if (statusEffect.AttackRandom())
				{
					creatureState = GetRandomTarget(attacker);
				}
			}
			if (creatureState != null)
			{
				foreach (StatusState statusEffect2 in attacker.StatusEffects)
				{
					if (statusEffect2.AutoMiss())
					{
						attacker.AutoMiss = true;
					}
				}
				foreach (StatusState statusEffect3 in creatureState.StatusEffects)
				{
					if (statusEffect3.Dodge(attackProgress.Cause))
					{
						attacker.AutoMiss = true;
					}
				}
				AttackMessage(attacker, GameEvent.ATTACK_HIT_START);
				attacker.Attack(creatureState, AttackBase.STR, attackProgress.Cause, null, -1f);
				attacker.AutoMiss = false;
				AttackMessage(attacker, GameEvent.ATTACK_HIT_END);
			}
			attackProgress.AttacksTaken++;
			return;
		}
		AttackMessage(attacker, GameEvent.ATTACK_END);
		if (!attacker.StatusEffects.Contains((StatusState m) => m.BlockDraw()))
		{
			int num = 1;
			foreach (StatusState statusEffect4 in attacker.StatusEffects)
			{
				num += statusEffect4.GetExtraCardCount();
			}
			for (int i = 0; i < num; i++)
			{
				attacker.DrawCard();
			}
		}
		if (attacker.AttackCost > 0)
		{
			CheckZeroAP();
		}
		AttackProgress.Attacks.RemoveAt(0);
	}

	public void ProcessCardPlay(CardData Card, int LaneIndex, PlayerType targetPlayer)
	{
		LaneState targetLane = GetTargetLane(Card, LaneIndex, targetPlayer);
		SpendActionPoints(Card.Cost);
		FillAPMeter(Card.Cost);
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.CARD_PLAYED;
		gameMessage.WhichPlayer = this;
		gameMessage.Card = Card;
		gameMessage.Lane = targetLane;
		gameMessage.Creature = ((targetLane == null) ? null : targetLane.Creature);
		Game.AddMessage(gameMessage);
		PlayedCards.Add(Card);
		DiscardFromHand(Card);
		if ((int)Card.Cost > 0)
		{
			CheckZeroAP();
		}
	}

	private int GetMultiplier(CardData Card, string key, CreatureState Target)
	{
		EffectMultiplier multiplier = Card.GetMultiplier(key);
		return GetCounter(multiplier, Target);
	}

	public void ProcessCardStatusEffects(CardData Card, int LaneIndex, PlayerType TargetPlayer, StatusType statusType)
	{
		LaneState targetLane = GetTargetLane(Card, LaneIndex, TargetPlayer);
		CreatureState creatureState = null;
		if (targetLane != null)
		{
			creatureState = targetLane.Creature;
		}
		foreach (StatusData key in Card.StatusValues.Keys)
		{
			int multiplier = GetMultiplier(Card, key.ID, creatureState);
			StatusInfo statusInfo = Card.StatusValues[key];
			if (key.StatusType == statusType)
			{
				float conditionalValue = statusInfo.Value.GetConditionalValue(new CardConditionData(Type, TargetPlayer, Game, LaneIndex), Card);
				ApplyStatus(targetLane, Card.TargetGroup, key, conditionalValue * (float)multiplier, Card);
				if (creatureState != null && creatureState.HP <= 0)
				{
					break;
				}
			}
		}
	}

	public void ProcessCardMeta(CardData Card, int LaneIndex, PlayerType TargetPlayer)
	{
		LaneState targetLane = GetTargetLane(Card, LaneIndex, TargetPlayer);
		CreatureState creatureState = null;
		if (targetLane != null)
		{
			creatureState = targetLane.Creature;
		}
		int multiplier = GetMultiplier(Card, "GainAP", creatureState);
		AddActionPoints(Card.GainAP.GetConditionalValue(new CardConditionData(Type, TargetPlayer, Game, LaneIndex), Card) * multiplier);
		multiplier = GetMultiplier(Card, "DrainAP", creatureState);
		Opponent.LoseActionPoints(Card.DrainAP.GetConditionalValue(new CardConditionData(Type, TargetPlayer, Game, LaneIndex), Card) * multiplier);
		int num = (int)((float)Opponent.ActionPoints * Card.SuckAP.GetConditionalValue(new CardConditionData(Type, TargetPlayer, Game, LaneIndex), Card) / 100f);
		multiplier = GetMultiplier(Card, "SuckAP", creatureState);
		Opponent.LoseActionPoints(num * multiplier);
		AddActionPoints(num * multiplier);
		Opponent.PullCreatures(Card.PullCreatures.GetConditionalValue(new CardConditionData(Type, TargetPlayer, Game, LaneIndex), Card));
		if (targetLane != null)
		{
			multiplier = GetMultiplier(Card, "DrawCards", creatureState);
			List<CreatureState> creatureRange = GetCreatureRange(creatureState.Owner, creatureState, Card.TargetGroup);
			int conditionalValue = Card.DrawCards.GetConditionalValue(new CardConditionData(Type, TargetPlayer, Game, LaneIndex), Card);
			foreach (CreatureState item in creatureRange)
			{
				for (int i = 0; i < conditionalValue * multiplier; i++)
				{
					item.DrawCard();
				}
			}
		}
		Statistics.mCardsPlayed++;
		if (Card.IsLeaderCard)
		{
			Statistics.mHeroCardsPlayed++;
		}
		CheckFullAPMeter();
	}

	public void TickStatusDurations(PlayerState WhichPlayer)
	{
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null)
			{
				lane.Creature.TickStatusDuration(WhichPlayer);
			}
		}
	}

	public void EndTurn()
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.END_TURN;
		gameMessage.WhichPlayer = this;
		gameMessage.Amount = ActionPoints;
		Game.AddMessage(gameMessage);
		if ((int)CurrentActionPoints < MiscParams.MaxActionPoints)
		{
			CurrentPvpTimeLimit = (int)CurrentPvpTimeLimit + MiscParams.MultiplayerTimeLimitPerTurn;
		}
		SpendActionPoints(ActionPoints);
		if (Game.IsDeployment)
		{
			if (DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy != -1)
			{
				CurrentActionPoints = DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy;
			}
		}
		else if (DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy != -1)
		{
			CurrentActionPoints = DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy;
		}
		else
		{
			CurrentActionPoints = Math.Min(MiscParams.MaxActionPoints, (int)CurrentActionPoints + MiscParams.ActionPointIncrement);
		}
		EffectiveCurrentActionPoints = Mathf.Min(CurrentActionPoints, MiscParams.MaxActionPoints);
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature == null)
			{
				continue;
			}
			foreach (StatusState statusEffect in lane.Creature.StatusEffects)
			{
				EffectiveCurrentActionPoints = (int)EffectiveCurrentActionPoints + statusEffect.BonusEnergy();
			}
		}
		AddActionPoints(EffectiveCurrentActionPoints);
		foreach (LaneState lane2 in Lanes)
		{
			if (lane2.Creature != null)
			{
				lane2.Creature.EndTurn();
			}
		}
	}

	public void RefillActionPoints()
	{
		SpendActionPoints(ActionPoints);
		AddActionPoints(EffectiveCurrentActionPoints);
	}

	public void RemoveCreature(LaneState Lane)
	{
		if (Lane.Creature != null)
		{
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.CREATURE_REMOVED;
			gameMessage.WhichPlayer = this;
			gameMessage.Lane = Lane;
			gameMessage.Creature = Lane.Creature;
			Lane.Creature = null;
			Game.AddMessage(gameMessage);
			UpdateStatistics();
		}
	}

	public void PlaceCreature(CreatureState Creature, LaneState Lane)
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.CREATURE_PLACED;
		gameMessage.WhichPlayer = this;
		gameMessage.Lane = Lane;
		gameMessage.Creature = Creature;
		Lane.Creature = Creature;
		Lane.Creature.Lane = Lane;
		Game.AddMessage(gameMessage);
		UpdateStatistics();
	}

	public void ResurectCreature(CreatureState Creature, float HPPct)
	{
		if (!Graveyard.Contains(Creature))
		{
		}
		PlaceCreature(Creature, Creature.Lane);
		Creature.Reset(HPPct);
		Graveyard.Remove(Creature);
	}

	public void ResurectRandomCreature(float HPPct)
	{
		if (Graveyard.Count > 0)
		{
			int index = KFFRandom.RandomIndex(Graveyard.Count);
			CreatureState creature = Graveyard[index];
			ResurectCreature(creature, HPPct);
		}
	}

	public void PlaceCreatureInGraveyard(CreatureState State)
	{
		Graveyard.Add(State);
	}

	public CreatureState GetCreature(int LaneIndex)
	{
		if (LaneIndex < 0)
		{
			return null;
		}
		if (LaneIndex >= Lanes.Count)
		{
			return null;
		}
		LaneState laneState = Lanes[LaneIndex];
		return laneState.Creature;
	}

	public CreatureState GetCreature(CreatureItem Item)
	{
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null && lane.Creature.Data == Item)
			{
				return lane.Creature;
			}
		}
		return null;
	}

	public List<CreatureState> GetCreatures()
	{
		List<CreatureState> list = new List<CreatureState>();
		foreach (LaneState lane in Lanes)
		{
			if (lane.Creature != null)
			{
				list.Add(lane.Creature);
			}
		}
		return list;
	}

	public LaneState GetLaneState(int LaneIndex)
	{
		return Lanes[LaneIndex];
	}

	public void PlaceCardInHand(CardData item)
	{
		if (Hand.Count < 10)
		{
			Hand.Add(item);
			return;
		}
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.FAILED_DRAW_HAND_FULL;
		gameMessage.WhichPlayer = this;
		gameMessage.Card = item;
		Game.AddMessage(gameMessage);
		DiscardCard(item);
	}

	public void RemoveCardFromHand(CardData item)
	{
		if (Hand.Contains(item))
		{
			Hand.Remove(item);
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.DROP_CARD;
			gameMessage.WhichPlayer = this;
			gameMessage.Card = item;
			Game.AddMessage(gameMessage);
		}
	}

	public void DiscardCard(CardData item)
	{
		if (item != null)
		{
			DiscardPile.Insert(0, item);
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.DISCARD_CARD;
			gameMessage.WhichPlayer = this;
			gameMessage.Card = item;
			Game.AddMessage(gameMessage);
			UpdateStatistics();
		}
	}

	public void DiscardFromHand(CardData Card)
	{
		RemoveCardFromHand(Card);
	}

	public void DiscardHand()
	{
		while (Hand.Count > 0)
		{
			CardData card = Hand[0];
			DiscardFromHand(card);
		}
	}

	public void MoveCardFromDiscardToDraw()
	{
		CardData item = DiscardPile[0];
		DiscardPile.RemoveAt(0);
		DrawPile.Add(item);
	}

	public int GetCreatureCount()
	{
		return Statistics.mCreatureCount;
	}

	public int GetDeadCreatureCount()
	{
		return Statistics.mDeadCreatureCount;
	}

	public bool HasCreatureInPlay(CreatureState creatureState)
	{
		return Lanes.Contains((LaneState m) => m.Creature == creatureState);
	}
}
