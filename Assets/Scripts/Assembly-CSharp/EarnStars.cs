public class EarnStars : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.StarsEarned;
		}
	}
}
