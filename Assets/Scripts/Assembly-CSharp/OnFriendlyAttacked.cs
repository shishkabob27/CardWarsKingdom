public abstract class OnFriendlyAttacked : AbilityState
{
	protected CreatureState Attacker;

	protected CreatureState Attacked;

	protected float Damage;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.SecondCreature.Owner == base.Owner.Owner)
		{
			Attacker = Message.Creature;
			Attacked = Message.SecondCreature;
			Damage = Message.Amount;
			return OnEnable();
		}
		return false;
	}
}
