public class Transmogrify : StatusState
{
	protected override void OnEnable()
	{
		float num = 1f - base.Intensity;
		float num2 = (float)base.Target.Data.STR * num;
		base.Target.STR -= num2;
		num2 = (float)base.Target.Data.INT * num;
		base.Target.INT -= num2;
	}

	protected override void OnDisable()
	{
		float num = 1f - base.Intensity;
		float num2 = (float)base.Target.Data.STR * num;
		base.Target.STR += num2;
		num2 = (float)base.Target.Data.INT * num;
		base.Target.INT += num2;
	}
}
