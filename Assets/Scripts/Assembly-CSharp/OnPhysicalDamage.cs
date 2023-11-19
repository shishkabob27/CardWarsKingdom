public abstract class OnPhysicalDamage : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DAMAGE_CREATURE && Message.PhysicalDamage && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
