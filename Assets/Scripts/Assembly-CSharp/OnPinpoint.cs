public abstract class OnPinpoint : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_PINPOINT && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
