public abstract class OnDiscardCount : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.START_TURN)
		{
			StoredValue = 0;
			IsActive = false;
		}
		if (Message.Action == GameEvent.MANUAL_DISCARD_CARD && Message.WhichPlayer == base.Owner.Owner)
		{
			StoredValue++;
			if (!IsActive && StoredValue >= base.Val1)
			{
				IsActive = true;
				result = OnEnable();
			}
		}
		return result;
	}
}
