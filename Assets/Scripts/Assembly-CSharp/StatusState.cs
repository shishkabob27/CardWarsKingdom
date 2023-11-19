using System;
using UnityEngine;

public class StatusState
{
	public StatusData Data { get; set; }

	public CreatureState Target { get; set; }

	public float Intensity { get; set; }

	public float Intensity2 { get; set; }

	public int Duration { get; set; }

	public int Count { get; set; }

	public int EndCount { get; set; }

	public CardData SourceCard { get; set; }

	public StatusRemovalData RemovalData { get; set; }

	public static Type GetStatusClass(string Name)
	{
		Type type = Type.GetType(Name);
		if (type == null)
		{
			type = Type.GetType("StatusState");
		}
		return type;
	}

	public static StatusState Create(StatusData Source, float varValue)
	{
		Type statusClass = GetStatusClass(Source.ID);
		StatusState statusState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(statusClass) as StatusState;
		statusState.Init(Source, varValue);
		return statusState;
	}

	private static StatusState Create(StatusState Source)
	{
		StatusState statusState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(Source.GetType()) as StatusState;
		statusState.Init(Source);
		return statusState;
	}

	public static void Destroy(StatusState State)
	{
		State.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(State);
	}

	public void Init(StatusState Source)
	{
		Data = Source.Data;
		Intensity = Source.Intensity;
		Intensity2 = Source.Intensity2;
		Duration = Source.Duration;
		Count = Source.Count;
		EndCount = Source.EndCount;
		SourceCard = Source.SourceCard;
	}

	public void Init(StatusData Source, float varValue)
	{
		Data = Source;
		switch (Data.StackRule)
		{
		case StackType.Intensity:
			Intensity = varValue / 100f;
			Duration = (int)Data.FixedValue1;
			Count = 0;
			break;
		case StackType.Duration:
			Intensity = Data.FixedValue1 / 100f;
			Intensity2 = Data.FixedValue2 / 100f;
			Duration = (int)varValue;
			Count = 0;
			break;
		case StackType.Count:
			Intensity = Data.FixedValue1 / 100f;
			Duration = 0;
			Count = (int)varValue;
			break;
		case StackType.None:
			Intensity = Data.FixedValue1 / 100f;
			Intensity2 = Data.FixedValue2 / 100f;
			Duration = 0;
			Count = 0;
			break;
		}
	}

	public void Stack(float amount, AbilityState CausedByPassive)
	{
		OnDisable();
		float num = 0f;
		switch (Data.StackRule)
		{
		case StackType.Intensity:
			num = amount / 100f;
			Intensity += num;
			break;
		case StackType.Duration:
			num = Mathf.Floor(amount);
			Duration += (int)amount;
			break;
		case StackType.Count:
			num = Mathf.Floor(amount);
			Count += (int)amount;
			break;
		}
		GameMessage gameMessage = new GameMessage();
		gameMessage.Action = GameEvent.STATUS_STACKED;
		gameMessage.Creature = Target;
		gameMessage.WhichPlayer = Target.Owner;
		gameMessage.Status = Data;
		Target.Owner.Game.AddMessage(gameMessage);
		Enable(num, CausedByPassive);
	}

	public void Apply(CreatureState TargetCreature, AbilityState CausedByPassive)
	{
		float amountApplied = 0f;
		switch (Data.StackRule)
		{
		case StackType.Intensity:
			amountApplied = Intensity;
			break;
		case StackType.Duration:
			amountApplied = Duration;
			break;
		case StackType.Count:
			amountApplied = Count;
			break;
		}
		Target = TargetCreature;
		Enable(amountApplied, CausedByPassive);
	}

	public float GetStackedAmount()
	{
		switch (Data.StackRule)
		{
		case StackType.Intensity:
			return (Data.DisplayType != DisplayStackType.Percent) ? Intensity : (Intensity * 100f);
		case StackType.Duration:
			return Duration;
		case StackType.Count:
			return Count;
		default:
			return 1f;
		}
	}

	protected virtual void OnEnable()
	{
	}

