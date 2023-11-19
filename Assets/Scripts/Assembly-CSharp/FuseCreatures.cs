public class FuseCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CreaturesFused;
		}
	}
}
