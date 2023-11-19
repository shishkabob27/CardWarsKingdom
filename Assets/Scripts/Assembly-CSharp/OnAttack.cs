public abstract class OnAttack : AbilityState
{
	protected CreatureState Target;

	protected float Damage;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner)
		{
			Target = Message.SecondCreature;
			Damage = Message.Amount;
			return OnEnable();
		}
		return false;
	}
}
