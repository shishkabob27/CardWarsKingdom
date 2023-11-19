public abstract class OnChilled : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_CHILLED && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
