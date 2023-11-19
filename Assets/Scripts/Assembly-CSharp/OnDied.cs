public abstract class OnDied : AbilityState
{
	protected StatusData CausedByStatus;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_DIED && Message.Creature == base.Owner)
		{
			CausedByStatus = Message.Status;
			return OnEnable();
		}
		return false;
	}
}
