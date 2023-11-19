public abstract class OnLastEnemyStanding : AbilityState
{
	private int CountEnemies()
	{
		int num = 0;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				num++;
			}
		}
		return num;
	}

	public override bool ProcessMessage(GameMessage Message)
	{
		bool result = false;
		if (!IsActive && Message.Action == GameEvent.CREATURE_REMOVED && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			int num = CountEnemies();
			if (num == 1)
			{
				result = OnEnable();
				IsActive = true;
			}
		}
		if (IsActive && Message.Action == GameEvent.CREATURE_PLACED && Message.Creature.Owner == base.Owner.Owner.Opponent)
		{
			int num2 = CountEnemies();
			if (num2 > 1)
			{
				OnDisable();
				IsActive = false;
			}
		}
		return result;
	}
}
