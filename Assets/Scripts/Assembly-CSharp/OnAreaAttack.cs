public abstract class OnAreaAttack : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.AREA_ATTACK_START && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
