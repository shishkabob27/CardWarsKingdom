public class GrantThornBraveryIfOppHasMoreCards : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Opponent.Hand.Count > base.Owner.Owner.Hand.Count)
		{
			ApplyStatus(base.Owner, StatusEnum.Thorns, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val1);
			return true;
		}
		return false;
	}
}
