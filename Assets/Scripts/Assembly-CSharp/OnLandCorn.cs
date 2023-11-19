public abstract class OnLandCorn : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_LANDCORN && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
