public abstract class CounterAttack : StatusState
{
	protected void Attack(GameMessage Message, AttackBase Base)
	{
		if ((bool)base.Target.IsFrozen || Message.Action != GameEvent.CREATURE_ATTACKED || Message.SecondCreature != base.Target || Message.SecondCreature.HP <= 0 || Message.IsCounter || base.Count <= 0)
		{
			return;
		}
		CreatureState creatureState = Message.Creature;
		if (!creatureState.HasBravery)
		{
			CreatureState randomTarget = base.Target.Owner.GetRandomTarget(base.Target);
			if (randomTarget != null && (bool)randomTarget.HasBravery)
			{
				creatureState = randomTarget;
			}
		}
		if (creatureState.HP > 0)
		{
			base.Target.Owner.Attack(base.Target, creatureState, Base, AttackCause.Counter, -1f);
			TickCount();
		}
	}
}
