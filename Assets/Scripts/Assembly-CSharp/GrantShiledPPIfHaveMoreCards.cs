public class GrantShiledPPIfHaveMoreCards : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > base.Owner.Owner.Opponent.Hand.Count)
		{
			ApplyStatus(base.Owner, StatusEnum.Shield, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.Pinpoint, base.Val1);
			return true;
		}
		return false;
	}
}
