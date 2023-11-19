public abstract class OnLandNice : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_LANDNICE && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
