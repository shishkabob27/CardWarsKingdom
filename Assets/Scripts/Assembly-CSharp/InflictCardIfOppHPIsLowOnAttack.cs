public class InflictCardIfOppHPIsLowOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (Target.HPPct < base.Val1Pct)
		{
			ApplyStatus(Target, StatusEnum.CardBlock, 1f);
			return true;
		}
		return false;
	}
}
