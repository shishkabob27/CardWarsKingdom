public abstract class OnAttackBravery : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && (bool)Message.SecondCreature.HasBravery)
		{
			return OnEnable();
		}
		return false;
	}
}
