public class EnergyForEachCardOnStartTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > 0)
		{
			base.Owner.Owner.AddActionPoints(base.Val1 * base.Owner.Owner.Hand.Count);
			return true;
		}
		return false;
	}
}
