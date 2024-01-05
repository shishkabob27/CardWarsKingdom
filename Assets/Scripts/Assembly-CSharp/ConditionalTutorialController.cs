using System;
using System.Collections.Generic;
using System.Globalization;

public class ConditionalTutorialController : DetachedSingleton<ConditionalTutorialController>
{
	private class Conditional
	{
		public TutorialState State;

		public bool Triggered;
	}

	private const float LowHealthThreshold = 0.5f;

	private const float LeaderBarThreshold = 0.5f;

	private const int NearFullHandThreshold = 7;

	private TutorialDataManager.TutorialBlock mActiveConditionalBlock;

	private List<Conditional> mConditionals = new List<Conditional>();

	private List<CreatureFaction> mAttackedWithTypesThisTurn = new List<CreatureFaction>();

	private List<CreatureFaction> mAttackedWithTypesLastTurn = new List<CreatureFaction>();

	public void StartConditionalBlock(TutorialDataManager.TutorialBlock block)
	{
		mActiveConditionalBlock = block;
		mConditionals.Clear();
		TutorialState data = TutorialDataManager.Instance.GetData(block.StartState);
		while (data != null && data.Block == block.ID)
		{
			if (data.ConditionType != 0)
			{
				Conditional conditional = new Conditional();
				conditional.State = data;
				mConditionals.Add(conditional);
			}
			data = TutorialDataManager.Instance.GetData(data.Index + 1);
		}
	}

	public void EndConditionalBlock()
	{
		mActiveConditionalBlock = null;
		mConditionals.Clear();
	}

	private List<Conditional> GetConditionals(TutorialState.ConditionTypeEnum type)
	{
		return mConditionals.FindAll((Conditional m) => (!m.Triggered || m.State.Repeat) && m.State.ConditionType == type);
	}

	private bool CanEnterConditionalState(TutorialState conditionalState)
	{
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		int num = 0;
		int num2 = 0;
		TutorialState nextState = conditionalState;
		do
		{
			if (nextState.Target == TutorialState.TargetEnum.CreatureAttack)
			{
				if (nextState.TargetId(0) == "Cheapest")
				{
					int num3 = -1;
					foreach (CreatureState creature in playerState.GetCreatures())
					{
						if (num3 == -1 || creature.AttackCost < num3)
						{
							num3 = creature.AttackCost;
						}
					}
					num2 += num3;
				}
				else
				{
					CreatureState creatureState = playerState.GetCreatures().Find((CreatureState m) => m.Data.Form.Faction.ClassName() == nextState.TargetId(0));
					if (creatureState == null)
					{
						return false;
					}
					num2 += creatureState.AttackCost;
				}
			}
			else if (nextState.Target == TutorialState.TargetEnum.CardPlay)
			{
				if (nextState.TargetId(0) != "Any")
				{
					if (playerState.Hand.Find((CardData m) => m.ID == nextState.TargetId(0)) == null)
					{
						return false;
					}
					CardData data = CardDataManager.Instance.GetData(nextState.TargetId(0));
					num2 += (int)data.Cost;
				}
				if (nextState.TargetId(1) != null)
				{
					if (nextState.TargetId(1) == "Debuffed")
					{
						CreatureState creatureState2 = playerState.GetCreatures().Find((CreatureState m) => m.HasDebuff);
						if (creatureState2 == null)
						{
							return false;
						}
					}
					else
					{
						CreatureState creatureState3 = playerState.GetCreatures().Find((CreatureState m) => m.Data.Form.Faction.ClassName() == nextState.TargetId(1));
						if (creatureState3 == null)
						{
							return false;
						}
					}
				}
			}
			num += nextState.ForceActions.FindAll((TutorialState.ForceAction m) => m.Action == TutorialState.ForceActionEnum.Draw).Count;
			nextState = TutorialDataManager.Instance.GetData(nextState.Index + 1);
		}
		while (nextState != null && nextState.ConditionType == TutorialState.ConditionTypeEnum.None && !(nextState.Block != conditionalState.Block));
		if (num + playerState.Hand.Count > 10)
		{
			return false;
		}
		if (num2 > playerState.ActionPoints)
		{
			return false;
		}
		return true;
	}

