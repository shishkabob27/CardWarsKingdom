public abstract class OnVampiric : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_VAMPIRIC && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
