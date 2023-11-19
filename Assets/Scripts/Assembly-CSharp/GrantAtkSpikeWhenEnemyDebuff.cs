public class GrantAtkSpikeWhenEnemyDebuff : OnEnemyDebuff
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
		return true;
	}
}
