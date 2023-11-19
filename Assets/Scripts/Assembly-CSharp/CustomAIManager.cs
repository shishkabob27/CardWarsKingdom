using System;
using System.Collections.Generic;
using System.Reflection;

public class CustomAIManager : DetachedSingleton<CustomAIManager>
{
	public class ParsedRow
	{
		public CustomAIData.Row Data;

		public int RunCount;

		public List<ParsedCondition> Conditions = new List<ParsedCondition>();

		public List<ParsedAction> Actions = new List<ParsedAction>();

		public static ParsedRow Parse(CustomAIData.Row row)
		{
			ParsedRow parsedRow = new ParsedRow();
			parsedRow.Data = row;
			foreach (CustomAIData.FunctionCall condition in row.Conditions)
			{
				ParsedCondition parsedCondition = ParsedCondition.Parse(condition);
				if (parsedCondition != null)
				{
					parsedRow.Conditions.Add(parsedCondition);
				}
			}
			foreach (CustomAIData.FunctionCall action in row.Actions)
			{
				ParsedAction parsedAction = ParsedAction.ParseType(action);
				if (parsedAction != null)
				{
					parsedRow.Actions.Add(parsedAction);
				}
			}
			return parsedRow;
		}

		public PlanNode TryToExecute(PlanNode node)
		{
			bool flag = false;
			bool flag2 = true;
			foreach (ParsedAction action in Actions)
			{
				PlanNode nextNode;
				if (action.TryToExecute(node, out nextNode))
				{
					node = nextNode;
					flag = true;
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				OnExecuted();
			}
			return node;
		}

		public bool CheckConditions(BoardState state)
		{
			if (RunCount > 0 && Data.RepeatCooldown == -99)
			{
				return false;
			}
			foreach (ParsedCondition condition in Conditions)
			{
				if (!condition.CheckCondition(state))
				{
					return false;
				}
			}
			return true;
		}

		public void OnExecuted()
		{
			RunCount++;
		}

		public bool AtGameStart()
		{
			bool flag = false;
			bool flag2 = false;
			foreach (ParsedAction action in Actions)
			{
				if (action.AtGameStart)
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
			}
			if (flag && flag2)
			{
				return false;
			}
			return flag;
		}

		public void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
			foreach (ParsedAction action in Actions)
			{
				action.AddForbids(conditionPassed, creatures, cards, creatureTargets);
			}
		}

