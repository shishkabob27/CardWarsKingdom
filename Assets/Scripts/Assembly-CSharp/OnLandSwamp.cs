public abstract class OnLandSwamp : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_LANDSWAMP && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
