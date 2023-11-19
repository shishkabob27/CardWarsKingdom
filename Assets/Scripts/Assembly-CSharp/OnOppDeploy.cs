public abstract class OnOppDeploy : AbilityState
{
	protected CreatureState CreatureDeployed;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_DEPLOYED && Message.WhichPlayer == base.Owner.Owner.Opponent)
		{
			CreatureDeployed = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
