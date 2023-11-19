public abstract class OnSufferCritical : AbilityState
{
	protected CreatureState Attacker;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.IsCritical && Message.SecondCreature == base.Owner)
		{
			Attacker = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
