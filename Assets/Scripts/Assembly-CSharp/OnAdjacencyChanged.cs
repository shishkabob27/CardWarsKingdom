public abstract class OnAdjacencyChanged : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (Message.Action == GameEvent.CREATURE_DEPLOYED && (Message.Creature == base.Owner || base.Owner.Lane.AdjacentLanes.Contains(Message.Lane)))
		{
			result = OnEnable();
		}
		if (Message.Action == GameEvent.CREATURE_REMOVED)
		{
			if (Message.Creature == base.Owner)
			{
				OnDisable();
			}
			else if (base.Owner.Lane.AdjacentLanes.Contains(Message.Lane))
			{
				result = OnEnable();
			}
		}
		if (Message.Action == GameEvent.CREATURE_PLACED && (Message.Creature == base.Owner || base.Owner.Lane.AdjacentLanes.Contains(Message.Lane)))
		{
			result = OnEnable();
		}
		return result;
	}
}
