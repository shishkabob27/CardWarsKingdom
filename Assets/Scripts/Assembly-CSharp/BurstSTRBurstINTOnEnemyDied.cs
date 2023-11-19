public class BurstSTRBurstINTOnEnemyDied : OnEnemyDied
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.MagicSpike, base.Val1);
		return true;
	}
}
