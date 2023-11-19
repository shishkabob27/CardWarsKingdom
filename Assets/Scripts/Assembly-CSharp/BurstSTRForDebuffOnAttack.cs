public class BurstSTRForDebuffOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		int statusTypeCount = Target.GetStatusTypeCount(StatusType.Debuff);
		if (statusTypeCount > 0)
		{
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1 * statusTypeCount);
			return true;
		}
		return false;
	}
}
