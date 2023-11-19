public class PlagueAndVampiricOnAttackCrippled : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && Target.HasCripple)
		{
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val2);
			return true;
		}
		return false;
	}
}
