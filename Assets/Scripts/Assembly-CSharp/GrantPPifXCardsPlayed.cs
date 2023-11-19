public class GrantPPifXCardsPlayed : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.PlayedCards.Count >= base.Val1)
		{
			ApplyStatus(base.Owner, StatusEnum.Pinpoint, 1f);
			return true;
		}
		return false;
	}
}
