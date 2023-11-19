public class ParalyzeOnAttackHighHP : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && Target.HPPct > base.Val1Pct)
		{
			ApplyStatus(Target, StatusEnum.Paralyze, base.Val2);
			return true;
		}
		return false;
	}
}
