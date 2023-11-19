public class WeakenStrAndIntOnAttackWhenCardsHeld : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > base.Val1)
		{
			ApplyStatus(Target, StatusEnum.WeakenStrength, base.Val2);
			ApplyStatus(Target, StatusEnum.WeakenMagic, base.Val2);
			return true;
		}
		return false;
	}
}
