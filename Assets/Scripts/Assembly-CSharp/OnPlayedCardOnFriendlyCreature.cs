public abstract class OnPlayedCardOnFriendlyCreature : AbilityState
{
	protected CreatureState PlayedOnCreature;

	protected CardData CardPlayed;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CARD_PLAYED && Message.Creature != null && Message.Creature.Owner == base.Owner.Owner)
		{
			PlayedOnCreature = Message.Creature;
			CardPlayed = Message.Card;
			return OnEnable();
		}
		return false;
	}
}
