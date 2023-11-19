public abstract class OnOpponentHeroCardDraw : AbilityState
{
	protected CardData CardDrawn;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DRAW_CARD && Message.WhichPlayer == base.Owner.Owner.Opponent && Message.Creature == null)
		{
			CardDrawn = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
