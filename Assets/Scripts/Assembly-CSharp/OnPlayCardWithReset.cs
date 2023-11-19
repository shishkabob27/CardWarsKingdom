public abstract class OnPlayCardWithReset : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.CARD_PLAYED && Message.WhichPlayer == base.Owner.Owner)
		{
			result = OnEnable();
		}
		if (Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Owner.Owner)
		{
			OnDisable();
		}
		return result;
	}
}
