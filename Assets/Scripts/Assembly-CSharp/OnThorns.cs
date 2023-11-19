public abstract class OnThorns : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_THORNS && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
