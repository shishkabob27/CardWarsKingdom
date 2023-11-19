public abstract class OnAttackPoisoned : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && Message.SecondCreature.IsPoisoned)
		{
			return OnEnable();
		}
		return false;
	}
}
