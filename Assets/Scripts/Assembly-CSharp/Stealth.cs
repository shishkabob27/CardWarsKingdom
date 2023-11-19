public class Stealth : StatusState
{
	protected override void OnEnable()
	{
		base.Target.HasStealth = true;
	}

	public override bool Dodge(AttackCause attackCause)
	{
		if (attackCause == AttackCause.Drag || attackCause == AttackCause.Berserk)
		{
			ReportStatusAction(GameEvent.STATUS_ACTION_STEALTH);
			return true;
		}
		return false;
	}

	protected override void OnDisable()
	{
		base.Target.HasStealth = false;
	}
}
