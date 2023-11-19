public abstract class OnFriendlyThorns : AbilityState
{
	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_THORNS && Message.Creature.Owner == base.Owner.Owner)
		{
			Target = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
