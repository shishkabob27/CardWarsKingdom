public class Pinpoint : StatusState
{
	public override bool GuaranteeCrit()
	{
		if (base.Target == null)
		{
			return false;
		}
		if (base.Count > 0)
		{
			ReportStatusAction(GameEvent.STATUS_ACTION_MARKED);
			TickCount();
		}
		return true;
	}
}
