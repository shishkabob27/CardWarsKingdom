public class ParalyzeOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(Target, StatusEnum.Paralyze, base.Val2);
			return true;
		}
		return false;
	}
}
