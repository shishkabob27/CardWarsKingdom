public class EnhanceSTR : StatusState
{
	protected override void OnEnable()
	{
		float num = (float)base.Target.Data.STR * base.Intensity;
		base.Target.STR += num;
	}

	protected override void OnDisable()
	{
		float num = (float)base.Target.Data.STR * base.Intensity;
		base.Target.STR -= num;
	}
}
