public abstract class OnEnemyAttack : AbilityState
{
	protected CreatureState Attacker;

	protected CreatureState Target;

	protected bool IsDragAttack;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			Attacker = Message.Creature;
			Target = Message.SecondCreature;
			IsDragAttack = Message.IsDrag;
			return OnEnable();
		}
		return false;
	}
}
