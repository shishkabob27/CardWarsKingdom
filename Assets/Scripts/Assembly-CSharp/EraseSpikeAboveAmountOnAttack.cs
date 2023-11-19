public class EraseSpikeAboveAmountOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		if (Target != null)
		{
			StatusState statusState = Target.StatusEffects.Find((StatusState m) => m is BurstSTR);
			if (statusState != null && statusState.Intensity > base.Val1Pct)
			{
				CancelStatus(Target, StatusEnum.AttackSpike);
				result = true;
			}
			statusState = Target.StatusEffects.Find((StatusState m) => m is BurstINT);
			if (statusState != null && statusState.Intensity > base.Val1Pct)
			{
				CancelStatus(Target, StatusEnum.MagicSpike);
				result = true;
			}
		}
		return result;
	}
}
