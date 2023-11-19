public class BreakOnAttackWhenHoldingCards : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > base.Val1)
		{
			ApplyStatus(Target, StatusEnum.BreakDefense, base.Val2);
			ApplyStatus(Target, StatusEnum.BreakResistance, base.Val2);
			return true;
		}
		return false;
	}
}
