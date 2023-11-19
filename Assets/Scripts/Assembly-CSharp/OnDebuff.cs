public abstract class OnDebuff : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.GAIN_DEBUFF && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
