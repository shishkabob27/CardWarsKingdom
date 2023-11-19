public abstract class OnPlayedCardOnAnyCreature : AbilityState
{
	protected CardData CardPlayed;

	protected CreatureState PlayedOnCreature;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CARD_PLAYED)
		{
			CardPlayed = Message.Card;
			PlayedOnCreature = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
