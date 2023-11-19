public class RemoveStatus : StatusState
{
	protected override void OnEnable()
	{
		if (base.RemovalData.Targets != null)
		{
			foreach (StatusData target in base.RemovalData.Targets)
			{
				base.Target.CancelStatusEffect(target);
			}
		}
		if (base.Count <= 0)
		{
			return;
		}
		if (base.RemovalData.RandomBuffs)
		{
			GameMessage gameMessage = new GameMessage();
			gameMessage.Action = GameEvent.REMOVE_BUFF;
			gameMessage.WhichPlayer = base.Target.Owner;
			gameMessage.Creature = base.Target;
			gameMessage.Amount = base.Count;
			base.Target.Owner.Game.AddMessage(gameMessage);
			for (int i = 0; i < base.Count; i++)
			{
				base.Target.RemoveStatusEffect(StatusType.Buff);
			}
		}
		if (base.RemovalData.RandomDebuffs)
		{
			GameMessage gameMessage2 = new GameMessage();
			gameMessage2.Action = GameEvent.REMOVE_DEBUFF;
			gameMessage2.WhichPlayer = base.Target.Owner;
			gameMessage2.Creature = base.Target;
			gameMessage2.Amount = base.Count;
			base.Target.Owner.Game.AddMessage(gameMessage2);
			for (int j = 0; j < base.Count; j++)
			{
				base.Target.RemoveStatusEffect(StatusType.Debuff);
			}
		}
	}
}
