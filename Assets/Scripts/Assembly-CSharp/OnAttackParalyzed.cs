public abstract class OnAttackParalyzed : AbilityState
{
	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && (bool)Message.SecondCreature.IsParalyzed)
		{
			Target = Message.SecondCreature;
			return OnEnable();
		}
		return false;
	}
}
