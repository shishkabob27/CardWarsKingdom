public class Paralyze : StatusState
{
	protected override void OnEnable()
	{
		base.Target.IsParalyzed = true;
	}

	protected override void OnDisable()
	{
		base.Target.IsParalyzed = false;
	}
}
