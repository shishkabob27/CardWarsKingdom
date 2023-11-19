public abstract class OnAttackHPThreshold : AbilityState
{
	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && Message.SecondCreature.HPPct <= base.Val1Pct)
		{
			Target = Message.SecondCreature;
			return OnEnable();
		}
		return false;
	}
}
