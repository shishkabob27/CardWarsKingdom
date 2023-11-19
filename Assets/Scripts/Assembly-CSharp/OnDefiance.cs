public abstract class OnDefiance : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_DEFIANCE && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
