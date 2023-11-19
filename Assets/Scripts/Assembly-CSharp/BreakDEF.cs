public class BreakDEF : StatusState
{
	protected override void OnEnable()
	{
		base.Target.BreakDEF = true;
	}

	protected override void OnDisable()
	{
		base.Target.BreakDEF = false;
	}
}
