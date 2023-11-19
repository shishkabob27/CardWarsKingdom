public abstract class OnCardThreshold : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.START_TURN && Message.WhichPlayer == base.Owner.Owner)
		{
			IsActive = false;
			StoredValue = 0;
		}
		if (!IsActive && Message.Action == GameEvent.CARD_PLAYED && Message.WhichPlayer == base.Owner.Owner)
		{
			StoredValue++;
			if (StoredValue >= base.Val1)
			{
				result = OnEnable();
				IsActive = true;
			}
		}
		return result;
	}
}