	public void OnTurnStart()
	{
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		CheckLeaderBar();
		List<CreatureState> creatures = playerState.GetCreatures();
		CheckCardCombos();
		if (mAttackedWithTypesThisTurn.Count == 1 && mAttackedWithTypesLastTurn.Count == 1 && mAttackedWithTypesThisTurn[0] == mAttackedWithTypesLastTurn[0])
		{
			CheckCondition(TutorialState.ConditionTypeEnum.CreatureOverused);
		}
		mAttackedWithTypesLastTurn = mAttackedWithTypesThisTurn.GetRange(0, mAttackedWithTypesThisTurn.Count);
		mAttackedWithTypesThisTurn.Clear();
		if (playerState.Hand.Count == 0 && creatures.Count > 0)
		{
			CheckCondition(TutorialState.ConditionTypeEnum.HandEmpty);
		}
		else if (playerState.Hand.Count >= 7)
		{
			CheckCondition(TutorialState.ConditionTypeEnum.HandAlmostFull);
		}
	}

	public void OnCreatureAttack(CreatureState creature)
	{
		if (!mAttackedWithTypesThisTurn.Contains(creature.Data.Form.Faction))
		{
			mAttackedWithTypesThisTurn.Add(creature.Data.Form.Faction);
		}
		CheckCondition(TutorialState.ConditionTypeEnum.AttackWith, (string m) => creature.Data.Form.Faction.ClassName() == m);
	}

	public void OnCreatureDied()
	{
		CheckCondition(TutorialState.ConditionTypeEnum.CreatureDied);
	}

	public void OnCardDrawn(CardData card)
	{
		CheckCardCombos();
		CheckCardOnStatus(card);
	}

	public void OnGainedStatusEffect(CreatureState creature, StatusData status)
	{
		CheckCardOnStatus(creature, status);
		CheckStatusCombos(creature, status);
	}

	public void OnGainedLandscape(CreatureState creature, StatusData status)
	{
		CheckCardOnLandscape(creature, status);
		CheckStatusCombos(creature, status);
	}

