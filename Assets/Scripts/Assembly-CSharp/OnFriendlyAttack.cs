public abstract class OnFriendlyAttack : AbilityState
{
	protected CreatureState Attacker;

	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature.Owner == base.Owner.Owner)
		{
			Attacker = Message.Creature;
			Target = Message.SecondCreature;
			return OnEnable();
		}
		return false;
	}
}
