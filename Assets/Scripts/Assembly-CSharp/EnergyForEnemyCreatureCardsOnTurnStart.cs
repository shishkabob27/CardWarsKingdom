public class EnergyForEnemyCreatureCardsOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		int num = base.Owner.Owner.Opponent.DeploymentList.Count;
		if (num > 5)
		{
			num = 5;
		}
		if (num > 0)
		{
			base.Owner.Owner.AddActionPoints(num * base.Val1);
			return true;
		}
		return false;
	}
}
