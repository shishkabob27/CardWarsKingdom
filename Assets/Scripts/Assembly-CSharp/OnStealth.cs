public abstract class OnStealth : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_STEALTH && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
