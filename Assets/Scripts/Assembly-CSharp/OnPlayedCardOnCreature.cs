public abstract class OnPlayedCardOnCreature : AbilityState
{
	protected CardData CardPlayed;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CARD_PLAYED && Message.Creature == base.Owner)
		{
			CardPlayed = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
