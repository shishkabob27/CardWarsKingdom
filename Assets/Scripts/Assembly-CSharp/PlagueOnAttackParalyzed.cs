public class PlagueOnAttackParalyzed : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && (bool)Target.IsParalyzed)
		{
			ApplyStatus(Target, StatusEnum.Plague, base.Val1);
			return true;
		}
		return false;
	}
}
