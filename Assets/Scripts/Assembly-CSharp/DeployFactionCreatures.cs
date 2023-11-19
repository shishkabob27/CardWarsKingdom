public class DeployFactionCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.DeployedFaction[base.Val2];
		}
	}
}
