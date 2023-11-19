public class CollectCreatures : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CreaturesCollected;
		}
	}
}
