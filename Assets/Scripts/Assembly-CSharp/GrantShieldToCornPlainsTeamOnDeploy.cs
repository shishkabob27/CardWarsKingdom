public class GrantShieldToCornPlainsTeamOnDeploy : OnDeployed
{
	public override bool OnEnable()
	{
		if (DeployedCreature.Data.Faction == CreatureFaction.Red || DeployedCreature.Data.Faction == CreatureFaction.Blue)
		{
			ApplyStatus(DeployedCreature, StatusEnum.Shield, base.Val1);
			return true;
		}
		return false;
	}
}
