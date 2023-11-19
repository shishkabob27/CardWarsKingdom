public abstract class OnDiscard : AbilityState
{
	public CardData CardDiscared;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.MANUAL_DISCARD_CARD && Message.WhichPlayer == base.Owner.Owner)
		{
			CardDiscared = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
