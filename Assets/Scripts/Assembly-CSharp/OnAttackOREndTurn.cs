public abstract class OnAttackOREndTurn : AbilityState
{
	protected int ActionPoints;

	protected CreatureState Target;

	protected float Damage;

	protected bool isEndTurn;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.END_TURN && Message.WhichPlayer == base.Owner.Owner)
		{
			ActionPoints = (int)Message.Amount;
			isEndTurn = true;
			return OnEnable();
		}
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner)
		{
			Target = Message.SecondCreature;
			Damage = Message.Amount;
			isEndTurn = false;
			return OnEnable();
		}
		return false;
	}
}
