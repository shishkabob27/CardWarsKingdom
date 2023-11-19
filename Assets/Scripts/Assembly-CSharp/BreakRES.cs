public class BreakRES : StatusState
{
	protected override void OnEnable()
	{
		base.Target.BreakRES = true;
	}

	protected override void OnDisable()
	{
		base.Target.BreakRES = false;
	}
}
