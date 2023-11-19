public abstract class OnFriendlyDamaged : AbilityState
{
	protected CreatureState Attacked;

	protected float Damage;

	protected bool Physical;

	protected bool Magic;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature.Owner == base.Owner.Owner)
		{
			Attacked = Message.Creature;
			Damage = Message.Amount;
			Physical = Message.PhysicalDamage;
			Magic = Message.MagicDamage;
			return OnEnable();
		}
		return false;
	}
}