		public void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
			foreach (ParsedAction action in Actions)
			{
				action.ApplySpecialRules(state, playerTurn);
			}
		}
	}

	public class ParsedCondition
	{
		private MethodInfo mMethodInfo;

		private object[] mParams;

		private bool mNegated;

		public static ParsedCondition Parse(CustomAIData.FunctionCall data)
		{
			ParsedCondition parsedCondition = new ParsedCondition();
			parsedCondition.mMethodInfo = typeof(CustomAIScripts).GetMethod(data.FunctionName);
			if (parsedCondition.mMethodInfo == null)
			{
				return null;
			}
			if (parsedCondition.mMethodInfo.ReturnType != typeof(bool))
			{
				return null;
			}
			ParameterInfo[] parameters = parsedCondition.mMethodInfo.GetParameters();
			if (parameters.Length - 1 != data.Parameters.Count)
			{
				return null;
			}
			parsedCondition.mParams = new object[data.Parameters.Count + 1];
			for (int i = 0; i < data.Parameters.Count; i++)
			{
				try
				{
					parsedCondition.mParams[i + 1] = Convert.ChangeType(data.Parameters[i], parameters[i + 1].ParameterType);
				}
				catch (Exception)
				{
				}
				if (parsedCondition.mParams[i + 1] == null)
				{
					return null;
				}
			}
			parsedCondition.mNegated = data.Negated;
			return parsedCondition;
		}

		public bool CheckCondition(BoardState state)
		{
			mParams[0] = state;
			bool flag = (bool)mMethodInfo.Invoke(null, mParams);
			return flag != mNegated;
		}
	}

	public abstract class ParsedAction
	{
		public virtual bool AtGameStart
		{
			get
			{
				return false;
			}
		}

		public static ParsedAction ParseType(CustomAIData.FunctionCall action)
		{
			//Discarded unreachable code: IL_0038, IL_0045
			Type type = Type.GetType("CustomAIScripts+" + action.FunctionName);
			if (type == null)
			{
				return null;
			}
			ParsedAction parsedAction = (ParsedAction)Activator.CreateInstance(type);
			try
			{
				parsedAction.Parse(action);
				return parsedAction;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public virtual void Parse(CustomAIData.FunctionCall action)
		{
		}

		public virtual bool TryToExecute(PlanNode node, out PlanNode nextNode)
		{
			nextNode = null;
			return false;
		}

		public virtual void AddForbids(bool conditionPassed, List<string> creatures, List<string> cards, List<string> creatureTargets)
		{
		}

		public virtual void ApplySpecialRules(BoardState state, PlayerType playerTurn)
		{
		}
	}

	private List<ParsedRow> mParsedRows;

	private Dictionary<string, object> mForbiddenCreatures = new Dictionary<string, object>();

	private Dictionary<string, object> mForbiddenCards = new Dictionary<string, object>();

	private Dictionary<string, object> mForbiddenCreatureTargets = new Dictionary<string, object>();

	private List<CustomAIScripts.Dialog> mDialogToShow = new List<CustomAIScripts.Dialog>();

	public int OverrideEnergy;

	public int OverrideHandSize;

	public int NoCardDrawForPlayer;

	public int OnlyMagicAttacksDamageForPlayer;

	public int OnlyCritsDamageForPlayer;

	public int SwapAttackStatsForPlayer;

	public int OverrideDragAttackCost;

	public int OverrideDragAttackCostForPlayer;

	public bool CheckNoCardDrawForPlayer(PlayerType player)
	{
		return NoCardDrawForPlayer == 2 || NoCardDrawForPlayer == (int)player;
	}

	public bool CheckOnlyMagicAttacksDamageForPlayer(PlayerType player)
	{
		return OnlyMagicAttacksDamageForPlayer == 2 || OnlyMagicAttacksDamageForPlayer == (int)player;
	}

	public bool CheckOnlyCritsDamageForPlayer(PlayerType player)
	{
		return OnlyCritsDamageForPlayer == 2 || OnlyCritsDamageForPlayer == (int)player;
	}

	public bool CheckSwapAttackStatsForPlayer(PlayerType player)
	{
		return SwapAttackStatsForPlayer == 2 || SwapAttackStatsForPlayer == (int)player;
	}

	public bool CheckOverrideDragAttackCostForPlayer(PlayerType player)
	{
		return OverrideDragAttackCostForPlayer == 2 || OverrideDragAttackCostForPlayer == (int)player;
	}

	private void ResetSpecialRules(bool forNewGame)
	{
		NoCardDrawForPlayer = -1;
		OnlyMagicAttacksDamageForPlayer = -1;
		OnlyCritsDamageForPlayer = -1;
		SwapAttackStatsForPlayer = -1;
		OverrideDragAttackCostForPlayer = -1;
		if (forNewGame)
		{
			OverrideEnergy = -1;
			OverrideHandSize = -1;
		}
	}

	public void ParseAIData(CustomAIData data)
	{
		ResetSpecialRules(true);
		mParsedRows = new List<ParsedRow>();
		if (data == null)
		{
			return;
		}
		foreach (CustomAIData.Row row in data.Rows)
		{
			ParsedRow item = ParsedRow.Parse(row);
			mParsedRows.Add(item);
		}
	}

	public List<ParsedRow> GetCustomActionsAtGameStart()
	{
		List<ParsedRow> list = new List<ParsedRow>();
		foreach (ParsedRow mParsedRow in mParsedRows)
		{
			if (mParsedRow.AtGameStart())
			{
				list.Add(mParsedRow);
			}
		}
		return list;
	}

	public void SetSpecialRulesThisTurn(BoardState state, PlayerType playerTurn)
	{
		ResetSpecialRules(false);
		foreach (ParsedRow mParsedRow in mParsedRows)
		{
			if (mParsedRow.CheckConditions(state))
			{
				mParsedRow.ApplySpecialRules(state, playerTurn);
			}
		}
	}

	public List<ParsedRow> ReadCustomActionsThisTurn(BoardState state)
	{
		mForbiddenCreatures.Clear();
		mForbiddenCards.Clear();
		mForbiddenCreatureTargets.Clear();
		List<ParsedRow> list = new List<ParsedRow>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		foreach (ParsedRow mParsedRow in mParsedRows)
		{
			bool flag = mParsedRow.CheckConditions(state);
			mParsedRow.AddForbids(flag, list2, list3, list4);
			if (flag)
			{
				list.Add(mParsedRow);
			}
		}
		foreach (string item in list2)
		{
			if (!mForbiddenCreatures.ContainsKey(item))
			{
				mForbiddenCreatures.Add(item, null);
			}
		}
		foreach (string item2 in list3)
		{
			if (!mForbiddenCards.ContainsKey(item2))
			{
				mForbiddenCards.Add(item2, null);
			}
		}
		foreach (string item3 in list4)
		{
			if (!mForbiddenCreatureTargets.ContainsKey(item3))
			{
				mForbiddenCreatureTargets.Add(item3, null);
			}
		}
		return list;
	}

	public bool IsCreatureForbidden(CreatureItem creature)
	{
		return mForbiddenCreatures.ContainsKey(creature.Form.ID);
	}

	public bool IsCardForbidden(CardData card)
	{
		return mForbiddenCards.ContainsKey(card.ID);
	}

	public bool IsCreatureTargetForbidden(CreatureItem creature)
	{
		return mForbiddenCreatureTargets.ContainsKey(creature.Form.ID);
	}

	public void AddDialogToShow(CustomAIScripts.Dialog dialog)
	{
		mDialogToShow.Add(dialog);
	}

	public CustomAIScripts.Dialog PopDialog()
	{
		if (mDialogToShow.Count == 0)
		{
			return null;
		}
		CustomAIScripts.Dialog result = mDialogToShow[0];
		mDialogToShow.RemoveAt(0);
		return result;
	}
}
