public abstract class OnPercentAttack : AbilityState
{
	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && base.Val1Chance)
		{
			Target = Message.SecondCreature;
			return OnEnable();
		}
		return false;
	}
}
