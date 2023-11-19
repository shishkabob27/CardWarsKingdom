public abstract class OnOppPlayedCard : AbilityState
{
	protected CardData CardPlayed;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CARD_PLAYED && Message.WhichPlayer == base.Owner.Owner.Opponent)
		{
			CardPlayed = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
