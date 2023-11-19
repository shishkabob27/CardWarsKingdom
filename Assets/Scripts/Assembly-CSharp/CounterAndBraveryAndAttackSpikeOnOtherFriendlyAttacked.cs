public class CounterAndBraveryAndAttackSpikeOnOtherFriendlyAttacked : OnFriendlyAttacked
{
	public override bool OnEnable()
	{
		if (Attacked != base.Owner)
		{
			ApplyStatus(base.Owner, StatusEnum.Counterattack, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val3);
			return true;
		}
		return false;
	}
}
