public class Blind : StatusState
{
	public override bool AutoMiss()
	{
		if (base.Target == null)
		{
			return false;
		}
		if (base.Count > 0)
		{
			ReportStatusAction(GameEvent.STATUS_ACTION_BLIND);
			TickCount();
		}
		return true;
	}
}
