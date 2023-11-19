public abstract class OnEnemyPlague : AbilityState
{
	protected CreatureState CreatureAffected;

	protected float AmountChange;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.ENABLE_PLAGUE && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			CreatureAffected = Message.Creature;
			AmountChange = Message.AmountChange;
			return OnEnable();
		}
		return false;
	}
}
