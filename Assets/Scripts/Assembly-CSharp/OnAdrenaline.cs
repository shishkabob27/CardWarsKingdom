public abstract class OnAdrenaline : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_ADRENALINE && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
