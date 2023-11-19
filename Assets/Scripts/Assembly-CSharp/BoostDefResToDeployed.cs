public class BoostDefResToDeployed : OnDeployed
{
	public override bool OnEnable()
	{
		ApplyStatus(DeployedCreature, StatusEnum.DefenseBoost, base.Val1);
		ApplyStatus(DeployedCreature, StatusEnum.ResistanceBoost, base.Val1);
		return true;
	}
}
