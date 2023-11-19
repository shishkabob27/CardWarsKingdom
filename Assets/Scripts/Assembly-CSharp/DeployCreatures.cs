public class DeployCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.DeployedCreatures;
		}
	}
}
