public class PlagueAndVampiricOnAttackParalyzed : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && (bool)Target.IsParalyzed)
		{
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val2);
			return true;
		}
		return false;
	}
}
