public abstract class OnZeroAP : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ZERO_ACTION_POINTS && Message.WhichPlayer == base.Owner.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
