public class SellRunes : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.RunesSold;
		}
	}
}
