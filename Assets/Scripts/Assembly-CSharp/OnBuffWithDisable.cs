public abstract class OnBuffWithDisable : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.GAIN_BUFF && Message.Creature == base.Owner)
		{
			result = OnEnable();
		}
		if (Message.Action == GameEvent.LOSE_BUFF && Message.Creature == base.Owner)
		{
			OnDisable();
		}
		return result;
	}
}
