public class AtkSpikeForEachCardOnStartTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > 0)
		{
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1 * base.Owner.Owner.Hand.Count);
			return true;
		}
		return false;
	}
}
