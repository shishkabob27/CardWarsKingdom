public abstract class OnEnemyDied : AbilityState
{
	protected StatusData CausedByStatus;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_DIED && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			CausedByStatus = Message.Status;
			return OnEnable();
		}
		return false;
	}
}
