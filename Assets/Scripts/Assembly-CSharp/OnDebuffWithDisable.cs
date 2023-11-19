public abstract class OnDebuffWithDisable : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.GAIN_DEBUFF && Message.Creature == base.Owner)
		{
			result = OnEnable();
		}
		if (Message.Action == GameEvent.LOSE_DEBUFF && Message.Creature == base.Owner)
		{
			OnDisable();
		}
		return result;
	}
}
