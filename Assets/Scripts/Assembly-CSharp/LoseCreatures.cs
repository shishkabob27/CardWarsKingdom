public class LoseCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CreaturesLost;
		}
	}
}
