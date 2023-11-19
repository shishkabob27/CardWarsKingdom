public class Shield : StatusState
{
	public override float InteruptDamageTaken(float damage)
	{
		if (base.Target == null)
		{
			return damage;
		}
		if (base.Count > 0)
		{
			ReportStatusAction(GameEvent.STATUS_ACTION_SHIELD);
			TickCount();
		}
		return 0f;
	}
}
