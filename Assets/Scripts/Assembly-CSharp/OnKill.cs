public abstract class OnKill : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature == base.Owner && Message.SecondCreature.HP <= 0)
		{
			return OnEnable();
		}
		return false;
	}
}
