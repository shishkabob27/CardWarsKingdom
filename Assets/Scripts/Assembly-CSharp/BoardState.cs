using System;
using System.Collections.Generic;
using System.Reflection;

public class BoardState
{
	public const int MAX_PLAYERS = 2;

	private List<PlayerState> PlayerStates = new List<PlayerState>();

	private List<PlayerState> OrderedPlayerStates = new List<PlayerState>();

	private List<LaneState> LaneList = new List<LaneState>();

	private List<GameMessage> MessageQueue = new List<GameMessage>();

	private List<GameMessage> ProcessedList = new List<GameMessage>();

	private GameMessage LastMessage;

	private bool FirstTurn;

	private bool FirstTurnAfterRevive;

	private int PlayerTurnsTaken;

	public bool IsDeployment;

	public PlayerState WhoseTurn;

	public int TurnNumber
	{
		get
		{
			return PlayerTurnsTaken / 2;
		}
	}

	public bool IsFirstTurn()
	{
		return FirstTurn;
	}

	public void ClearFirstTurnFlag()
	{
		FirstTurn = false;
	}

	public bool IsFirstTurnAfterRevive()
	{
		return FirstTurnAfterRevive;
	}

	public static BoardState Create()
	{
		BoardState boardState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(BoardState)) as BoardState;
		boardState.Init();
		return boardState;
	}

	private static BoardState Create(BoardState Source)
	{
		BoardState boardState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(BoardState)) as BoardState;
		boardState.Init(Source);
		return boardState;
	}

	public static void Destroy(BoardState State)
	{
		State.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(State);
	}

	public void Init()
	{
		FirstTurn = true;
		IsDeployment = true;
		PlayerTurnsTaken = 0;
		AddPlayers();
	}

	public void Init(BoardState Source)
	{
		PlayerState playerState = Source.PlayerStates[PlayerType.User].DeepCopy();
		PlayerState playerState2 = Source.PlayerStates[PlayerType.Opponent].DeepCopy();
		FirstTurn = Source.FirstTurn;
		IsDeployment = Source.IsDeployment;
		PlayerTurnsTaken = Source.PlayerTurnsTaken;
		if (Source.WhoseTurn.Type == PlayerType.User)
		{
			WhoseTurn = playerState;
		}
		else
		{
			WhoseTurn = playerState2;
		}
		AddPlayers(playerState, playerState2);
	}

	public BoardState DeepCopy()
	{
		return Create(this);
	}

	public void Clean()
	{
		LaneList.Clear();
		MessageQueue.Clear();
		ProcessedList.Clear();
		LastMessage = null;
		FirstTurn = false;
		IsDeployment = false;
		PlayerTurnsTaken = 0;
		foreach (PlayerState orderedPlayerState in OrderedPlayerStates)
		{
			PlayerState.Destroy(orderedPlayerState);
		}
		PlayerStates.Clear();
		OrderedPlayerStates.Clear();
	}

	private void AddPlayers(PlayerState User = null, PlayerState Opponent = null)
	{
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && !Singleton<PlayerInfoScript>.Instance.PvPData.AmIPrimary)
		{
			if (Opponent == null)
			{
				Opponent = PlayerState.Create(PlayerType.Opponent);
			}
			if (User == null)
			{
				User = PlayerState.Create(PlayerType.User);
			}
			OrderedPlayerStates.Add(Opponent);
			OrderedPlayerStates.Add(User);
		}
		else
		{
			if (User == null)
			{
				User = PlayerState.Create(PlayerType.User);
			}
			if (Opponent == null)
			{
				Opponent = PlayerState.Create(PlayerType.Opponent);
			}
			OrderedPlayerStates.Add(User);
			OrderedPlayerStates.Add(Opponent);
		}
		User.Game = this;
		Opponent.Game = this;
		foreach (LaneState lane in User.Lanes)
		{
			int index = MiscParams.CreaturesOnBoard - lane.Index - 1;
			LaneState laneState2 = (lane.OpponentLane = Opponent.Lanes[index]);
			laneState2.OpponentLane = lane;
			LaneList.Add(lane);
			LaneList.Add(laneState2);
		}
		PlayerStates.Add(User);
		PlayerStates.Add(Opponent);
	}

	public void InitPlayer(PlayerType idx, Loadout loadout, bool reviving)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.Init(loadout, reviving);
		FirstTurnAfterRevive = reviving;
	}

	public void Setup()
	{
		foreach (PlayerState orderedPlayerState in OrderedPlayerStates)
		{
			orderedPlayerState.Setup();
		}
		MessageQueue.Clear();
	}

	public void Reset()
	{
		foreach (PlayerState orderedPlayerState in OrderedPlayerStates)
		{
			orderedPlayerState.Reset();
		}
	}

	public void StartTurn(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.StartTurn();
		WhoseTurn = playerState;
		playerState.TickStatusDurations(playerState);
		playerState.Opponent.TickStatusDurations(playerState);
	}

	public void AddMessage(GameMessage message)
	{
		message.Parent = LastMessage;
		if (LastMessage != null)
		{
			LastMessage.Children.Add(message);
		}
		MessageQueue.Add(message);
	}

	public void RemoveMessage(GameMessage message)
	{
		MessageQueue.Remove(message);
		if (message.Parent != null)
		{
			message.Parent.Children.Remove(message);
		}
	}

	public bool IsQueueEmpty()
	{
		return MessageQueue.Count <= 0;
	}

	public List<GameMessage> GetMessages(Predicate<GameMessage> p)
	{
		return ProcessedList.FindAll(p);
	}

	public GameMessage PopMessage()
	{
		LastMessage = null;
		if (MessageQueue.Count > 0)
		{
			LastMessage = MessageQueue[0];
			MessageQueue.RemoveAt(0);
			ProcessedList.Add(LastMessage);
		}
		return LastMessage;
	}

	public void ClearProcessedMessageList()
	{
		ProcessedList.Clear();
	}

	public void ProcessMessage(GameMessage Message)
	{
		foreach (PlayerState orderedPlayerState in OrderedPlayerStates)
		{
			orderedPlayerState.ProcessMessage(Message);
		}
		if (Message.Action == GameEvent.CREATURE_DIED && !Message.Creature.Owner.HasCreatureInPlay(Message.Creature))
		{
			Message.Creature.ProcessMessage(Message);
		}
	}

	public void RecordMessages()
	{
		foreach (GameMessage item in MessageQueue)
		{
			DetachedSingleton<MissionManager>.Instance.ProcessMessage(item);
		}
	}

	public List<GameMessage> ProcessMessages()
	{
		GameMessage gameMessage = PopMessage();
		GameMessage gameMessage2 = gameMessage;
		List<GameMessage> list = new List<GameMessage>();
		while (gameMessage2 != null)
		{
			list.Add(gameMessage2);
			ProcessMessage(gameMessage2);
			gameMessage2 = PopMessage();
		}
		return list;
	}

	public bool CanDeploy(PlayerType idx, CreatureItem Item)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.CanDeploy(Item);
	}

	public PlayerState.CanPlayResult CanPlay(PlayerType idx, CardData card, PlayerType targetPlayer = null, int LaneIndex = -1)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.CanPlay(card, targetPlayer, LaneIndex);
	}

	public bool HasLegalPlay(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.HasLegalPlay();
	}

	public bool CanPlayCard(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.CanPlayCard();
	}

	public List<LaneState> GetOccupiedLanes()
	{
		return LaneList.FindAll((LaneState m) => m.Creature != null);
	}

	public void PlayActionCard(PlayerType idx, CardData Card, PlayerType targetPlayer, int LaneIndex)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.PlayActionCard(idx, Card, targetPlayer, LaneIndex);
	}

	public void UpdateActionCard(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.UpdateCardState();
	}

	public void UpdateAttackState(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.UpdateAttackState();
	}

	public List<LaneState> GetFirstTargetList(PlayerType idx, CardData source)
	{
		List<LaneState> list = new List<LaneState>(LaneList);
		MethodInfo[] targetFilters = source.TargetFilters1;
		foreach (MethodInfo filter in targetFilters)
		{
			list = TargetFilters.FilterLaneList(idx, list, filter);
		}
		return list;
	}

	public List<LaneState> GetSecondTargetList(PlayerType idx, CardData source)
	{
		List<LaneState> list = new List<LaneState>(LaneList);
		MethodInfo[] targetFilters = source.TargetFilters2;
		foreach (MethodInfo filter in targetFilters)
		{
			list = TargetFilters.FilterLaneList(idx, list, filter);
		}
		return list;
	}

	public CreatureState DeployCreature(PlayerType idx, CreatureItem item, int LaneIndex)
	{
		if (idx.ToString() == "User")
		{
			Singleton<DWGame>.Instance.turnActions.Add(DWGame.TurnActions.PlayMonster);
		}
		PlayerState playerState = PlayerStates[idx];
		return playerState.DeployCreature(item, LaneIndex);
	}

	public void DragAttack(PlayerType idx, int AttackLane, int TargetLane)
	{
		if (idx.ToString() == "User")
		{
			Singleton<DWGame>.Instance.turnActions.Add(DWGame.TurnActions.Attack);
		}
		PlayerState playerState = PlayerStates[idx];
		playerState.AttackAndDraw(AttackLane, TargetLane, AttackCause.Drag);
	}

	public void EndTurn(PlayerType idx, bool reviving = false)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.EndTurn();
		if (!reviving)
		{
			FirstTurnAfterRevive = false;
		}
		if (FirstTurn)
		{
			FirstTurn = false;
		}
		else
		{
			IsDeployment = false;
		}
		PlayerTurnsTaken++;
	}

	public int GetCreatureCount(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.GetCreatureCount();
	}

	public int ScoreCreatureItem(CreatureItem Item)
	{
		float num = Item.STR;
		num += num * ((float)MiscParams.CriticalPercent / 100f) * ((float)Item.DEX / 100f);
		num *= (float)(MiscParams.MaxActionPoints / (int)Item.Form.AttackCost);
		float num2 = (float)(Item.DEF + Item.RES) / 2f;
		if (num2 > 99f)
		{
			num2 = 99f;
		}
		float num3 = 100f / (100f - num2);
		num += (float)Item.HP * num3;
		return (int)num;
	}

	public int ScoreBoardSide(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		PlayerState playerState2 = PlayerStates[!idx];
		int num = 0;
		foreach (LaneState lane in playerState.Lanes)
		{
			num += lane.Score();
		}
		foreach (CreatureState deployment in playerState.DeploymentList)
		{
			num += ScoreCreatureItem(deployment.Data);
		}
		num += playerState.DamageDealt;
		int creatureCount = GetCreatureCount(idx);
		int creatureCount2 = GetCreatureCount(!idx);
		if (creatureCount < creatureCount2)
		{
			num = (int)((float)num * 0.1f);
		}
		num += playerState2.GetDeadCreatureCount() * 100000;
		if (creatureCount2 == 0 && !IsDeployment)
		{
			num += 100000000;
		}
		return num;
	}

	public int ScoreEntireBoard(PlayerType WhichPlayer)
	{
		int num = ScoreBoardSide(WhichPlayer);
		int num2 = ScoreBoardSide(!WhichPlayer);
		return num - num2;
	}

	public bool IsGameOver()
	{
		if (IsDeployment)
		{
			return false;
		}
		if (PlayerStates[PlayerType.User].Statistics.mCreatureCount == 0 || PlayerStates[PlayerType.Opponent].Statistics.mCreatureCount == 0)
		{
			return true;
		}
		return false;
	}

	public List<LaneState> GetLanes(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.Lanes;
	}

	public LaneState GetLaneState(PlayerType idx, int LaneIndex)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.GetLaneState(LaneIndex);
	}

	public void SetTarget(PlayerType idx, CreatureState Target)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.SetTarget(Target);
	}

	public void ClearTarget(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.SetTarget(null);
	}

	public PlayerState GetPlayerState(PlayerType idx)
	{
		return PlayerStates[idx];
	}

	public CreatureState GetCreature(PlayerType idx, int LaneIndex)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.GetCreature(LaneIndex);
	}

	public List<CreatureState> GetCreatures(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.GetCreatures();
	}

	public List<CardData> GetHand(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.Hand;
	}

	public List<CreatureState> GetDeployList(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.DeploymentList;
	}

	public List<CardData> GetDiscardPile(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.DiscardPile;
	}

	public List<CardData> GetDrawPile(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.DrawPile;
	}

	public LeaderItem GetLeader(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.Leader;
	}

	public int GetHandCount(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.Hand.Count;
	}

	public int GetActionPoints(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.ActionPoints;
	}

	public bool IsDrawPileEmpty(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.DrawPile.Count <= 0;
	}

	public bool IsDiscardPileEmpty(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		return playerState.DiscardPile.Count <= 0;
	}

	public void RemoveCardFromHand(PlayerType idx, CardData item)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.RemoveCardFromHand(item);
	}

	public void DiscardCard(PlayerType idx, CardData item)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.ManualDiscard(item);
	}

	public void DiscardHand(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.DiscardHand();
	}

	public void MoveCardFromDiscardToDraw(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.MoveCardFromDiscardToDraw();
	}

	public void Reshuffle(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.ShuffleDrawPile();
	}

	public void DrawHeroCard(PlayerType idx)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.DrawHeroCard();
	}

	public void DrawCreatureCard(PlayerType idx, int LaneIndex)
	{
		PlayerState playerState = PlayerStates[idx];
		playerState.DrawCreatureCard(LaneIndex);
	}
}
