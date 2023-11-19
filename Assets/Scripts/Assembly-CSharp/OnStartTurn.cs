public abstract class OnStartTurn : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Owner.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
