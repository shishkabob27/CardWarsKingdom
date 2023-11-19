public abstract class OnLandSand : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_LANDSAND && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
