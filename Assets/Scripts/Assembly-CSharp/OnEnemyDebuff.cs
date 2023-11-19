public abstract class OnEnemyDebuff : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.GAIN_DEBUFF && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			return OnEnable();
		}
		return false;
	}
}
