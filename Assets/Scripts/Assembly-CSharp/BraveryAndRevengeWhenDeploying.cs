public class BraveryAndRevengeWhenDeploying : OnDeployed
{
	public override bool OnEnable()
	{
		if (DeployedCreature.Data.Form.Faction == CreatureFaction.Green)
		{
			ApplyStatus(DeployedCreature, StatusEnum.Bravery, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.Revenge, 1f);
			return true;
		}
		return false;
	}
}
