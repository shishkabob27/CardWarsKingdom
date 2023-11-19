public class Wisdom : StatusState
{
	protected override void OnEnable()
	{
		base.Target.IgnoreRES = true;
	}

	protected override void OnDisable()
	{
		base.Target.IgnoreRES = false;
	}
}
