public abstract class OnDealCard : AbilityState
{
	protected CardData CardDrawn;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DRAW_CARD && Message.Creature == base.Owner)
		{
			CardDrawn = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
