public abstract class OnDeployed : AbilityState
{
	protected CreatureState DeployedCreature;

	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_DEPLOYED && Message.Creature != base.Owner && Message.WhichPlayer == base.Owner.Owner)
		{
			DeployedCreature = Message.Creature;
			return OnEnable();
		}
		return false;
	}
}
