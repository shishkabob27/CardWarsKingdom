public abstract class OnDamaged : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
