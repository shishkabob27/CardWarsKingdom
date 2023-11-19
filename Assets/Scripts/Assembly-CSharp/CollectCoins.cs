public class CollectCoins : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CoinsCollected;
		}
	}
}
