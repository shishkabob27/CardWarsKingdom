public abstract class OnMagicAttack : AbilityState
{
	protected CreatureState Target;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && Message.AttackType == AttackBase.INT)
		{
			Target = Message.SecondCreature;
			return OnEnable();
		}
		return false;
	}
}
