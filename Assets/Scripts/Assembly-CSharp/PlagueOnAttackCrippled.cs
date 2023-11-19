public class PlagueOnAttackCrippled : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && Target.HasCripple)
		{
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			return true;
		}
		return false;
	}
}
