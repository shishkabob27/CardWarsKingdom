public class EnergyForEachOpponentCardOnStartTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Opponent.Hand.Count > 0)
		{
			base.Owner.Owner.AddActionPoints(base.Val1 * base.Owner.Owner.Opponent.Hand.Count);
			return true;
		}
		return false;
	}
}
