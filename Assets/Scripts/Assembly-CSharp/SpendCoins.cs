public class SpendCoins : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CoinsSpent;
		}
	}
}
