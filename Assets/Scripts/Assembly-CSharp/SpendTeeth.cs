public class SpendTeeth : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.TeethSpent;
		}
	}
}
