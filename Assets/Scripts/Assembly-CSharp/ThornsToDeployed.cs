public class ThornsToDeployed : OnDeployed
{
	public override bool OnEnable()
	{
		ApplyStatus(DeployedCreature, StatusEnum.Thorns, base.Val1);
		return true;
	}
}
