public class FortifyRES : StatusState
{
	protected override void OnEnable()
	{
		base.Target.RES += base.Intensity * 100f;
	}

	protected override void OnDisable()
	{
		base.Target.RES -= base.Intensity * 100f;
	}
}
