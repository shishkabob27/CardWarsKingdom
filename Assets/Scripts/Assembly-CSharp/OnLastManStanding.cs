public abstract class OnLastManStanding : AbilityState
{
	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (!IsActive && Message.Action == GameEvent.CREATURE_REMOVED && Message.Creature != base.Owner && Message.Creature.Owner == base.Owner.Owner)
		{
			int num = 0;
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null)
				{
					num++;
				}
			}
			if (num == 1)
			{
				result = OnEnable();
				IsActive = true;
			}
		}
		if (IsActive && Message.Action == GameEvent.CREATURE_PLACED && Message.Creature != base.Owner)
		{
			OnDisable();
			IsActive = false;
		}
		return result;
	}
}
