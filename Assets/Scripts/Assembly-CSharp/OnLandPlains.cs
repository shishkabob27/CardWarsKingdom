public abstract class OnLandPlains : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_LANDPLAINS && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
