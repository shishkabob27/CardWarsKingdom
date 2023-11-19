public class Immunity : StatusState
{
	protected override void OnEnable()
	{
		base.Target.HasImmunity = true;
	}

	protected override void OnDisable()
	{
		base.Target.HasImmunity = false;
	}
}
