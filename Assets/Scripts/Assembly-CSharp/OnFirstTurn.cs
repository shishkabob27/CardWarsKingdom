public abstract class OnFirstTurn : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.START_TURN && base.Owner.Owner.Game.IsFirstTurn())
		{
			return OnEnable();
		}
		return false;
	}
}
