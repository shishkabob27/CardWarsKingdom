public abstract class OnEnemyPoisonOrPlague : AbilityState
{
	protected CreatureState CreatureAffected;

	public override bool ProcessMessage(GameMessage Message)
	{
		if ((Message.Action == GameEvent.ENABLE_POISON || Message.Action == GameEvent.ENABLE_PLAGUE) && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			CreatureAffected = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
