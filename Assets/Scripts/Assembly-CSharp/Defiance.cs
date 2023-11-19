public class Defiance : StatusState
{
	protected override void OnEnable()
	{
		base.Target.IgnoreDEF = true;
	}

	protected override void OnDisable()
	{
		base.Target.IgnoreDEF = false;
	}
}