	public void OnAPMeterFull()
	{
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.HeroCard);
		foreach (Conditional item in conditionals)
		{
			Singleton<TutorialController>.Instance.manulSetAutoFillMeterBar = false;
			EnterConditionalState(item);
		}
	}

	public void OnLeaderBarChange(float meter)
	{
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.LeaderBar);
		foreach (Conditional item in conditionals)
		{
			if (meter >= float.Parse(item.State.ConditionIds[0], CultureInfo.InvariantCulture))
			{
				Singleton<TutorialController>.Instance.manulSetAutoFillMeterBar = true;
				EnterConditionalState(item);
			}
		}
	}

	public void OnLootDropped()
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			Singleton<PlayerInfoScript>.Instance.StateData.ActiveConditionalState = null;
		}
		CheckCondition(TutorialState.ConditionTypeEnum.CreatureDrop);
	}

	private void CheckCondition(TutorialState.ConditionTypeEnum type, Predicate<string> conditionIdMatch = null)
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		List<Conditional> conditionals = GetConditionals(type);
		foreach (Conditional item in conditionals)
		{
			if (conditionIdMatch == null)
			{
				if (CanEnterConditionalState(item.State))
				{
					EnterConditionalState(item);
					break;
				}
				continue;
			}
			string[] conditionIds = item.State.ConditionIds;
			foreach (string text in conditionIds)
			{
				if (text != null && conditionIdMatch(text) && CanEnterConditionalState(item.State))
				{
					EnterConditionalState(item);
					return;
				}
			}
		}
	}

	private void CheckCardCombos()
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.KeywordCombo);
		foreach (Conditional item in conditionals)
		{
			CardPrefabScript foundCard;
			StatusIconItem foundStatusIcon;
			CardPrefabScript foundCard2;
			StatusIconItem foundStatusIcon2;
			if (Singleton<DWGame>.Instance.FindKeywordForTutorial(item.State.ConditionIds[0], out foundCard, out foundStatusIcon) && Singleton<DWGame>.Instance.FindKeywordForTutorial(item.State.ConditionIds[1], out foundCard2, out foundStatusIcon2) && (!(foundCard == null) || !(foundCard2 == null)))
			{
				int num = 0;
				if (foundCard != null)
				{
					num += (int)foundCard.Card.Cost;
				}
				if (foundCard2 != null)
				{
					num += (int)foundCard2.Card.Cost;
				}
				if (num <= playerState.ActionPoints && CanEnterConditionalState(item.State))
				{
					EnterConditionalState(item);
					return;
				}
			}
		}
		conditionals = GetConditionals(TutorialState.ConditionTypeEnum.CardCombo);
		CardData card1;
		foreach (CardData item2 in playerState.Hand)
		{
			card1 = item2;
			List<Conditional> list = conditionals.FindAll((Conditional m) => m.State.ConditionIds[0] == card1.ID);
			foreach (Conditional item3 in list)
			{
				foreach (CardData item4 in playerState.Hand)
				{
					int num2 = (int)card1.Cost + (int)item4.Cost;
					if (num2 <= playerState.ActionPoints && item3.State.ConditionIds[1] == item4.ID && CanEnterConditionalState(item3.State))
					{
						EnterConditionalState(item3);
						return;
					}
				}
			}
		}
	}

	private void CheckCardOnStatus(CardData card)
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.CardOnStatus);
		conditionals = conditionals.FindAll((Conditional m) => m.State.ConditionIds[0] == card.ID);
		foreach (Conditional item in conditionals)
		{
			if (CanEnterConditionalState(item.State))
			{
				EnterConditionalState(item);
				return;
			}
		}
		foreach (Conditional item2 in conditionals)
		{
			foreach (CreatureState creature in playerState.GetCreatures())
			{
				foreach (StatusState statusEffect in creature.StatusEffects)
				{
					if (statusEffect.Data.FXData.Keyword.ID == item2.State.ConditionIds[1] && CanEnterConditionalState(item2.State))
					{
						EnterConditionalState(item2);
						return;
					}
				}
			}
		}
		conditionals = GetConditionals(TutorialState.ConditionTypeEnum.StackableEffects);
		foreach (Conditional item3 in conditionals)
		{
			foreach (CreatureState creature2 in playerState.GetCreatures())
			{
				StatusState status;
				foreach (StatusState statusEffect2 in creature2.StatusEffects)
				{
					status = statusEffect2;
					if (card.DescriptionKeywords.Find((KeyWordData m) => m.ID == status.Data.FXData.Keyword.ID) != null && CanEnterConditionalState(item3.State))
					{
						Singleton<TutorialController>.Instance.SetWildcardID(card.ID);
						EnterConditionalState(item3);
						return;
					}
				}
			}
		}
	}

	private void CheckCardOnStatus(CreatureState creature, StatusData status)
	{
		if (Singleton<TutorialController>.Instance.manulSetAutoFillMeterBar || Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.CardOnStatus);
		conditionals = conditionals.FindAll((Conditional m) => m.State.ConditionIds[1] == status.FXData.Keyword.ID || status.StatusType != StatusType.None);
		foreach (Conditional item in conditionals)
		{
			foreach (CardData item2 in playerState.Hand)
			{
				foreach (KeyValuePair<StatusData, StatusInfo> statusValue in item2.StatusValues)
				{
				}
				bool flag = false;
				switch (item.State.ConditionIds[0])
				{
				case "All":
					foreach (KeyValuePair<StatusData, StatusInfo> statusValue2 in item2.StatusValues)
					{
						if (statusValue2.Key.StatusType != 0)
						{
							flag = true;
						}
					}
					break;
				case "Buff":
					foreach (KeyValuePair<StatusData, StatusInfo> statusValue3 in item2.StatusValues)
					{
						if (statusValue3.Key.StatusType != StatusType.Buff)
						{
							flag = true;
						}
					}
					break;
				case "Debuff":
					foreach (KeyValuePair<StatusData, StatusInfo> statusValue4 in item2.StatusValues)
					{
						if (statusValue4.Key.StatusType != StatusType.Debuff)
						{
							flag = true;
						}
					}
					break;
				}
				if ((item2.ID == item.State.ConditionIds[1] || flag) && CanEnterConditionalState(item.State))
				{
					EnterConditionalState(item);
					return;
				}
			}
		}
		conditionals = GetConditionals(TutorialState.ConditionTypeEnum.StackableEffects);
		foreach (Conditional item3 in conditionals)
		{
			foreach (CardData item4 in playerState.Hand)
			{
				if (item4.DescriptionKeywords.Find((KeyWordData m) => m.ID == status.FXData.Keyword.ID) != null && CanEnterConditionalState(item3.State))
				{
					Singleton<TutorialController>.Instance.SetWildcardID(item4.ID);
					EnterConditionalState(item3);
					return;
				}
			}
		}
	}

	private void CheckCardOnLandscape(CreatureState creature, StatusData status)
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.CardOnLandscape);
		foreach (Conditional item in conditionals)
		{
			EnterConditionalState(item);
		}
	}

	private void CheckStatusCombos(CreatureState creature, StatusData gainedStatus)
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.PlayedCombo);
		conditionals = conditionals.FindAll((Conditional m) => m.State.ConditionIds[0] == gainedStatus.FXData.Keyword.ID || m.State.ConditionIds[1] == gainedStatus.FXData.Keyword.ID);
		foreach (Conditional item in conditionals)
		{
			int num = ((item.State.ConditionIds[0] == gainedStatus.FXData.Keyword.ID) ? 1 : 0);
			foreach (StatusState statusEffect in creature.StatusEffects)
			{
				if (item.State.ConditionIds[num] == statusEffect.Data.FXData.Keyword.ID && CanEnterConditionalState(item.State))
				{
					Singleton<TutorialController>.Instance.SetWildcardID(creature.Data.Form.ID);
					EnterConditionalState(item);
					return;
				}
			}
		}
	}

	private void CheckLeaderBar()
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		float num = (float)(int)playerState.APMeter / (float)playerState.Leader.Form.APThreshold;
		if (num > 0.5f)
		{
			CardData cardData = null;
			foreach (CardData item in playerState.Hand)
			{
				if ((int)item.Cost > 0 && Singleton<DWGame>.Instance.CanPlay(PlayerType.User, item) == PlayerState.CanPlayResult.CanPlay)
				{
					cardData = item;
					break;
				}
			}
			if (cardData != null)
			{
				List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.CanFillLeaderBar);
				foreach (Conditional item2 in conditionals)
				{
					if (CanEnterConditionalState(item2.State))
					{
						Singleton<TutorialController>.Instance.SetWildcardID(cardData.ID);
						EnterConditionalState(item2);
						return;
					}
				}
			}
		}
		OnLeaderBarChange(num);
	}

	public bool CheckDidNothing()
	{
		if (Singleton<TutorialController>.Instance.InConditionalState())
		{
			return false;
		}
		PlayerState playerState = Singleton<DWGame>.Instance.CurrentBoardState.GetPlayerState(PlayerType.User);
		if (playerState.ActionPoints < (int)playerState.EffectiveCurrentActionPoints)
		{
			return false;
		}
		if (playerState.Statistics.mCardsPlayed > 0)
		{
			return false;
		}
		List<Conditional> conditionals = GetConditionals(TutorialState.ConditionTypeEnum.DidNothing);
		foreach (Conditional item in conditionals)
		{
			if (CanEnterConditionalState(item.State))
			{
				EnterConditionalState(item);
				return true;
			}
		}
		return false;
	}

	private void EnterConditionalState(Conditional conditional)
	{
		conditional.Triggered = true;
		Singleton<TutorialController>.Instance.EnterConditionalState(conditional.State);
	}
}
