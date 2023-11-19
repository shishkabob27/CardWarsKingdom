public abstract class InflictStatus : StatusState
{
	public override void ProcessMessage(GameMessage Message)
	{
		if (Message.Action != GameEvent.CREATURE_ATTACKED || Message.SecondCreature != base.Target || Message.IsMiss)
		{
			return;
		}
		StatusData data = StatusDataManager.Instance.GetData(base.Data.InflictedStatus);
		if (data != null)
		{
			StackType stackRule = data.StackRule;
			if (stackRule == StackType.Intensity)
			{
				Message.Creature.ApplyStatus(data, base.Intensity * 100f, null);
			}
			else
			{
				Message.Creature.ApplyStatus(data, 1f, null);
			}
		}
	}
}
