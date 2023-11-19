public abstract class OnAttacked : AbilityState
{
	protected CreatureState Attacker;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.SecondCreature == base.Owner)
		{
			Attacker = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
