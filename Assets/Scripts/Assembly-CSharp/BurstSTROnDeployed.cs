public class BurstSTROnDeployed : OnDeployed
{
	public override bool OnEnable()
	{
		ApplyStatus(DeployedCreature, StatusEnum.AttackSpike, base.Val1);
		return true;
	}
}
