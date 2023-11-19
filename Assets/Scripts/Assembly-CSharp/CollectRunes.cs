public class CollectRunes : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.RunesCollected;
		}
	}
}
