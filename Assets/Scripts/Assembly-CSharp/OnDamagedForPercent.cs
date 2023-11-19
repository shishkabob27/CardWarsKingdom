public abstract class OnDamagedForPercent : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.DAMAGE_CREATURE && Message.Creature == base.Owner && Message.Amount >= (float)Message.Creature.MaxHP * base.Val1Pct)
		{
			return OnEnable();
		}
		return false;
	}
}
