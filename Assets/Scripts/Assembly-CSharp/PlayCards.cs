public class PlayCards : Mission
{
	public override int ProgressValue
	{
		get
		{
			return base.Progress.CardsPlayed;
		}
	}
}
