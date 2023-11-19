public abstract class OnFlaming : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_FLAMING && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
