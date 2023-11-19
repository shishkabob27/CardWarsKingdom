public class Mindless : StatusState
{
	protected override void OnEnable()
	{
		base.Target.IsMindless = true;
	}

	protected override void OnDisable()
	{
		base.Target.IsMindless = false;
	}
}
