public class DirectDamage : StatusState
{
	protected override void OnEnable()
	{
		if (base.Intensity != 0f && !base.Target.IsDDImmune)
		{
			float num = (float)base.Target.MaxHP * base.Intensity;
			if (num < 1f)
			{
				num = 1f;
			}
			base.Target.DealDamage((int)num, AttackBase.None, true, base.SourceCard, this);
		}
	}
}
