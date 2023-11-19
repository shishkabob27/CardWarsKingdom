using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager>
{
	public const float MAX_THINKING_TIME = 5f;

	public const int MAX_PLANS = 512;

	public const int MAX_DEPTH = 25;

	private KFFPoolManager PM;

	private AIState State;

	private PlanNode Root;

	private int CreatureIndex;

	private float YIELD_TIME = 0.008f;

	public bool DisableCreatureMovement;

	private float lastYieldTime;

	private float planningStartTime;

	private List<PlanNode> Plans = new List<PlanNode>();

	private List<AIDecision> DecisionQueue = new List<AIDecision>();

	private AISubState SubState;

	private StatusData BraveryStatus;

	private StatusData RemoveStatusEffect;

	private StatusData ReviveStatus;

	private StatusData HealingStatus;

	private StatusData VampiricStatus;

	private StatusData RegenStatus;

	private StatusData TransmogStatus;

	private StatusData FreezeStatus;

	private StatusData ParalyzeStatus;

	private StatusData RevengeStatus;

	private bool mInitialized;

	private int PlanCount;

	public AIDecision Decision { get; set; }

	public IEnumerator CreatePool()
	{
		PM = DetachedSingleton<KFFPoolManager>.Instance;
		PM.CreatePool(typeof(PlanNode), 1000);
		yield return null;
		PM.CreatePool(typeof(AIDecision), 1000);
		yield return null;
		PM.CreatePool(typeof(BoardState), 1000);
		yield return null;
		PM.CreatePool(typeof(PlayerState), 2000);
		yield return null;
		PM.CreatePool(typeof(LaneState), 8000);
		yield return null;
		PM.CreatePool(typeof(CreatureState), 10000);
		yield return null;
	}

	private void OnDestroy()
	{
		if (PM != null)
		{
			PM.DestroyAllPools();
		}
	}

	public void StartPlanning(PlayerType player)
	{
		Singleton<DWGame>.Instance.ProcessMessages();
		Root = PlanNode.Create(null);
		Root.State = Singleton<DWGame>.Instance.CurrentBoardState.DeepCopy();
		Root.State.GetHand(player).Sort((CardData x, CardData y) => ((int)x.Cost).CompareTo(y.Cost));
		State = AIState.Thinking;
		StartCoroutine(Plan(player, Root, true, true));
	}

	public bool IsPlanning()
	{
		return State == AIState.Thinking;
	}

	public AIDecision MakeDecision()
	{
		Decision = null;
		if (DecisionQueue.Count > 0)
		{
			Decision = DecisionQueue[0];
			DecisionQueue.RemoveAt(0);
			if (Decision.EndTurn)
			{
				PlanNode.Destroy(Root);
				Root = null;
				DetachedSingleton<KFFPoolManager>.Instance.CheckUnreleasedObjects();
				State = AIState.Waiting;
			}
		}
		return Decision;
	}

	public void ResetCreatureIndex()
	{
		CreatureIndex = 0;
	}

	public AIDecision GetAIDecision()
	{
		return MakeDecision();
	}

	private void Update()
	{
		if (!mInitialized && SessionManager.Instance.IsLoadDataDone())
		{
			BraveryStatus = StatusDataManager.Instance.GetData("Bravery");
			RemoveStatusEffect = StatusDataManager.Instance.GetData("RemoveStatus");
			ReviveStatus = StatusDataManager.Instance.GetData("AutoRevive");
			HealingStatus = StatusDataManager.Instance.GetData("DirectHealing");
			VampiricStatus = StatusDataManager.Instance.GetData("Vampiric");
			RegenStatus = StatusDataManager.Instance.GetData("Regen");
			TransmogStatus = StatusDataManager.Instance.GetData("Transmogrify");
			FreezeStatus = StatusDataManager.Instance.GetData("Frozen");
			ParalyzeStatus = StatusDataManager.Instance.GetData("Paralyze");
			RevengeStatus = StatusDataManager.Instance.GetData("Revenge");
			mInitialized = true;
		}
	}

	public void ResetYield()
	{
		lastYieldTime = Time.realtimeSinceStartup;
	}

	public bool ShouldYield()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		return realtimeSinceStartup - lastYieldTime > YIELD_TIME;
	}

	private bool TimeOut()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		return realtimeSinceStartup - planningStartTime > 5f;
	}

	public PlanNode PlanDeployCreature(PlayerType WhichPlayer, PlanNode Node, CreatureItem Item, int LaneIndex)
	{
		PlanNode planNode = PlanNode.Create(Node);
		planNode.Decision.Creature = Item;
		planNode.Decision.LaneIndex1 = LaneIndex;
		planNode.Decision.IsDeploy = true;
		planNode.Decision.Seed = KFFRandom.Seed;
		planNode.State.DeployCreature(WhichPlayer, Item, LaneIndex);
		planNode.Execute();
		return planNode;
	}

	public PlanNode PlanCard(PlayerType WhichPlayer, PlanNode Node, CardData Card, PlayerType TargetPlayer, int LaneIndex1, int LaneIndex2)
	{
		PlanNode planNode = PlanNode.Create(Node);
		planNode.Decision.Card = Card;
		planNode.Decision.TargetPlayer = TargetPlayer;
		planNode.Decision.LaneIndex1 = LaneIndex1;
		planNode.Decision.LaneIndex2 = LaneIndex2;
		planNode.Decision.Seed = KFFRandom.Seed;
		planNode.State.PlayActionCard(WhichPlayer, Card, TargetPlayer, LaneIndex1);
		while (CardProgress.Instance.State != 0)
		{
			planNode.State.UpdateActionCard(WhichPlayer);
			planNode.Execute();
		}
		return planNode;
	}

	public PlanNode PlanAttack(PlayerType WhichPlayer, PlanNode Node, int LaneIndex, int TargetIndex)
	{
		PlanNode planNode = PlanNode.Create(Node);
		planNode.Decision.LaneIndex1 = LaneIndex;
		planNode.Decision.LaneIndex2 = TargetIndex;
		planNode.Decision.IsAttack = true;
		planNode.Decision.Seed = KFFRandom.Seed;
		planNode.State.DragAttack(WhichPlayer, LaneIndex, TargetIndex);
		while (AttackProgress.AttacksInProgress)
		{
			planNode.State.UpdateAttackState(WhichPlayer);
			planNode.Execute();
		}
		planNode.Execute();
		return planNode;
	}

	private bool ShouldPlayCard(PlayerType WhichPlayer, CardData Card, List<CardData> Hand, LaneState Lane)
	{
		if ((bool)Lane.Creature.IsMindless)
		{
			return false;
		}
		bool flag = Card.MaxConditionalAttacks() > 0;
		if (flag && (bool)Lane.Creature.IsFrozen)
		{
			return false;
		}
		if (Card.AnyPositiveEffects && Lane.Creature.Owner.Type != WhichPlayer)
		{
			return false;
		}
		if (!Card.AnyPositiveEffects && Lane.Creature.Owner.Type == WhichPlayer)
		{
			return false;
		}
		bool result = false;
		if (flag)
		{
			result = true;
		}
		foreach (StatusData status in Card.StatusValues.Keys)
		{
			if (status == ReviveStatus)
			{
				if (Lane.Creature.HasAutoRevive)
				{
					return false;
				}
				result = true;
			}
			else if (status == RevengeStatus)
			{
				if (Lane.Creature.HasRevenge)
				{
					return false;
				}
				result = true;
			}
			else if (status == TransmogStatus || status == FreezeStatus)
			{
				if (Lane.Creature.IsTransfmogrified || (bool)Lane.Creature.IsFrozen)
				{
					return false;
				}
				result = true;
			}
			else if (status == HealingStatus || status == VampiricStatus || status == RegenStatus)
			{
				if (Lane.Creature.Damage > 0)
				{
					result = true;
				}
			}
			else if (status == ParalyzeStatus)
			{
				if (Lane.Creature.Abilities.Count > 0)
				{
					result = true;
				}
			}
			else if (status == RemoveStatusEffect)
			{
				if (Lane.Creature.HasBuff && Card.RemoveStatusData.RandomBuffs)
				{
					result = true;
				}
				if (Lane.Creature.HasDebuff && Card.RemoveStatusData.RandomDebuffs)
				{
					result = true;
				}
				if (Card.RemoveStatusData.Targets == null)
				{
					continue;
				}
				for (int i = 0; i < Card.RemoveStatusData.Targets.Count; i++)
				{
					if (Lane.Creature.StatusEffects.Contains((StatusState m) => m.Data == Card.RemoveStatusData.Targets[i]))
					{
						result = true;
						break;
					}
				}
			}
			else if (status == BraveryStatus)
			{
				if (Lane.Creature.Owner.GetCreatureCount() > 1 && Lane.Owner.Lanes.Find((LaneState m) => m.Creature != null && (bool)m.Creature.HasBravery && m != Lane) == null)
				{
					result = true;
				}
			}
			else if (status.IsLandscape)
			{
				if (!Lane.Creature.StatusEffects.Contains((StatusState m) => m.Data == status))
				{
					result = true;
				}
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	private bool ShouldPlayNonTargetedCard(BoardState Board, PlayerType WhichPlayer, CardData Card)
	{
		PlayerState playerState = Board.GetPlayerState(WhichPlayer);
		if (Card.RemoveStatusData.RandomDebuffs && !playerState.GetCreatures().Contains((CreatureState m) => m.HasDebuff))
		{
			return false;
		}
		if (Card.RemoveStatusData.RandomBuffs && !playerState.GetCreatures().Contains((CreatureState m) => m.HasBuff))
		{
			return false;
		}
		if (Card.RemoveStatusData.Targets != null)
		{
			bool flag = false;
			List<CreatureState> creatures = playerState.GetCreatures();
			for (int i = 0; i < Card.RemoveStatusData.Targets.Count; i++)
			{
				if (creatures.Contains((CreatureState creature) => creature.StatusEffects.Contains((StatusState status) => status.Data == Card.RemoveStatusData.Targets[i])))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	private IEnumerator PlanNextMove(PlayerType WhichPlayer, PlanNode Node)
	{
		bool debugHaltPlanning = false;
		int creatureCount = Node.State.GetCreatureCount(WhichPlayer);
		List<CardData> Hand = Node.State.GetHand(WhichPlayer);
		List<CreatureState> DeployList = Node.State.GetDeployList(WhichPlayer);
		foreach (CreatureState Item in DeployList)
		{
			if (Node.State.CanDeploy(WhichPlayer, Item.Data))
			{
				PlanNode NextNode5 = PlanDeployCreature(WhichPlayer, Node, Item.Data, creatureCount);
				Node.Children.Add(NextNode5);
			}
		}
		if (!debugHaltPlanning)
		{
			PlayerType WhichPlayer2 = default(PlayerType);
			foreach (CardData Card in Hand)
			{
				if (Node.State.GetCreatureCount(WhichPlayer) == 0 || Node.State.IsDeployment)
				{
					continue;
				}
				if (Node.State.CanPlay(WhichPlayer, Card) == PlayerState.CanPlayResult.CanPlay)
				{
					if (Card.TargetType1 == SelectionType.None)
					{
						if (!ShouldPlayNonTargetedCard(Node.State, WhichPlayer, Card))
						{
							continue;
						}
						PlanNode NextNode4 = PlanCard(WhichPlayer, Node, Card, WhichPlayer, -1, -1);
						Node.Children.Add(NextNode4);
					}
					else if (Card.TargetType1 == SelectionType.Lane)
					{
						List<LaneState> FirstList = Node.State.GetFirstTargetList(WhichPlayer, Card);
						FirstList.RemoveAll((LaneState l) => l.Creature != null && l.Creature.Owner.Type == WhichPlayer2 && DetachedSingleton<CustomAIManager>.Instance.IsCreatureTargetForbidden(l.Creature.Data));
						List<int> sortedLaneIndexes = new List<int>(FirstList.Count);
						for (int i = 0; i < FirstList.Count; i++)
						{
							sortedLaneIndexes.Add(i);
						}
						if (Card.StatusValues.ContainsKey(BraveryStatus))
						{
							sortedLaneIndexes.Sort(delegate(int lhs, int rhs)
							{
								float value = (float)FirstList[lhs].Creature.HP * (1f + (FirstList[lhs].Creature.DEF + FirstList[lhs].Creature.RES) / 200f);
								return ((float)FirstList[rhs].Creature.HP * (1f + (FirstList[rhs].Creature.DEF + FirstList[rhs].Creature.RES) / 200f)).CompareTo(value);
							});
						}
						else if (Card.AttackBase != 0)
						{
							sortedLaneIndexes.Sort(delegate(int lhs, int rhs)
							{
								float expectedDamagePerAttack = FirstList[lhs].Creature.GetExpectedDamagePerAttack(Card.AttackBase);
								return FirstList[rhs].Creature.GetExpectedDamagePerAttack(Card.AttackBase).CompareTo(expectedDamagePerAttack);
							});
						}
						foreach (int firstListIndex in sortedLaneIndexes)
						{
							LaneState Lane2 = FirstList[firstListIndex];
							if (!ShouldPlayCard(WhichPlayer, Card, Hand, Lane2))
							{
								continue;
							}
							if (Lane2.Owner.Type == WhichPlayer)
							{
								if (Card.TargetGroup == AttackRange.Triple && creatureCount >= 3 && (Lane2.Index == 0 || Lane2.Index == creatureCount - 1))
								{
									continue;
								}
							}
							else
							{
								int enemyCreatureCount2 = Node.State.GetCreatureCount(!WhichPlayer);
								if (Card.TargetGroup == AttackRange.Triple && enemyCreatureCount2 >= 3 && (Lane2.Index == 0 || Lane2.Index == enemyCreatureCount2 - 1))
								{
									continue;
								}
							}
							if (Card.TargetType2 == SelectionType.None)
							{
								PlanNode NextNode3 = PlanCard(WhichPlayer, Node, Card, Lane2.Owner.Type, Lane2.Index, -1);
								Node.Children.Add(NextNode3);
							}
							else
							{
								if (Card.TargetType2 != SelectionType.Lane)
								{
									continue;
								}
								List<LaneState> SecondList = Node.State.GetSecondTargetList(WhichPlayer, Card);
								foreach (LaneState Lane3 in SecondList)
								{
									if (Lane2 == Lane3)
									{
										continue;
									}
									if (Lane3.Owner.Type == WhichPlayer)
									{
										if (Card.Target2Group == AttackRange.Triple && creatureCount >= 3 && (Lane3.Index == 0 || Lane3.Index == creatureCount - 1))
										{
											continue;
										}
									}
									else
									{
										int enemyCreatureCount = Node.State.GetCreatureCount(!WhichPlayer);
										if (Card.Target2Group == AttackRange.Triple && enemyCreatureCount >= 3 && (Lane3.Index == 0 || Lane3.Index == enemyCreatureCount - 1))
										{
											continue;
										}
									}
									PlanNode NextNode2 = PlanCard(WhichPlayer, Node, Card, Lane2.Owner.Type, Lane2.Index, Lane3.Index);
									Node.Children.Add(NextNode2);
								}
							}
						}
					}
				}
				if (ShouldYield())
				{
					yield return null;
					ResetYield();
				}
			}
		}
		if (!debugHaltPlanning)
		{
			PlayerState playerState = Node.State.GetPlayerState(WhichPlayer);
			foreach (LaneState Lane in playerState.Lanes)
			{
				if (Lane.Creature != null && Lane.Creature.AttackCost <= playerState.ActionPoints && !DetachedSingleton<CustomAIManager>.Instance.IsCreatureTargetForbidden(Lane.Creature.Data))
				{
					CreatureState Target = playerState.GetTarget(Lane.Creature, AttackBase.STR, AttackRange.Single);
					if (!Lane.Creature.IsFrozen)
					{
						PlanNode NextNode = PlanAttack(TargetIndex: (Target == null) ? (-1) : Target.Lane.Index, WhichPlayer: WhichPlayer, Node: Node, LaneIndex: Lane.Index);
						Node.Children.Add(NextNode);
					}
				}
				if (ShouldYield())
				{
					yield return null;
					ResetYield();
				}
			}
		}
		SubState = AISubState.BuildingPlans;
	}

	private PlanNode FinalizePlan(PlayerType WhichPlayer, PlanNode Node)
	{
		PlanNode planNode = PlanNode.Create(Node);
		bool isDeployment = planNode.State.IsDeployment;
		planNode.Decision.EndTurn = true;
		planNode.State.EndTurn(WhichPlayer);
		planNode.Execute();
		planNode.State.StartTurn(!WhichPlayer);
		planNode.Execute();
		if (!isDeployment)
		{
			while (AttackProgress.AttacksInProgress)
			{
				planNode.State.UpdateAttackState(!WhichPlayer);
				planNode.Execute();
			}
			planNode.State.EndTurn(!WhichPlayer);
			planNode.Execute();
			planNode.State.StartTurn(WhichPlayer);
			planNode.Execute();
			while (AttackProgress.AttacksInProgress)
			{
				planNode.State.UpdateAttackState(WhichPlayer);
				planNode.Execute();
			}
		}
		planNode.Score = planNode.State.ScoreEntireBoard(WhichPlayer);
		return planNode;
	}

	private void ClearPlans()
	{
		foreach (PlanNode plan in Plans)
		{
			PlanNode.Destroy(plan);
		}
		Plans.Clear();
	}

	private void AddPlan(PlayerType WhichPlayer, PlanNode Plan, bool TrimList = false)
	{
		if (TrimList && Plans.Count > 0)
		{
			PlayerState playerState = Plan.State.GetPlayerState(WhichPlayer);
			PlayerState playerState2 = Plans[0].State.GetPlayerState(WhichPlayer);
			if (Plan.Score > Plans[0].Score || (Plan.Score == Plans[0].Score && playerState.DamageDealt > playerState2.DamageDealt))
			{
				ClearPlans();
				Plans.Add(Plan);
			}
			else
			{
				PlanNode.Destroy(Plan);
			}
		}
		else
		{
			Plans.Add(Plan);
		}
	}

	private void ProcessTerminalNode(PlayerType WhichPlayer, PlanNode Node, bool TrimList = false)
	{
		PlanNode plan = FinalizePlan(WhichPlayer, Node);
		PlanCount++;
		AddPlan(WhichPlayer, plan, TrimList);
	}

	private bool ValidateHand(PlayerType WhichPlayer, PlanNode Node)
	{
		List<CardData> hand = Node.State.GetHand(WhichPlayer);
		if (Node.State.IsDeployment)
		{
			return false;
		}
		foreach (CardData item in hand)
		{
			if (Node.State.CanPlay(WhichPlayer, item) != 0)
			{
				continue;
			}
			if (item.TargetType1 == SelectionType.None)
			{
				return ShouldPlayNonTargetedCard(Node.State, WhichPlayer, item);
			}
			if (item.TargetType1 != SelectionType.Lane)
			{
				continue;
			}
			List<LaneState> firstTargetList = Node.State.GetFirstTargetList(WhichPlayer, item);
			foreach (LaneState item2 in firstTargetList)
			{
				if (!ShouldPlayCard(WhichPlayer, item, hand, item2))
				{
					continue;
				}
				return true;
			}
		}
		return false;
	}

	private IEnumerator Plan(PlayerType WhichPlayer, PlanNode Root, bool LimitTime, bool TrimList = false)
	{
		List<PlanNode> Stack = new List<PlanNode>();
		PlanCount = 0;
		if (WhichPlayer == PlayerType.Opponent)
		{
			List<CustomAIManager.ParsedRow> customAIActions = DetachedSingleton<CustomAIManager>.Instance.ReadCustomActionsThisTurn(Root.State);
			foreach (CustomAIManager.ParsedRow actionSet in customAIActions)
			{
				Root = actionSet.TryToExecute(Root);
			}
		}
		Stack.Add(Root);
		YIELD_TIME = Time.deltaTime / 2f;
		ResetYield();
		planningStartTime = lastYieldTime;
		SubState = AISubState.BuildingPlans;
		while (Stack.Count > 0)
		{
			PlanNode Node = Stack[0];
			Stack.RemoveAt(0);
			if (Node.Depth < 25 && !Node.State.IsGameOver() && (Node.State.HasLegalPlay(WhichPlayer) || ValidateHand(WhichPlayer, Node)))
			{
				SubState = AISubState.PopulatingChildren;
				StartCoroutine(PlanNextMove(WhichPlayer, Node));
				while (SubState == AISubState.PopulatingChildren)
				{
					yield return null;
				}
				Stack.InsertRange(0, Node.Children);
			}
			else
			{
				ProcessTerminalNode(WhichPlayer, Node, TrimList);
			}
			if (ShouldYield())
			{
				yield return null;
				ResetYield();
			}
			if (LimitTime && PlanCount >= 512)
			{
				break;
			}
		}
		SubState = AISubState.None;
		SelectPlan();
	}

	private void QueueDecisions(PlanNode BestPlan)
	{
		DecisionQueue.Clear();
		while (BestPlan != null && BestPlan.Decision != null)
		{
			DecisionQueue.Insert(0, BestPlan.Decision);
			BestPlan = BestPlan.Parent;
		}
	}

	private void SelectPlan()
	{
		int index = Random.Range(0, Plans.Count);
		PlanNode bestPlan = Plans[index];
		QueueDecisions(bestPlan);
		ClearPlans();
		State = AIState.Executing;
	}
}
