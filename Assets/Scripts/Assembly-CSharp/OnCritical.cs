public abstract class OnCritical : AbilityState
{
	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.IsCritical && Message.Creature == base.Owner)
		{
			Target = Message.SecondCreature;
			return OnEnable();
		}
		return false;
	}
}
