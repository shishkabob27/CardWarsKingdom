public class DoAttacks : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.Attacks;
		}
	}
}
