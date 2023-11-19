public class FortifyDEF : StatusState
{
	protected override void OnEnable()
	{
		base.Target.DEF += base.Intensity * 100f;
	}

	protected override void OnDisable()
	{
		base.Target.DEF -= base.Intensity * 100f;
	}
}
