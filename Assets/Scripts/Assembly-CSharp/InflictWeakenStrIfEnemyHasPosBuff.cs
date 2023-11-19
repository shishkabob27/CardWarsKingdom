public class InflictWeakenStrIfEnemyHasPosBuff : OnAttack
{
	public override bool OnEnable()
	{
		if (Target.HasBuff)
		{
			ApplyStatus(Target, StatusEnum.WeakenStrength, base.Val1);
			return true;
		}
		return false;
	}
}
