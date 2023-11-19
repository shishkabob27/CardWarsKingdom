public class CollectTeeth : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.TeethCollected;
		}
	}
}
