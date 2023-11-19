public class ParalyzeOnHPThreshold : OnAttackHPThreshold
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Paralyze, base.Val2);
		return true;
	}
}
