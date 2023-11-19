public abstract class OnOpponentCardDraw : AbilityState
{
	protected CardData CardDrawn;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DRAW_CARD && Message.WhichPlayer == base.Owner.Owner.Opponent)
		{
			CardDrawn = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
