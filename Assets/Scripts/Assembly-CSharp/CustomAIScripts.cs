using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CustomAIScripts
{
	public class PlayCreature : CustomAIManager.ParsedAction
	{
		private enum PositionEnum
		{
			Right,
			Center,
			Left
		}

		private string CreatureID;

		private PositionEnum Position;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			CreatureID = action.GetParam(0);
			string param = action.GetParam(1, "Right");
			Position = (PositionEnum)(int)Enum.Parse(typeof(PositionEnum), param);
		}

		public override void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
			if (!conditionPassed)
			{
				creatures.Add(CreatureID);
			}
		}

		public override bool TryToExecute(PlanNode node, out PlanNode nextNode)
		{
			nextNode = node;
			CreatureState creatureState = node.State.GetPlayerState(PlayerType.Opponent).DeploymentList.Find((CreatureState m) => m.Data.Form.ID == CreatureID);
			if (creatureState == null)
			{
				return false;
			}
			if (!node.State.GetPlayerState(PlayerType.Opponent).CanDeploy(creatureState.Data, true))
			{
				return false;
			}
			int creatureCount = node.State.GetCreatureCount(PlayerType.Opponent);
			int laneIndex = 0;
			switch (Position)
			{
			case PositionEnum.Left:
				laneIndex = 0;
				break;
			case PositionEnum.Right:
				laneIndex = creatureCount;
				break;
			case PositionEnum.Center:
				laneIndex = (creatureCount + 1) / 2;
				break;
			}
			nextNode = Singleton<AIManager>.Instance.PlanDeployCreature(PlayerType.Opponent, node, creatureState.Data, laneIndex);
			node.Children.Add(nextNode);
			return true;
		}
	}

	public class PlayCard : CustomAIManager.ParsedAction
	{
		private string CardID;

		private string OnCreatureID;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			CardID = action.GetParam(0);
			OnCreatureID = action.GetParam(1);
		}

		public override void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
			if (!conditionPassed)
			{
				creatures.Add(CardID);
			}
		}

		public override bool TryToExecute(PlanNode node, out PlanNode nextNode)
		{
			nextNode = node;
			LaneState laneState = node.State.GetLanes(PlayerType.Opponent).Find((LaneState m) => m != null && m.Creature != null && m.Creature.Data.Form.ID == OnCreatureID);
			if (laneState == null)
			{
				return false;
			}
			CardData data = CardDataManager.Instance.GetData(CardID);
			if (node.State.GetPlayerState(PlayerType.Opponent).CanPlay(data, PlayerType.Opponent, laneState.Index, true) != 0)
			{
				return false;
			}
			nextNode = Singleton<AIManager>.Instance.PlanCard(PlayerType.Opponent, node, data, PlayerType.Opponent, laneState.Index, -1);
			node.Children.Add(nextNode);
			return true;
		}
	}

	public class StartWithCard : CustomAIManager.ParsedAction
	{
		public CardData Card;

		public override bool AtGameStart
		{
			get
			{
				return true;
			}
		}

		public override void Parse(CustomAIData.FunctionCall action)
		{
			string param = action.GetParam(0);
			Card = CardDataManager.Instance.GetData(param);
			if (Card == null)
			{
				throw new Exception("card " + param + " not found");
			}
		}
	}

	public class ForbidPlayCard : CustomAIManager.ParsedAction
	{
		private string CardID;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			CardID = action.GetParam(0);
		}

		public override void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
			if (conditionPassed)
			{
				cards.Add(CardID);
			}
		}
	}

	public class ForbidPlayCreature : CustomAIManager.ParsedAction
	{
		private string CreatureID;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			CreatureID = action.GetParam(0);
		}

		public override void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
			if (conditionPassed)
			{
				creatures.Add(CreatureID);
			}
		}
	}

	public class ForbidAllCreatureAction : CustomAIManager.ParsedAction
	{
		private string CreatureID;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			CreatureID = action.GetParam(0);
		}

		public override void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
			if (conditionPassed)
			{
				creatureTargets.Add(CreatureID);
			}
		}
	}

	public class Dialog : CustomAIManager.ParsedAction
	{
		public string Text;

		public float Time;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Text = action.GetParam(0);
			Time = Convert.ToSingle(action.GetParam(1), CultureInfo.InvariantCulture);
		}

		public override bool TryToExecute(PlanNode node, out PlanNode nextNode)
		{
			nextNode = node;
			DetachedSingleton<CustomAIManager>.Instance.AddDialogToShow(this);
			return true;
		}
	}

	public class OnlyCritsDamage : CustomAIManager.ParsedAction
	{
		private int Player;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Player = ParsePlayerString(action.GetParam(0, string.Empty));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.OnlyCritsDamageForPlayer = Player;
		}
	}

	public class RandomEnergy : CustomAIManager.ParsedAction
	{
		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			bool flag = false;
			if ((!state.IsDeployment) ? (playerTurn == PlayerType.User) : (DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy == -1))
			{
				DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy = UnityEngine.Random.Range(MiscParams.StartingActionPoints, MiscParams.MaxActionPoints + 1);
			}
		}
	}

	public class ForceEnergy : CustomAIManager.ParsedAction
	{
		public int Energy;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Energy = Convert.ToInt32(action.GetParam(0));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.OverrideEnergy = Energy;
		}
	}

	public class StartingHandSize : CustomAIManager.ParsedAction
	{
		public int HandSize;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			HandSize = Convert.ToInt32(action.GetParam(0));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.OverrideHandSize = HandSize;
		}
	}

	public class NoCardDraw : CustomAIManager.ParsedAction
	{
		private int Player;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Player = ParsePlayerString(action.GetParam(0, string.Empty));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.NoCardDrawForPlayer = Player;
		}
	}

	public class OnlyMagicAttacksDamage : CustomAIManager.ParsedAction
	{
		private int Player;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Player = ParsePlayerString(action.GetParam(0, string.Empty));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.OnlyMagicAttacksDamageForPlayer = Player;
		}
	}

	public class SwapAttackStats : CustomAIManager.ParsedAction
	{
		private int Player;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Player = ParsePlayerString(action.GetParam(0, string.Empty));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.SwapAttackStatsForPlayer = Player;
		}
	}

	public class OverrideDragAttackCost : CustomAIManager.ParsedAction
	{
		private int Cost;

		private int Player;

		public override void Parse(CustomAIData.FunctionCall action)
		{
			Cost = Convert.ToInt32(action.GetParam(0));
			Player = ParsePlayerString(action.GetParam(1, string.Empty));
		}

		public override void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			DetachedSingleton<CustomAIManager>.Instance.OverrideDragAttackCost = Cost;
			DetachedSingleton<CustomAIManager>.Instance.OverrideDragAttackCostForPlayer = Player;
		}
	}

	public const int BothPlayers = 2;

	public static bool OnOrAfterTurn(BoardState state, int turnNum)
	{
		return state.TurnNumber >= turnNum;
	}

	public static bool OnEvenNumberedTurn(BoardState state)
	{
		return state.TurnNumber % 2 == 0;
	}

	public static bool OnOddNumberedTurn(BoardState state)
	{
		return state.TurnNumber % 2 == 1;
	}

	public static bool LostCreatures(BoardState state, int creatures)
	{
		return state.GetPlayerState(PlayerType.Opponent).GetDeadCreatureCount() >= creatures;
	}

	public static bool CreaturesLeftInHand(BoardState state, int creatures)
	{
		return state.GetPlayerState(PlayerType.Opponent).DeploymentList.Count <= creatures;
	}

	public static bool CreatureBelowHPPercent(BoardState state, string creatureID, float hpPercent)
	{
		PlayerState playerState = state.GetPlayerState(PlayerType.Opponent);
		LaneState laneState = playerState.Lanes.Find((LaneState m) => m.Creature != null && m.Creature.Data.Form.ID == creatureID);
		if (laneState == null)
		{
			return false;
		}
		return laneState.Creature.HPPct < hpPercent / 100f;
	}

	public static bool CreatureAboveHPPercent(BoardState state, string creatureID, float hpPercent)
	{
		PlayerState playerState = state.GetPlayerState(PlayerType.Opponent);
		LaneState laneState = playerState.Lanes.Find((LaneState m) => m.Creature != null && m.Creature.Data.Form.ID == creatureID);
		if (laneState == null)
		{
			return false;
		}
		return laneState.Creature.HPPct > hpPercent / 100f;
	}

	public static bool CreatureMissingStatus(BoardState state, string creatureID, string statusID)
	{
		PlayerState playerState = state.GetPlayerState(PlayerType.Opponent);
		LaneState laneState = playerState.Lanes.Find((LaneState m) => m.Creature != null && m.Creature.Data.Form.ID == creatureID);
		if (laneState == null)
		{
			return false;
		}
		return !laneState.Creature.StatusEffects.Contains((StatusState m) => m.Data.ID == statusID);
	}

	public static bool HasCreature(BoardState state, string creatureID)
	{
		PlayerState playerState = state.GetPlayerState(PlayerType.Opponent);
		return playerState.Lanes.Contains((LaneState m) => m.Creature != null && m.Creature.Data.Form.ID == creatureID);
	}

	private static int ParsePlayerString(string playerString)
	{
		string text = playerString.ToLower();
		if (text == "user")
		{
			return 0;
		}
		if (text == "opponent")
		{
			return 1;
		}
		return 2;
	}
}
