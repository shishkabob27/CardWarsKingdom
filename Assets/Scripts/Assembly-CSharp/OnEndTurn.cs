public abstract class OnEndTurn : AbilityState
{
	protected int ActionPoints;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.END_TURN && Message.WhichPlayer == base.Owner.Owner)
		{
			ActionPoints = (int)Message.Amount;
			return OnEnable();
		}
		return false;
	}
}
