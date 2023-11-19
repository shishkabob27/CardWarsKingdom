public class Bravery : StatusState
{
	protected override void OnEnable()
	{
		base.Target.HasBravery = true;
	}

	protected override void OnDisable()
	{
		base.Target.HasBravery = false;
	}
}
