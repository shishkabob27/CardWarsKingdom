public abstract class OnHeal : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.HEAL_CREATURE && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
