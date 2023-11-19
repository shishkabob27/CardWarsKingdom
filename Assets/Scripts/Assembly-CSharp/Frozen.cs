public class Frozen : StatusState
{
	public override bool PreventAttack()
	{
		return true;
	}

	protected override void OnEnable()
	{
		base.Target.IsFrozen = true;
	}

	protected override void OnDisable()
	{
		base.Target.IsFrozen = false;
	}
}
