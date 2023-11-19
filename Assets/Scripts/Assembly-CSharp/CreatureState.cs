using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class CreatureState
{
	private ObscuredInt _MaxHP;

	private ObscuredFloat _STR;

	private ObscuredFloat _DEF;

	private ObscuredFloat _INT;

	private ObscuredFloat _RES;

	private ObscuredFloat _DEX;

	private ObscuredInt _Damage;

	private float[] mBaseStats = new float[6];

	public List<CardData> DrawPile = new List<CardData>();

	public List<int> WeightList = new List<int>();

	public List<CardData> DrawQueue = new List<CardData>();

	public List<CardData> DiscardPile = new List<CardData>();

	public int TotalWeight;

	public List<CardData> OverrideDrawPile;

	public List<AbilityState> Abilities = new List<AbilityState>();

	public List<StatusState> StatusEffects = new List<StatusState>();

	public CreatureItem Data { get; set; }

	public PlayerState Owner { get; set; }

	public LaneState Lane { get; set; }

	public CreatureState Opponent
	{
		get
		{
			return Lane.Opponent;
		}
	}

	public int MaxHP
	{
		get
		{
			return _MaxHP;
		}
		set
		{
			_MaxHP = Math.Max(0, value);
		}
	}

	public int Damage
	{
		get
		{
			return _Damage;
		}
		set
		{
			_Damage = Math.Max(0, value);
		}
	}

	public int HP
	{
		get
		{
			return Math.Max(MaxHP - Damage, 0);
		}
	}

	public float HPPct
	{
		get
		{
			return (float)HP / (float)MaxHP;
		}
	}

	public float STR
	{
		get
		{
			return _STR;
		}
		set
		{
			_STR = Math.Max(0f, value);
		}
	}

	public float DEF
	{
		get
		{
			return _DEF;
		}
		set
		{
			_DEF = Math.Max(0f, value);
		}
	}

	public float INT
	{
		get
		{
			return _INT;
		}
		set
		{
			_INT = Math.Max(0f, value);
		}
	}

	public float RES
	{
		get
		{
			return _RES;
		}
		set
		{
			_RES = Math.Max(0f, value);
		}
	}

	public float DEX
	{
		get
		{
			return _DEX;
		}
		set
		{
			_DEX = Math.Max(0f, value);
		}
	}

	public bool AutoMiss { get; set; }

	public int DrawIndex { get; set; }

	public int DragAttacksThisTurn { get; set; }

	public int AttackCount { get; set; }

	public int DamageDealt { get; set; }

	public int AttackCost
	{
		get
		{
			int num = ((!DetachedSingleton<CustomAIManager>.Instance.CheckOverrideDragAttackCostForPlayer(Owner.Type)) ? ((int)Data.Form.AttackCost) : DetachedSingleton<CustomAIManager>.Instance.OverrideDragAttackCost);
			int num2 = 0;
			foreach (StatusState statusEffect in StatusEffects)
			{
				num2 += statusEffect.AttackDiscount();
			}
			num2 += ((Data.QuestLoadoutEntryData != null) ? Data.QuestLoadoutEntryData.AttackCostFactor : 0);
			return Mathf.Max(1, num - num2);
		}
	}

	public ObscuredBool IgnoreDEF { get; set; }

	public ObscuredBool BreakDEF { get; set; }

	public ObscuredBool IgnoreRES { get; set; }

	public ObscuredBool BreakRES { get; set; }

	public ObscuredBool HasBravery { get; set; }

	public ObscuredBool HasImmunity { get; set; }

	public ObscuredBool HasStealth { get; set; }

	public ObscuredBool IsFrozen { get; set; }

	public ObscuredBool IsMindless { get; set; }

	public ObscuredBool IsParalyzed { get; set; }

	public bool HasArmor
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is InflictStatus);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasSTRCounter
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is StrengthCounter);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasINTCounter
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is MagicCounter);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasTransmogrify
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Transmogrify);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsBlind
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Blind);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsTransfmogrified
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Transmogrify);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasShield
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Shield);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsPoisoned
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Poison);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsPlagued
	{
		get
		{
			return StatusEffects.Contains((StatusState s) => s is Plague);
		}
	}

	public bool IsMarked
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Marked);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasPinpoint
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Pinpoint);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasAutoRevive
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is AutoRevive);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasRevenge
	{
		get
		{
			return StatusEffects.Contains((StatusState m) => m is Revenge);
		}
	}

	public bool HasThorns
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Thorns);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsBurned
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Burned);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasRegen
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Regen);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasCardBlock
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is CardBlock);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasSiphon
	{
		get
		{
			return StatusEffects.Contains((StatusState s) => s is APSiphon);
		}
	}

	public bool HasMagicSpike
	{
		get
		{
			return StatusEffects.Contains((StatusState s) => s is BurstINT);
		}
	}

	public bool HasBuff
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s.Data.StatusType == StatusType.Buff);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasDebuff
	{
		get
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s.Data.StatusType == StatusType.Debuff);
			if (statusState != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool HasCripple
	{
		get
		{
			return StatusEffects.Contains((StatusState s) => s is Cripple);
		}
	}

	public bool IsDDImmune
	{
		get
		{
			return StatusEffects.Contains((StatusState s) => s is DirectDamageImmune);
		}
	}

	public bool IsPoisonImmune
	{
		get
		{
			return StatusEffects.Contains((StatusState s) => s is PoisonImmune);
		}
	}

	public bool AtFullHealth
	{
		get
		{
			return HP >= MaxHP;
		}
	}

	public float GetStat(CreatureStat stat)
	{
		switch (stat)
		{
		case CreatureStat.STR:
			return STR;
		case CreatureStat.INT:
			return INT;
		case CreatureStat.DEX:
			return DEX;
		case CreatureStat.DEF:
			return DEF;
		case CreatureStat.RES:
			return RES;
		case CreatureStat.HP:
			return HP;
		default:
			return 0f;
		}
	}

	public static CreatureState Create(PlayerState Player, CreatureItem Item)
	{
		CreatureState creatureState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(CreatureState)) as CreatureState;
		creatureState.Init(Player, Item);
		return creatureState;
	}

	private static CreatureState Create(CreatureState Source)
	{
		CreatureState creatureState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(CreatureState)) as CreatureState;
		creatureState.Init(Source);
		return creatureState;
	}

	public static void Destroy(CreatureState State)
	{
		State.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(State);
	}

	private void CreateAbilities(CreatureItem Item)
	{
		if (Item.PassiveSkillAbility != null && Item.PassiveSkillAbility.ScriptName != string.Empty)
		{
			AbilityState abilityState = AbilityState.Create(Item.PassiveSkillAbility);
			abilityState.Owner = this;
			Abilities.Add(abilityState);
		}
	}

	private void InitStats(CreatureItem Item)
	{
		MaxHP = (int)((float)Data.HP * ((Item.QuestLoadoutEntryData == null) ? 1f : Item.QuestLoadoutEntryData.HPFactor));
		STR = (int)((float)Data.STR * ((Item.QuestLoadoutEntryData == null) ? 1f : Item.QuestLoadoutEntryData.STRFactor));
		INT = (int)((float)Data.INT * ((Item.QuestLoadoutEntryData == null) ? 1f : Item.QuestLoadoutEntryData.INTFactor));
		DEF = Data.DEF + ((Item.QuestLoadoutEntryData != null) ? Item.QuestLoadoutEntryData.DEFFactor : 0);
		RES = Data.RES + ((Item.QuestLoadoutEntryData != null) ? Item.QuestLoadoutEntryData.RESFactor : 0);
		DEX = Data.DEX + ((Item.QuestLoadoutEntryData != null) ? Item.QuestLoadoutEntryData.DEXFactor : 0);
		for (int i = 0; i < 6; i++)
		{
			mBaseStats[i] = GetStat((CreatureStat)i);
		}
	}

	public void Init(PlayerState Player, CreatureItem Item)
	{
		Data = Item;
		InitStats(Item);
		CreateAbilities(Item);
		List<string> currentCardOverrides = Singleton<TutorialController>.Instance.GetCurrentCardOverrides(Player.Type, Item.Form.Faction);
		if (currentCardOverrides != null)
		{
			for (int i = 0; i < currentCardOverrides.Count; i++)
			{
				CardData data = CardDataManager.Instance.GetData(currentCardOverrides[i]);
				int num = 1;
				TotalWeight += num;
				DrawPile.Add(data);
				WeightList.Add(TotalWeight);
			}
		}
		else
		{
			for (int j = 0; j < Item.ActionCards.Count; j++)
			{
				CardData item = Item.ActionCards[j];
				int num2 = Item.Form.ActionCardWeights[j];
				TotalWeight += num2;
				DrawPile.Add(item);
				WeightList.Add(TotalWeight);
			}
			for (int k = 0; k < Item.ExCards.Length; k++)
			{
				InventorySlotItem inventorySlotItem = Item.ExCards[k];
				if (inventorySlotItem != null)
				{
					TotalWeight += MiscParams.ExtensionCardWeight;
					DrawPile.Add(inventorySlotItem.Card.Form);
					WeightList.Add(TotalWeight);
				}
			}
		}
		Owner = Player;
		DrawIndex = 0;
		PopulateDrawQueue(true);
		PopulateDrawQueue();
	}

	public void Init(CreatureState Source)
	{
		Data = Source.Data;
		MaxHP = Source.MaxHP;
		Damage = Source.Damage;
		STR = Source.STR;
		DEF = Source.DEF;
		INT = Source.INT;
		RES = Source.RES;
		DEX = Source.DEX;
		DrawPile.AddRange(Source.DrawPile);
		WeightList.AddRange(Source.WeightList);
		TotalWeight = Source.TotalWeight;
		DrawQueue.AddRange(Source.DrawQueue);
		DiscardPile.AddRange(Source.DiscardPile);
		DrawIndex = Source.DrawIndex;
		AttackCount = Source.AttackCount;
		DamageDealt = Source.DamageDealt;
		IgnoreDEF = Source.IgnoreDEF;
		IgnoreRES = Source.IgnoreRES;
		BreakDEF = Source.BreakDEF;
		BreakRES = Source.BreakRES;
		HasBravery = Source.HasBravery;
		HasImmunity = Source.HasImmunity;
		HasStealth = Source.HasStealth;
		IsFrozen = Source.IsFrozen;
		IsMindless = Source.IsMindless;
		IsParalyzed = Source.IsParalyzed;
		foreach (StatusState statusEffect in Source.StatusEffects)
		{
			StatusState statusState = statusEffect.DeepCopy();
			statusState.Target = this;
			StatusEffects.Add(statusState);
		}
		foreach (AbilityState ability in Source.Abilities)
		{
			AbilityState abilityState = ability.DeepCopy();
			abilityState.Owner = this;
			Abilities.Add(abilityState);
		}
	}

	public void Clean()
	{
		Data = null;
		Owner = null;
		Lane = null;
		MaxHP = 0;
		Damage = 0;
		DrawIndex = 0;
		AttackCount = 0;
		DamageDealt = 0;
		TotalWeight = 0;
		STR = 0f;
		DEF = 0f;
		INT = 0f;
		RES = 0f;
		DEX = 0f;
		DrawPile.Clear();
		WeightList.Clear();
		DrawQueue.Clear();
		DiscardPile.Clear();
		IgnoreDEF = false;
		IgnoreRES = false;
		BreakDEF = false;
		BreakRES = false;
		HasBravery = false;
		HasStealth = false;
		HasImmunity = false;
		IsFrozen = false;
		IsMindless = false;
		IsParalyzed = false;
		AutoMiss = false;
		foreach (StatusState statusEffect in StatusEffects)
		{
			StatusState.Destroy(statusEffect);
		}
		StatusEffects.Clear();
		foreach (AbilityState ability in Abilities)
		{
			AbilityState.Destroy(ability);
		}
		Abilities.Clear();
	}

	public CreatureState DeepCopy()
	{
		return Create(this);
	}

	public float AssessThreat(CreatureState Attacker, AttackBase Base)
	{
		int num = Attacker.PredictDamage(this, Base, true);
		float num2 = (float)num / (float)HP;
		return num2 * (float)AttackCount;
	}

	public float GetAttackOutput()
	{
		return GetExpectedDamagePerAttack(AttackBase.STR) * (float)(MiscParams.MaxActionPoints / (int)Data.Form.AttackCost);
	}

	public float GetExpectedDamagePerAttack(AttackBase attackBase)
	{
		float num = 0f;
		if (attackBase != AttackBase.INT && DetachedSingleton<CustomAIManager>.Instance.CheckOnlyMagicAttacksDamageForPlayer(Owner.Type))
		{
			num = 1f;
		}
		else if (attackBase == AttackBase.Both)
		{
			num = STR + INT;
		}
		else if (DetachedSingleton<CustomAIManager>.Instance.CheckSwapAttackStatsForPlayer(Owner.Type))
		{
			switch (attackBase)
			{
			case AttackBase.STR:
				num = INT;
				break;
			case AttackBase.INT:
				num = STR;
				break;
			}
		}
		else
		{
			switch (attackBase)
			{
			case AttackBase.STR:
				num = STR;
				break;
			case AttackBase.INT:
				num = INT;
				break;
			}
		}
		float num2 = DEX / 100f;
		float num3 = 1f - num2;
		float num4 = CriticalDamageMult();
		float num5 = NonCriticalDamageMult();
		return num * (num3 * num5 + num2 * num4);
	}

	private float CriticalDamageMult()
	{
		return 1f + (float)MiscParams.CriticalPercent / 100f;
	}

	private float NonCriticalDamageMult()
	{
		if (DetachedSingleton<CustomAIManager>.Instance.CheckOnlyCritsDamageForPlayer(Owner.Type))
		{
			return 0f;
		}
		return 1f;
	}

	public float ScoreThreat()
	{
		float attackOutput = GetAttackOutput();
		attackOutput *= (float)AttackCount;
		if (IsBlind)
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Blind);
			attackOutput -= attackOutput * (float)statusState.Count;
		}
		if ((bool)IsFrozen)
		{
			StatusState statusState2 = StatusEffects.Find((StatusState s) => s is Frozen);
			attackOutput -= attackOutput * (float)statusState2.Duration;
		}
		if ((bool)IsParalyzed && Abilities.Count > 0)
		{
			StatusState statusState3 = StatusEffects.Find((StatusState s) => s is Paralyze);
			attackOutput -= attackOutput * (float)statusState3.Duration;
		}
		return attackOutput;
	}

	public int GetExpectedDamage()
	{
		int num = 0;
		foreach (LaneState lane in Owner.Opponent.Lanes)
		{
			if (lane.Creature == null)
			{
				continue;
			}
			CreatureState autoTarget = Owner.Opponent.GetAutoTarget(lane.Creature, AttackBase.STR, AttackRange.Single);
			if (autoTarget == this)
			{
				int num2 = (int)lane.Creature.GetAttackOutput();
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return (int)((float)num * (1f - HPPct));
	}

	private void GetTotalEnemyAttackValues(out float attack, out float magic)
	{
		attack = 0f;
		magic = 0f;
		foreach (LaneState lane in Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				attack += lane.Creature.STR;
				magic += lane.Creature.INT;
			}
		}
		if (attack == 0f)
		{
			attack = 1f;
		}
		if (magic == 0f)
		{
			magic = 1f;
		}
	}

	public int Score()
	{
		float attack;
		float magic;
		GetTotalEnemyAttackValues(out attack, out magic);
		float num = attack + magic;
		float num2 = DEF * (attack / num) + RES * (magic / num);
		num2 /= 100f;
		float num3 = 0f;
		num3 = ScoreThreat();
		float num4 = (float)HP * num2;
		if ((bool)HasBravery)
		{
			num4 *= 3f;
		}
		num3 += num4;
		if (HasAutoRevive)
		{
			num3 += (float)Damage / (float)MaxHP * (float)AttackCount;
		}
		if (HasShield)
		{
			StatusState statusState = StatusEffects.Find((StatusState s) => s is Shield);
			num3 += (float)Damage / (float)MaxHP * (float)statusState.Count * (float)AttackCount;
		}
		if (HasSTRCounter)
		{
			StatusState statusState2 = StatusEffects.Find((StatusState s) => s is StrengthCounter);
			num3 += STR * (float)statusState2.Count;
		}
		if (HasINTCounter)
		{
			StatusState statusState3 = StatusEffects.Find((StatusState s) => s is MagicCounter);
			num3 += INT * (float)statusState3.Count;
		}
		if (HasRegen)
		{
			StatusState statusState4 = StatusEffects.Find((StatusState s) => s is Regen);
			num3 += (float)Damage / (float)MaxHP * (float)statusState4.Count * (float)AttackCount;
		}
		if (HasThorns)
		{
			StatusState statusState5 = StatusEffects.Find((StatusState s) => s is Thorns);
			int expectedDamage = GetExpectedDamage();
			num3 += (float)expectedDamage * statusState5.Intensity * (float)statusState5.Duration;
		}
		if ((bool)HasStealth)
		{
			StatusState statusState6 = StatusEffects.Find((StatusState s) => s is Stealth);
			int expectedDamage2 = GetExpectedDamage();
			num3 += (float)expectedDamage2 * statusState6.Intensity * (float)statusState6.Duration;
		}
		if (HasArmor)
		{
			int num5 = GetExpectedDamage();
			if ((bool)HasBravery)
			{
				num5 *= 2;
			}
			num3 += (float)num5;
		}
		if (IsPoisoned)
		{
			StatusState statusState7 = StatusEffects.Find((StatusState s) => s is Poison);
			num3 -= (float)MaxHP * statusState7.Intensity * (float)statusState7.Count * (float)AttackCount;
		}
		if (IsPlagued)
		{
			StatusState statusState8 = StatusEffects.Find((StatusState s) => s is Plague);
			num3 -= (float)MaxHP * statusState8.Intensity * (float)AttackCount;
		}
		if (IsBurned)
		{
			StatusState statusState9 = StatusEffects.Find((StatusState s) => s is Burned);
			CreatureState autoTarget = Owner.GetAutoTarget(this, AttackBase.STR, AttackRange.Single);
			if (autoTarget != null)
			{
				int num6 = PredictDamage(autoTarget, AttackBase.STR);
				num3 -= (float)num6 * statusState9.Intensity * (float)statusState9.Duration * (float)AttackCount;
			}
		}
		if (HasCardBlock)
		{
			StatusState statusState10 = StatusEffects.Find((StatusState s) => s is CardBlock);
			int num7 = 0;
			foreach (CardData item in DrawPile)
			{
				num7 += (int)item.Cost;
			}
			if (DrawPile.Count > 0)
			{
				num7 /= DrawPile.Count;
			}
			num3 -= (float)(num7 * statusState10.Duration * AttackCount);
		}
		num3 += (float)DamageDealt;
		return (int)num3;
	}

	public void Reset(float HPPct)
	{
		RemoveStatusEffects(StatusType.Buff);
		RemoveStatusEffects(StatusType.Debuff);
		InitStats(Data);
		int hP = (int)((float)MaxHP * HPPct);
		SetHP(hP);
		DrawIndex = 0;
		IgnoreDEF = false;
		IgnoreRES = false;
		BreakDEF = false;
		BreakRES = false;
		HasImmunity = false;
		HasStealth = false;
		HasBravery = false;
		AutoMiss = false;
		foreach (StatusState statusEffect in StatusEffects)
		{
			StatusState.Destroy(statusEffect);
		}
		StatusEffects.Clear();
		foreach (AbilityState ability in Abilities)
		{
			AbilityState.Destroy(ability);
		}
		Abilities.Clear();
		CreateAbilities(Data);
	}

	public void ApplyStatus(StatusData status, float varValue, CardData Card, AbilityState CausedByPassive = null, StatusRemovalData removalData = null)
	{
		if (varValue == 0f || (status.StatusType == StatusType.Debuff && (bool)HasImmunity && !status.Instant) || (status.EnableMessage == GameEvent.ENABLE_PLAGUE && IsPoisonImmune))
		{
			return;
		}
		string[] statusCancels = status.StatusCancels;
		foreach (string statusName in statusCancels)
		{
			CancelStatusEffect(statusName);
		}
		StatusState statusState = StatusEffects.Find((StatusState s) => s.Data == status);
		if (statusState != null && statusState.Target != null)
		{
			statusState.Stack(varValue, CausedByPassive);
			return;
		}
		statusState = StatusState.Create(status, varValue);
		statusState.SourceCard = Card;
		statusState.RemovalData = removalData;
		statusState.Apply(this, CausedByPassive);
		if (!status.Instant)
		{
			StatusEffects.Add(statusState);
		}
		else
		{
			StatusState.Destroy(statusState);
		}
	}

	public void CancelStatusEffect(string statusName)
	{
		StatusData data = StatusDataManager.Instance.GetData(statusName);
		if (data != null)
		{
			CancelStatusEffect(data);
		}
	}

	public void CancelStatusEffect(StatusData status)
	{
		StatusState statusState = StatusEffects.Find((StatusState s) => s.Data == status);
		if (statusState != null)
		{
			statusState.Disable();
			CleanStatusEffects();
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = ((status.StatusType != StatusType.Buff) ? GameEvent.REMOVE_DEBUFF : GameEvent.REMOVE_BUFF);
			gameMessage.WhichPlayer = Owner;
			gameMessage.Creature = this;
			Owner.Game.AddMessage(gameMessage);
		}
	}

	public void ProcessMessage(GameMessage Message)
	{
		int i = 0;
		bool flag = IsParalyzed;
		for (; i < StatusEffects.Count; i++)
		{
			StatusState statusState = StatusEffects[i];
			statusState.ProcessMessage(Message);
		}
		CleanStatusEffects();
		if (flag)
		{
			return;
		}
		foreach (AbilityState ability in Abilities)
		{
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.PASSIVE_TRIGGERED;
			gameMessage.Creature = this;
			Owner.Game.AddMessage(gameMessage);
			if (!ability.ProcessMessage(Message))
			{
				Owner.Game.RemoveMessage(gameMessage);
			}
		}
	}

	public void CleanStatusEffects()
	{
		int num = 0;
		while (num < StatusEffects.Count)
		{
			StatusState statusState = StatusEffects[num];
			if (statusState.Target == null)
			{
				StatusState.Destroy(statusState);
				StatusEffects.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public void RemoveStatusEffects(StatusType statusType)
	{
		int num = 0;
		foreach (StatusState statusEffect in StatusEffects)
		{
			if (statusEffect.Target != null && statusEffect.Data.StatusType == statusType)
			{
				statusEffect.Disable();
			}
		}
	}

	public void RemoveStatusEffect(StatusType statusType)
	{
		StatusState statusState = StatusEffects.Find((StatusState s) => s.Data.StatusType == statusType && s.Target != null);
		if (statusState != null)
		{
			statusState.Disable();
		}
		CleanStatusEffects();
	}

	public void DealDamage(int amount, AttackBase DamageType, bool IsDirect, CardData Card, StatusState causedByStatus, int preDefenseAmount = -1)
	{
		bool flag = HP <= 0;
		GameMessage gameMessage = new GameMessage();
		if (amount > 1)
		{
			if (DamageType != AttackBase.INT && DetachedSingleton<CustomAIManager>.Instance.CheckOnlyMagicAttacksDamageForPlayer(Owner.Opponent.Type))
			{
				amount = 1;
			}
			else if (DamageType == AttackBase.None && DetachedSingleton<CustomAIManager>.Instance.CheckOnlyCritsDamageForPlayer(Owner.Opponent.Type))
			{
				amount = 1;
			}
		}
		if (preDefenseAmount == -1)
		{
			preDefenseAmount = amount;
		}
		gameMessage.AmountChange = preDefenseAmount;
		if (amount > 0)
		{
			foreach (StatusState statusEffect in StatusEffects)
			{
				amount = (int)statusEffect.InteruptDamageTaken(amount);
				if (amount == 0)
				{
					gameMessage.IsShield = true;
					break;
				}
			}
		}
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && Owner.Type == PlayerType.Opponent && Owner.Opponent.GetCreatureCount() == 0)
		{
			amount = 0;
		}
		gameMessage.RawAmount = amount;
		if (amount > HP)
		{
			amount = HP;
		}
		if (amount == HP && Singleton<TutorialController>.Instance.DisallowPlayerLoss() && Owner.Type == PlayerType.User && Owner.GetCreatureCount() <= 1)
		{
			amount = HP - 1;
		}
		Owner.Opponent.DamageDealt += amount;
		Damage += amount;
		gameMessage.Action = GameEvent.DAMAGE_CREATURE;
		gameMessage.Lane = Lane;
		gameMessage.WhichPlayer = Owner;
		gameMessage.Creature = this;
		gameMessage.Amount = amount;
		gameMessage.IsDirect = IsDirect;
		gameMessage.Card = Card;
		if (DamageType == AttackBase.STR || DamageType == AttackBase.Both)
		{
			gameMessage.PhysicalDamage = true;
		}
		if (DamageType == AttackBase.INT || DamageType == AttackBase.Both)
		{
			gameMessage.MagicDamage = true;
		}
		Owner.Game.AddMessage(gameMessage);
		if (!flag && HP <= 0)
		{
			gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.CREATURE_DIED;
			gameMessage.Lane = Lane;
			gameMessage.WhichPlayer = Owner;
			gameMessage.Creature = this;
			gameMessage.AutoReviveDeath = HasAutoRevive;
			if (causedByStatus != null)
			{
				gameMessage.Status = causedByStatus.Data;
			}
			Owner.Game.AddMessage(gameMessage);
			if (!gameMessage.AutoReviveDeath)
			{
				Owner.RemoveCreature(Lane);
			}
			Owner.PlaceCreatureInGraveyard(this);
			Owner.Opponent.Statistics.mCreaturesKilledThisTurn++;
		}
	}

	public void Heal(float pct)
	{
		int num = (int)((float)MaxHP * pct);
		if (num < 1)
		{
			num = 1;
		}
		Heal(num);
	}

	public void Heal(int amount)
	{
		if (Damage > 0)
		{
			GameMessage gameMessage = new GameMessage();
			gameMessage.RawAmount = amount;
			if (amount > Damage)
			{
				amount = Damage;
			}
			Damage -= amount;
			gameMessage.Action = GameEvent.HEAL_CREATURE;
			gameMessage.WhichPlayer = Owner;
			gameMessage.Creature = this;
			gameMessage.Amount = amount;
			Owner.Game.AddMessage(gameMessage);
		}
	}

	public void SetHP(int value)
	{
		Damage = Math.Max(0, MaxHP - value);
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.SET_CREATURE_HP;
		gameMessage.WhichPlayer = Owner;
		gameMessage.Creature = this;
		gameMessage.Amount = HP;
		Owner.Game.AddMessage(gameMessage);
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

	public void StartTurn()
	{
		DamageDealt = 0;
		DragAttacksThisTurn = 0;
	}

	public void TickStatusDuration(PlayerState WhichPlayer)
	{
		foreach (StatusState statusEffect in StatusEffects)
		{
			statusEffect.StartTurn(WhichPlayer);
		}
		CleanStatusEffects();
	}

	public void EndTurn()
	{
	}

	private int PredictDamage(CreatureState Target, AttackBase Base, bool IgnoreShield = false)
	{
		float num = 0f;
		float num2 = 0f;
		if (IsBlind)
		{
			return 0;
		}
		if (Target.HasShield && !IgnoreShield)
		{
			return 0;
		}
		float num3 = ((!HasPinpoint) ? NonCriticalDamageMult() : CriticalDamageMult());
		if (Base != AttackBase.INT && DetachedSingleton<CustomAIManager>.Instance.CheckOnlyMagicAttacksDamageForPlayer(Owner.Type))
		{
			num = 1f;
		}
		else if (Base != AttackBase.Both)
		{
			num = (DetachedSingleton<CustomAIManager>.Instance.CheckSwapAttackStatsForPlayer(Owner.Type) ? ((Base != AttackBase.STR) ? (STR * num3 * (1f - ((!IgnoreDEF && !Target.BreakDEF) ? (Target.DEF / 100f) : 0f))) : (INT * num3 * (1f - ((!IgnoreRES && !Target.BreakRES) ? (Target.RES / 100f) : 0f)))) : ((Base != AttackBase.STR) ? (INT * num3 * (1f - ((!IgnoreRES && !Target.BreakRES) ? (Target.RES / 100f) : 0f))) : (STR * num3 * (1f - ((!IgnoreDEF && !Target.BreakDEF) ? (Target.DEF / 100f) : 0f)))));
		}
		else
		{
			num = (STR + INT) * num3;
			num2 = ((!IgnoreDEF && !Target.BreakDEF) ? (Target.DEF / 100f) : 0f);
			num2 += ((!IgnoreRES && !Target.BreakRES) ? (Target.RES / 100f) : 0f);
			num2 /= 2f;
			num *= 1f - num2;
		}
		if (num < 1f)
		{
			num = 1f;
		}
		return (int)num;
	}

	public List<int> PredictDamage(CreatureState Target, AttackBase Base, AttackRange EnemyGroup)
	{
		List<int> list = new List<int>(Owner.Opponent.Lanes.Count);
		for (int i = 0; i < Owner.Opponent.Lanes.Count; i++)
		{
			list.Add(0);
		}
		if (Target != null && Base != 0 && EnemyGroup != 0)
		{
			List<CreatureState> creatureRange = PlayerState.GetCreatureRange(Target.Owner, Target, EnemyGroup);
			{
				foreach (CreatureState item in creatureRange)
				{
					list[item.Lane.Index] = PredictDamage(Target, Base);
				}
				return list;
			}
		}
		return list;
	}

	public List<int> PredictDragAttackDamage(CreatureState Target)
	{
		return PredictDamage(Target, AttackBase.STR, AttackRange.Single);
	}

	public List<int> PredictCardDamage(CardData Card)
	{
		CreatureState target = Owner.GetTarget(this, Card.AttackBase, Card.Target2Group);
		return PredictDamage(target, Card.AttackBase, Card.Target2Group);
	}

	public void DrawCard(bool initialDraw = false)
	{
		if (!initialDraw && DetachedSingleton<CustomAIManager>.Instance.CheckNoCardDrawForPlayer(Owner.Type))
		{
			return;
		}
		List<CardData> list = ((OverrideDrawPile == null) ? DrawPile : OverrideDrawPile);
		if (list.Count <= 0)
		{
			return;
		}
		int index = DrawIndex;
		if (Data.QuestLoadoutEntryData == null || !Data.QuestLoadoutEntryData.InOrderDraw)
		{
			index = KFFRandom.RandomIndex(list.Count);
		}
		CardData cardData = null;
		if (OverrideDrawPile != null || (Data.QuestLoadoutEntryData != null && Data.QuestLoadoutEntryData.InOrderDraw))
		{
			cardData = list[index];
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
		DrawIndex++;
		if (DrawIndex >= list.Count)
		{
			DrawIndex = 0;
		}
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = ((!initialDraw) ? GameEvent.DRAW_CARD : GameEvent.INITIAL_DRAW_CARD);
		gameMessage.WhichPlayer = Owner;
		gameMessage.Creature = this;
		gameMessage.Card = cardData;
		Owner.Game.AddMessage(gameMessage);
		Owner.PlaceCardInHand(cardData);
	}

	public bool Attack(CreatureState Target, AttackBase Base, AttackCause cause, CardData causedByCard = null, float damageOverride = -1f)
	{
		float num = 0f;
		int num2 = (int)DEX;
		foreach (StatusState statusEffect in Target.StatusEffects)
		{
			num2 = statusEffect.AdjustCritChance(num2);
		}
		num2 += Singleton<TutorialController>.Instance.GetTutorialCritRateAdjust(Owner.Type, num2);
		bool flag;
		if (Singleton<TutorialController>.Instance.ForceCrit())
		{
			flag = true;
		}
		else
		{
			flag = KFFRandom.Percent(num2);
			bool flag2 = false;
			if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
			{
				flag2 = Owner.Type == PlayerType.Opponent;
			}
			if (!Singleton<AIManager>.Instance.IsPlanning() && flag2 && flag && !Target.HasBravery)
			{
				int num3 = Owner.AICritComp(num2);
				int num4 = UnityEngine.Random.Range(1, 101);
				flag = num4 <= num3;
			}
		}
		foreach (StatusState statusEffect2 in StatusEffects)
		{
			if (statusEffect2.GuaranteeCrit())
			{
				flag = true;
			}
		}
		foreach (StatusState statusEffect3 in Target.StatusEffects)
		{
			if (statusEffect3.GuaranteeCritAgainst())
			{
				flag = true;
			}
		}
		float num5 = 0f;
		if (damageOverride != -1f)
		{
			num5 = damageOverride;
		}
		else if (Base == AttackBase.Both)
		{
			num5 = STR + INT;
		}
		else if (DetachedSingleton<CustomAIManager>.Instance.CheckSwapAttackStatsForPlayer(Owner.Type))
		{
			switch (Base)
			{
			case AttackBase.STR:
				num5 = INT;
				break;
			case AttackBase.INT:
				num5 = STR;
				break;
			}
		}
		else
		{
			switch (Base)
			{
			case AttackBase.STR:
				num5 = STR;
				break;
			case AttackBase.INT:
				num5 = INT;
				break;
			}
		}
		float num6 = ((!flag) ? NonCriticalDamageMult() : CriticalDamageMult());
		num5 *= num6;
		float num7 = 0f;
		switch (Base)
		{
		case AttackBase.STR:
			num7 = ((!IgnoreDEF && !Target.BreakDEF) ? (Target.DEF / 100f) : 0f);
			break;
		case AttackBase.INT:
			num7 = ((!IgnoreRES && !Target.BreakRES) ? (Target.RES / 100f) : 0f);
			break;
		case AttackBase.Both:
			num7 += ((!IgnoreDEF && !Target.BreakDEF) ? (Target.DEF / 100f) : 0f);
			num7 += ((!IgnoreRES && !Target.BreakRES) ? (Target.RES / 100f) : 0f);
			num7 /= 2f;
			break;
		}
		num = num5 * (1f - num7);
		if (num < 1f)
		{
			num = 1f;
		}
		else if (Base != AttackBase.INT && DetachedSingleton<CustomAIManager>.Instance.CheckOnlyMagicAttacksDamageForPlayer(Owner.Type))
		{
			num = 1f;
		}
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.CREATURE_ATTACKED;
		gameMessage.AttackType = Base;
		gameMessage.Lane = Target.Lane;
		gameMessage.Creature = this;
		gameMessage.WhichPlayer = Owner;
		gameMessage.SecondCreature = Target;
		gameMessage.IsCounter = cause == AttackCause.Counter;
		gameMessage.IsDrag = cause == AttackCause.Drag;
		gameMessage.RawAmount = num5;
		if (AutoMiss)
		{
			num = 0f;
			gameMessage.IsMiss = true;
		}
		if (Target.HasShield)
		{
			gameMessage.Amount = 0f;
			gameMessage.IsShield = true;
		}
		else
		{
			gameMessage.Amount = num;
		}
		if (gameMessage.IsMiss)
		{
			flag = false;
		}
		gameMessage.IsCritical = flag;
		Owner.Game.AddMessage(gameMessage);
		DamageDealt += (int)num;
		float hPPct = Target.HPPct;
		Target.DealDamage((int)num, Base, false, null, null, (int)num5);
		AttackCount++;
		if (cause == AttackCause.Drag)
		{
			DragAttacksThisTurn++;
		}
		return flag;
	}

	public int GetStatusTypeCount(StatusType statusType)
	{
		int num = 0;
		foreach (StatusState statusEffect in StatusEffects)
		{
			if (statusEffect.Data.StatusType == statusType)
			{
				num++;
			}
		}
		return num;
	}

	private void MessageStatChange(GameEvent Action, int amount)
	{
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = Action;
		gameMessage.WhichPlayer = Owner;
		gameMessage.Creature = this;
		gameMessage.Lane = Lane;
		gameMessage.Amount = amount;
		Owner.Game.AddMessage(gameMessage);
	}

	public void GainMaxHP(int amount)
	{
		MaxHP += amount;
		MessageStatChange(GameEvent.GAIN_MAX_HP, amount);
	}

	public void GainSTR(int amount)
	{
		STR += amount;
		MessageStatChange(GameEvent.GAIN_STR, amount);
	}

	public void LoseSTR(int amount)
	{
		STR -= amount;
		MessageStatChange(GameEvent.LOSE_STR, amount);
	}

	public void GainINT(int amount)
	{
		INT += amount;
		MessageStatChange(GameEvent.GAIN_INT, amount);
	}

	public void LoseINT(int amount)
	{
		INT -= amount;
		MessageStatChange(GameEvent.LOSE_INT, amount);
	}

	public void GainDEF(int amount)
	{
		DEF += amount;
		MessageStatChange(GameEvent.GAIN_DEF, amount);
	}

	public void LoseDEF(int amount)
	{
		DEF -= amount;
		MessageStatChange(GameEvent.LOSE_DEF, amount);
	}

	public void GainRES(int amount)
	{
		RES += amount;
		MessageStatChange(GameEvent.GAIN_RES, amount);
	}

	public void LoseRES(int amount)
	{
		RES -= amount;
		MessageStatChange(GameEvent.LOSE_RES, amount);
	}

	public void GainDEX(int amount)
	{
		DEX += amount;
		MessageStatChange(GameEvent.GAIN_DEX, amount);
	}

	public void LoseDEX(int amount)
	{
		DEX -= amount;
		MessageStatChange(GameEvent.LOSE_DEX, amount);
	}

	public void GetStatString(CreatureStat stat, out string result, out int boost)
	{
		boost = 0;
		if (stat == CreatureStat.HP)
		{
			result = HP + "/" + MaxHP;
			float num = mBaseStats[(int)stat];
			if (num != 0f)
			{
				boost = (int)(100f * ((float)MaxHP / num - 1f));
				if (boost > 0)
				{
					result = result + " (+" + boost + "%)";
				}
				else if (boost < 0)
				{
					result = result + " (" + boost + "%)";
				}
			}
			return;
		}
		float stat2 = GetStat(stat);
		result = ((int)stat2).ToString();
		if (stat.IsPercent())
		{
			result += "%";
		}
		float num2 = mBaseStats[(int)stat];
		if (num2 != 0f)
		{
			if (stat.IsPercent())
			{
				boost = (int)(stat2 - num2);
			}
			else
			{
				boost = (int)(100f * (stat2 / num2 - 1f));
			}
			if (boost > 0)
			{
				result = result + " (+" + boost + "%)";
			}
			else if (boost < 0)
			{
				result = result + " (" + boost + "%)";
			}
		}
	}

	public override string ToString()
	{
		if (Data != null)
		{
			if (Owner != null)
			{
				return Data.ToString() + ", " + Owner.Type.ToString();
			}
			return Data.ToString() + ", null owner";
		}
		return "null data";
	}
}
