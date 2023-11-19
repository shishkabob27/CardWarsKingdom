public class EnhanceINT : StatusState
{
	protected override void OnEnable()
	{
		float num = (float)base.Target.Data.INT * base.Intensity;
		base.Target.INT += num;
	}

	protected override void OnDisable()
	{
		float num = (float)base.Target.Data.INT * base.Intensity;
		base.Target.INT -= num;
	}
}
