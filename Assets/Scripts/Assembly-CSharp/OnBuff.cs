public abstract class OnBuff : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.GAIN_BUFF && Message.Creature == base.Owner)
		{
			return OnEnable();
		}
		return false;
	}
}
