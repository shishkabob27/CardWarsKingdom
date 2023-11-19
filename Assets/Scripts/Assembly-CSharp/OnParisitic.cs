public abstract class OnParisitic : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_PARISITIC && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
