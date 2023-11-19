public class BurstINTOnFactionCardDraw : OnCardDraw
{
	public override bool OnEnable()
	{
		if (CardDrawn.Faction == (CreatureFaction)base.Val2)
		{
			ApplyStatus(base.Owner, StatusEnum.MagicSpike, base.Val1);
			return true;
		}
		return false;
	}
}
