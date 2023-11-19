public class PlagueAndCrippleToDeployedEnemies : OnOppDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(CreatureDeployed, StatusEnum.Plague, base.Val1);
		ApplyStatus(CreatureDeployed, StatusEnum.Cripple, base.Val2);
		return true;
	}
}
