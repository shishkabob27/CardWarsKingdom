public class RemoveLandCornAttkSpikeOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		StatusState statusState = Target.StatusEffects.Find((StatusState s) => s is LandCorn);
		if (statusState != null)
		{
			CancelStatus(Target, StatusEnum.LandCorn);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
			result = true;
		}
		return result;
	}
}