	public void Enable(float amountApplied, AbilityState CausedByPassive)
	{
		OnEnable();
		if (!Data.Instant)
		{
			GameMessage gameMessage = new GameMessage();
			if (Data.StatusType == StatusType.Buff)
			{
				gameMessage.Action = GameEvent.GAIN_BUFF;
			}
			else
			{
				gameMessage.Action = GameEvent.GAIN_DEBUFF;
			}
			gameMessage.Creature = Target;
			gameMessage.Lane = Target.Lane;
			gameMessage.WhichPlayer = Target.Owner;
			gameMessage.Status = Data;
			gameMessage.RawAmount = Intensity;
			gameMessage.AmountChange = amountApplied;
			switch (Data.StackRule)
			{
			case StackType.Intensity:
			case StackType.Duration:
				gameMessage.Amount = Duration;
				break;
			case StackType.Count:
				gameMessage.Amount = Count;
				break;
			}
			Target.Owner.Game.AddMessage(gameMessage);
			GameMessage gameMessage2 = new GameMessage();
			gameMessage2.Action = Data.EnableMessage;
			gameMessage2.Status = Data;
			gameMessage2.Creature = Target;
			gameMessage2.Lane = Target.Lane;
			gameMessage2.WhichPlayer = Target.Owner;
			gameMessage2.RawAmount = gameMessage.RawAmount;
			gameMessage2.Amount = gameMessage.Amount;
			gameMessage2.AmountChange = gameMessage.AmountChange;
			gameMessage2.CausedByPassive = CausedByPassive;
			Target.Owner.Game.AddMessage(gameMessage2);
		}
	}

	protected void ReportStatusAction(GameEvent Reason, float amount = 0f, bool rawAmount = false, CreatureState secondCreature = null)
	{
		if (Target != null)
		{
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = Reason;
			gameMessage.Creature = Target;
			gameMessage.SecondCreature = secondCreature;
			gameMessage.Lane = Target.Lane;
			gameMessage.WhichPlayer = Target.Owner;
			gameMessage.Status = Data;
			if (rawAmount)
			{
				gameMessage.RawAmount = amount;
			}
			else
			{
				gameMessage.Amount = amount;
			}
			Target.Owner.Game.AddMessage(gameMessage);
		}
	}

	public virtual void ProcessMessage(GameMessage Message)
	{
	}

	public virtual float InteruptDamageGiven(float damage)
	{
		return damage;
	}

	public virtual float InteruptDamageTaken(float damage)
	{
		return damage;
	}

	public virtual int AdjustCritChance(int chance)
	{
		return chance;
	}

	public virtual int AttackDiscount()
	{
		return 0;
	}

	public virtual bool BlockDraw()
	{
		return false;
	}

	public virtual bool PreventAttack()
	{
		return false;
	}

	public virtual int BonusDragAttacks()
	{
		return 0;
	}

	public virtual bool GuaranteeCrit()
	{
		return false;
	}

	public virtual bool GuaranteeCritAgainst()
	{
		return false;
	}

	public virtual bool AutoMiss()
	{
		return false;
	}

	public virtual bool Dodge(AttackCause attackCause)
	{
		return false;
	}

	public virtual bool AttackRandom()
	{
		return false;
	}

	public virtual int GetExtraCardCount()
	{
		return 0;
	}

	public virtual int BonusEnergy()
	{
		return 0;
	}

	protected virtual void OnDisable()
	{
	}

	public void Disable()
	{
		GameMessage gameMessage = null;
		OnDisable();
		gameMessage = new GameMessage();
		if (Data.StatusType == StatusType.Buff)
		{
			gameMessage.Action = GameEvent.LOSE_BUFF;
		}
		else
		{
			gameMessage.Action = GameEvent.LOSE_DEBUFF;
		}
		gameMessage.Creature = Target;
		gameMessage.Status = Data;
		gameMessage.Lane = Target.Lane;
		gameMessage.WhichPlayer = Target.Owner;
		Target.Owner.Game.AddMessage(gameMessage);
		gameMessage = new GameMessage();
		gameMessage.Action = Data.DisableMessage;
		gameMessage.Creature = Target;
		gameMessage.Status = Data;
		gameMessage.Lane = Target.Lane;
		gameMessage.WhichPlayer = Target.Owner;
		Target.Owner.Game.AddMessage(gameMessage);
		Target = null;
	}

	private void TickDuration()
	{
		Duration--;
		ReportStatusAction(GameEvent.TICK_STATUS, Duration);
		if (Duration == 0)
		{
			Disable();
		}
	}

	protected void TickCount()
	{
		Count--;
		ReportStatusAction(GameEvent.TICK_STATUS, Count);
		if (Count == 0)
		{
			Disable();
		}
	}

	protected virtual void OnStartTurn(PlayerState WhichPlayer)
	{
	}

	public void StartTurn(PlayerState WhichPlayer)
	{
		OnStartTurn(WhichPlayer);
		if (Duration > 0)
		{
			EndCount++;
			if (EndCount == 2)
			{
				TickDuration();
				EndCount = 0;
			}
		}
	}

	public void EndTurn()
	{
	}

	public void Clean()
	{
		Target = null;
		Intensity = 0f;
		Intensity2 = 0f;
		Duration = 0;
		Count = 0;
		EndCount = 0;
	}

	public StatusState DeepCopy()
	{
		return Create(this);
	}
}
