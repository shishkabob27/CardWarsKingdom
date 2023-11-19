public abstract class OnBravery : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_BRAVERY && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
