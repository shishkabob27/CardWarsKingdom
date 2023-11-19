public class InflictParalyzePlagueIfEnemyMultiAtk : OnEnemyAttackThreshold
{
	public override bool OnEnable()
	{
		ApplyStatus(Attacker, StatusEnum.Paralyze, base.Val2);
		ApplyStatus(Attacker, StatusEnum.Plague, base.Val2);
		return true;
	}
}
