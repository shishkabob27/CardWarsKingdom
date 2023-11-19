public class SellCards : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CardsSold;
		}
	}
}
