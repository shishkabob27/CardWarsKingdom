public abstract class OnMagicDamage : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DAMAGE_CREATURE && Message.MagicDamage && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
