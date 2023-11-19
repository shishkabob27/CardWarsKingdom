public abstract class OnDeploy : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_DEPLOYED && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
