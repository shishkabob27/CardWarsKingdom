public class Nervous : StatusState
{
	protected override void OnEnable()
	{
		float num = (float)base.Target.Data.DEX * base.Intensity;
		base.Target.DEX -= num;
	}

	protected override void OnDisable()
	{
		float num = (float)base.Target.Data.DEX * base.Intensity;
		base.Target.DEX += num;
	}
}
